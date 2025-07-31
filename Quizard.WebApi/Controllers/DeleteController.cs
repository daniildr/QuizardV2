using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Quizard.Core.Interfaces;
using Quizard.WebApi.Constants;
using Quizard.WebApi.Controllers.BaseControllers;
using Quizard.WebApi.Options;
using Quizard.WebApi.Routes;

namespace Quizard.WebApi.Controllers;

/// <summary> Контроллер для удаления игровых сценариев </summary>
[ApiVersion("1.0")]
[EnableCors(CorsPolicyNames.PublicPolicy)]
[EnableRateLimiting(RateLimitLabels.LimitPolicy)]
[ApiExplorerSettings(GroupName = ApiDomains.ScenariosDomain)]
[Route($"{ApiRoutes.ApiPrefix}/{ApiRoutes.VersionPrefix}{{version:apiVersion}}/[controller]")]
public class DeleteController : BaseDbController
{
    private readonly IScenarioFacade _scenarioFacade;

    /// <summary> Конструктор контроллера </summary>
    /// <param name="scenarioFacade"> Менеджер сценариев </param>
    /// <param name="options"> Конфигурация API </param>
    /// <param name="dbManager"> Менеджер для работы с БД </param>
    /// <param name="logger"> Логгер </param>
    public DeleteController(
        IScenarioFacade scenarioFacade,
        IOptions<ApiOptions> options,
        IQuizardDbManager dbManager, 
        ILogger<DeleteController> logger) : base(dbManager, options, logger)
    {
        _scenarioFacade = scenarioFacade;
        
        Logger.LogDebug("Контроллер для удаления игровых сценариев проинициализирован");
    }

    /// <summary> Метод для удаления определенного игрового сценария </summary>
    /// <param name="scenarioId"> Уникальный идентификатор сценария </param>
    /// <returns> Результат операции </returns>
    /// <response code="204"> Сценарий удален </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="403"> Ошибка уровня лицензии </response>
    /// <response code="404"> Сценарий не найден </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов</response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpDelete("{scenarioId}"), MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> DeleteScenario([FromRoute] string scenarioId) =>
        await HandleRequest(async () =>
        {
            var scenario = await DbManager.ScenarioRepository.GetScenarioAsync(scenarioId).ConfigureAwait(false);
            await _scenarioFacade.DeleteScenarioAsync(scenario.Id).ConfigureAwait(false);
            
            return NoContent();
        }).ConfigureAwait(false);
}