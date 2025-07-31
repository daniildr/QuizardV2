namespace Quizard.Core.Interfaces;

/// <summary> Менеджер игровых событий </summary>
public interface IGameManager
{
    /// <summary> Вызывается игроками для применения и передачи мины </summary>
    /// <param name="connectionId"> SignalR идентификатор клиента </param>
    /// <param name="nickname"> Никней цели </param>
    /// <returns> Асинхронная операция </returns>
    public Task HandleMineApplication(string connectionId, string nickname);
    
    /// <summary> Вызывается игроками для применения и передачи мины </summary>
    /// <param name="connectionId"> SignalR идентификатор клиента </param>
    /// <returns> Асинхронная операция </returns>
    public Task HandleMineExploded(string connectionId);
    
    /// <summary> Метод для оповещения бекенда о том, что раунд становится перевернутым </summary>
    /// <param name="connectionId"> SignalR идентификатор клиента </param>
    /// <returns> Асинхронная операция </returns>
    public Task HandleMirroredModifierProtection(string connectionId);
    
    /// <summary> Метод для применения перемешивания на конкретного игрока </summary>
    /// <param name="connectionId"> SignalR идентификатор клиента </param>
    /// <param name="nickname"> Никней цели </param>
    /// <returns> Асинхронная операция </returns>
    public Task HandleShakerModifierApplication(string connectionId, string nickname);
}