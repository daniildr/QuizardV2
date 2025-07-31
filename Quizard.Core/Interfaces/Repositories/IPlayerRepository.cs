using NUlid;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces.Repositories;

/// <summary> Репозиторий игроков </summary>
public interface IPlayerRepository
{
    /// <summary> Метод для получения пользователя по его идентификатору </summary>
    /// <param name="playerId"> Уникальный идентификатор пользователя </param>
    /// <returns> Пользователь </returns>
    /// <exception cref="PlayerDoesNotExistException"> Исключение, если игрок уже существует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Player> GetPlayerAsync(Ulid playerId);
    
    /// <summary> Метод для получения пользователя с помощью его никнейма </summary>
    /// <param name="nickname"> Никнейм пользователя </param>
    /// <returns> Пользователь </returns>
    /// <exception cref="PlayerDoesNotExistException"> Исключение, если игрок уже существует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Player> GetPlayerAsync(string nickname);
    
    /// <summary> Метод для добавления пользователя в игру </summary>
    /// <param name="newPlayer"> Сущность игрока </param>
    /// <returns> Новый игрок </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Player> AddPlayerAsync(Player newPlayer);
    
    /// <summary> Метод для проверки доступности никнейма игрока </summary>
    /// <param name="nickname"> Никнейм </param>
    /// <returns> Результат проверки. <c>true</c> - никнейм занят, <c>false</c> - свободен </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<bool> IsNicknameTakenAsync(string nickname);
}