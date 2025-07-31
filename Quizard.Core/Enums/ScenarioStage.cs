namespace Quizard.Core.Enums;

/// <summary> Перечисление этапов сценария </summary>
public enum ScenarioStage
{
    /// <summary> Пауза </summary>
    Pause,
    
    /// <summary> Показ медиа </summary>
    Media,
    
    /// <summary> Игровой раунд </summary>
    Round,
    
    /// <summary> Голосование за следующий раунд </summary>
    Vote, 
    
    /// <summary> Этап магазина </summary>
    Shop, 
    
    /// <summary> Этап завершения игры </summary>
    Finish,
}