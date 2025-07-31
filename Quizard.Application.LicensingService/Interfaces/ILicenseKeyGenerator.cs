namespace Quizard.Application.LicensingService.Interfaces;

/// <summary>
/// Интерфейс генератора лицензионных ключей
/// </summary>
public interface ILicenseKeyGenerator
{
    /// <summary> Создаёт лицензионный ключ на основе переданных клеймов и секретной соли </summary>
    /// <param name="salt"> Секретная соль, сгенерированная клиентом </param>
    /// <param name="claims"> Клеймы ключа  </param>
    /// <returns> Лицензионный ключ (JWT-токен) </returns>
    public string GenerateLicenseKey(string salt, IReadOnlyDictionary<string, object> claims);
}