using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine;

/// <inheritdoc/>
public class StateMachineProvider : IStateMachineProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StateMachineProvider> _logger;
    private readonly IGameNotificationService  _gameNotificationService;
    private readonly IStateMachineConfigurator _stateMachineConfigurator;
    
    private bool _stateMachineAlreadyInitialized;
    
    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    /// <inheritdoc/>
    public StateMachine<GameState, GameTrigger> StateMachine { get; private set; }

    /// <summary> Конструктор конфигуратор </summary>
    /// <param name="serviceProvider"> Сервис провайдер </param>
    /// <param name="logger"> Логгер </param>
    /// <param name="gameNotificationService"> Сервис SignalR уведомлений </param>
    /// <param name="stateMachineConfigurator"> Конфигуратор конечного автомата игры </param>
    public StateMachineProvider(
        IServiceProvider serviceProvider, 
        ILogger<StateMachineProvider> logger, 
        IGameNotificationService gameNotificationService, 
        IStateMachineConfigurator stateMachineConfigurator)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _gameNotificationService = gameNotificationService;
        _stateMachineConfigurator = stateMachineConfigurator;

        StateMachine = new StateMachine<GameState, GameTrigger>(GameState.NotStarted); 
        
        _logger.LogInformation("Провайдер машины состояний проинициализирован");
    }

    /// <inheritdoc/>
    public StateMachine<GameState, GameTrigger> Create(IGameSession session)
    {
        if (_stateMachineAlreadyInitialized)
        {
            _logger.LogWarning("Конечный автомат уже был проинициализирован. Провайдер вернет его инстанс");
            return StateMachine;
        }
        
        _logger.LogInformation(
            "Создаем новый конечный автомат для игровой сессии в состоянии {gameState}", session.CurrentState);

        StateMachine = new StateMachine<GameState, GameTrigger>(session.CurrentState);
        _stateMachineConfigurator.Configure(StateMachine);
        
        _logger.LogTrace("Регистрируем обратный вызов, который будет вызываться при изменении состояния");
        StateMachine.OnTransitioned(async void (transition) =>
        {
            try
            {
                GameSessionService.GetSession(true).CurrentState = transition.Destination;
                await GameSessionService.SaveChangesAsync().ConfigureAwait(false);
                await _gameNotificationService
                    .GameStateChanged(transition.Destination, transition.Trigger)
                    .ConfigureAwait(false);

                _logger.LogInformation(
                    "Переход: {Source} -> {Destination} через {Trigger}",
                    transition.Source,
                    transition.Destination,
                    transition.Trigger);
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Переход состояния вызвал исключение - {@Exception}", exception);
            }
        });

        _stateMachineAlreadyInitialized = true;
        return StateMachine;
    }

    /// <inheritdoc/>
    public async Task Destroy()
    {
        _logger.LogInformation(
            "Текущая машина состояний будет пересоздана в базовом состоянии - {NotStarted}", GameState.NotStarted);

        _stateMachineAlreadyInitialized = false;
        await StateMachine.DeactivateAsync().ConfigureAwait(false);
    }
}