using Quizard.Core.Enums;

namespace Quizard.Core.Entities;

/// <summary> Сущность типа раунда </summary>
public class RoundType
{
    /// <summary> Уникальный идентификатор сущности </summary>
    public RoundTypeId RoundTypeId { get; set; }

    /// <summary> Человеко-читаемое название типа или механики </summary>
    public string Name { get; set; } = null!;

    /// <summary> Описание типа или механики для контент-менеджеров </summary>
    public string Description { get; set; } = null!;

    /// <summary> Класс механики (enum), определяющий логику начисления баллов и поведение </summary>
    public RoundClass RoundClass { get; set; }
    
    /// <summary> Требование ожидать таймаута раунда, даже если игроки уже закончили давать ответы </summary>
    public bool WaitingRoundTimeout { get; set; }
    
    /// <summary> Показывать ли текст/медиа вопроса на Информаторе </summary>
    public bool DisplayQuestionOnInformator { get; set; }
    
    /// <summary> Показывать ли текст/медиа вопроса игрокам </summary>
    public bool DisplayQuestionOnPlayers { get; set; }
}