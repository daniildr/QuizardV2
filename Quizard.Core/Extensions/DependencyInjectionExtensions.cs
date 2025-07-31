using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Quizard.Core.Extensions;

/// <summary> Расширения для введения DI зависимостей ядра системы </summary>
public static class DependencyInjectionExtensions
{
    private static Serilog.Core.Logger CreateLogger(IConfiguration configuration) 
        => new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

    /// <summary>
    /// Расширение для введения зависимостей логирования через конфигурацию сервисов <see cref="ILoggingBuilder"/>
    /// </summary>
    /// <param name="logging"> Интерфейс конфигурации логирования </param>
    /// <param name="configuration"> Интерфейс доступа к общей конфигурации хост сервиса </param>
    public static void ConfigureStructuredLogger(this ILoggingBuilder logging, IConfiguration configuration)
    {
        Log.Logger = CreateLogger(configuration);

        logging.ClearProviders();
        logging.AddSerilog(dispose: true);
    }
}