using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using NUlid;
using Quizard.Core.Entities;
using Quizard.Core.Interfaces;
using Quizard.WebApi.Constants;
using Quizard.WebApi.Controllers.BaseControllers;
using Quizard.WebApi.Options;
using Quizard.WebApi.Routes;

namespace Quizard.WebApi.Controllers;

/// <summary> Контроллер для управления всеми игровыми сессиями </summary>
[ApiVersion("1.0")]
[EnableCors(CorsPolicyNames.PublicPolicy)]
[EnableRateLimiting(RateLimitLabels.LimitPolicy)]
[ApiExplorerSettings(GroupName = ApiDomains.GameDomain)]
[Route($"{ApiRoutes.ApiPrefix}/{ApiRoutes.VersionPrefix}{{version:apiVersion}}/[controller]")]
public class GameSessionsController : BaseDbController
{
    private readonly IGameSessionService _gameSessionService;
    private readonly IStateMachineProvider _stateMachineProvider;
    private readonly IGameNotificationService _gameNotificationService;
    
    /// <summary> Конструктор контроллера </summary>
    /// <param name="options"> Конфигурация API </param>
    /// <param name="dbManager"> Менеджер для работы с БД </param>
    /// <param name="stateMachineProvider"> Поставщик машины состояний </param>
    /// <param name="gameNotificationService"> Сервис SignalR‑уведомлений </param>
    /// <param name="gameSessionService"> Хранилище текущей игровой сессии </param>
    /// <param name="logger"> Логгер </param>
    public GameSessionsController(
        IOptions<ApiOptions> options,
        IQuizardDbManager dbManager, 
        IGameSessionService gameSessionService,
        IStateMachineProvider stateMachineProvider,
        IGameNotificationService gameNotificationService,
        ILogger<GameSessionsController> logger) : base(dbManager, options, logger)
    {
        _gameSessionService = gameSessionService;
        _stateMachineProvider = stateMachineProvider;
        _gameNotificationService = gameNotificationService;
        
        Logger.LogDebug("Контроллер для управления всеми игровыми сессиями проинициализирован");
    }

    /// <summary> Метод для получения списка запущенных (не завершенных) игр в БД </summary>
    /// <returns> Список запущенных игр </returns>
    /// <response code="200"> Список игр успешно получен </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet, MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(Game[]),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetLaunchedGames() => await HandleRequest(async () => 
        Ok(await DbManager.GameRepository.GetGamesAsync(game => game.IsRunning == true).ConfigureAwait(false)))
        .ConfigureAwait(false);

    /// <summary> Метод для остановки всех запущенных игр </summary>
    /// <returns> Результат операции </returns>
    /// <response code="204"> Игры успешно остановлены </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpDelete("stopAll"), MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> StopGames() =>
        await HandleRequest(async () =>
        {
            Logger.LogDebug("Проверяем запущенные игры и все отключаем");
            await foreach (var pastGame in DbManager.GameRepository.IterateGamesAsync())
            {
                if (!pastGame.IsRunning) continue;
            
                Logger.LogWarning("Обнаружена не завершенная игровая сессия");
                await _gameNotificationService.ForceDisconnect().ConfigureAwait(false);
                await _gameSessionService.ResetSession().ConfigureAwait(false);
                await _stateMachineProvider.Destroy();
                
                pastGame.IsRunning = false;
                await DbManager.GameRepository
                    .UpdateGameAsync((Ulid)pastGame.Id!, pastGame)
                    .ConfigureAwait(false);
            }
            
            return NoContent();
        }).ConfigureAwait(false);
}