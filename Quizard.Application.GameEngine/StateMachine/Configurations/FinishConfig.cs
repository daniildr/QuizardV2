using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class FinishConfig : IGameStateConfiguration
{
    private readonly ILogger<FinishConfig> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IQuizardDbManager _quizardDbManager;
    
    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    private IGameNotificationService GameNotificationService
        => _serviceProvider.GetRequiredService<IGameNotificationService>();

    public FinishConfig(
        ILogger<FinishConfig> logger,
        IServiceProvider serviceProvider,
        IQuizardDbManager quizardDbManager)
    {
        _quizardDbManager = quizardDbManager;
        _serviceProvider = serviceProvider;
        _logger = logger;

        _logger.LogInformation("Конфигурация FSM - Finish");
    }

    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        _logger.LogTrace("Конфигурируем поведение FSM в состоянии Finish, ShowingScenarioStats");
        stateMachine.Configure(GameState.Finished)
            .OnEntryAsync(async () =>
            {
                _logger.LogInformation("Игровой движок ожидает статистику пользователей");
                var session = GameSessionService.GetSession(true);
                session.PlayersWhoCompleteState.Clear();
                
                await GameSessionService.SaveChangesAsync();
            })
            .Permit(GameTrigger.StatsRequested, GameState.ShowingScenarioStats)
            .OnExitAsync(async () =>
            {
                var session = GameSessionService.GetSession(true);
                session.PlayersHowAnsweredOrShowingQuestion.Clear();
                session.PlayersWhoCompleteState.Clear();
                
                await GameSessionService.SaveChangesAsync();
            });

        stateMachine.Configure(GameState.ShowingScenarioStats)
            .OnEntryAsync(async () =>
            {
                _logger.LogInformation("Игровая сессия находится в стадии показа итоговой статистики игроков");
                var session = GameSessionService.GetSession();

                var stats = await _quizardDbManager.StatisticRepository
                    .GetScenarioStatistic(scenarioStatistic => scenarioStatistic.ScenarioId == session.Scenario.Id
                                                               && scenarioStatistic.GameId == session.GameId)
                    .ConfigureAwait(true);
                await GameNotificationService.ShowScenarioStatistic(stats).ConfigureAwait(false);
            });
    }
}