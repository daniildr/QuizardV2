using Quizard.Core.Entities;

namespace Quizard.Core.Models.Responses;

/// <summary> Модель данных фазы покупок </summary>
public class PurchasePhaseConfiguration
{
    /// <summary> Продолжительность этапа в секундах </summary>
    public int Duration { get; set; }

    /// <summary> Текущий остаток в магазине </summary>
    public List<Stock> ShopStocks { get; set; } = null!;
}