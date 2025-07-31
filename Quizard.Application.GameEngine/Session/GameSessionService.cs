using Microsoft.Extensions.Logging;
using NUlid;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;
using Quizard.Core.Interfaces;

namespace Quizard.Application.GameEngine.Session;

/// <inheritdoc />
public class GameSessionService : IGameSessionService
{
    private const string GameCacheKey = "ACTIVE_GAME";

    private readonly IGameSession? _currentSession;
    private bool _isModified;

    private readonly ILogger<GameSession> _gameSessionLogger;
    private readonly ILogger<GameSessionService> _logger;
    private readonly ICacheManager _cacheManager;

    /// <summary> Конструктор хранилища </summary>
    /// <param name="cacheManager"> Менеджер кеширования </param>
    /// <param name="logger"> Логгер </param>
    /// <param name="gameSessionLogger"> Логгер для нового экземпляра игровой сессии </param>
    public GameSessionService(
        ICacheManager cacheManager,
        ILogger<GameSessionService> logger,
        ILogger<GameSession> gameSessionLogger)
    {
        _logger = logger;
        _cacheManager = cacheManager;
        _gameSessionLogger = gameSessionLogger;

        _logger.LogDebug("Выполняется попытка получение текущей игровой сессии");
        var (result, session) = _cacheManager.TryGetValueAsync<GameSession>(GameCacheKey).Result;

        if (!result)
            _logger.LogWarning("Игровая сессия еще не создана");

        _currentSession = session;
        _isModified = false;

        _logger.LogDebug("Хранилище игровых сессий проинициализирован");
    }

    /// <inheritdoc/>
    public async Task<IGameSession> CreateSessionAsync(
        Ulid gameId, 
        Scenario scenario, 
        IReadOnlyList<Player> players,
        CancellationTokenSource cancellationTokenSource)
    {
        _logger.LogDebug("Создаем новый экземпляр игровой сессии");
        var newGameSession = new GameSession(gameId, scenario, players, _gameSessionLogger, cancellationTokenSource);

        _logger.LogInformation("Сохраняем игровую сессию в памяти");
        await _cacheManager.AddOrUpdateAsync(GameCacheKey, newGameSession);
        
        return newGameSession;
    }

    /// <inheritdoc />
    public IGameSession GetSession(bool forModification)
    {
        _logger.LogTrace(
            "Будет выполнен возврат полученной из кеша игровой сессии. " +
            "Флаг модификации - {Modification}", forModification);
        _isModified = forModification;
        return _currentSession ?? throw new GameSessionDoesNotExistException();
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync()
    {
        _logger.LogDebug("Выполняется сохранение изменений в текущей игровой сессии");
        if (_isModified && _currentSession is not null)
        {
            await _cacheManager.AddOrUpdateAsync(GameCacheKey, _currentSession);
            _isModified = false;
        }
    }

    /// <inheritdoc />
    public async Task ResetSession()
    {
        if (_currentSession is null) return;

        _logger.LogDebug("Выполняется сброс текущей игровой сессии");
        await _cacheManager.ClearAllCacheAsync(GameCacheKey);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _logger.LogTrace("Утилизация сервиса для работы с хранилищем игровой сессии");
        if (_isModified) SaveChangesAsync().Wait();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _logger.LogTrace("[Async] Утилизация сервиса для работы с хранилищем игровой сессии");
        if (_isModified) await SaveChangesAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}