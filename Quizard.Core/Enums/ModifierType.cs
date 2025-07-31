namespace Quizard.Core.Enums;

/// <summary> Перечисление типов модификаторов </summary>
public enum ModifierType
{
    /// <summary> Перемешивание кнопок </summary>
    ShuffleButtons,
    
    /// <summary> Дополнительные голоса </summary>
    ExtraVotes,
    
    /// <summary> Мина </summary>
    Mine,
    
    /// <summary> Золотой раунд </summary>
    GoldRound,
    
    /// <summary> Право на ошибку </summary>
    MistakePass,
    
    /// <summary> Зеркальная ловушка </summary>
    MirrorTrap,
    
    /// <summary> Уменьшение размера текста </summary>
    ReduceTextSize,
    
    /// <summary> Яркий интерфейс </summary>
    BrightInterface
}