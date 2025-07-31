using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class StatsConfig : IGameStateConfiguration
{
    private readonly ILogger<StatsConfig> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IQuizardDbManager _quizardDbManager;
    
    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    private IGameNotificationService GameNotificationService
        => _serviceProvider.GetRequiredService<IGameNotificationService>();

    public StatsConfig(
        ILogger<StatsConfig> logger,
        IServiceProvider serviceProvider,
        IQuizardDbManager quizardDbManager)
    {
        _quizardDbManager = quizardDbManager;
        _serviceProvider = serviceProvider;
        _logger = logger;

        _logger.LogInformation("Конфигурация FSM - Statistic");
    }
    
    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        _logger.LogTrace("Конфигурируем поведение FSM в состоянии WaitStats, ShowingStats");
        stateMachine.Configure(GameState.WaitStats)
            .OnEntryAsync(async () =>
            {
                _logger.LogInformation("Игровой движок ожидает статистику пользователей");
                var session = GameSessionService.GetSession(true);
                session.PlayersWhoCompleteState.Clear();
                
                await GameSessionService.SaveChangesAsync();
            })
            .Ignore(GameTrigger.RevealShowed)
            .Permit(GameTrigger.StatsRequested, GameState.ShowingStats)
            .Permit(GameTrigger.EndRequested, GameState.Finished)
            .PermitDynamic(GameTrigger.PauseRequested, () => context.ProcessPause(stateMachine))
            .OnExitAsync(async () =>
            {
                var session = GameSessionService.GetSession(true);
                session.PlayersHowAnsweredOrShowingQuestion.Clear();
                session.PlayersWhoCompleteState.Clear();
                
                await GameSessionService.SaveChangesAsync();
            });
        
        stateMachine.Configure(GameState.ShowingStats)
            .OnEntryAsync(async () =>
            {
                _logger.LogInformation("Игровая сессия находится в стадии показа статистики игроков");
                var session = GameSessionService.GetSession(true);
                session.PlayersWhoCompleteState.Clear();
                await GameSessionService.SaveChangesAsync();
                
                var stats = await _quizardDbManager.StatisticRepository
                    .GetRoundStatistic(roundStatistic => roundStatistic.RoundId == session.CurrentRound!.RoundId 
                                               && roundStatistic.GameId == session.GameId).ConfigureAwait(true);
                await GameNotificationService.ShowRoundStatistic(stats).ConfigureAwait(false);
            })
            .PermitDynamicAsync(GameTrigger.Skip, async () =>
            {
                var session = GameSessionService.GetSession(true);
                session.CurrentStateIndex++;
                await GameSessionService.SaveChangesAsync();
                
                var nextState = session.Scenario.Stages
                    .FirstOrDefault(stage => stage.Index == session.CurrentStateIndex);
                return context.SwitchStateByStages(nextState, stateMachine);
            })
            .PermitDynamicAsync(GameTrigger.StatsDisplayed, async () =>
            {
                var session = GameSessionService.GetSession(true);
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
                session.CurrentQuestionIndex = 0;
                session.RoundChosenByPlayers = null;
                session.CurrentRound = null;
                session.PlayersHowAnsweredOrShowingQuestion.Clear();
                session.PlayersWhoCompleteState.Clear();
                
                await GameSessionService.SaveChangesAsync();
            });
    }
}