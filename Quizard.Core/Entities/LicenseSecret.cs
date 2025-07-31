using Quizard.Core.Entities.Abstraction;

namespace Quizard.Core.Entities;

/// <summary> Сущность секретной соли лицензии </summary>
public class LicenseSecret : InternalBaseEntity
{
    /// <summary> Секретная соль. Зашифрованная строка </summary>
    public string Salt { get; set; } = null!;
    
    /// <summary> Время создания записи. Зашифрованная строка </summary>
    public string CreatedAt { get; set; } = null!;
    
    /// <summary> Ссылка на лицензию </summary>
    public License? License { get; set; }
}