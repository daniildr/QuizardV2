using Microsoft.Extensions.Configuration;

namespace Quizard.Core.Utils;

/// <summary>
/// Билдер конфигурации на базе <see cref="Microsoft.Extensions.Configuration.ConfigurationBuilder"/>
/// </summary>
public abstract class ConfigurationBuilder
{
    /// <summary> Метод возвращает конфигурацию из переданного файла appsettings </summary>
    /// <param name="appSettingsFileName"> Имя файла appsettings </param>
    /// <returns> Интерфейс конфигурации </returns>
    public static IConfigurationRoot BuildConfiguration(string appSettingsFileName = "appsettings.json") =>
        new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .AddJsonFile(appSettingsFileName, optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
}