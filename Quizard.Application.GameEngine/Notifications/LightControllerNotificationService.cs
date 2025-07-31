using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quizard.Application.GameEngine.Interfaces;
using Quizard.Core.Constants;
using Quizard.SignalR;

namespace Quizard.Application.GameEngine.Notifications;

/// <inheritdoc />
public class LightControllerNotificationService : ILightControllerNotificationService
{
    private readonly ILogger<LightControllerNotificationService> _logger;
    private readonly IHubContext<GameHub> _hubContext;

    /// <summary> Конструктор интерфейса подсветки </summary>
    /// <param name="hubContext"> SignalR хаб </param>
    /// <param name="logger"> Логгер </param>
    public LightControllerNotificationService(
        IHubContext<GameHub> hubContext, 
        ILogger<LightControllerNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
        
        _logger.LogDebug("Интерфейс подсветки пользователей проинициализирован");
    }

    /// <inheritdoc />
    public async Task HighlightPlayersAsync(IEnumerable<string> highlightedNicknames)
    {
        _logger.LogTrace("Будет выполнено оповещение общей группы пользователей о необходимости изменения света");
        await _hubContext.Clients.Group(ClientGroup.All)
            .SendCoreAsync(HubMethods.UpdatePlayerHighlighting, [highlightedNicknames]);
    }

    /// <inheritdoc />
    public async Task HighlightPlayersAsync(string nickname, string rackId)
    {
        _logger.LogTrace(
            "Будет выполнено оповещение общей группы пользователей о инициализации игрока {Nickname}", nickname);
        await _hubContext.Clients.Group(ClientGroup.All)
            .SendAsync(HubMethods.InitPlayerHighlighting, nickname, rackId);
    }

    /// <inheritdoc />
    public async Task PlayerHasDisconnectedAsync(string nickname)
    {
        _logger.LogTrace("Будет выполнено оповещение общей группы пользователей о необходимости изменения света");
        await _hubContext.Clients.Group(ClientGroup.All)
            .SendAsync(HubMethods.PlayerHasDisconnected, nickname);
    }
}