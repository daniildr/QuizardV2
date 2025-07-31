using NUlid;
using Quizard.Core.Entities.Abstraction;

namespace Quizard.Core.Entities;

/// <summary> Сущность подсказки </summary>
public class Hint : BaseEntity
{
    /// <summary> Уникальный идентификатор вопроса </summary>
    public Ulid? QuestionId { get; set; }
    
    /// <summary> Текст подсказки </summary>
    public string Text { get; set; } = null!;
}