using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Quizard.Core.Entities;
using Quizard.Core.Interfaces;
using Quizard.WebApi.Constants;
using Quizard.WebApi.Controllers.BaseControllers;
using Quizard.WebApi.Options;
using Quizard.WebApi.Routes;

namespace Quizard.WebApi.Controllers;

/// <summary> Контроллер для управления игровыми сценариями </summary>
[ApiVersion("1.0")]
[EnableCors(CorsPolicyNames.PublicPolicy)]
[EnableRateLimiting(RateLimitLabels.LimitPolicy)]
[ApiExplorerSettings(GroupName = ApiDomains.ScenariosDomain)]
[Route($"{ApiRoutes.ApiPrefix}/{ApiRoutes.VersionPrefix}{{version:apiVersion}}/[controller]")]
public class ScenariosController : BaseDbController
{
    /// <summary> Конструктор контроллера </summary>
    /// <param name="options"> Конфигурация API </param>
    /// <param name="dbManager"> Менеджер для работы с БД </param>
    /// <param name="logger"> Логгер </param>
    public ScenariosController(
        IOptions<ApiOptions> options,
        IQuizardDbManager dbManager,
        ILogger<ScenariosController> logger) : base(dbManager, options, logger)
    {
        Logger.LogDebug("Контроллер для управления лицензией проинициализирован");
    }

    /// <summary> Метод для получения всех доступных игровых сценариев </summary>
    /// <returns> Коллекция доступных сценариев </returns>
    /// <response code="200"> Сценарии получены </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet, MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(Scenario[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetAllAvailableScenarios() =>
        await HandleRequest(async () => Ok((await DbManager.ScenarioRepository
                    .GetScenariosAsync()
                    .ConfigureAwait(false))
                .ToArray())).ConfigureAwait(false);

    /// <summary> Метод для получения конкретного игрового сценария </summary>
    /// <param name="scenarioId"> Уникальный идентификатор сценария </param>
    /// <returns> Сценарий </returns>
    /// <response code="200"> Сценарий найден </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="404"> Сценарий не найден </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов</response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("{scenarioId}"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(Scenario), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetScenario([FromRoute] string scenarioId) =>
        await HandleRequest(async () => Ok(await DbManager.ScenarioRepository
                .GetScenarioAsync(scenarioId)
                .ConfigureAwait(false)))
            .ConfigureAwait(false);
}