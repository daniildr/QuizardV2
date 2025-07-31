using Microsoft.Extensions.Logging;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class PauseConfig : IGameStateConfiguration
{
    private readonly ILogger<PauseConfig> _logger;
    
    public PauseConfig(ILogger<PauseConfig> logger)
    {
        _logger = logger;

        _logger.LogInformation("Конфигурация FSM - Pause");
    }

    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        _logger.LogTrace("Конфигурируем поведение FSM в состоянии Pause");
        stateMachine.Configure(GameState.Pause)
            .PermitDynamic(GameTrigger.ResumeRequested, () =>
            {
                _logger.LogInformation("Возобновляем сессию, возврат в состояние - {State}", context.StateBeforePause);
                return context.StateBeforePause ?? GameState.WaitingForPlayers;
            })
            .Permit(GameTrigger.EndRequested, GameState.Finished)
            .OnExit(() =>
            {
                _logger.LogTrace("Выход из паузы, сбрасываем StateBeforePause");
                context.StateBeforePause = null;
            });
    }
}