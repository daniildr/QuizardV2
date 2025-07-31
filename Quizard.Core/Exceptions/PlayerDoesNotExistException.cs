using NUlid;

namespace Quizard.Core.Exceptions;

/// <summary> Исключение вызываемое при отсутствии игрока </summary>
public class PlayerDoesNotExistException : Exception
{
    /// <summary> Исключение вызываемое при отсутствии игрока </summary>
    /// <param name="nickname"> Никнейм игрока </param>
    public PlayerDoesNotExistException(string nickname) : base($"Пользователь {nickname} не найден") { }
    
    /// <summary> Исключение вызываемое при отсутствии игрока </summary>
    /// <param name="playerId"> Уникальный идентификатор игрока </param>
    public PlayerDoesNotExistException(Ulid playerId) : base($"Пользователь {playerId} не найден") { }
}