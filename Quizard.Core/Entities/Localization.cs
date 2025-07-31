using Quizard.Core.Entities.Abstraction;

namespace Quizard.Core.Entities;

/// <summary> Сущность локализации </summary>
public class Localization : BaseEntity
{
    /// <summary> Уникальный идентификатор сценария </summary>
    public string ScenarioId { get; set; } = null!;

    #region Buttons
    /// <summary> Кнопка "Вверх" </summary>
    public string ButtonUp { get; set; } = null!;
    
    /// <summary> Кнопка "Ввниз" </summary>
    public string ButtonDown { get; set; } = null!;
    
    /// <summary> Кнопка "Подтвердить" </summary>
    public string ButtonConfirm { get; set; } = null!;
    
    /// <summary> Кнопка "Отмена" </summary>
    public string ButtonCancel { get; set; } = null!;
    
    /// <summary> Кнопка "Продолжить" </summary>
    public string ButtonContinue { get; set; } = null!;
    
    /// <summary> Кнопка "Купить" </summary>
    public string ButtonBuy { get; set; } = null!;
    
    /// <summary> Кнопка "Закончить" </summary>
    public string ButtonFinish { get; set; } = null!;
    
    /// <summary> Заглушка кнопки. Кнопка "N/A" </summary>
    public string ButtonStub { get; set; } = null!;
    
    /// <summary> Лейбл кнопки "Ставка ..." </summary>
    public string BidButtonLabel { get; set; } = null!;
    #endregion

    #region CentralLabels
    /// <summary> Лейбл "Внимание на экран" </summary>
    public string AttentionLabel { get; set; } = null!;
    
    /// <summary> Лейбл "Ожидание остальных игроков" </summary>
    public string WaitOtherPlayersLabel { get; set; } = null!;
    
    /// <summary> Лейбл "Ожидайте своей очереди" </summary>
    public string WaitYourTurnLabel { get; set; } = null!;
    
    /// <summary> Лейбл "Текс Подсказок" </summary>
    public string HintLabel { get; set; } = null!;
    
    /// <summary> Лейбл "Вы оказались быстрейшим!" </summary>
    public string SpeedWinnerLabel { get; set; } = null!;
    
    /// <summary> Лейбл "Угадайте, что показывает вам игрок" </summary>
    public string PantomimeLabel { get; set; } = null!;
    
    /// <summary> Лейбл "Выберите раунд, который бы Вы хотели сыграть следующим" </summary>
    public string VotingLabel { get; set; } = null!;
    
    /// <summary> Лейбл "Игрок" </summary>
    public string PlayerLabel { get; set; } = null!;
    
    /// <summary> Лейбл "Счет" </summary>
    public string ScoreLabel { get; set; } = null!;
    
    /// <summary> Лейбл "Время ставок" </summary>
    public string BetTime { get; set; } = null!;
    
    /// <summary> Первая часть лейбла "Сделайте ставку" (слово "Сделайте") </summary>
    public string MakeBetLabelPart1 { get; set; } = null!;
    
    /// <summary> Первая часть лейбла "Сделайте ставку" (слово "ставку") </summary>
    public string MakeBetLabelPart2 { get; set; } = null!;
    
    /// <summary> Первая часть лейбла "Ставка @PlayerBet принята" (слово "Ставка") </summary>
    public string BidAcceptedLabelPart1 { get; set; } = null!;
    
    /// <summary> Первая часть лейбла "Ставка @PlayerBet принята" (слово "принята") </summary>
    public string BidAcceptedLabelPart2 { get; set; } = null!;

    /// <summary> Лейбл "Цена" </summary>
    public string PriceLabel { get; set; } = null!;
    #endregion

    #region HeaderLebels
    /// <summary> Лейбл хедера "LOGIN" </summary>
    public string LoginLabel { get; set; } = null!;
    
    /// <summary> Лейбл хедера "Раунда" </summary>
    public string RoundLabel { get; set; } = null!;

    /// <summary> Лейбл хедера "Результаты раунда" </summary>
    public string RoundResultsLabel { get; set; } = null!;

    /// <summary> Лейбл хедера "Выбор следующего раунда" </summary>
    public string RoundChoiceLabel { get; set; } = null!;

    /// <summary> Лейбл хедера "Магазин" </summary>
    public string ShopLabel { get; set; } = null!;

    /// <summary> Лейбл хедера "Недостаточно средств" </summary>
    public string InsufficientFunds { get; set; } = null!;

    /// <summary> Лейбл хедера "Товар уже покупался" </summary>
    public string ProductHasBeenPurchased { get; set; } = null!;

    /// <summary> Лейбл хедера "Товар уже приобретен" </summary>
    public string ProductAlreadyPurchased { get; set; } = null!;
    
    /// <summary> Лейбл хедера "Ожидайте..." </summary>
    public string ShopWaitLabel { get; set; } = null!;
    
    /// <summary> Лейбл хедера "Активация предметов" </summary>
    public string ApplyingModifiersLabel { get; set; } = null!;
    
    /// <summary> Лейбл хедера "Игра завершена" </summary>
    public string GameFinishLabel { get; set; } = null!;
    #endregion
}