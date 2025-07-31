using System.Collections.Concurrent;
using NUlid;
using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces;

/// <summary> Текущая игровая сессия — все параметры и аудит подключения/опросов </summary>
public interface IGameSession
{
    #region Инициализационные данные
    /// <summary> Уникальный идентификатор игровой сессии </summary>
    public Ulid GameId { get; }
    
    /// <summary> Текущий игровой сценарий </summary>
    public Scenario Scenario { get; }
    
    /// <summary> Текущее состояние остатков в магазине </summary>
    public List<Stock> ShopStocks { get; }
    
    /// <summary> Коллекция игроков </summary>
    public IReadOnlyList<Player> Players { get; }
    #endregion

    #region Подключения
    /// <summary> ConnectionIds игроков </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <listheader>
    /// <description> Расшифровка Key | Value: </description>
    /// </listheader>
    /// <item><description> Key - Никнейм пользователя </description></item>
    /// <item><description> Value - SignalR connectionId </description></item>
    /// </list>
    /// Связь игроков и connectionId: словарь playerId → connectionId. 
    /// С помощью данной коллекции определяется кол-во готовых игроков,
    /// тк игрок считается готовым после подключения к хабу
    /// </remarks>
    public ConcurrentDictionary<string, string> Connections { get; }
    
    /// <summary> ConnectionId экрана информатора </summary>
    public string? InformerConnectionId { get; set; }
    
    /// <summary> ConnectionId администратора </summary>
    public string? AdminConnectionId { get; set; }
    #endregion

    /// <summary> Текущее состояние игровой сессии </summary>
    /// <remarks>
    /// Резервное хранилище состояние игры.
    /// В логике игры используется состояние стейт машины
    /// </remarks>
    public GameState CurrentState { get; set; }
    
    /// <summary> Порядковый номер текущего этап игрового сценария </summary>
    public int CurrentStateIndex { get; set; }
    
    /// <summary> Раунд, который игроки выбрали на голосовании </summary>
    public Round? RoundChosenByPlayers { get; set; }
    
    /// <summary> Текущий игровой раунд </summary>
    public Round? CurrentRound { get; set; }
    
    /// <summary> Порядковый номер текущего вопроса </summary>
    public int CurrentQuestionIndex { get; set; }
    
    /// <summary> Коллекция игроков, которые уже показывали или отвечали на вопрос по очереди </summary>
    public ConcurrentBag<Player> PlayersHowAnsweredOrShowingQuestion { get; }
    
    /// <summary> Коллекция скоростных ответов игроков </summary>
    public ConcurrentDictionary<Player, long> PlayersSpeedAnswers { get; }
    
    /// <summary> Коллекция ответов игроков на интерактивный вопрос </summary>
    public ConcurrentDictionary<Player, bool> PlayersAnswersOnInteractiveQuestion { get; }
    
    /// <summary>
    /// Коллекция игроков, которые уже завершили текущую стадию (ответили на вопрос, проголосовали и тд)
    /// </summary>
    public ConcurrentBag<Player> PlayersWhoCompleteState { get; }
    
    /// <summary> Коллекция голосов игроков на голосовании за раунд </summary>
    public ConcurrentDictionary<Round, int> PlayersVoices { get; }

    /// <summary> Токен отмены, для контроля игровой сессии </summary>
    public CancellationTokenSource CancellationToken { get; }
    
    /// <summary> Метод для получения игрока </summary>
    /// <param name="connectionId"> SignalR идентификатор игрока </param>
    /// <returns> Сущность игрока <see cref="Player"/> </returns>
    /// <exception cref="PlayerDoesNotExistException"> Исключение, если игрок с таким Id не найден </exception>
    public Player GetPlayer(string connectionId);
    
    /// <summary> Метод для получения игрока </summary>
    /// <param name="nickname"> Никнейм игрока </param>
    /// <returns> Сущность игрока <see cref="Player"/> </returns>
    /// <exception cref="PlayerDoesNotExistException"> Исключение, если игрок с таким ником не найден </exception>
    public Player GetPlayerByNickname(string nickname);

    /// <summary>
    /// Метод для определения следующего случайного игрока, который будет отвечать или показывать вопрос
    /// </summary>
    /// <returns> Случайный игрок </returns>
    public Player GetNextRandomPlayer();

    /// <summary> Попытаться купить один предмет заданного типа </summary>
    /// <param name="modifier"> Тип модификатора, который хочет купить игрок </param>
    /// <exception cref="ProductIsOutOfStockException"> Исключение, если предмет закончился </exception>
    public Task PurchaseAsync(ModifierType modifier);
}