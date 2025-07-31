using Quizard.Core.Enums;

namespace Quizard.Core.Entities;

/// <summary> Сущность раунда игры </summary>
public class Round
{
    /// <summary> Уникальный идентификатор раунда </summary>
    public string RoundId { get; set; } = null!;
    
    /// <summary> Уникальный идентификатор сценария </summary>
    public string ScenarioId { get; set; } = null!;
    
    /// <summary> Уникальный идентификатор типа раунда </summary>
    public RoundTypeId RoundTypeId { get; set; }
    
    /// <summary> Ссылка на тип раунда </summary>
    public RoundType? RoundType { get; set; }
    
    /// <summary> Отображаемое имя </summary>
    public string? Name { get; set; }
    
    /// <summary> Отображаемое описание </summary>
    public string? Description { get; set; }
    
    /// <summary> Ссылка на превью </summary>
    public string? PreviewUrl { get; set; }
    
    /// <summary> Максимальная продолжительность раунда </summary>
    public int? RoundDuration { get; set; }
    
    /// <summary> Множитель правильного ответа </summary>
    public int CorrectMultiplier { get; set; }
    
    /// <summary> Множитель пропущенного ответа </summary>
    public int MissedMultiplier { get; set; }
    
    /// <summary> Множитель неправильного ответа </summary>
    public int IncorrectMultiplier { get; set; }

    /// <summary> Коллекция вопросов </summary>
    public ICollection<Question> Questions { get; set; } = [];
}