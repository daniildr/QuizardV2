using Quizard.Core.Entities.Abstraction;
using Quizard.Core.Enums;

namespace Quizard.Core.Entities;

/// <summary> Сущность остатка в магазине </summary>
public class Stock : BaseEntity
{
    /// <summary> Уникальный идентификатор сценария </summary>
    public string ScenarioId { get; set; } = null!;
    
    /// <summary> Тип предмета </summary>
    public ModifierType ModifierType { get; set; }
    
    /// <summary> Название предмета для текущего игрового сценария </summary>
    public string Name { get; set; } = null!;
    
    /// <summary> Описание предмета для текущего раунда </summary>
    public string Description { get; set; } = null!;
    
    /// <summary> Ссылка на иконку предмета </summary>
    public string IconUrl { get; set; } = null!;
    
    /// <summary> Количество </summary>
    public int Quantity { get; set; }

    /// <summary> Множитель стоимости </summary>
    public int CostMultiplier { get; set; }
    
    /// <summary> Уникальность предмета для пользователя </summary>
    public bool UniqForPlayer { get; set; }
}