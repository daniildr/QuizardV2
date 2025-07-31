using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizard.Application.GameEngine.Lifecycle;
using Quizard.Application.GameEngine.Options;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Quizard.Core.Models.Responses;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class ShopConfig : IGameStateConfiguration
{
    private readonly ILogger<ShopConfig> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<GameOptions> _gameOptions;
    
    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    private IGameNotificationService GameNotificationService
        => _serviceProvider.GetRequiredService<IGameNotificationService>();

    public ShopConfig(ILogger<ShopConfig> logger, IOptions<GameOptions> options, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _gameOptions = options;
        _logger = logger;
        
        _logger.LogInformation("Конфигурация FSM - Shop");
    }

    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        stateMachine.Configure(GameState.Shop)
            .OnEntryAsync(async () =>
            {
                _logger.LogTrace("Фаза покупок (отображения магазина) начата");
                var session = GameSessionService.GetSession(true);

                _logger.LogTrace("Сброс коллекции готовности пользователей");
                session.PlayersWhoCompleteState.Clear();
                var shopStage = session.Scenario.Stages.First(stage => stage.Index == session.CurrentStateIndex);

                await GameSessionService.SaveChangesAsync();

                _logger.LogInformation("Оповещаем клиентов о старте этапа 'магазина'");
                var shopConfiguration = new PurchasePhaseConfiguration
                {
                    Duration = shopStage.StageDuration ?? _gameOptions.Value.DefaultShopDuration,
                    ShopStocks = session.ShopStocks
                };
                await GameNotificationService.ShopStarted(shopConfiguration);

                _logger.LogTrace("Запуск таймера магазина");
                _ = Task.Run(async () =>
                {
                    var timerProperties =
                        new GameTimerProperties(_serviceProvider, stateNumber: session.CurrentStateIndex);
                    
                    await Task.Delay(TimeSpan.FromSeconds(shopConfiguration.Duration));
                    
                    var currentGameSession = timerProperties.GameSessionService.GetSession(true);
                    var currentStageNumber = currentGameSession.CurrentStateIndex;
                    if (currentGameSession.CurrentState == GameState.Shop 
                        && currentStageNumber == timerProperties.StateNumber)
                    {
                        _logger.LogInformation("Таймаут фаза покупок истёк — форсим завершение");
                        await stateMachine.FireAsync(GameTrigger.ShopTimeout).ConfigureAwait(false);
                    }
                });
            })
            .Permit(GameTrigger.Skip, GameState.ApplyingTargetModifiers)
            .Permit(GameTrigger.ShopEnded, GameState.ApplyingTargetModifiers)
            .Permit(GameTrigger.ShopTimeout, GameState.ApplyingTargetModifiers)
            .Permit(GameTrigger.EndRequested, GameState.Finished)
            .PermitDynamic(GameTrigger.PauseRequested, () => context.ProcessPause(stateMachine));

        stateMachine.Configure(GameState.ApplyingTargetModifiers)
            .OnEntryAsync(async () =>
            {
                _logger.LogTrace("Фаза применения этапа модификаторов начата");
                var session = GameSessionService.GetSession(true);

                _logger.LogTrace("Сброс коллекции готовности пользователей");
                session.PlayersWhoCompleteState.Clear();

                await GameSessionService.SaveChangesAsync();
                
            })
            .PermitDynamicAsync(GameTrigger.ApplyTargetModifiersCompleted, async () =>
            {
                var session = GameSessionService.GetSession(true);
                session.CurrentStateIndex++;
                await GameSessionService.SaveChangesAsync();
                
                var nextState = session.Scenario.Stages
                    .FirstOrDefault(stage => stage.Index == session.CurrentStateIndex);
                return context.SwitchStateByStages(nextState, stateMachine);
            })
            .Permit(GameTrigger.EndRequested, GameState.Finished)
            .OnExitAsync(async () =>
            {
                var session = GameSessionService.GetSession(true);
                session.PlayersWhoCompleteState.Clear();
                
                await GameSessionService.SaveChangesAsync();
            });
    }
}