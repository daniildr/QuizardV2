namespace Quizard.Core.Constants;

/// <summary> Перечисление типов игроков </summary>
public enum ClientType
{
    /// <summary> Клиент игрок </summary>
    Player,
    
    /// <summary> Клиент экран-информатор </summary>
    Informer,
    
    /// <summary> Клиент администратор </summary>
    Admin,
    
    /// <summary> Любые другие клиенты - контроллер света и тд </summary>
    Other
}