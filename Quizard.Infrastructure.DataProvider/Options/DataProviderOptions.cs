namespace Quizard.Infrastructure.DataProvider.Options;

/// <summary> Опции для провайдера данных </summary>
public class DataProviderOptions
{
    /// <summary> Название секции в appsettings </summary>
    public static string ConfigurationSection => "DataProviderOptions";
    
    /// <summary>
    /// Максимальное кол-во попыток переподключения
    /// </summary>
    public int MaxRetries { get; set; }
    
    /// <summary>
    /// Delay между попытками подключится (в миллисекундах)
    /// </summary>
    public int RetryDelayMilliseconds { get; set; }
}