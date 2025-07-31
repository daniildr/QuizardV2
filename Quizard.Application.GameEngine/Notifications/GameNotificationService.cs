using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quizard.Application.GameEngine.Interfaces;
using Quizard.Core.Constants;
using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Quizard.Core.Models.Responses;
using Quizard.SignalR;

namespace Quizard.Application.GameEngine.Notifications;

/// <inheritdoc/> 
public class GameNotificationService : IGameNotificationService
{
    private readonly IHubContext<GameHub> _hubContext;
    private readonly ILogger<GameNotificationService> _logger;
    private readonly ILightControllerNotificationService _lightControllerNotificationService;

    /// <summary> Конструктор сервиса уведомлений </summary>
    /// <param name="logger"> Логгер </param>
    /// <param name="hubContext"> Контекст SignalR хаба </param>
    /// <param name="lightControllerNotificationService"> Сервис подсветки пользователя </param>
    public GameNotificationService(
        IHubContext<GameHub> hubContext,
        ILogger<GameNotificationService> logger,
        ILightControllerNotificationService lightControllerNotificationService)
    {
        _lightControllerNotificationService = lightControllerNotificationService;
        _hubContext = hubContext;
        _logger = logger;

        _logger.LogInformation("Сервис SignalR‑уведомлений проинициализирован");
    }

    /// <inheritdoc/>
    public async Task Skip()
    {
        await SendLogNoticeAsync(ClientGroup.Informer, HubMethods.Skip).ConfigureAwait(false);

        await _hubContext.Clients.Group(ClientGroup.Informer).SendAsync(HubMethods.Skip).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task GameStateChanged(GameState state, GameTrigger trigger)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.GameStateChanged, state, trigger)
            .ConfigureAwait(false);

        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.GameStateChanged, state, trigger)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task GamePaused()
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.GamePaused).ConfigureAwait(false);
        await _hubContext.Clients.Group(ClientGroup.All).SendAsync(HubMethods.GamePaused).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task GameResumed()
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.GameResumed).ConfigureAwait(false);
        await _hubContext.Clients.Group(ClientGroup.All).SendAsync(HubMethods.GameResumed).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ForceDisconnect()
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.ForceDisconnect).ConfigureAwait(false);
        await _hubContext.Clients.Group(ClientGroup.All)
            .SendAsync(HubMethods.ForceDisconnect)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ClientDisconnected(ClientType clientType, string? clientIdentifier, Exception? exception)
    {
        _logger.LogTrace("Будет выполнено уведомление всех связи с отключение клиента");
        switch (clientType)
        {
            case ClientType.Player:
                await _lightControllerNotificationService
                    .PlayerHasDisconnectedAsync(clientIdentifier!)
                    .ConfigureAwait(false);
                await SendLogNoticeAsync(
                        ClientGroup.All,
                        HubMethods.PlayerHasDisconnected,
                        clientIdentifier!,
                        exception?.Message ?? "Unknown error")
                    .ConfigureAwait(false);

                await _hubContext.Clients
                    .Group(ClientGroup.All)
                    .SendAsync(
                        HubMethods.PlayerHasDisconnected,
                        clientIdentifier!,
                        exception?.Message ?? "Unknown error")
                    .ConfigureAwait(false);
                break;

            case ClientType.Informer:
                await SendLogNoticeAsync(
                        ClientGroup.All,
                        HubMethods.ClientDisconnected,
                        nameof(ClientType.Informer),
                        exception?.Message ?? "Unknown error")
                    .ConfigureAwait(false);

                await _hubContext.Clients
                    .Group(ClientGroup.All)
                    .SendAsync(
                        HubMethods.ClientDisconnected,
                        nameof(ClientType.Informer),
                        exception?.Message ?? "Unknown error")
                    .ConfigureAwait(false);
                break;

            case ClientType.Admin:
                await SendLogNoticeAsync(
                        ClientGroup.All,
                        HubMethods.ClientDisconnected,
                        nameof(ClientType.Admin),
                        exception?.Message ?? "Unknown error")
                    .ConfigureAwait(false);

                await _hubContext.Clients
                    .Group(ClientGroup.All)
                    .SendAsync(
                        HubMethods.ClientDisconnected,
                        nameof(ClientType.Admin),
                        exception?.Message ?? "Unknown error")
                    .ConfigureAwait(false);
                break;

            case ClientType.Other:
            default:
                await SendLogNoticeAsync(
                    ClientGroup.All,
                    HubMethods.ClientDisconnected,
                    "Unknown client type",
                    exception?.Message ?? "Unknown error");

                await _hubContext.Clients
                    .Group(ClientGroup.All)
                    .SendAsync(
                        HubMethods.ClientDisconnected,
                        "Unknown client type",
                        exception?.Message ?? "Unknown error")
                    .ConfigureAwait(false);
                break;
        }
    }

    /// <inheritdoc/>
    public async Task ReceivePlayerList(IClientProxy caller, string[] players)
    {
        _logger.LogTrace("Будет выполнено уведомление вызывающего клиента, ему будет отправлен список игроков");
        await caller.SendCoreAsync(HubMethods.ReceivePlayerList, [players]).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SendGameScenario(Scenario scenario)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.ReceiveGameScenario, scenario)
            .ConfigureAwait(false);

        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.ReceiveGameScenario, scenario)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SendGameScenario(IClientProxy caller, Scenario scenario)
    {
        _logger.LogTrace("Будет выполнено уведомление вызывающего клиента, ему будет отправлен текущий сценарий");
        await caller.SendAsync(HubMethods.ReceiveGameScenario, scenario).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task HighlightPlayersAsync(IEnumerable<string> highlightedNicknames)
    {
        _logger.LogTrace("Будет выполнена подсветка пользователя");
        await _lightControllerNotificationService.HighlightPlayersAsync(highlightedNicknames);
    }

    /// <inheritdoc/>
    public async Task InitialHighlightPlayerAsync(string highlightedNickname, string rackId)
    {
        _logger.LogTrace("Будет выполнена подсветка пользователя при его инициализации");
        await _lightControllerNotificationService.HighlightPlayersAsync(highlightedNickname, rackId);
    }

    /// <inheritdoc/>
    public async Task IdentifyInformerNotice()
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.InformerIdentified).ConfigureAwait(false);
        await _hubContext.Clients.Group(ClientGroup.All)
            .SendAsync(HubMethods.InformerIdentified)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task IdentifyAdminNotice()
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.AdminIdentified).ConfigureAwait(false);
        await _hubContext.Clients.Group(ClientGroup.All)
            .SendAsync(HubMethods.AdminIdentified)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task MediaStarted(Media media)
    {
        await SendLogNoticeAsync(ClientGroup.Informer, HubMethods.ShowMedia, media).ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.Informer)
            .SendAsync(HubMethods.ShowMedia, media)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task RoundStarted(Round round)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.RoundStarted, round).ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.RoundStarted, round)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task QuestionStarted(Question question)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.QuestionStarted, question).ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.QuestionStarted, question)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task QuestionStarted(Player player, Question question)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.TargetQuestionStarted, player, question)
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.TargetQuestionStarted, player, question)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SpeedWinner(Player player)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.SpeedQuestionWinner, player.Nickname)
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.SpeedQuestionWinner, player.Nickname)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task InteractiveQuestionResults(int points)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.InteractiveQuestionResults, points)
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.InteractiveQuestionResults, points)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ShowReveal(Reveal reveal)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.ShowReveal, reveal).ConfigureAwait(false);
        await _hubContext.Clients.Group(ClientGroup.All)
            .SendAsync(HubMethods.ShowReveal, reveal)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ShowRoundStatistic(RoundStatistic[] statistic)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.ShowStatistics, [statistic])
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendCoreAsync(HubMethods.ShowStatistics, [statistic]).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ShowScenarioStatistic(ScenarioStatistic[] statistic)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.ShowScenarioStatistics, [statistic])
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendCoreAsync(HubMethods.ShowScenarioStatistics, [statistic]).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task VotingStarted(Round[] roundsForVoting)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.VotingStarted, [roundsForVoting])
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendCoreAsync(HubMethods.VotingStarted, [roundsForVoting])
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ShopStarted(PurchasePhaseConfiguration purchasePhaseConfiguration)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.ShopStarted, purchasePhaseConfiguration)
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.ShopStarted, purchasePhaseConfiguration)
            .ConfigureAwait(false);
    }

    public async Task ShopUpdated(Stock[] shopStocks)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.StockUpdated, [shopStocks])
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendCoreAsync(HubMethods.StockUpdated, [shopStocks])
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ProductIsOutOfStock(ModifierType modifier)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.ProductIsOutOfStock, modifier)
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.ProductIsOutOfStock, modifier)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ApplyNotTargetModifier(ModifierType modifier)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.ModifierApplied, modifier)
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.ModifierApplied, modifier)
            .ConfigureAwait(false);
    }
    
    


    /// <inheritdoc/>
    public async Task MineApplied(string nickname)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.MineApplied, nickname)
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.MineApplied, nickname)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task MineExploded(string nickname, string[] minedPlayer)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.MineExploded, nickname, minedPlayer)
            .ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.MineExploded, nickname, minedPlayer)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ShakerApplied(string nickname)
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.ShakerApplied, nickname).ConfigureAwait(false);
        await _hubContext.Clients
            .Group(ClientGroup.All)
            .SendAsync(HubMethods.ShakerApplied, nickname)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task MirroredRound()
    {
        await SendLogNoticeAsync(ClientGroup.All, HubMethods.MirroredRound).ConfigureAwait(false);
        await _hubContext.Clients.Group(ClientGroup.All).SendAsync(HubMethods.MirroredRound).ConfigureAwait(false);
    }

    private async Task SendLogNoticeAsync(
        string groupName, string methodName, params object[] args)
    {
        _logger.LogDebug("Будет выполнено уведомление {Group} в связи с {Method}", groupName, methodName);
        _logger.LogTrace("Отправляем лог уведомления, Аргументы: {@Args}", args);

        await _hubContext.Clients.Group(ClientGroup.Admin)
            .SendAsync(HubMethods.Log, methodName, args)
            .ConfigureAwait(false);
    }
}