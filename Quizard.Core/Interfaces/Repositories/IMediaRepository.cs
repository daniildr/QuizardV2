using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces.Repositories;

/// <summary> Репозиторий медиа-файлов </summary>
public interface IMediaRepository
{
    /// <summary> Метод для асинхронной итерации по всем медиа-файлам в БД </summary>
    /// <param name="cancellationToken"> Токен отмены </param>
    /// <returns> Перечисление <see cref="Media"/> </returns>
    public IAsyncEnumerable<Media> IterateMediaAsync(CancellationToken cancellationToken = default);
    
    /// <summary> Метод для получения данных медиа из БД </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    /// <returns> Сущность <see cref="Media"/> </returns>
    /// <exception cref="MediaDoesNotExistException"> Исключение, если ответ не существует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Media> GetMediaAsync(Expression<Func<Media, bool>> predicate);

    /// <summary> Метод для добавления нового медиа в БД </summary>
    /// <param name="newMedia"> Новый медиа </param>
    /// <returns> Новый медиа </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Media> AddMediaAsync(Media newMedia);

    /// <summary> Метод для пакетного добавления новых медиа в БД </summary>
    /// <param name="newMedia"> Новый медиа </param>
    /// <returns> Новый медиа </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<List<Media>> AddMediaAsync(List<Media> newMedia);

    /// <summary> Метод для удаления медиа из БД </summary>
    /// <param name="mediaId"> Уникальный идентификатор медип </param>
    /// <returns> Результат операции </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task RemoveMediaAsync(string mediaId);
}