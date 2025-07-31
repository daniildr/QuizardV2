namespace Quizard.Application.GameEngine.Interfaces;

/// <summary> Интерфейс подсветки пользователя </summary>
public interface ILightControllerNotificationService
{
    /// <summary> Метод подсветки пользователя </summary>
    /// <param name="highlightedNicknames"> Коллекция никнеймов пользователей, требующих подсветки </param>
    /// <returns> Асинхронная операция </returns>
    Task HighlightPlayersAsync(IEnumerable<string> highlightedNicknames);

    /// <summary> Метод подсветки только что инициализированного пользователя </summary>
    /// <param name="nickname"> Никнейм пользователей, требующего подсветки </param>
    /// <param name="rackId"> Уникальный идентификатор стойки </param>
    /// <returns> Асинхронная операция </returns>
    Task HighlightPlayersAsync(string nickname, string rackId);
    
    /// <summary> Метод подсветки отключившегося пользователя </summary>
    /// <param name="nickname"> Никнейм пользователей, требующего подсветки </param>
    /// <returns> Асинхронная операция </returns>
    Task PlayerHasDisconnectedAsync(string nickname);
}