namespace Quizard.Core.Entities;

/// <summary> Сущность сценария игры </summary>
public class Scenario
{
    /// <summary> Уникальный идентификатор сценария </summary>
    public string Id { get; set; } = null!;

    /// <summary> Уникальное название сценария </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary> Описание сценария </summary>
    public string Description { get; set; } = null!;

    /// <summary> Продолжительность сценария в минутах </summary>
    /// <remarks> Может быть <c>null</c>, в этом случае будет использоваться значение по умолчанию </remarks>
    public int? GameDuration { get; set; }

    /// <summary> Этапы сценария </summary>
    public ICollection<Stage> Stages { get; set; } = [];

    /// <summary> Список раундов, из которых игроки могу выбирать раунды </summary>
    public ICollection<Round> RoundDefinitions { get; set; } = [];

    /// <summary> Конфигурпация остатков на складе магазина </summary>
    public ICollection<Stock> ShopStocks { get; set; } = [];

    /// <summary> Цена базового игрового балла </summary>
    public int BasePointPrice { get; set; }
    
    /// <summary> Количество очков, с которым игрок начинает раунд </summary>
    public int StartPlayerScore { get; set; }
    
    /// <summary> Время, которое раунд будет "представляться" игрокам </summary>
    public int? RoundPresentationDuration { get; set; }
    
    /// <summary> Флаг необходимости показать статистику сценария (игровой сессии) в конце игры </summary>
    public bool ShowScenarioStatsOnFinish { get; set; }
    
    /// <summary> Ссылка на плейсхолдер, который будет показан в конце игры </summary>
    public string? FinishPlaceholder { get; set; }
    
    /// <summary> Ссылка на общий плейсхолдер, который будет показан на экране информаторе </summary>
    public string Placeholder { get; set; } = null!;
    
    /// <summary> Ссылка на файл окализации сценария </summary>
    public Localization Localization { get; set; } = null!;
}