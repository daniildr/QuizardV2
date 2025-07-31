using System.Linq.Expressions;
using NUlid;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces.Repositories;

/// <summary> Репозиторий игр (игровых сессий) </summary>
public interface IGameRepository
{
    /// <summary> Метод для асинхронной итерации по данным игровых сессий в БД </summary>
    /// <param name="cancellationToken"> Токен отмены </param>
    /// <returns> Перечисление <see cref="Game"/> </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public IAsyncEnumerable<Game> IterateGamesAsync(CancellationToken cancellationToken = default);
    
    /// <summary> Метод для получения данных игровых сессий из БД </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    /// <returns> Массив <see cref="Game"/> </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Game[]> GetGamesAsync(Expression<Func<Game, bool>> predicate);
    
    /// <summary> Метод для получения данных игровой сессий из БД </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    /// <returns> Массив <see cref="Game"/> </returns>
    /// <exception cref="GameDoesNotExistException"> Исключение, если игра отсутствует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Game> GetGameAsync(Expression<Func<Game, bool>> predicate);

    /// <summary> Метод для добавления новой игровой сессии в БД </summary>
    /// <param name="game"> Новая игровая сессия </param>
    /// <returns> Сущность новой игры </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Game> AddGameAsync(Game game);

    /// <summary> Метод для добавления новой игровой сессии в БД </summary>
    /// <param name="gameId"> Уникальный идентификатор игровой сессии </param>
    /// <param name="game"> Новая игровая сессия </param>
    /// <returns> Сущность новой игры </returns>
    /// <exception cref="GameDoesNotExistException"> Исключение, если игра отсутствует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Game> UpdateGameAsync(Ulid gameId, Game game);

    /// <summary> Метод для удаления игровой сессии из БД </summary>
    /// <param name="gameId"> Уникальный идентификатор игровой сессии </param>
    /// <returns> Асинхронная операция </returns>
    /// <exception cref="GameDoesNotExistException"> Исключение, если игра отсутствует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task DeleteGameAsync(Ulid gameId);

}