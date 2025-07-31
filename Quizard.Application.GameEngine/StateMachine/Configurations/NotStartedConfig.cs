using Microsoft.Extensions.Logging;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class NotStartedConfig : IGameStateConfiguration
{
    private readonly ILogger<NotStartedConfig> _logger;

    public NotStartedConfig(ILogger<NotStartedConfig> logger)
    {
        _logger = logger;
        
        _logger.LogInformation("Конфигурация FSM - NotStarted");
    }


    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        _logger.LogTrace("Конфигурируем поведение FSM в состоянии NotStarted");
        stateMachine.Configure(GameState.NotStarted)
            .Permit(GameTrigger.StartRequested, GameState.WaitingForPlayers);
    }
}