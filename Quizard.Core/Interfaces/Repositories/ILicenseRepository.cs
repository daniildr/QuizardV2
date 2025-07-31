using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces.Repositories;

/// <summary> Репозиторий данных лицензий </summary>
public interface ILicenseRepository
{
    /// <summary> Метод для асинхронной итерации по данным лицензий в БД </summary>
    /// <param name="withSalt"> Флаг необходимости подгрузить данные секретной соли </param>
    /// <param name="cancellationToken"> Токен отмены </param>
    /// <returns> Перечисление <see cref="License"/> </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public IAsyncEnumerable<License> IterateLicenseAsync(bool withSalt, CancellationToken cancellationToken = default);
    
    /// <summary> Метод для получения данных лицензии из БД </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    /// <param name="withSalt"> Флаг необходимости подгрузить данные лицензии </param>
    /// <returns> Сущность <see cref="License"/> </returns>
    /// <exception cref="LicenseDoesNotExistException"> Исключение, если лицензия не существует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<License> GetLicenseAsync(Expression<Func<License, bool>> predicate, bool withSalt);
    
    /// <summary> Метод для редактирования данных лицензии в БД </summary>
    /// <remarks> Метод обновляет только статус лицензии и счетчик игр</remarks>
    /// <param name="licenseId"> Уникальный идентификатор лицензии </param>
    /// <param name="updatedLicense"> Обновленная лицензии </param>
    /// <returns> Сущность <see cref="License"/> </returns>
    /// <exception cref="LicenseDoesNotExistException"> Исключение, если лицензия не существует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<License> UpdateLicenseAsync(long licenseId, License updatedLicense);

    /// <summary> Метод для добавления новой лицензии в БД </summary>
    /// <param name="license"> Лицензия </param>
    /// <returns> Новая запись данных лицензии </returns>
    /// <remarks> Деактивирует все активированные до этого лицензии </remarks>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<License> AddLicenseAsync(License license);

    /// <summary> Метод для деактивации лицензии в БД </summary>
    /// <param name="licenseId"> Уникальный идентификатор лицензии </param>
    /// <returns> Результат операции </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task DeactivateLicenseAsync(long licenseId);
}