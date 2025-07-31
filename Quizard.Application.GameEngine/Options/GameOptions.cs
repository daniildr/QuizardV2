namespace Quizard.Application.GameEngine.Options;

/// <summary> Конфигурация игры </summary>
public class GameOptions
{
    /// <summary> Название секции в appsettings </summary>
    public static string ConfigurationSection => "GameOptions";

    /// <summary> Дефолтная продолжительность игры в минутах </summary>
    /// <remarks> Дефолтная продолжительность игры - 60 минут </remarks>
    public int DefaultGameDuration { get; set; } = 60;
    
    /// <summary> Задержка для показа игрокам раунда </summary>
    public int DefaultRoundQuestionDelay { get; set; } = 5;
    
    /// <summary> Время показа победителя скоростного раунда </summary>
    public int SpeedWinnerShowTime { get; set; } = 10;
    
    /// <summary> Дефолтная длительность этапа магазина </summary>
    public int DefaultShopDuration { get; set; } = 60;
}