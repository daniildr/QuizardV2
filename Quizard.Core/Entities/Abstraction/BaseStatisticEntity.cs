using Newtonsoft.Json;
using NUlid;

namespace Quizard.Core.Entities.Abstraction;

/// <summary> Базовая сущность статистических данных </summary>
public abstract class BaseStatisticEntity : BaseEntity
{
    /// <summary> Уникальный идентификатор игры </summary>
    public Ulid GameId { get; set; }
    
    /// <summary> Ссылка на игру </summary>
    [JsonIgnore]
    public Game Game { get; set; } = null!;
    
    /// <summary> Уникальный идентификатор игрока </summary>
    public Ulid PlayerId { get; set; }
    
    /// <summary> Ссылка на игрока </summary>
    [JsonIgnore]
    public Player Player { get; set; } = null!;
    
    /// <summary> Количество баллов, набранное игроком </summary>
    public int TotalPoints { get; set; }
    
}