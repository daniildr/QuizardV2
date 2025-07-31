using Microsoft.AspNetCore.SignalR;
using Quizard.Core.Constants;
using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Quizard.Core.Models.Responses;

namespace Quizard.Core.Interfaces;

/// <summary> Интерфейс SignalR‑уведомления </summary>
public interface IGameNotificationService
{
    /// <summary> Оповещает об изменении состояния игровой сессии </summary>
    /// <param name="state"> Текущее (новое) состояние игровой сессии</param>
    /// <param name="trigger"> Триггер обновления состояния </param>
    /// <returns> Асинхронная операция </returns>
    public Task GameStateChanged(GameState state, GameTrigger trigger);
    
    /// <summary> Оповещает о приостановке игры </summary>
    /// <returns> Асинхронная операция </returns>
    public Task GamePaused();
    
    /// <summary> Метод уведомляет экран информатор о том, что надо пропустить текущий этап </summary>
    /// <returns> Асинхронная операция </returns>
    public Task Skip();
    
    /// <summary> Оповещает о возобновлении игры </summary>
    /// <returns> Асинхронная операция </returns>
    public Task GameResumed();
    
    /// <summary> Оповещает о принудительном завершении игры </summary>
    /// <returns> Асинхронная операция </returns>
    public Task ForceDisconnect();
    
    /// <summary> Оповещает об отключении определенного клиента </summary>
    /// <param name="clientType"> Тип клиента </param>
    /// <param name="clientIdentifier"> Какой-либо идентификатор клиента </param>
    /// <param name="exception"> Исключение </param>
    /// <returns> Асинхронная операция </returns>
    public Task ClientDisconnected(ClientType clientType, string? clientIdentifier, Exception? exception);

    /// <summary> Отправляет список игроков вызывающему клиенту </summary>
    /// <param name="caller"> Вызывающий клиент </param>
    /// <param name="players"> Массив игроков </param>
    /// <returns> Асинхронная операция </returns>
    public Task ReceivePlayerList(IClientProxy caller, string[] players);

    /// <summary> Отправляет игровой сценарий всем подключенным клиентам </summary>
    /// <param name="scenario"> Игровой сценарий </param>
    /// <returns> Асинхронная операция </returns>
    public Task SendGameScenario(Scenario scenario);

    /// <summary> Отправляет игровой сценарий идентифицировавшему себя игрока </summary>
    /// <param name="caller"> Абстракция вызывающего абонента для концентратора </param>
    /// <param name="scenario"> Игровой сценарий </param>
    /// <returns> Асинхронная операция </returns>
    public Task SendGameScenario(IClientProxy caller, Scenario scenario);
    
    /// <summary> Подсвечивает выбранных пользователей </summary>
    /// <param name="highlightedNicknames"> Коллекция никнеймов пользователей, требующих подсветки </param>
    /// <returns> Асинхронная операция </returns>
    public Task HighlightPlayersAsync(IEnumerable<string> highlightedNicknames);

    /// <summary> Подсвечивает пользователя при его инициализации </summary>
    /// <remarks> Передает идентификатор стойки игрока, для его инициализации </remarks>
    /// <param name="highlightedNickname"> Коллекция никнеймов пользователей, требующих подсветки </param>
    /// <param name="rackId"> Идентификатор стойки </param>
    /// <returns> Асинхронная операция </returns>
    public Task InitialHighlightPlayerAsync(string highlightedNickname, string rackId);

    /// <summary> Уведомляет всех клиентов, что экран информатор подключился </summary>
    /// <returns> Асинхронная операция </returns>
    public Task IdentifyInformerNotice();

    /// <summary> Уведомляет всех клиентов, что админ подключился </summary>
    /// <returns> Асинхронная операция </returns>
    public Task IdentifyAdminNotice();

    /// <summary> Уведомляет информатор о том, что необходимо показать медиа-файл </summary>
    /// <param name="media"> Медиа файл </param>
    /// <returns> Асинхронная операция </returns>
    public Task MediaStarted(Media media);

    /// <summary> Оповещает о начале нового раунда </summary>
    /// <param name="round"> Раунда </param>
    /// <returns> Асинхронная операция </returns>
    public Task RoundStarted(Round round);
    
    /// <summary> Оповещаем игроков о старте вопроса </summary>
    /// <param name="question"> Вопрос </param>
    /// <returns> Асинхронная операция </returns>
    public Task QuestionStarted(Question question);
    
    /// <summary> Оповещаем игроков о старте вопроса с "отвечающим/показывающим" игроком </summary>
    /// <param name="player"> Случайный игрок выбранный бекендом </param>
    /// <param name="question"> Вопрос </param>
    /// <returns> Асинхронная операция </returns>
    public Task QuestionStarted(Player player, Question question);
    
    /// <summary> Метод, оповещающий о победители в скоростном раунде </summary>
    /// <param name="player"> Игрок победитель </param>
    /// <returns> Асинхронная операция </returns>
    public Task SpeedWinner(Player player);
    
    /// <summary> Метод, оповещающий о том, сколько баллов набрал игрок в интерактивном раунде </summary>
    /// <param name="points"> Набранные им очки </param>
    /// <returns> Асинхронная операция </returns>
    public Task InteractiveQuestionResults(int points);
    
    /// <summary> Оповещает информатор о необходимости показать правильный ответ </summary>
    /// <param name="reveal"> Правильный ответ </param>
    /// <returns> Асинхронная операция </returns>
    public Task ShowReveal(Reveal reveal);
    
    /// <summary> Оповещает информатор о необходимости показать статистику </summary>
    /// <param name="statistic"> Статистика прошедшего раунда </param>
    /// <returns> Асинхронная операция </returns>
    public Task ShowRoundStatistic(RoundStatistic[] statistic);
    
    /// <summary> Оповещает информатор о необходимости показать статистику сценария </summary>
    /// <param name="statistic"> Статистика сценария </param>
    /// <returns> Асинхронная операция </returns>
    public Task ShowScenarioStatistic(ScenarioStatistic[] statistic);
    
    /// <summary> Оповещает о старте голосования </summary>
    /// <param name="roundsForVoting"> Раунды для голосования </param>
    /// <returns> Асинхронная операция </returns>
    public Task VotingStarted(Round[] roundsForVoting);

    /// <summary> Оповещает о старте этапа магазина </summary>
    /// <param name="purchasePhaseConfiguration"> Конфигурация магазина </param>
    /// <returns> Асинхронная операция </returns>
    public Task ShopStarted(PurchasePhaseConfiguration purchasePhaseConfiguration);

    /// <summary> Оповещает игроков об изменении стоков магазина </summary>
    /// <param name="shopStocks"> Остатки на складе </param>
    /// <returns> Асинхронная операция </returns>
    public Task ShopUpdated(Stock[] shopStocks);
    
    /// <summary> Оповещает о том, что предмет был куплен </summary>
    /// <param name="modifier"> Модификатор, который закончился </param>
    /// <returns> Асинхронная операция </returns>
    public Task ProductIsOutOfStock(ModifierType modifier);
    
    /// <summary> Оповещает о том, что был применен общий модификатор </summary>
    /// <param name="modifier"> Модификатор </param>
    /// <returns> Асинхронная операция </returns>
    public Task ApplyNotTargetModifier(ModifierType modifier);
    
    

    /// <summary> Оповещает игроков о том, что была применена мина </summary>
    /// <param name="nickname"> Заминированный игрок </param>
    /// <returns> Асинхронная операция </returns>
    public Task MineApplied(string nickname);

    /// <summary> Уведомляет всех о том, что мина взорвалась в руках у конкретного игрока </summary>
    /// <param name="nickname"> Игрок, у которого взорвалась мина </param>
    /// <param name="minedPlayer"> Оставшиеся заминированные игроки</param>
    /// <returns> Асинхронная операция </returns>
    public Task MineExploded(string nickname, string[] minedPlayer);
    
    /// <summary> Оповещает игроков о том, что было применено перемешивание кнопок для конкретного игрока </summary>
    /// <param name="nickname"> Перемешанный игрок </param>
    /// <returns> Асинхронная операция </returns>
    public Task ShakerApplied(string nickname);

    /// <summary> Оповещает игроков о том, что раунд будет перевернут </summary>
    /// <returns> Асинхронная операция </returns>
    public Task MirroredRound();
    
    
}