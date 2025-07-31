namespace Quizard.Core.Enums;

/// <summary> Перечень триггеров Game State Machine </summary>
public enum GameTrigger
{
    /// <summary> Триггер пропуска текущего этапа игровой сессии </summary>
    Skip,
    
    /// <summary> Триггер запуска сессии </summary>
    StartRequested,
    
    /// <summary> Триггер завершения игры </summary>
    EndRequested,
    
    /// <summary> Триггер идентификации игрока </summary>
    PlayerIdentified,
    
    /// <summary> Триггер - все игроки готовы </summary>
    AllPlayersReady,
    
    /// <summary> Триггер - завершен показ медиа </summary>
    MediaEnded,
    
    /// <summary> Триггер запуска раунда - переход к обработке вопросов </summary>
    RoundStarted,
    
    /// <summary> Триггер запуска аукциона </summary>
    AuctionStarted,
    
    /// <summary> Триггер завершения аукциона </summary>
    AuctionCompleted,
    
    /// <summary> Триггер окончания раунда по таймауту </summary>
    RoundTimeout,
    
    /// <summary> Триггер ответа на вопрос - все игроки ответили на вопрос </summary>
    QuestionCompleted,
    
    /// <summary> Правильный ответ на вопрос показан </summary>
    RevealShowed,
    
    /// <summary> Триггер получения статистики от всех игроков </summary>
    StatsRequested,
    
    /// <summary> Триггер завершения фазы показа статистики </summary>
    StatsDisplayed,
    
    /// <summary> Триггер завершения фазы голосования </summary>
    VotingCompleted,
    
    /// <summary> Триггер завершения фазы покупки </summary>
    ShopEnded,
    
    /// <summary> Триггер тайм-аута этапа магазина (закупки) </summary>
    ShopTimeout,
    
    /// <summary> Триггер завершения фазы применения целевых приобретенных модификаторов </summary>
    ApplyTargetModifiersCompleted,
    
    /// <summary> Триггер требования приостановить игру </summary>
    PauseRequested,
    
    /// <summary> Триггер требования продолжить игру </summary>
    ResumeRequested
}