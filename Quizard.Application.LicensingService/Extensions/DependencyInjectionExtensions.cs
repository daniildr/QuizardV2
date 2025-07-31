using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quizard.Core.Interfaces;

namespace Quizard.Application.LicensingService.Extensions;

/// <summary>
/// Расширения для введения DI зависимостей сервиса лицензирования
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary> Расширение для введения зависимостей менеджера БД </summary>
    /// <param name="services"> Коллекция сервисов </param>
    /// <param name="configuration"> Конфигурация сервисов </param>
    public static void AddLicensingService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IAesEncryptor, AesEncryptor>();
        services.AddTransient<ILicensingService, LicensingService>();
    }
}