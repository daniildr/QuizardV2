namespace Quizard.Core.Entities.Abstraction;

/// <summary> Абстрактная сущность локальной БД </summary>
public class InternalBaseEntity
{
    /// <summary> Уникальный идентификатор сущности </summary>
    public long Id { get; set; }
}