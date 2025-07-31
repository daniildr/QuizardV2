using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class VotingConfig : IGameStateConfiguration
{
    private readonly ILogger<VotingConfig> _logger;
    private readonly IServiceProvider _serviceProvider;
    
    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    private IGameNotificationService GameNotificationService
        => _serviceProvider.GetRequiredService<IGameNotificationService>();
    
    public VotingConfig(
        ILogger<VotingConfig> logger,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _logger.LogInformation("Конфигурация FSM - VotingConfig");
    }

    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        _logger.LogTrace("Конфигурируем поведение FSM в состоянии RoundPlaying");
        stateMachine.Configure(GameState.Voting)
            .OnEntryAsync(async () =>
            {
                var session = GameSessionService.GetSession(true);
                
                _logger.LogTrace("Сбрасываем коллекции игроков, которые завершили раунд");
                session.PlayersHowAnsweredOrShowingQuestion.Clear();
                session.PlayersWhoCompleteState.Clear();
                
                _logger.LogTrace("Сбрасываем предыдущие голоса");
                session.PlayersVoices.Clear();

                _logger.LogTrace("Устанавливаем выбор раунда в null (маркер)");
                session.CurrentRound = null;
                session.RoundChosenByPlayers = null;

                await GameSessionService.SaveChangesAsync();

                _logger.LogInformation("Уведомляем клиентов о начале голосования. Рассылаем раунды для голосования");
                if (session.Scenario.RoundDefinitions.Count > 4)
                    await GameNotificationService
                        .VotingStarted(session.Scenario.RoundDefinitions.ToArray()[..4])
                        .ConfigureAwait(false);
                else if (session.Scenario.RoundDefinitions.Count == 1)
                    await stateMachine.FireAsync(GameTrigger.VotingCompleted);
                else 
                    await GameNotificationService
                        .VotingStarted(session.Scenario.RoundDefinitions.ToArray())
                        .ConfigureAwait(false);
            })
            .PermitDynamicAsync(GameTrigger.VotingCompleted, async () =>
            {
                var session = GameSessionService.GetSession(true);

                _logger.LogInformation("Подсчет голосов: группа по RoundName, считаем сумму Votes");
                var winnerRound = session.Scenario.RoundDefinitions.Count > 1 
                    ? GetWinnerRound(session) 
                    : session.Scenario.RoundDefinitions.First();

                _logger.LogTrace("Устанавливаем победителя голосования");
                session.RoundChosenByPlayers = winnerRound;

                session.CurrentStateIndex++;
                await GameSessionService.SaveChangesAsync();
                
                var nextState = session.Scenario.Stages
                    .FirstOrDefault(stage => stage.Index == session.CurrentStateIndex);
                return context.SwitchStateByStages(nextState, stateMachine);
            })
            .PermitDynamic(GameTrigger.PauseRequested, () => context.ProcessPause(stateMachine))
            .Permit(GameTrigger.EndRequested, GameState.Finished)
            .OnExitAsync(async () =>
            {
                var session = GameSessionService.GetSession(true);
                session.PlayersHowAnsweredOrShowingQuestion.Clear();
                session.PlayersWhoCompleteState.Clear();
                
                await GameSessionService.SaveChangesAsync();
            });
    }
    
    private static Round GetWinnerRound(IGameSession session)
    {
        var maxVoices = session.PlayersVoices.Values.Max();
        var maxRounds = session.PlayersVoices
            .Where(kv => kv.Value == maxVoices)
            .Select(kv => kv.Key)
            .ToList();

        return maxRounds.Count == 1 
            ? maxRounds[0] 
            : maxRounds[new Random().Next(maxRounds.Count)];
    }
}