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

/// <summary> Контроллер для работы с данными игроков </summary>
[ApiVersion("1.0")]
[EnableCors(CorsPolicyNames.PublicPolicy)]
[EnableRateLimiting(RateLimitLabels.PublicPolicy)]
[ApiExplorerSettings(GroupName = ApiDomains.GameDomain)]
[Route($"{ApiRoutes.ApiPrefix}/{ApiRoutes.VersionPrefix}{{version:apiVersion}}/[controller]")]
public class PlayerController : BaseDbController
{
    /// <summary> Конструктор контроллера </summary>
    /// <param name="options"> Конфигурация API </param>
    /// <param name="dbManager"> Менеджер для работы с БД </param>
    /// <param name="logger"> Логгер </param>
    public PlayerController(
        IOptions<ApiOptions> options,
        IQuizardDbManager dbManager, 
        ILogger<PlayerController> logger) : base(dbManager, options, logger)
    {
        Logger.LogDebug("Контроллер для работы с данными игроков проинициализирован");
    }

    /// <summary> Метод для проверки доступности никнейма </summary>
    /// <returns> Результат прореки </returns>
    /// <response code="200"> Никнейм доступен </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="409"> Никнейм занят </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("nickname/{nickname}"), MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CheckPlayerNickname([FromRoute] string nickname) => 
        await HandleRequest(async () => 
            await DbManager.PlayerRepository.IsNicknameTakenAsync(nickname) 
                ? Conflict() 
                : Ok()).ConfigureAwait(false);
}