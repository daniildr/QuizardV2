namespace Quizard.WebApi.Options;

/// <summary> Конфигурация api интерфейса </summary>
public class ApiOptions
{
    /// <summary> Название секции в appsettings </summary>
    public static string ConfigurationSection => "ApiOptions";
    
    /// <summary> Базовый URL web api </summary>
    public string BaseUrl { get; set; } = string.Empty;
    
    /// <summary> Внешний URL web api </summary>
    public string ExternalBaseUrl { get; set; } = string.Empty;

    /// <summary> Разрешенные домены стронних приложений </summary>
    public string[] AllowedOrigins { get; set; } = [];
}