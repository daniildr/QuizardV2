namespace Quizard.Core.Constants;

/// <summary> Константы имен методов клиентов </summary>
public static class HubMethods
{
    /// <summary> Метод подписки на логирование </summary>
    public const string Log = "Log";
    
    /// <summary> Метод пропуска какой-либо стадии работы экрана информатора </summary>
    public const string Skip = "Skip";
    
    /// <summary> Игра приостановлена </summary>
    public const string GamePaused = "GamePaused";
    
    /// <summary> Игра возобновлена </summary>
    public const string GameResumed = "GameResumed";
    
    /// <summary> Игра завершена. Можно отключаться </summary>
    public const string ForceDisconnect = "ForceDisconnect";
    
    /// <summary> Состояние игровой сессии изменено </summary>
    public const string GameStateChanged = "GameStateChanged";
    
    /// <summary> Необходимо подсветить отключившегося игрока </summary>
    public const string PlayerHasDisconnected = "PlayerHasDisconnected";
    
    /// <summary> Клиент отключился </summary>
    public const string ClientDisconnected = "ClientDisconnected";
    
    /// <summary> Отправлен список игроков </summary>
    public const string ReceivePlayerList = "ReceivePlayerList";
    
    /// <summary> Отправлен текущий сценарий </summary>
    public const string ReceiveGameScenario = "ReceiveGameScenario";
    
    /// <summary> Требование показать медиа файл (вне раунда) </summary>
    public const string ShowMedia = "ShowMedia";
    
    /// <summary> Раунд начался </summary>
    public const string RoundStarted = "RoundStarted";
    
    /// <summary> Начался вопрос </summary>
    public const string QuestionStarted = "QuestionStarted";
    
    /// <summary> Начался вопрос для определенного игрока </summary>
    public const string TargetQuestionStarted = "TargetQuestionStarted";
    
    /// <summary> Победитель скоростного раунда </summary>
    public const string SpeedQuestionWinner = "SpeedQuestionWinner";
    
    /// <summary> Количество баллов, которые набрал игрок в рамках интерактивного раунда </summary>
    public const string InteractiveQuestionResults = "InteractiveQuestionResults";
    
    /// <summary> Необходимо показать правильный ответ </summary>
    public const string ShowReveal = "ShowReveal";
    
    /// <summary> Требование показать статистику  </summary>
    public const string ShowStatistics = "ShowStatistics";
    
    /// <summary> Требование показать статистику </summary>
    public const string ShowScenarioStatistics = "ShowScenarioStatistics";
    
    /// <summary> Отправлен список раундов для голосования </summary>
    public const string VotingStarted = "VotingStarted";
    
    /// <summary> Оповещает о начале стадии магазина, рассылает конфигурацию магазина </summary>
    public const string ShopStarted = "ShopStarted";
    
    /// <summary> Оповещает клиентов об обновлении остатков магазина </summary>
    public const string StockUpdated = "StockUpdated";
    
    /// <summary> Оповещает, что определенный товар в магазине закончился </summary>
    public const string ProductIsOutOfStock = "ProductIsOutOfStock";
    
    /// <summary> Оповещает игрока о результате его покупок </summary>
    public const string ShopResults = "ShopResults";
    
    /// <summary> Оповещает о применение модификатора </summary>
    public const string ModifierApplied = "ModifierApplied";
    
    
    
    /// <summary> Оповещает всех клиентов, о применении мины </summary>
    public const string MineApplied = "MineApplied";

    /// <summary> Оповещает всех клиентов, о взрыве мины </summary>
    public const string MineExploded = "MineExploded";
    
    /// <summary> Оповещает всех клиентов, о взрыве мины </summary>
    public const string ShakerApplied = "ShakerApplied";

    /// <summary> Оповещает всех клиентов о том, что раунд буе перевернут </summary>
    public const string MirroredRound = "MirroredRound";
    

    /// <summary> Клиент информатор идентифицирован </summary>
    public const string InformerIdentified = "InformerIdentified";

    /// <summary> Клиент администратор идентифицирован </summary>
    public const string AdminIdentified = "AdminIdentified";
    
    /// <summary> Необходимо подсветить игрока </summary>
    public const string UpdatePlayerHighlighting = "UpdatePlayerHighlighting";
    
    /// <summary> Необходимо подсветить игрока </summary>
    public const string InitPlayerHighlighting = "InitPlayerHighlighting";
}