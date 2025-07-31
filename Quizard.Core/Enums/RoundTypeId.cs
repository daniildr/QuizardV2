namespace Quizard.Core.Enums;

/// <summary> Уникальный идентификатор типа раунда (primary key в БД) </summary>
public enum RoundTypeId
{
    /// <summary> Разминка </summary>
    Warmup,
    
    /// <summary> Верю не Верю </summary>
    TrueFalse,
    
    /// <summary> Раунд Блиц </summary>
    Blitz,
    
    /// <summary> Горячая картошка </summary>
    HotPotato,
    
    /// <summary> Пантомима </summary>
    Pantomime,
    
    /// <summary> Угадай мелодию </summary>
    GuessMelody,
    
    /// <summary> Ступеньки </summary>
    Steps,
    
    /// <summary> Вопрос Расположение </summary>
    Ordering,
    
    /// <summary> Вопрос Аукцион </summary>
    Auction
}