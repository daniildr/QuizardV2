using Microsoft.Extensions.Logging;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class ModifiersConfig : IGameStateConfiguration
{
    private readonly ILogger<ModifiersConfig> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IQuizardDbManager _quizardDbManager;

    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        //throw new NotImplementedException();
    }
}