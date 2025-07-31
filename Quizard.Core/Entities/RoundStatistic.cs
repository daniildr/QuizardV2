using Newtonsoft.Json;
using NUlid;
using Quizard.Core.Entities.Abstraction;

namespace Quizard.Core.Entities;

/// <summary> Сущность статистики раунда </summary>
public class RoundStatistic : BaseEntity
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
    
    /// <summary> Уникальный идентификатор раунда </summary>
    public string RoundId { get; set; } = null!;
    
    /// <summary> Ссылка на раунд </summary>
    [JsonIgnore]
    public Round Round { get; set; } = null!;
    
    /// <summary> Счет игрока в раунде </summary>
    public int Score { get; set; }
}