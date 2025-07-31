namespace Quizard.Core.Models.Responses;

/// <summary> Модель данных активации лицензионного ключа </summary>
public class UploadLicenseDto
{
    /// <summary> Лицензионный ключ </summary>
    public string LicenseKey { get; set; } = null!;
}