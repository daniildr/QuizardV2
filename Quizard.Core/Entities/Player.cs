using Quizard.Core.Entities.Abstraction;

namespace Quizard.Core.Entities;

/// <summary> Сущность игрока викторины </summary>
public class Player : BaseEntity
{
    /// <summary> Никнейм пользователя </summary>
    public string Nickname { get; set; } = string.Empty;
}