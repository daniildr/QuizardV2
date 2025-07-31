using Quizard.Core.Entities.Abstraction;

namespace Quizard.Core.Entities;

/// <summary> Сущность вопроса игры </summary>
public class Question : BaseEntity
{
    /// <summary> Уникальный идентификатор раунда </summary>
    public string RoundId { get; set; } = null!;
    
    /// <summary> Порядковый номер вопроса </summary>
    public int QuestionNumber { get; set; }
    
    /// <summary> Текст вопроса </summary>
    public string QuestionText { get; set; } = null!;
    
    /// <summary> Уникальный идентификатор медиа-файла </summary>
    public string? MediaId { get; set; }
    
    /// <summary> Ссылка на медиа-файл </summary>
    public Media? Media { get; set; }
    
    /// <summary> Задержка показа вариантов ответа </summary>
    public int? AnswerDelay { get; set; }
    
    /// <summary> Максимальное время на ответ на данный вопрос </summary>
    public int QuestionTimeout { get; set; }

    /// <summary> Массив ответов (вариантов ответов) на вопрос </summary>
    public ICollection<Answer> Answers { get; set; } = [];
    
    /// <summary> Ссылка на правильный ответ (или что показывать после данного ответа) </summary>
    public Reveal Reveal { get; set; } = null!;

    /// <summary> Массив подсказок </summary>
    public ICollection<Hint> Hints { get; set; } = [];
}