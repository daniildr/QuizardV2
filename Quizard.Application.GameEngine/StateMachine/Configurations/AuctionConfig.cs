using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizard.Application.GameEngine.Options;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class AuctionConfig : IGameStateConfiguration
{
    private readonly ILogger<AuctionConfig> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<GameOptions> _gameOptions;

    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    private IGameNotificationService GameNotificationService
        => _serviceProvider.GetRequiredService<IGameNotificationService>();
    
    public AuctionConfig(
        ILogger<AuctionConfig> logger,
        IOptions<GameOptions> options,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _gameOptions = options;
        _logger = logger;

        _logger.LogInformation("Конфигурация FSM - Auction");
    }

    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        _logger.LogTrace("Конфигурируем поведение FSM в состоянии Auction");
        stateMachine.Configure(GameState.Auction)
            .OnEntryAsync(async () =>
            {
                _logger.LogInformation("Запускается аукцион");
                var session = GameSessionService.GetSession(true);
                session.PlayersWhoCompleteState.Clear();
                await GameSessionService.SaveChangesAsync().ConfigureAwait(false);
            })
            .Permit(GameTrigger.EndRequested, GameState.Finished)
            .Permit(GameTrigger.AuctionCompleted, GameState.QuestionPlaying)
            .PermitDynamic(GameTrigger.PauseRequested, () => context.ProcessPause(stateMachine))
            .OnExitAsync(async () =>
            {
                _logger.LogInformation("Аукцион завершен");
                var session = GameSessionService.GetSession(true);
                session.PlayersWhoCompleteState.Clear();
                await GameSessionService.SaveChangesAsync().ConfigureAwait(false);
            });
    }
}