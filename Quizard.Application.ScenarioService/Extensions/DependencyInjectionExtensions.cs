using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quizard.Core.Interfaces;

namespace Quizard.Application.ScenarioService.Extensions;

/// <summary> Расширения для введения DI зависимостей сервиса для работы со сценариями </summary>
public static class DependencyInjectionExtensions
{
    /// <summary> Расширение для введения сервиса для работы со сценариями </summary>
    /// <param name="services"> Коллекция сервисов </param>
    /// <param name="configuration"> Конфигурация сервисов </param>
    public static void AddScenarioService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IScenarioChecksumCalculator, ScenarioChecksumCalculator>();
        services.AddTransient<IScenarioValidator, ScenarioValidator>();
        services.AddTransient<IScenarioFacade, ScenarioFacade>();
    }
}