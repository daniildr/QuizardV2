using Newtonsoft.Json;
using NUlid;
using Quizard.Core.Entities.Abstraction;

namespace Quizard.Core.Entities;

/// <summary> Сущность статистики сценария </summary>
public class ScenarioStatistic : BaseEntity
{
    /// <summary> Уникальный идентификатор игры </summary>
    public Ulid GameId { get; set; }

    /// <summary> Ссылка на игру </summary>
    [JsonIgnore]
    public Game Game { get; set; } = null!;
    
    /// <summary> Уникальный идентификатор игрока </summary>
    public Ulid PlayerId { get; set; }
    
    /// <summary> Ссылка на игрока </summary>
    public Player Player { get; set; } = null!;
    
    /// <summary> Уникальный идентификатор сценария </summary>
    public string ScenarioId { get; set; } = null!;
    
    /// <summary> Ссылка на сценарий </summary>
    [JsonIgnore]
    public Scenario Scenario { get; set; } = null!;
    
    /// <summary> Счет игрока в раунде </summary>
    public int Score { get; set; }
}