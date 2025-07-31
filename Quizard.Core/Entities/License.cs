using Quizard.Core.Entities.Abstraction;

namespace Quizard.Core.Entities;

/// <summary> Сущность лицензии </summary>
public class License : InternalBaseEntity
{
    /// <summary> Лицензионный ключ </summary>
    public string LicenseKey { get; set; } = null!;
    
    /// <summary> Время истечения срока действия (UNIX timestamp). Зашифрованная строка </summary>
    public string ExpirationTime { get; set; } = null!;

    /// <summary> Количество оставшихся игровых сессий. Зашифрованная строка </summary>
    public string GamesLeft { get; set; } = null!;
    
    /// <summary> Флаг действительности лицензии. </summary>
    public bool Active { get; set; } = true;
    
    /// <summary> Уникальный идентификатор секретной соли </summary>
    public long SaltId { get; set; }
    
    /// <summary> Ссылка на секретную соль </summary>
    public LicenseSecret Salt { get; set; } = null!;
}