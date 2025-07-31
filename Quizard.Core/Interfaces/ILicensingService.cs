using System.Security.Claims;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces;

/// <summary> Интерфейс для работы с сервисом лицензирования </summary>
public interface ILicensingService
{
    /// <summary> Метод для генерации секретной соли </summary>
    /// <returns> Секретная соль </returns>
    public Task<string> GenerateSecretSalt();

    /// <summary> Метод для загрузки лицензионного ключа в игру </summary>
    /// <param name="licenseKey"> Лицензионный ключ </param>
    /// <returns> Клеймы токена (лицензионного ключа) </returns>
    /// <exception cref="LicenseValidationException"> Исключение, если ключ лицензии будет некорректным </exception>
    public Task<ClaimsPrincipal> UploadLicenseKey(string licenseKey);

    /// <summary> Метод для проверки активной лицензии </summary>
    /// <param name="changeGameCounter"> Флаг необходимости переключит счетчик игр </param>
    /// <returns> Результат проверки </returns>
    /// <exception cref="LicenseValidationException">  Исключение, если в системе нет валидных лицензий </exception>
    /// <exception cref="ActiveLicenseNotFoundException"> Исключение, если в системе нет активных лицензий </exception>
    public Task<bool> CheckActiveLicense(bool changeGameCounter = false);
    
    /// <summary> Метод для проверки возможности загружать кастомные сценарии и вопросы </summary>
    /// <returns> Результат проверки - <c>true</c>, если разрешено загружать не-licensed контент</returns>
    /// <exception cref="ActiveLicenseNotFoundException"> Исключение, если в системе нет активных лицензий </exception>
    public Task<bool> CanUploadNonLicensedContentAsync();
}