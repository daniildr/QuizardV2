using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quizard.Core.Interfaces;

namespace Quizard.Application.GameEngine;

/// <inheritdoc />
public class GameManager : IGameManager
{
    private readonly IQuizardDbManager _dbManager;
    private readonly ILogger<GameManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IStateMachineProvider _stateMachineProvider;
    private readonly IGameNotificationService _gameNotificationService;

    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    /// <summary> Конструктор менеджера </summary>
    /// <param name="logger"> Логгер </param>
    /// <param name="dbManager"> Менеджер БД </param>
    /// <param name="serviceProvider"> Сервис провайдер </param>
    /// <param name="stateMachineProvider"> Поставщик машины состояний </param>
    /// <param name="gameNotificationService"> Сервис SignalR уведомлений </param>
    public GameManager(
        ILogger<GameManager> logger, 
        IQuizardDbManager dbManager, 
        IServiceProvider serviceProvider, 
        IStateMachineProvider stateMachineProvider,
        IGameNotificationService gameNotificationService)
    {
        _dbManager = dbManager;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _gameNotificationService = gameNotificationService;
        _stateMachineProvider = stateMachineProvider;
        
        _logger.LogInformation("Менеджер игры проинициализирован");
    }

    /// <inheritdoc />
    public async Task HandleMineApplication(string connectionId, string nickname)
    {
        _logger.LogInformation("Игрок {Id} уведомил о минирование игрока {Nickname}", connectionId, nickname);
        throw new NotImplementedException();
        
        // var session = GameSessionService.GetSession(true);
        // var player = session.GetPlayer(connectionId);
        // var minedPlayer = session.GetPlayerByNickname(nickname);
        // session.MinedPlayers.Remove(player);
        // session.MinedPlayers.Add(minedPlayer);
        //
        // if (session.MinedPlayers.Count > 0)
        //     await _gameNotificationService.MineApplied(minedPlayer.Nickname).ConfigureAwait(false);
        //
        // await GameSessionService.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task HandleMineExploded(string connectionId)
    {
        _logger.LogInformation("Игрок {Id} уведомил о взрыве мины", connectionId);
        throw new NotImplementedException();
        
        // var session = GameSessionService.GetSession(true);
        // var player = session.GetPlayer(connectionId);
        // session.MinedPlayers.Remove(player);
        //
        // await _gameNotificationService
        //     .MineExploded(
        //         player.Nickname, session.MinedPlayers.Select(minedPlayer => minedPlayer.Nickname)
        //             .ToArray())
        //     .ConfigureAwait(false);
        //
        // await GameSessionService.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task HandleMirroredModifierProtection(string connectionId)
    {
        _logger.LogInformation("Игрок {Id} уведомил о применении зеркальной ловушки", connectionId);
        var session = GameSessionService.GetSession(true);

        await _gameNotificationService.MirroredRound().ConfigureAwait(false);
        await GameSessionService.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task HandleShakerModifierApplication(string connectionId, string nickname)
    {
        _logger.LogInformation("Игрок {Id} уведомил о перемешивание кнопок {Nickname}", connectionId, nickname);
        var session = GameSessionService.GetSession(true);
        var shakingPlayer = session.GetPlayerByNickname(nickname);
        
        await _gameNotificationService.ShakerApplied(shakingPlayer.Nickname).ConfigureAwait(false);
        await GameSessionService.SaveChangesAsync().ConfigureAwait(false);
    }
}