using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using NUlid;
using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Quizard.Core.Exceptions;
using Quizard.Core.Interfaces;

namespace Quizard.Application.GameEngine.Session;

/// <summary> Текущая игровая сессия </summary>
public class GameSession : IGameSession
{
    private readonly ILogger<GameSession> _logger;
    
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    #region Инициализационные данные
    /// <inheritdoc/>
    public Ulid GameId { get; }

    /// <inheritdoc/>
    public Scenario Scenario { get; }

    public List<Stock> ShopStocks { get; }

    /// <inheritdoc/>
    public IReadOnlyList<Player> Players { get; }
    #endregion

    #region Подключения
    /// <inheritdoc/>
    public ConcurrentDictionary<string, string> Connections { get; } = [];

    /// <inheritdoc/>
    public string? InformerConnectionId { get; set; }

    /// <inheritdoc/>
    public string? AdminConnectionId { get; set; }
    #endregion

    /// <inheritdoc/>
    public GameState CurrentState { get; set; }

    /// <inheritdoc/>
    public int CurrentStateIndex { get; set; }

    /// <inheritdoc/>
    public Round? RoundChosenByPlayers { get; set; } = null;
    
    /// <inheritdoc/>
    public Round? CurrentRound { get; set; } = null;

    /// <inheritdoc/>
    public int CurrentQuestionIndex { get; set; }
    
    /// <inheritdoc/>
    public ConcurrentBag<Player> PlayersHowAnsweredOrShowingQuestion { get; } = [];

    /// <inheritdoc/>
    public ConcurrentDictionary<Player, long> PlayersSpeedAnswers { get; } = [];

    /// <inheritdoc/>
    public ConcurrentDictionary<Player, bool> PlayersAnswersOnInteractiveQuestion { get; } = [];

    /// <inheritdoc/>
    public ConcurrentBag<Player> PlayersWhoCompleteState { get; } = [];

    /// <inheritdoc/>
    public ConcurrentDictionary<Round, int> PlayersVoices { get; } = [];

    /// <inheritdoc/>
    public CancellationTokenSource CancellationToken { get; }

    /// <summary> Конструктор игровой сессии </summary>
    /// <param name="gameId"> Внутренний идентификатор игровой сессии </param>
    /// <param name="scenario"> Сценарий конкретной игры </param>
    /// <param name="players"> Коллекция игроков </param>
    /// <param name="cancellationToken"> Токен отмены </param>
    /// <param name="logger"> Логгер </param>
    /// <exception cref="ScenarioValidationException"> Если сценарий не содержит 'Первый'</exception>
    public GameSession(
        Ulid gameId,
        Scenario scenario,
        IReadOnlyList<Player> players,
        ILogger<GameSession> logger,
        CancellationTokenSource cancellationToken)
    {
        _logger = logger;
        
        _logger.LogTrace("Установка основных данных сценария");
        GameId = gameId;
        Scenario = scenario;
        ShopStocks = scenario.ShopStocks.ToList();
        Players = players;
        
        _logger.LogTrace("Установка первичного состояния сценария");
        CurrentState = GameState.NotStarted;
        CurrentStateIndex = 0;
        CurrentQuestionIndex = 0;
        
        CancellationToken = cancellationToken;
    }

    /// <inheritdoc/>
    public Player GetPlayer(string connectionId)
    {
        _logger.LogTrace("Выполняется получение игрока с помощью его ConnectionId - {Id}", connectionId);
        var nickname = Connections.FirstOrDefault(pair => pair.Value == connectionId).Key 
                       ?? throw new PlayerDoesNotExistException(connectionId);
        
        return Players.FirstOrDefault(p => p.Nickname == nickname)
               ?? throw new PlayerDoesNotExistException(connectionId);
    }

    /// <inheritdoc/>
    public Player GetPlayerByNickname(string nickname)
    {
        _logger.LogTrace("Выполняется получение игрока с помощью его никнейма - {Nickname}", nickname);
        var internalNickname = Connections.FirstOrDefault(pair => pair.Key == nickname).Key 
                       ?? throw new PlayerDoesNotExistException(nickname);
        
        return Players.FirstOrDefault(p => p.Nickname == internalNickname)
               ?? throw new PlayerDoesNotExistException(internalNickname);
    }

    /// <inheritdoc/>
    public Player GetNextRandomPlayer()
    {
        _logger.LogDebug("Выполняем получение случайного игрока для вопроса");
        if (PlayersHowAnsweredOrShowingQuestion.Count == Players.Count)
        {
            _logger.LogTrace("Сбрасываем коллекцию игроков, которые уже показывали/отвечали");
            PlayersHowAnsweredOrShowingQuestion.Clear();
        }
        
        _logger.LogTrace("Получаем список игроков, которые еще не участвовали");
        var availablePlayers = Players
            .Where(player => !PlayersHowAnsweredOrShowingQuestion.Contains(player))
            .ToList();

        _logger.LogTrace("Выбираем случайного игрока");
        var selectedPlayer = availablePlayers[new Random().Next(availablePlayers.Count)];

        _logger.LogTrace("Обновляем коллекцию игроков, которые уже показывали/отвечали");
        PlayersHowAnsweredOrShowingQuestion.Add(selectedPlayer);
        
        _logger.LogDebug("Выбран игрок {Player}", selectedPlayer.Nickname);
        return selectedPlayer;
    }
    
    /// <inheritdoc/>
    public async Task PurchaseAsync(ModifierType modifier)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            _logger.LogTrace("Находим запись остатка по типу модификатора");
            var stock = ShopStocks.FirstOrDefault(stock => stock.ModifierType == modifier);
                
            _logger.LogTrace("Если нет записи или количество 0 — выбрасываем исключение");
            if (stock is not { Quantity: > 0 })
                throw new ProductIsOutOfStockException(modifier);

            _logger.LogTrace("Уменьшаем количество на 1");
            stock.Quantity--;
            
            // if (stock.Quantity == 0)
            // {
            //     _logger.LogTrace("Предмет закончился, полностью убираем из списка");
            //     ShopStocks.Remove(stock);
            // }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}