using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Quizard.WebApi.Constants;
using Quizard.WebApi.Controllers.BaseControllers;
using Quizard.WebApi.Options;
using Quizard.WebApi.Routes;

namespace Quizard.WebApi.Controllers;

/// <summary> Контроллер дляполучения справочной информации </summary>
[ApiVersion("1.0")]
[EnableCors(CorsPolicyNames.PublicPolicy)]
[EnableRateLimiting(RateLimitLabels.PublicPolicy)]
[ApiExplorerSettings(GroupName = ApiDomains.HelpDeskDomain)]
[Route($"{ApiRoutes.ApiPrefix}/{ApiRoutes.VersionPrefix}{{version:apiVersion}}/[controller]")]
public class HelpController : BaseDbController
{
    private readonly IWebHostEnvironment _env;

    /// <summary> Конструктор контроллера </summary>
    /// <param name="env"> Веб-хост окружение </param>
    /// <param name="options"> Конфигурация API </param>
    /// <param name="dbManager"> Менеджер для работы с БД </param>
    /// <param name="logger"> Логгер </param>
    public HelpController(
        IWebHostEnvironment env,
        IOptions<ApiOptions> options,
        IQuizardDbManager dbManager,
        ILogger<HelpController> logger) : base(dbManager, options, logger)
    {
        _env = env;
        Logger.LogDebug("Контроллер для управления лицензией проинициализирован");
    }
    
    /// <summary> Метод для получения расположения папки wwwroot на файловой системе </summary>
    /// <returns> Адрес папки </returns>
    /// <response code="200"> Справка получена </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов</response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("wwwroot-folder"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(RoundType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public Task<IActionResult> GetWwwrootFolder() => Task.FromResult<IActionResult>(Ok(_env.WebRootPath));

    /// <summary> Метод для получения справки по доступным типам игровых стадий </summary>
    /// <returns> Справка </returns>
    /// <response code="200"> Справка получена </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("game-states"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public Task<IActionResult> GetGameState() =>
        Task.FromResult<IActionResult>(Ok(Enum.GetValues<GameState>()
            .ToDictionary(type => type.ToString(), type => (int) type)));

    /// <summary> Метод для получения справки по доступным типам игровых триггеров </summary>
    /// <returns> Справка </returns>
    /// <response code="200"> Справка получена </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("game-triggers"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public Task<IActionResult> GetGameTrigger() =>
        Task.FromResult<IActionResult>(Ok(Enum.GetValues<GameTrigger>()
            .ToDictionary(type => type.ToString(), type => (int) type)));

    /// <summary> Метод для получения справки по доступным типам медиа </summary>
    /// <returns> Справка </returns>
    /// <response code="200"> Справка получена </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("media-types"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public Task<IActionResult> GetMediaTypes() =>
        Task.FromResult<IActionResult>(Ok(Enum.GetValues<MediaType>()
            .ToDictionary(type => type.ToString(), type => (int) type)));

    /// <summary> Метод для получения справки по доступным типам модификаторов (предметов) </summary>
    /// <returns> Справка </returns>
    /// <response code="200"> Справка получена </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("modifier-types"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public Task<IActionResult> GetModifierTypes() =>
        Task.FromResult<IActionResult>(Ok(Enum.GetValues<ModifierType>()
            .ToDictionary(type => type.ToString(), type => (int) type)));

    /// <summary> Метод для получения справки по доступным типам рандов </summary>
    /// <returns> Справка </returns>
    /// <response code="200"> Справка получена </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов</response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("round-types"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(RoundType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetRoundTypes() =>
        await HandleRequest(async () =>
                Ok(await DbManager.RoundTypeRepository.GetAllRoundTypes().ConfigureAwait(false)))
            .ConfigureAwait(false);

    /// <summary> Метод для получения справки по доступным типам стадий сценария </summary>
    /// <returns> Справка </returns>
    /// <response code="200"> Справка получена </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов</response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("scenario-stages"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(RoundType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetScenarioStages() =>
        await HandleRequest(async () =>
                Ok(await DbManager.RoundTypeRepository.GetAllRoundTypes().ConfigureAwait(false)))
            .ConfigureAwait(false);
}