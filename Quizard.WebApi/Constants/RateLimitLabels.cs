namespace Quizard.WebApi.Constants;

/// <summary> Константы лейблов политики ограничения количества запросов </summary>
internal static class RateLimitLabels
{
    /// <summary> Публичный контроллер </summary>
    public const string PublicPolicy = "PublicPolicy";
    
    /// <summary> Контроллер с ограничениями </summary>
    public const string LimitPolicy = "LimitPolicy";
    
    /// <summary> Контроллер с максимальными ограничениями </summary>
    public const string ExtraLimitPolicy = "ExtraLimitPolicy";
}