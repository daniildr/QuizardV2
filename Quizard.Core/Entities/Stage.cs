using Quizard.Core.Entities.Abstraction;
using Quizard.Core.Enums;

namespace Quizard.Core.Entities;

/// <summary> Сущность этапа сценария </summary>
public class Stage : BaseEntity
{
    /// <summary> Уникальный идентификатор сценария </summary>
    public string ScenarioId { get; set; } = null!;
    
    /// <summary> Порядковый номер этапа </summary>
    public int Index { get; set; }
    
    /// <summary> Тип стадия - медиа, пауза, раунд, и тд </summary>
    public ScenarioStage Type { get; set; }
    
    /// <summary> Уникальный идентификатор медиа </summary>
    public string? MediaId { get; set; }
    
    /// <summary> Ссылка на медиа файл </summary>
    public Media? Media { get; set; }
    
    /// <summary> Уникальный идентификатор раунда </summary>
    public string? RoundId { get; set; }
    
    /// <summary> Ссылка на раунд </summary>
    public Round? Round { get; set; }
    
    /// <summary> Длительность этапа </summary>
    public int? StageDuration { get; set; }
}