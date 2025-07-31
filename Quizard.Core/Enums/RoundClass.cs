namespace Quizard.Core.Enums;

/// <summary> Класс механики раунда </summary>
public enum RoundClass
{
    /// <summary> Базовый раунд (выбор одного из N вариантов) </summary>
    Base,
    
    /// <summary> «Очередной» (игроки отвечают по порядку) </summary>
    Sequential,
    
    /// <summary> «Вопрос-расположение» (переставить элементы в правильном порядке) </summary>
    Ordering,
    
    /// <summary> «Скоростной» (учёт времени ответа, кто первый) </summary>
    Speed,
    
    /// <summary> «Ступеньки» (показываем подсказки по очереди) </summary>
    Steps,
    
    /// <summary> «Интерактивный» раунд </summary>
    Interactive,
    
    /// <summary> Вопрос-аукцион </summary>
    Auction
}