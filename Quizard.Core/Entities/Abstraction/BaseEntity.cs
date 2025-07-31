using NUlid;

namespace Quizard.Core.Entities.Abstraction;

/// <summary> Абстрактная сущность БД </summary>
public abstract class BaseEntity
{
    /// <summary> ULID идентификатор сущности </summary>
    public Ulid? Id { get; set; } = Ulid.NewUlid();
}