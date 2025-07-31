using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces.Repositories;

/// <summary> Репозиторий секретных данных лицензий в БД </summary>
public interface ILicenseSecretRepository
{
    /// <summary> Метод для асинхронной итерации по секретным данным лицензий в БД </summary>
    /// <param name="withLicense"> Флаг необходимости подгрузить данные лицензии </param>
    /// <param name="cancellationToken"> Токен отмены </param>
    /// <returns> Перечисление <see cref="LicenseSecret"/> </returns>
    public IAsyncEnumerable<LicenseSecret> IterateLicenseSecretsAsync(
        bool withLicense, CancellationToken cancellationToken = default);
    
    /// <summary> Метод для получения секретных данных лицензии из БД </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    /// <param name="withLicense"> Флаг необходимости подгрузить данные лицензии </param>
    /// <returns> Сущность <see cref="LicenseSecret"/> </returns>
    /// <exception cref="LicenseSecretDoesNotExistException"> Исключение, если соль не существует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<LicenseSecret> GetLicenseSecretAsync(Expression<Func<LicenseSecret, bool>> predicate, bool withLicense);
    
    /// <summary> Метод для получения коллекции секретных данных лицензии из БД </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    /// <param name="withLicense"> Флаг необходимости подгрузить данные лицензии </param>
    /// <returns> Коллекция <see cref="LicenseSecret"/> </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<LicenseSecret[]> GetLicenseSecretsAsync(
        Expression<Func<LicenseSecret, bool>> predicate, bool withLicense);

    /// <summary> Метод для добавления новой секретной записи в БД </summary>
    /// <param name="secret"> Секретная данные </param>
    /// <returns> Новая запись секретных данных лицензии </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<LicenseSecret> AddLicenseSecretAsync(LicenseSecret secret);
}