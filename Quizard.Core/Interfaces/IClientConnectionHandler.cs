namespace Quizard.Core.Interfaces;

/// <summary> Интерфейс обработчика подключений клиентов </summary>
public interface IClientConnectionHandler
{
    /// <summary> Игрок подтвердил свой никнейм и получил connectionId </summary>
    /// <remarks>
    /// <list type="number">
    /// <listheader>
    /// <description> Пайп-лайн метода: </description>
    /// </listheader>
    /// <item><description> Регистрирует подключение в GameSession </description></item>
    /// <item><description> Триггерит FSM PlayerIdentified / AllPlayersReady </description></item>
    /// <item><description> Авто‑резюм из паузы, если все подключены </description></item>
    /// </list>
    /// </remarks>
    /// <param name="connectionId"> SignalR ConnectionId клиента‑игрока </param>
    /// <param name="nickname"> Никнейм игрока </param>
    /// <param name="rackId"> Уникальный идентификатор стойки игрока </param>
    /// <returns> Асинхронная операция </returns>
    public Task HandlePlayerIdentifiedAsync(string connectionId, string nickname, string rackId);

    /// <summary> Экран‑информатор подтвердил подключение </summary>
    /// <remarks>
    /// <list type="number">
    /// <listheader>
    /// <description> Пайп-лайн метода: </description>
    /// </listheader>
    /// <item><description> Сохраняет connectionId в GameSession </description></item>
    /// <item><description> Авто‑резюм из паузы, если необходмо </description></item>
    /// </list>
    /// </remarks>
    /// <returns> Асинхронная операция </returns>
    public Task HandleInformerIdentifiedAsync(string connectionId);

    /// <summary> Администратор подтвердил подключение </summary>
    /// <remarks>
    /// <list type="number">
    /// <listheader>
    /// <description> Пайп-лайн метода: </description>
    /// </listheader>
    /// <item><description> Сохраняет connectionId в GameSession </description></item>
    /// <item><description> Авто‑резюм из паузы, если необходмо </description></item>
    /// </list>
    /// </remarks>
    /// <returns> Асинхронная операция </returns>
    public Task HandleAdminIdentifiedAsync(string connectionId);

    /// <summary> Реагирует на разрыв соединения любого клиента </summary>
    /// <returns> Асинхронная операция </returns>
    public Task HandleClientDisconnectedAsync(string connectionId, Exception exception);
}