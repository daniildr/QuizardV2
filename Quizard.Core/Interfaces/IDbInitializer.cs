namespace Quizard.Core.Interfaces;

/// <summary>
/// Выполняет проверку/создание служебных данных в базе сразу после миграций.
/// </summary>
public interface IDbInitializer
{
    /// <summary>
    /// Гарантирует, что база существует, все миграции применены и
    /// служебные сущности посеяны.
    /// </summary>
    public Task InitializeAsync(CancellationToken cancellationToken = default);
}