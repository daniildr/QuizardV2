using Quizard.Core.Enums;

namespace Quizard.Core.Entities;

/// <summary> Тип данных медиа файла </summary>
public class Media
{
    /// <summary> Уникальный идентификатор медиа файла </summary>
    public string MediaId { get; set; } = null!;
    
    /// <summary> Тип медиа файла </summary>
    public MediaType Type { get; set; }
    
    /// <summary> Опционально. URL </summary>
    public string? Url { get; set; }
    
    /// <summary> Контент для типа Текст </summary>
    public string? Content { get; set; }
    
    /// <summary> Опционально. Задержка показа, сек </summary>
    public int DelaySeconds { get; set; }
    
    /// <summary> Продолжительность медиа </summary>
    /// <remarks> Для текста и картинок, чтобы знать сколько отображать </remarks>
    public int? Duration { get; set; }
    
    /// <summary> Флаг отображения медиа на экране игрока </summary>
    /// <remarks> Актуально только для изображений </remarks>
    public bool ShowOnPlayer { get; set; }
}