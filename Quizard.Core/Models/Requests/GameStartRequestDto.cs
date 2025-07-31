using Quizard.Core.Models.Responses;

namespace Quizard.Core.Models.Requests;

/// <summary> Модель данных для запуска игры </summary>
public class GameStartRequestDto
{
    /// <summary> Уникальный идентификатор сценария </summary>
    public string ScenarioId { get; set; } = null!;
    
    /// <summary> Коллекция игроков </summary>
    public PlayerDto[] Players { get; set; } = [];
}