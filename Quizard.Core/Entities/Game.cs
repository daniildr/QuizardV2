using Quizard.Core.Entities.Abstraction;

namespace Quizard.Core.Entities;

/// <summary> Сущность сыгранной игры </summary>
public class Game : BaseEntity
{
    /// <summary> Флаг запуска. Отображает статус игры </summary>
    public bool IsRunning { get; set; }
    
    /// <summary> Штамп времени создания записи </summary>
    public long CreatedAt { get; set; }

    /// <summary> Уникальный идентификатор сценария, который использовался в игре </summary>
    public string ScenarioId { get; set; } = null!;
}