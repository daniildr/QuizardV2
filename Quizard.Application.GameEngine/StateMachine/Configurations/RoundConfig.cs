using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizard.Application.GameEngine.Lifecycle;
using Quizard.Application.GameEngine.Options;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class RoundConfig : IGameStateConfiguration
{
    private readonly ILogger<RoundConfig> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<GameOptions> _gameOptions;

    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    private IGameNotificationService GameNotificationService
        => _serviceProvider.GetRequiredService<IGameNotificationService>();

    public RoundConfig(
        ILogger<RoundConfig> logger,
        IOptions<GameOptions> options,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _gameOptions = options;
        _logger = logger;

        _logger.LogInformation("Конфигурация FSM - RoundPlaying");
    }

    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        _logger.LogTrace("Конфигурируем поведение FSM в состоянии RoundPlaying");
        stateMachine.Configure(GameState.RoundPlaying)
            .OnEntryAsync(async () =>
            {
                _logger.LogInformation("Запускается игровой раунд");
                var session = GameSessionService.GetSession(true);
                var scenario = session.Scenario;
                session.CurrentQuestionIndex++;

                var round = session.Scenario.Stages.First(stage => stage.Index == session.CurrentStateIndex).Round;
                if (round == null)
                {
                    _logger.LogTrace("В текущем этапе нет раунда. Раунд был получен по итогам голосования");
                    round = session.RoundChosenByPlayers!;
                }

                _logger.LogTrace("Обновляем коллекцию вопросов в сценарии");
                session.Scenario.RoundDefinitions.Remove(round);
                session.CurrentRound = round;

                _logger.LogTrace("Отправляем данные раунда");
                await GameNotificationService.RoundStarted(session.CurrentRound);

                await GameSessionService.SaveChangesAsync();

                _logger.LogTrace("Ожидаем демонстрацию раунда на экране информаторе");
                await Task.Delay(TimeSpan.FromSeconds(scenario.RoundPresentationDuration 
                                                      ?? _gameOptions.Value.DefaultRoundQuestionDelay));

                if (round.RoundDuration != null)
                {
                    _logger.LogTrace("Запуск таймера раунда");
                    _ = Task.Run(async () =>
                    {
                        var timerProperties = new GameTimerProperties(_serviceProvider, roundId: round.RoundId);

                        await Task.Delay(TimeSpan.FromSeconds((int)round.RoundDuration! + 1));

                        var currentGameSession = timerProperties.GameSessionService.GetSession(true);
                        if (timerProperties.RoundId != null)
                        {
                            if (currentGameSession.CurrentRound != null)
                            {
                                if (currentGameSession.CurrentRound.RoundId == timerProperties.RoundId)
                                {
                                    _logger.LogInformation("Таймаут раунда истёк — переключаем счетчик вопросов");
                                    currentGameSession.CurrentQuestionIndex =
                                        currentGameSession.CurrentRound.Questions.Count;

                                    if (currentGameSession.CurrentState == GameState.QuestionPlaying)
                                    {
                                        _logger.LogInformation("Таймаут раунда истёк — форсим завершение");
                                        await stateMachine.FireAsync(GameTrigger.RoundTimeout).ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                    });
                }

                if (round.RoundTypeId == RoundTypeId.Auction)
                {
                    _logger.LogInformation("Играем аукцион, будет выполнен переход в стадию аукцион");
                    await stateMachine.FireAsync(GameTrigger.AuctionStarted).ConfigureAwait(false);
                }
                else
                {
                    await stateMachine.FireAsync(GameTrigger.RoundStarted).ConfigureAwait(false);
                }
            })
            .Ignore(GameTrigger.ApplyTargetModifiersCompleted)
            .Permit(GameTrigger.Skip, GameState.QuestionPlaying)
            .Permit(GameTrigger.RoundStarted, GameState.QuestionPlaying)
            .Permit(GameTrigger.AuctionStarted, GameState.Auction)
            .Permit(GameTrigger.EndRequested, GameState.Finished)
            .PermitDynamic(GameTrigger.PauseRequested, () => context.ProcessPause(stateMachine));
    }
}