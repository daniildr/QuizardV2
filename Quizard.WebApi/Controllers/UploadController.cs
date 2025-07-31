using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;
using Quizard.Core.Interfaces;
using Quizard.WebApi.Constants;
using Quizard.WebApi.Controllers.BaseControllers;
using Quizard.WebApi.Options;
using Quizard.WebApi.Routes;

namespace Quizard.WebApi.Controllers;

/// <summary> Контроллер для загрузки игровых сценариев </summary>
[ApiVersion("1.0")]
[EnableCors(CorsPolicyNames.PublicPolicy)]
[EnableRateLimiting(RateLimitLabels.LimitPolicy)]
[ApiExplorerSettings(GroupName = ApiDomains.ScenariosDomain)]
[Route($"{ApiRoutes.ApiPrefix}/{ApiRoutes.VersionPrefix}{{version:apiVersion}}/[controller]")]
public class UploadController : BaseDbController
{
    private readonly IScenarioFacade _scenarioFacade;
    private readonly ILicensingService _licensingService;

    /// <summary> Конструктор контроллера </summary>
    /// <param name="scenarioFacade"> Менеджер для работы со сценариями </param>
    /// <param name="licensingService"> Сервис лицензирования </param>
    /// <param name="options"> Конфигурация API </param>
    /// <param name="dbManager"> Менеджер для работы с БД </param>
    /// <param name="logger"> Логгер </param>
    public UploadController(
        IScenarioFacade scenarioFacade,
        ILicensingService licensingService,
        IOptions<ApiOptions> options,
        IQuizardDbManager dbManager, 
        ILogger<UploadController> logger) : base(dbManager, options, logger)
    {
        _scenarioFacade = scenarioFacade;
        _licensingService = licensingService;
        
        Logger.LogDebug("Контроллер для загрузки игровых сценариев проинициализирован");
    }
    
    /// <summary> Метод для загрузки нового игрового сценария </summary>
    /// <param name="scenario"> Новый игровой сценарий </param>
    /// <returns> Созданный игровой сценарий </returns>
    /// <response code="201"> Сценарий создан </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="401"> Ошибка активации лицензии </response>
    /// <response code="403"> Ошибка уровня лицензии </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpPost, MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(Scenario),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> UploadScenario([FromBody] Scenario scenario) =>
        await HandleRequest(async () =>
        {
            if (!await _licensingService.CheckActiveLicense().ConfigureAwait(false))
                return InternalServerError(new LicenseValidationException("Проверка невозможна"));
            
            var newScenario = await _scenarioFacade.CreateScenarioAsync(scenario).ConfigureAwait(false);
            return Created(string.Empty, newScenario);
        }).ConfigureAwait(false);
}