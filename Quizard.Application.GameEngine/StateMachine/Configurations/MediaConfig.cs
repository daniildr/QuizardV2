using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class MediaConfig : IGameStateConfiguration
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MediaConfig> _logger;

    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    private IGameNotificationService GameNotificationService
        => _serviceProvider.GetRequiredService<IGameNotificationService>();

    public MediaConfig(IServiceProvider serviceProvider, ILogger<MediaConfig> logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        _logger.LogInformation("Конфигурация FSM - Media");
    }

    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        _logger.LogTrace("Конфигурируем поведение FSM в состоянии Media");
        stateMachine.Configure(GameState.Media)
            .OnEntryAsync(async () =>
            {
                var session = GameSessionService.GetSession();

                _logger.LogInformation("Получаем текущий медиа этап");
                var media = session.Scenario.Stages.First(stage => stage.Index == session.CurrentStateIndex).Media!;

                _logger.LogInformation("Уведомляем информатор о старте видео");
                await GameNotificationService.MediaStarted(media);
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
            .PermitDynamicAsync(GameTrigger.MediaEnded, async () =>
            {
                var session = GameSessionService.GetSession(true);
                session.CurrentStateIndex++;
                await GameSessionService.SaveChangesAsync();
                var nextState = session.Scenario.Stages
                    .FirstOrDefault(stage => stage.Index == session.CurrentStateIndex);

                return context.SwitchStateByStages(nextState, stateMachine);
            })
            .PermitDynamic(GameTrigger.PauseRequested, () => context.ProcessPause(stateMachine))
            .Permit(GameTrigger.EndRequested, GameState.Finished);
    }
}