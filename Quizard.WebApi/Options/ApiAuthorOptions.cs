using Microsoft.OpenApi.Models;

namespace Quizard.WebApi.Options;

/// <summary>
/// Данные для обратной связи <see cref="OpenApiContact"/>
/// </summary>
internal class ApiAuthorOptions
{
    /// <summary> Название секции в appsettings </summary>
    public static string ConfigurationSection => "ApiAuthorOptions";
    
    /// <summary> Имя контактного лица </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary> Email контактного лица </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary> Описание интерфейса </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary> Название интерфейса </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary> Тип лицензирования </summary>
    public string License { get; set; } = string.Empty;
}