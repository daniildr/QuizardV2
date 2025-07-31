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

/// <summary> Контроллер для работы со статистикой раундов </summary>
[ApiVersion("1.0")]
[EnableCors(CorsPolicyNames.PublicPolicy)]
[EnableRateLimiting(RateLimitLabels.PublicPolicy)]
[ApiExplorerSettings(GroupName = ApiDomains.StatisticsDomain)]
[Route($"{ApiRoutes.ApiPrefix}/{ApiRoutes.VersionPrefix}{{version:apiVersion}}/[controller]/{{roundId}}")]
public class RoundStatisticsController : BaseDbController
{
    /// <summary> Конструктор контроллера </summary>
    /// <param name="options"> Конфигурация API </param>
    /// <param name="dbManager"> Менеджер для работы с БД </param>
    /// <param name="logger"> Логгер </param>
    public RoundStatisticsController(
        IOptions<ApiOptions> options,
        IQuizardDbManager dbManager,
        ILogger<RoundStatisticsController> logger) : base(dbManager, options, logger)
    {
        Logger.LogDebug("Контроллер для работы со статистикой раундов проинициализирован");
    }

    /// <summary> Метод для запроса статистики для определенного игрового раунда </summary>
    /// <param name="gameId"> Уникальный идентификатор игры </param>
    /// <param name="roundId"> <see cref="Ulid"/>. Идентификатор раунда </param>
    /// <param name="playerId"> Уникальный идентификатор игрока </param>
    /// <returns> Статистические данные </returns>
    /// <response code="200"> Данные успешно получены </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet, MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(RoundStatistic[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetRoundStats(
        [FromRoute] string gameId,
        [FromRoute] string roundId,
        [FromRoute] string playerId) =>
        await HandleRequest(async () =>
            Ok(await DbManager.StatisticRepository
                .GetRoundStatistic(roundStatistic => roundStatistic.GameId == Ulid.Parse(gameId)
                                                     && roundStatistic.RoundId == roundId
                                                     && roundStatistic.PlayerId == Ulid.Parse(playerId))
                .ConfigureAwait(false))).ConfigureAwait(false);
}