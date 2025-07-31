using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quizard.Core.Interfaces;

namespace Quizard.Infrastructure.CacheManager.Extensions;

/// <summary> Расширения для введения DI зависимостей сервиса кеширования </summary>
public static class DependencyInjectionExtensions
{
    /// <summary> Расширение для введения сервиса для работы со сценариями </summary>
    /// <param name="services"> Коллекция сервисов </param>
    /// <param name="configuration"> Конфигурация сервисов </param>
    public static void AddCacheManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 200;
            options.TrackStatistics = true;
        });
        services.AddSingleton<ICacheManager, CacheFacade>();
    }
}