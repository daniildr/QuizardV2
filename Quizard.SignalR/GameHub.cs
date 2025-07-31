using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUlid;
using Quizard.Core.Constants;
using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Quizard.Core.Exceptions;
using Quizard.Core.Interfaces;
using Quizard.Core.Utils;
using Quizard.SignalR.Abstractions;

namespace Quizard.SignalR;

/// <summary> Главный хаб игрового движка </summary>
public class GameHub : BaseHub
{
    private readonly IGameManager _gameManager;
    private readonly IQuizardDbManager _dbManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly IGameLifecycleService _gameLifecycleService;
    private readonly IStateMachineProvider _stateMachineProvider;
    private readonly IClientConnectionHandler _clientConnectionHandler;
    private readonly IGameNotificationService _gameNotificationService;

    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    private IGameNotificationService GameNotificationService
        => _serviceProvider.GetRequiredService<IGameNotificationService>();

    /// <summary> Конструктор хаба </summary>
    /// <param name="logger"> Логгер </param>
    /// <param name="gameManager"> Менеджер игры </param>
    /// <param name="dbManager"> Менеджер БД </param>
    /// <param name="serviceProvider"> Сервис провайдер </param>
    /// <param name="gameLifecycleService"> Менеджер управления жизненным циклом игры</param>
    /// <param name="stateMachineProvider"> Поставщик машины состояний </param>
    /// <param name="clientConnectionHandler"> Обработчик подключений клиентов </param>
    /// <param name="gameNotificationService"> Сервис SignalR‑уведомления </param>
    public GameHub(
        ILogger<GameHub> logger,
        IGameManager gameManager,
        IQuizardDbManager dbManager,
        IServiceProvider serviceProvider,
        IGameLifecycleService gameLifecycleService,
        IStateMachineProvider stateMachineProvider,
        IClientConnectionHandler clientConnectionHandler,
        IGameNotificationService gameNotificationService) : base(logger)
    {
        _dbManager = dbManager;
        _gameManager = gameManager;
        _serviceProvider = serviceProvider;
        _gameLifecycleService = gameLifecycleService;
        _stateMachineProvider = stateMachineProvider;
        _clientConnectionHandler = clientConnectionHandler;
        _gameNotificationService = gameNotificationService;

        Logger.LogDebug("Игровой хаб проинициализирован");
    }

    #region Методы подключения/отключения и идентификации

    /// <summary> Переопределение метода подключения клиентов </summary>
    /// <seealso cref="Hub.OnConnectedAsync"/>
    /// <remarks>
    /// При подключении любого клиента ему будет отправлен список игроков,
    /// и только клиент-игрок имеет логику работы с этими данными
    /// </remarks>
    public override async Task OnConnectedAsync()
    {
        Logger.LogDebug("К системе подключился новый клиент");

        List<string> players = [];
        IGameSession? session = null;
        try
        {
            session = GameSessionService.GetSession();
            players.AddRange(session.Players.Select(player => player.Nickname));
        }
        catch (GameSessionDoesNotExistException)
        {
            Logger.LogWarning("Игровая сессия еще не запущена");
        }

        var connectionId = Context.ConnectionId;
        await Groups.AddToGroupAsync(connectionId, ClientGroup.All).ConfigureAwait(false);

        if (session is not null)
        {
            Logger.LogInformation("Отправляем подключившемуся клиенту игровой сценарий");
            await _gameNotificationService.SendGameScenario(Clients.Caller, session.Scenario);
        }

        if (players.Count != 0)
        {
            Logger.LogTrace("Новому клиенту будет отправлен список игроков");
            await _gameNotificationService.ReceivePlayerList(Clients.Caller, players.ToArray()).ConfigureAwait(false);
        }

        await base.OnConnectedAsync().ConfigureAwait(false);
    }

    /// <summary> Переопределение метода отключения клиентов </summary>
    /// <seealso cref="Hub.OnDisconnectedAsync"/>
    /// <remarks> При отключении клиента, он будет идентифицирован, и эти данные будут отправлены админу </remarks>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        Logger.LogWarning("Один из клиентов отключился");

        var connectionId = Context.ConnectionId;
        await _clientConnectionHandler.HandleClientDisconnectedAsync(connectionId, exception).ConfigureAwait(false);

        await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
    }

    /// <summary> Метод для идентификации клиента-игрока </summary>
    /// <param name="nickname"> Никнейм игрока </param>
    /// <param name="rackId"> Уникальный идентификатор игровой стойки </param>
    public async Task IdentifyPlayer(string nickname, string rackId)
    {
        Logger.LogInformation("Игрок идентифицировал себя");
        var session = GameSessionService.GetSession();
        var connectionId = Context.ConnectionId;

        await Groups.AddToGroupAsync(connectionId, ClientGroup.Player(nickname)).ConfigureAwait(false);
        await Groups.AddToGroupAsync(connectionId, ClientGroup.Players).ConfigureAwait(false);
        await _clientConnectionHandler
            .HandlePlayerIdentifiedAsync(connectionId, nickname, rackId)
            .ConfigureAwait(false);
        await _gameNotificationService.SendGameScenario(Clients.Caller, session.Scenario).ConfigureAwait(false);
    }

    /// <summary> Метод для идентификации клиента-информатора </summary>
    public async Task IdentifyInformer()
    {
        Logger.LogInformation("Экран информатор идентифицировал себя");
        var session = GameSessionService.GetSession();
        var connectionId = Context.ConnectionId;

        await Groups.AddToGroupAsync(Context.ConnectionId, ClientGroup.Informer).ConfigureAwait(false);
        await _clientConnectionHandler.HandleInformerIdentifiedAsync(connectionId).ConfigureAwait(false);
        await _gameNotificationService.IdentifyInformerNotice().ConfigureAwait(false);
        //await _gameNotificationService.SendGameScenario(Clients.Caller, session.Scenario).ConfigureAwait(false);
    }

    /// <summary> Метод для идентификации клиента-админ </summary>
    public async Task IdentifyAdmin()
    {
        Logger.LogInformation("Админ идентифицировал себя");

        var connectionId = Context.ConnectionId;
        await Groups.AddToGroupAsync(Context.ConnectionId, ClientGroup.Admin).ConfigureAwait(false);
        await _clientConnectionHandler.HandleAdminIdentifiedAsync(connectionId).ConfigureAwait(false);

        await _gameNotificationService.IdentifyAdminNotice().ConfigureAwait(false);
    }

    #endregion

    #region Методы контроля течения игры

    /// <summary> Метод для приостановки игровой сессии </summary>
    public async Task PauseGame() => await _gameLifecycleService.PauseGameAsync().ConfigureAwait(false);

    /// <summary> Метод для возобновления игровой сессии </summary>
    public async Task ResumeGame() => await _gameLifecycleService.ResumeGameAsync().ConfigureAwait(false);

    /// <summary> Метод для пропуска текущего этапа игровой сессии </summary>
    public async Task SkipStage() => await _gameLifecycleService.SkipStage().ConfigureAwait(false);

    /// <summary> Метод для принудительно остановки игровой сессии </summary>
    public async Task StopGame() => await _gameLifecycleService.ForceStopGameAsync().ConfigureAwait(false);

    #endregion

    #region Служебные системные методы

    /// <summary> Метод для получения текущего сценария </summary>
    public async Task GetCurrentScenario() =>
        await _gameNotificationService
            .SendGameScenario(Clients.Caller, GameSessionService.GetSession().Scenario)
            .ConfigureAwait(false);

    /// <summary> Метод для получения списка игроков текущей игровой сессии </summary>
    public async Task GetPlayerList() =>
        await _gameNotificationService
            .ReceivePlayerList(
                Clients.Caller, GameSessionService.GetSession().Players.Select(player => player.Nickname).ToArray())
            .ConfigureAwait(false);

    #endregion

    #region Методы для экрана информатора

    /// <summary> Метод для оповещения FSM о завершении показа медиа (вне раунда) </summary>
    public async Task MediaEnded()
    {
        Logger.LogInformation("Информатор уведомил о завершении показа меда файла (вне раунда)");
        await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.MediaEnded);
    }

    #endregion

    #region Методы для ответа на вопрос

    /// <summary> Метод для уведомления игрового движка о том, что игрок ответил на вопрос </summary>
    /// <param name="isCorrect"> Флаг правильного ответа </param>
    public async Task AnswerQuestion(bool isCorrect)
    {
        var connectionId = Context.ConnectionId;
        var session = GameSessionService.GetSession(true);
        var round = session.CurrentRound!;
        var player = session.GetPlayer(connectionId);

        Logger.LogInformation("Игрок {Nickname} дал ответ на вопрос", player.Nickname);
        session.PlayersWhoCompleteState.Add(player);

        if (round.RoundTypeId is RoundTypeId.Pantomime)
            session.PlayersAnswersOnInteractiveQuestion.TryAdd(player, isCorrect);

        await GameSessionService.SaveChangesAsync();

        Logger.LogTrace("Проверка готовности всех игроков");
        switch (round.RoundTypeId)
        {
            case RoundTypeId.HotPotato:
                await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.QuestionCompleted);
                break;

            case RoundTypeId.Pantomime:
            {
                if (session.PlayersWhoCompleteState.Count == (session.Players.Count - 1))
                {
                    if (round.RoundTypeId == RoundTypeId.Pantomime)
                    {
                        var responderPoints = InteractivePointsCounter.CalculateShowPlayerScore(Logger, session);
                        await _gameNotificationService.InteractiveQuestionResults(responderPoints);
                    }

                    await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.QuestionCompleted);
                }

                break;
            }

            default:
            {
                if (session.PlayersWhoCompleteState.Count == session.Players.Count)
                {
                    await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.QuestionCompleted);
                }

                break;
            }
        }
    }

    /// <summary> Метод для уведомления игрового движка о том, что игрок ответил на "скоростной" вопрос </summary>
    /// <param name="isCorrect"> Флаг правильного ответа </param>
    /// <param name="timestamp"> Метка времени ответа </param>
    public async Task FastestAnswer(bool isCorrect, long timestamp)
    {
        var connectionId = Context.ConnectionId;
        var session = GameSessionService.GetSession(true);
        var player = session.GetPlayer(connectionId);

        Logger.LogInformation("Игрок {Nickname} дал ответ на скоростной вопрос. Метка времени - {Timestamp}",
            player.Nickname, timestamp);
        session.PlayersWhoCompleteState.Add(player);
        if (isCorrect)
            session.PlayersSpeedAnswers.TryAdd(player, timestamp);

        await GameSessionService.SaveChangesAsync();

        Logger.LogTrace("Раунд требует ожидания таймаута - {WaitingRoundTimeout}",
            session.CurrentRound!.RoundType!.WaitingRoundTimeout);
        if (!session.CurrentRound!.RoundType!.WaitingRoundTimeout)
        {
            Logger.LogTrace("Проверка готовности всех игроков");
            if (session.PlayersWhoCompleteState.Count == session.Players.Count)
            {
                await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.QuestionCompleted);
            }
        }
    }

    /// <summary> Метод для уведомления игрового движка о том, что правильный вопрос показан </summary>
    public async Task RevealShowed()
    {
        Logger.LogInformation("Экран информатор показал правильный ответ");
        await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.RevealShowed);
    }

    /// <summary> Метод для отправки игровому движку статистики (результатов) раунда </summary>
    /// <param name="score"> Набранные пользователем очки </param>
    public async Task SendStatistics(int score)
    {
        var connectionId = Context.ConnectionId;
        var session = GameSessionService.GetSession(true);
        var player = session.GetPlayer(connectionId);

        Logger.LogInformation("Игрок {Id}|{Nickname} отправил статистику", connectionId, player.Nickname);
        session.PlayersWhoCompleteState.Add(player);
        await GameSessionService.SaveChangesAsync();

        await _dbManager.StatisticRepository.SubmitRoundStatistic(new RoundStatistic
        {
            GameId = session.GameId,
            PlayerId = (Ulid)player.Id!,
            RoundId = session.CurrentRound!.RoundId,
            Score = score
        }).ConfigureAwait(false);
        Logger.LogTrace("Статистика сохранена");

        if (session.PlayersWhoCompleteState.Count == session.Players.Count)
        {
            Logger.LogTrace("Все игроки отправили статистку");
            await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.StatsRequested).ConfigureAwait(false);
        }
    }

    /// <summary> Метод для отправки игровому движку статистики (результатов) сценария </summary>
    /// <param name="score"> Набранные пользователем очки </param>
    public async Task SendScenarioStatistics(int score)
    {
        var connectionId = Context.ConnectionId;
        var session = GameSessionService.GetSession(true);
        var player = session.GetPlayer(connectionId);

        Logger.LogInformation("Игрок {Id}|{Nickname} отправил статистику сценария", connectionId, player.Nickname);
        session.PlayersWhoCompleteState.Add(player);
        await GameSessionService.SaveChangesAsync();

        await _dbManager.StatisticRepository.SubmitScenarioStatistic(new ScenarioStatistic()
        {
            GameId = session.GameId,
            PlayerId = (Ulid)player.Id!,
            ScenarioId = session.Scenario.Id,
            Score = score
        }).ConfigureAwait(false);
        Logger.LogTrace("Статистика сохранена");

        if (session.PlayersWhoCompleteState.Count == session.Players.Count)
        {
            Logger.LogTrace("Все игроки отправили статистку");
            await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.StatsRequested).ConfigureAwait(false);
        }
    }

    /// <summary> Метод для уведомления игрового движка о том, что игрок закончил просмотр статистики </summary>
    public async Task StatsDisplayed()
    {
        var connectionId = Context.ConnectionId;
        var session = GameSessionService.GetSession(true);
        var player = session.GetPlayer(connectionId);

        Logger.LogInformation("Игрок {Id}|{Nickname} ознакомился со статистикой", connectionId, player.Nickname);
        session.PlayersWhoCompleteState.Add(player);
        await GameSessionService.SaveChangesAsync();

        if (session.PlayersWhoCompleteState.Count == session.Players.Count)
        {
            Logger.LogTrace("Все игроки ознакомился со статистикой");
            await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.StatsDisplayed).ConfigureAwait(false);
        }
    }

    #endregion

    /// <summary> Метод для уведомления игрового движка о том, что игрок сделал ставку аукциона </summary>
    public async Task PlayerMakeBet()
    {
        var connectionId = Context.ConnectionId;
        var session = GameSessionService.GetSession(true);
        var player = session.GetPlayer(connectionId);
        session.PlayersWhoCompleteState.Add(player);

        if (session.PlayersWhoCompleteState.Count == session.Players.Count)
        {
            Logger.LogTrace("Все игроки сделали ставки");
            await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.AuctionCompleted).ConfigureAwait(false);
        }
    }

    /// <summary> Метод для отправки голоса(ов) игрока в рамках стадии голосования </summary>
    /// <param name="roundId"> Id выбранного раунда </param>
    /// <param name="votes"> Количество голосов </param>
    public async Task SubmitPlayerVoteChoice(string roundId, int votes)
    {
        var connectionId = Context.ConnectionId;
        var session = GameSessionService.GetSession(true);
        var player = session.GetPlayer(connectionId);
        var chosenRound = session.Scenario.RoundDefinitions.First(round => round.RoundId == roundId);

        Logger.LogInformation("Игрок {Id}|{Nickname} проголосовал за {Round}", connectionId, player.Nickname, roundId);
        var roundAlreadySelected = session.PlayersVoices.Keys.Any(round => round.RoundId == chosenRound.RoundId);
        if (roundAlreadySelected)
        {
            Logger.LogDebug("Раунд уже находится в словаре голосов. Добавляем ные голоса");
            session.PlayersVoices[chosenRound] = +session.PlayersVoices[chosenRound] + votes;
        }
        else
        {
            Logger.LogDebug("Добавляем раунд в словарь голосов");
            session.PlayersVoices.TryAdd(chosenRound, votes);
        }

        session.PlayersWhoCompleteState.Add(player);

        await GameSessionService.SaveChangesAsync();

        if (session.PlayersWhoCompleteState.Count == session.Players.Count)
        {
            Logger.LogTrace("Все игроки окончили голосование");
            await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.VotingCompleted).ConfigureAwait(false);
        }
    }

    /// <summary> Метод для уведомления игрового движка о том, что игрок совершил покупку </summary>
    /// <param name="modifier"></param>
    public async Task PlayerMakePurchase(ModifierType modifier)
    {
        var connectionId = Context.ConnectionId;
        var session = GameSessionService.GetSession(true);
        var player = session.GetPlayer(connectionId);

        Logger.LogInformation(
            "Игрок {Id}|{Nickname} хочет купить {Modifiers}", connectionId, player.Nickname, modifier);

        var purchasedResult = false;
        try
        {
            Logger.LogTrace("Выполняем покупку предмета {@Modifier}", modifier);
            await session.PurchaseAsync(modifier).ConfigureAwait(false);
            await GameSessionService.SaveChangesAsync();
            session = GameSessionService.GetSession(true);

            purchasedResult = true;
        }
        catch (ProductIsOutOfStockException)
        {
            Logger.LogWarning("Предмет {@Modifier} закончился!", modifier);
            await Clients.Caller.SendAsync(HubMethods.ShopResults, purchasedResult).ConfigureAwait(false);
        }

        Logger.LogTrace("Оповещаем клиента о результатах покупки");
        await Clients.Caller.SendAsync(HubMethods.ShopResults, purchasedResult).ConfigureAwait(false);

        Logger.LogTrace("Оповещаем клиентов об обновлении остатков");
        await GameNotificationService.ShopUpdated(session.Scenario.ShopStocks.ToArray()).ConfigureAwait(false);
    }

    /// <summary> Метод для уведомления игрового движка о том, что игрок закончил фазу покупок </summary>
    public async Task PlayerHasCompletedPurchases()
    {
        var connectionId = Context.ConnectionId;
        var session = GameSessionService.GetSession(true);
        var player = session.GetPlayer(connectionId);

        Logger.LogInformation("Игрок {Id}|{Nickname} завершил покупки", connectionId, player.Nickname);
        session.PlayersWhoCompleteState.Add(player);
        await GameSessionService.SaveChangesAsync();

        Logger.LogDebug("Состояние игровой сессии - {@GameSession}", session);

        if (session.PlayersWhoCompleteState.Count == session.Players.Count)
        {
            Logger.LogTrace("Все игроки окончили закупку");
            await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.ShopEnded).ConfigureAwait(false);
        }
    }

    /// <summary> Метод для применения мины к конкретному игроку </summary>
    /// <param name="nickname"> Игрок цель </param>
    public async Task SubmitMineModifierTarget(string nickname) =>
        await _gameManager.HandleMineApplication(Context.ConnectionId!, nickname).ConfigureAwait(false);

    /// <summary> Метод для оповещения о взырыве мины </summary>
    public async Task MineExploded() =>
        await _gameManager.HandleMineExploded(Context.ConnectionId!).ConfigureAwait(false);

    /// <summary> Метод для применения зеркальной ловушки </summary>
    public async Task MirroredModifierProtected() =>
        await _gameManager.HandleMirroredModifierProtection(Context.ConnectionId!).ConfigureAwait(false);

    /// <summary> Метод для применения перемешивания кнопок к конкретному игроку </summary>
    /// <param name="nickname"> Игрок цель </param>
    public async Task SubmitShakerModifierUsed(string nickname) =>
        await _gameManager.HandleShakerModifierApplication(Context.ConnectionId!, nickname).ConfigureAwait(false);

    /// <summary> Метод для оповещения других участников о применении общих модификаторов </summary>
    /// <param name="modifier"> Примененный модификатор </param>
    public async Task SubmitNotTargetModifier(ModifierType modifier)
    {
        var connectionId = Context.ConnectionId;
        var session = GameSessionService.GetSession();
        var player = session.GetPlayer(connectionId);

        Logger.LogInformation("Игрок {Id}|{Nickname} применил {Modifier}", connectionId, player.Nickname, modifier);
        await GameNotificationService.ApplyNotTargetModifier(modifier);
    }


    /// <summary> Метод для уведомления игрового движка о том, что игрок завершил применение модификаторов </summary>
    public async Task PlayerHasCompletedApplicationTargetModifier()
    {
        var connectionId = Context.ConnectionId;
        var session = GameSessionService.GetSession(true);
        var player = session.GetPlayer(connectionId);

        Logger.LogDebug("Состояние коллекции готовых игроков (До) - {@P}", session.PlayersWhoCompleteState);

        Logger.LogInformation("Игрок {Id}|{Nickname} завершил применение модификаторов", connectionId, player.Nickname);
        session.PlayersWhoCompleteState.Add(player);
        await GameSessionService.SaveChangesAsync();

        Logger.LogDebug("Состояние коллекции готовых игроков (После Add) - {@P}", session.PlayersWhoCompleteState);

        if (session.PlayersWhoCompleteState.Count == session.Players.Count)
        {
            Logger.LogTrace("Все игроки окончили применение");
            if (_stateMachineProvider.StateMachine.CanFire(GameTrigger.ApplyTargetModifiersCompleted))
                await _stateMachineProvider.StateMachine
                    .FireAsync(GameTrigger.ApplyTargetModifiersCompleted)
                    .ConfigureAwait(false);
        }
    }
}