using NUlid;
using Quizard.Core.Entities.Abstraction;

namespace Quizard.Core.Entities;

/// <summary> Сущность ответа на вопрос </summary>
public class Answer : BaseEntity
{
    /// <summary> Уникальный идентификатор вопроса </summary>
    public Ulid? QuestionId { get; set; }
    
    /// <summary> Текст ответа </summary>
    public string Text { get; set; } = null!;
    
    /// <summary> Кнопка, активирующая этот ответ </summary>
    public string Button { get; set; } = null!;

    /// <summary> Флаг указывающий, является ли ответ правильным (для раундов с выбором) </summary>
    public bool? IsCorrect { get; set; }
    
    /// <summary> Порядок ответа (для раундов с последовательностью) </summary>
    public int? Order { get; set; }
}