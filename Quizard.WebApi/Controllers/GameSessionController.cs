using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Quizard.Core.Entities;
using Quizard.Core.Interfaces;
using Quizard.Core.Models.Requests;
using Quizard.WebApi.Constants;
using Quizard.WebApi.Controllers.BaseControllers;
using Quizard.WebApi.Options;
using Quizard.WebApi.Routes;

namespace Quizard.WebApi.Controllers;

/// <summary> Контроллер для управления текущей игровой сессией </summary>
[ApiVersion("1.0")]
[EnableCors(CorsPolicyNames.PublicPolicy)]
[EnableRateLimiting(RateLimitLabels.LimitPolicy)]
[ApiExplorerSettings(GroupName = ApiDomains.GameDomain)]
[Route($"{ApiRoutes.ApiPrefix}/{ApiRoutes.VersionPrefix}{{version:apiVersion}}/[controller]")]
public class GameSessionController : BaseDbController
{
    private readonly IGameSessionService _gameSessionService;
    private readonly IGameLifecycleService _gameLifecycleService;

    /// <summary> Конструктор контроллера </summary>
    /// <param name="options"> Конфигурация API </param>
    /// <param name="dbManager"> Менеджер для работы с БД </param>
    /// <param name="gameSessionService"> Хранилище текущей игровой сессии </param>
    /// <param name="gameLifecycleService"> Менеджер управления жизненным циклом игры </param>
    /// <param name="logger"> Логгер </param>
    public GameSessionController(
        IOptions<ApiOptions> options,
        IQuizardDbManager dbManager,
        IGameSessionService gameSessionService,
        IGameLifecycleService gameLifecycleService,
        ILogger<GameSessionController> logger) : base(dbManager, options, logger)
    {
        _gameSessionService = gameSessionService;
        _gameLifecycleService = gameLifecycleService;

        Logger.LogDebug("Контроллер для управления игровой сессией проинициализирован");
    }

    /// <summary> Метод для получения статуса текущего игровой сессии </summary>
    /// <returns> Статус игры </returns>
    /// <response code="200"> Статус успешно получен </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("status"), MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetGameStatus() => 
        await HandleRequest(() => 
            Task.FromResult<IActionResult>(Ok(_gameSessionService.GetSession().CurrentState.ToString())))
            .ConfigureAwait(false);

    /// <summary> Метод для получения данных игрового сценария текущей игры </summary>
    /// <returns> Сценарий запущенной игры </returns>
    /// <response code="200"> Сценарий успешно получен </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="404"> Ошибка, если игра еще не запущена </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("scenario"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(Scenario), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetGameScenario() =>
        await HandleRequest(() => 
            Task.FromResult<IActionResult>(Ok(_gameSessionService.GetSession().Scenario))).ConfigureAwait(false);

    /// <summary> Метод для запуска новой игры с определенными параметрами </summary>
    /// <param name="request"> Запрос запуска новой игры </param>
    /// <returns> Уникальный идентификатор запущенной игры </returns>
    /// <response code="201"> Игра успешно создана </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="401"> Ошибка лицензирования </response>
    /// <response code="403"> Ошибка уровня лицензировании </response>
    /// <response code="409"> Ошибка, если игра уже запущена </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpPost("start"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> StartGame([FromBody] GameStartRequestDto request) =>
        await HandleRequest(async () =>
        {
            var gameId = await _gameLifecycleService.StartGameAsync(request).ConfigureAwait(false);
            
            return CreatedAtAction(nameof(GetGameScenario), GetRouteValues(gameId.ToString()), gameId);
        }).ConfigureAwait(false);
}