using Microsoft.Extensions.Logging;
using Quizard.Core.Constants;
using Quizard.Core.Enums;
using Quizard.Core.Exceptions;
using Quizard.Core.Interfaces;

namespace Quizard.Application.GameEngine.Connections;

/// <inheritdoc/>
public class ClientConnectionHandler : IClientConnectionHandler
{
    private readonly ILogger<ClientConnectionHandler> _logger;
    private readonly IGameSessionService _gameSessionService;
    private readonly IStateMachineProvider _stateMachineProvider;
    private readonly IGameLifecycleService _gameLifecycleService;
    private readonly IGameNotificationService _gameNotificationService;

    /// <summary> Конструктор обработчика </summary>
    /// <param name="logger"> Логгер </param>
    /// <param name="gameSessionService"> Хранилище текущей игровой сессии </param>
    /// <param name="stateMachineProvider"> Поставщика машины состояний </param>
    /// <param name="gameLifecycleService"> Сервис жизненного цикла игры </param>
    /// <param name="gameNotificationService"> Сервис SignalR‑уведомления </param>
    public ClientConnectionHandler(
        ILogger<ClientConnectionHandler> logger,
        IGameSessionService gameSessionService,
        IStateMachineProvider stateMachineProvider,
        IGameLifecycleService gameLifecycleService, 
        IGameNotificationService gameNotificationService)
    {
        _logger = logger;
        _gameSessionService = gameSessionService;
        _gameLifecycleService = gameLifecycleService;
        _gameNotificationService = gameNotificationService;
        _stateMachineProvider = stateMachineProvider;
        
        _logger.LogInformation("Обработчика подключений клиентов проинициализирован");
    }
    
    /// <inheritdoc/>
    public async Task HandlePlayerIdentifiedAsync(string connectionId, string nickname, string rackId)
    {
        _logger.LogDebug("Выполняется инициализация клиента-игрока");

        var session = _gameSessionService.GetSession(true);
        if (session.Players.Any(player => player.Nickname == nickname))
        {
            _logger.LogTrace("Сохраняем connectionId идентифицировавшего себя игрока");
            session.Connections[nickname] = connectionId;
            
            if (_stateMachineProvider.StateMachine.State == GameState.WaitingForPlayers)
            {
                await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.PlayerIdentified);
                
                _logger.LogTrace("Запрос 'подсветки' игрока, который готов к игре");
                _ = _gameNotificationService.InitialHighlightPlayerAsync(nickname, rackId).ConfigureAwait(false);

                if (session.Connections.Count == session.Players.Count)
                {
                    _logger.LogTrace("Все игроки подключились и идентифицировали себя");
                    await _gameNotificationService.HighlightPlayersAsync([]).ConfigureAwait(false);
                    await _stateMachineProvider.StateMachine
                        .FireAsync(GameTrigger.AllPlayersReady)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                _logger.LogTrace("Игрок {Nickname} возобновил подключение", nickname);
                await _gameNotificationService.HighlightPlayersAsync([]).ConfigureAwait(false);
            }

            await _gameSessionService.SaveChangesAsync().ConfigureAwait(false);

            if (_stateMachineProvider.StateMachine.State == GameState.Pause)
                await _gameLifecycleService.ResumeGameAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task HandleInformerIdentifiedAsync(string connectionId)
    {
        var session = _gameSessionService.GetSession(true);
        if (session.InformerConnectionId != null)
            throw new InformerAlreadyConnected();

        _logger.LogDebug("Выполняется инициализация клиента экрана информатора");

        session.InformerConnectionId = connectionId;
        await _gameSessionService.SaveChangesAsync().ConfigureAwait(false);


        if (_stateMachineProvider.StateMachine.State == GameState.Pause)
            await _gameLifecycleService.ResumeGameAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task HandleAdminIdentifiedAsync(string connectionId)
    {
        var session = _gameSessionService.GetSession(true);
        if (session.AdminConnectionId != null)
            throw new AdminAlreadyConnected();

        _logger.LogDebug("Выполняется инициализация администратора");

        session.AdminConnectionId = connectionId;
        await _gameSessionService.SaveChangesAsync().ConfigureAwait(false);

        if (_stateMachineProvider.StateMachine.State == GameState.Pause)
            await _gameLifecycleService.ResumeGameAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task HandleClientDisconnectedAsync(string connectionId, Exception exception)
    {
        _logger.LogWarning("Клиент отключился. Идентифицируем клиента. Исключение: {@Exception}", exception);
        var gameSession = _gameSessionService.GetSession(true);
        if (gameSession.AdminConnectionId == connectionId)
        {
            _logger.LogCritical("Админ отключился! Ожидаем переподключения. Игра будет приостановлена");
            await _gameLifecycleService.PauseGameAsync().ConfigureAwait(false);

            await _gameNotificationService
                .ClientDisconnected(ClientType.Admin, connectionId, exception)
                .ConfigureAwait(false);

            gameSession.AdminConnectionId = null;
            await _gameSessionService.SaveChangesAsync().ConfigureAwait(false);
        }
        else if (gameSession.InformerConnectionId == connectionId)
        {
            _logger.LogWarning("Экран информатор отключился! Ожидаем переподключения. Игра будет приостановлена");
            await _gameLifecycleService.PauseGameAsync().ConfigureAwait(false);
            await _gameNotificationService
                .ClientDisconnected(ClientType.Informer, connectionId, exception)
                .ConfigureAwait(false);

            gameSession.InformerConnectionId = null;
            await _gameSessionService.SaveChangesAsync().ConfigureAwait(false);
        }
        else if (gameSession.Connections.Any(connectionPair => connectionPair.Value == connectionId))
        {
            var player = gameSession.Connections.First(connectionPair => connectionPair.Value == connectionId);
            _logger.LogWarning("{Player} отключился!", player.Key);

            if (_stateMachineProvider.StateMachine.State != GameState.NotStarted)
            {
                _logger.LogInformation("Ожидаем переподключения. Игра будет приостановлена");
                await _gameLifecycleService.PauseGameAsync().ConfigureAwait(false);
            }

            await _gameNotificationService
                .ClientDisconnected(ClientType.Player, player.Key, exception)
                .ConfigureAwait(false);

            gameSession.Connections.TryRemove(player.Key, out _);
            await _gameSessionService.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            _logger.LogWarning("Не идентифицированный клиент отключился");
            await _gameNotificationService
                .ClientDisconnected(ClientType.Other, nameof(ClientType.Other), exception)
                .ConfigureAwait(false);
        }
    }
}