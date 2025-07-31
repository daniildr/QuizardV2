namespace Quizard.Core.Constants;

/// <summary> Группы пользователей </summary>
public static class ClientGroup
{
    /// <summary> Общая группа </summary>
    public static string All => "all";
    
    /// <summary> Группа администратора </summary>
    public static string Admin => "admin";
    
    /// <summary> Группа пользователей </summary>
    public static string Players => "players";
    
    /// <summary> Группы игроков </summary>
    public static string Player(string nickname) => $"player:{nickname}";
    
    /// <summary> Группа экрана информатора </summary>
    public static string Informer => "informer";
}