using Quizard.Core.Entities;

namespace Quizard.Core.Models.Responses;

/// <summary> Тип данных пользователя </summary>
public class PlayerDto
{
    /// <summary> Никнейм пользователя </summary>
    public string Nickname { get; set; } = string.Empty;
    
    /// <summary> Базовый конструктор </summary>
    public PlayerDto() { }

    /// <summary> Маппинг сущности <see cref="Player"/> </summary>
    /// <param name="player"> Сущность <see cref="Player"/> </param>
    public PlayerDto(Player player)
    {
        Nickname = player.Nickname;
    }
}