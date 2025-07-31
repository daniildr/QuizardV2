namespace Quizard.Core.Enums;

/// <summary> Перечень состояний Game State Machine </summary>
public enum GameState
{
    /// <summary> Игровая сессия не была запущена </summary>
    NotStarted,
    
    /// <summary> Ожидание подключения и готовности игроков </summary>
    WaitingForPlayers,
    
    /// <summary> Игрокам показывается медиа (вне раунда) </summary>
    Media,

    /// <summary> Игровой раунд </summary>
    RoundPlaying,
    
    /// <summary> Разыгрывается вопрос </summary>
    QuestionPlaying,
    
    /// <summary> Игроки делают ставки перед раундом </summary>
    Auction,
    
    /// <summary> Показывается правильный ответ </summary>
    RevealShowing,
    
    /// <summary> Ожидаем статистику пользователей </summary>
    WaitStats,

    /// <summary> Отображение промежуточной статистики </summary>
    ShowingStats,

    /// <summary> Голосование за следующий раунд (кроме первого) </summary>
    Voting,
    
    /// <summary> Фаза покупки модификаторов </summary>
    Shop,
    
    /// <summary> Фаза применения целевых модификаторов </summary>
    ApplyingTargetModifiers,

    /// <summary> Завершение игры и финальная статистика </summary>
    Finished,

    /// <summary> Отображение статистики сценария </summary>
    ShowingScenarioStats,
    
    /// <summary> Игровая сессия на паузе </summary>
    Pause
}