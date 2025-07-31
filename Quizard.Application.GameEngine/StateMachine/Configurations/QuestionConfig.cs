using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quizard.Application.GameEngine.Lifecycle;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class QuestionConfig : IGameStateConfiguration
{
    private readonly ILogger<QuestionConfig> _logger;
    private readonly IServiceProvider _serviceProvider;

    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    private IGameNotificationService GameNotificationService
        => _serviceProvider.GetRequiredService<IGameNotificationService>();
    
    public QuestionConfig(
        ILogger<QuestionConfig> logger,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _logger.LogInformation("Конфигурация FSM - Question Playing");
    }

    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        _logger.LogTrace("Конфигурируем поведение FSM в состоянии QuestionPlaying");
        stateMachine.Configure(GameState.QuestionPlaying)
            .OnEntryAsync(async () =>
            {
                var session = GameSessionService.GetSession(true);
                _logger.LogInformation("Играем вопрос № {Index}", session.CurrentQuestionIndex);
                var round = session.CurrentRound!;
                var question = session.CurrentRound!.Questions
                    .First(question => question.QuestionNumber == session.CurrentQuestionIndex);

                switch (round.RoundTypeId)
                {
                    case RoundTypeId.HotPotato:
                    case RoundTypeId.Pantomime:
                        var selectedPlayer = session.GetNextRandomPlayer();
                        await GameNotificationService.QuestionStarted(selectedPlayer, question);
                        break;

                    default:
                        await GameNotificationService.QuestionStarted(question);
                        break;
                }

                await GameSessionService.SaveChangesAsync();

                _logger.LogTrace("Запуск таймера вопроса");
                _ = Task.Run(async () =>
                {
                    var timerProperties = new GameTimerProperties(_serviceProvider, questionId: question.Id);

                    if (question.AnswerDelay.HasValue)
                        await Task.Delay(
                            TimeSpan.FromSeconds(question.QuestionTimeout + (int)question.AnswerDelay + 1));
                    else
                        await Task.Delay(TimeSpan.FromSeconds(question.QuestionTimeout + 1));

                    var currentGameSession = timerProperties.GameSessionService.GetSession();
                    if (timerProperties.QuestionId != null)
                    {
                        if (currentGameSession.CurrentState == GameState.QuestionPlaying)
                        {
                            var currentQuestion = currentGameSession.CurrentRound!.Questions
                                .First(q => q.QuestionNumber == currentGameSession.CurrentQuestionIndex);
                            if (currentQuestion.Id == timerProperties.QuestionId)
                            {
                                _logger.LogInformation("Таймаут вопроса истёк — форсим завершение");
                                await stateMachine.FireAsync(GameTrigger.QuestionCompleted).ConfigureAwait(false);
                            }
                        }
                    }
                });
            })
            .Permit(GameTrigger.Skip, GameState.RevealShowing)
            .PermitDynamicAsync(GameTrigger.QuestionCompleted, async () =>
            {
                var session = GameSessionService.GetSession();
                var round = session.CurrentRound!;

                if (round.RoundTypeId is not (RoundTypeId.GuessMelody or RoundTypeId.Blitz))
                    return GameState.RevealShowing;

                var winner = session.PlayersSpeedAnswers
                    .OrderBy(kvp => kvp.Value)
                    .Select(kvp => kvp.Key)
                    .FirstOrDefault();
                if (winner == null) return GameState.RevealShowing;

                await GameNotificationService.SpeedWinner(winner).ConfigureAwait(false);
                await Task.Delay(750);

                return GameState.RevealShowing;
            })
            .Permit(GameTrigger.EndRequested, GameState.Finished)
            .Permit(GameTrigger.RoundTimeout, GameState.RevealShowing)
            .PermitDynamic(GameTrigger.PauseRequested, () => context.ProcessPause(stateMachine));
    }
}