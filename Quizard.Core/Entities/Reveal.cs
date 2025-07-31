using NUlid;
using Quizard.Core.Entities.Abstraction;

namespace Quizard.Core.Entities;

/// <summary> Сущность раскрытого вопроса - что показывать игрокам после ответа на вопрос </summary>
public class Reveal : BaseEntity
{
    /// <summary> Уникальный идентификатор вопроса </summary>
    public Ulid? QuestionId { get; set; }
    
    /// <summary> Уникальный идентификатор медиа файла </summary>
    public string? MediaId { get; set; }
    
    /// <summary> Ссылка на медиа файл </summary>
    public Media? Media { get; set; }
    
    /// <summary> Текст, показанный пользователям </summary>
    public string Text { get; set; } = null!;
    
    /// <summary> Длительность показа </summary>
    public int Duration { get; set; }
}