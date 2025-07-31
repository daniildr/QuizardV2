namespace Quizard.Application.LicensingApplication.Models;

/// <summary> Внутренняя модель связки "Лицензия - Соль" приложения генерации лицензий </summary>
public class LicenseAndSaltDto
{
    /// <summary> Лицензия </summary>
    public string License { get; set; } = null!;

    /// <summary> Секретная соль </summary>
    public string Salt { get; set; } = null!;
}