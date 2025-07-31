using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Quizard.Core.Entities;
using Quizard.Core.Interfaces;
using Quizard.Core.Models.Responses;
using Quizard.WebApi.Constants;
using Quizard.WebApi.Controllers.BaseControllers;
using Quizard.WebApi.Options;
using Quizard.WebApi.Routes;

namespace Quizard.WebApi.Controllers;

/// <summary> Контроллер для управления игровой сессией </summary>
[ApiVersion("1.0")]
[EnableCors(CorsPolicyNames.PublicPolicy)]
[EnableRateLimiting(RateLimitLabels.LimitPolicy)]
[ApiExplorerSettings(GroupName = ApiDomains.LicensingDomain)]
[Route($"{ApiRoutes.ApiPrefix}/{ApiRoutes.VersionPrefix}{{version:apiVersion}}/[controller]")]
public class LicenseController : BaseDbController
{
    private readonly ILicensingService _licensingService;

    /// <summary> Конструктор контроллера </summary>
    /// <param name="licensingService"> Сервис лицензирования </param>
    /// <param name="dbManager"> Менеджер для работы с БД </param>
    /// <param name="options"> Конфигурация API </param>
    /// <param name="logger"> Логгер </param>
    public LicenseController(
        ILicensingService licensingService,
        IQuizardDbManager dbManager, 
        IOptions<ApiOptions> options,
        ILogger<LicenseController> logger) : base(dbManager, options, logger)
    {
        _licensingService = licensingService;
        Logger.LogDebug("Контроллер для управления лицензией проинициализирован");
    }

    /// <summary> Метод для генерации секретной соли, которая необходима для создания лицензии </summary>
    /// <returns> Секретная соль </returns>
    /// <response code="201"> Секретная соль сгенерирована </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("salt"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(string),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GenerateSecretSalt() =>
        await HandleRequest(async () => 
            Created(string.Empty, await _licensingService.GenerateSecretSalt())).ConfigureAwait(false);

    /// <summary> Метод для получения информации об используемой лицензии </summary>
    /// <returns> Данные лицензии </returns>
    /// <response code="200"> Лицензия успешно найдена </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="401"> Активная лицензия отсутствует </response>
    /// <response code="404"> Ошибка в файле (или его целостности) лицензии </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet("status"), MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(bool),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetLicenseStatus() => 
        await HandleRequest(async () => Ok(await _licensingService.CheckActiveLicense())).ConfigureAwait(false);
    
    /// <summary> Метод для получения информации о зарегистрированных лицензиях </summary>
    /// <returns> Коллекция лицензий лицензии </returns>
    /// <response code="200"> Лицензии успешно найдена </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpGet, MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(License),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetLicenses() => 
        await HandleRequest(async () =>
            Ok(await DbManager.LicenseRepository.GetLicenseAsync(license => license.Active == true, false)
                .ConfigureAwait(false)))
            .ConfigureAwait(false);

    /// <summary> Метод для активации лицензии </summary>
    /// <param name="license"> Лицензионный ключ </param>
    /// <returns> Данные лицензионного ключа </returns>
    /// <response code="201"> Лицензия успешно активирована </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="409"> Лицензия уже была активирована ранее </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpPost("upload"), MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(License),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> SubmitLicense([FromBody] UploadLicenseDto license) =>
        await HandleRequest(async () =>
        {
            try
            {
                await _licensingService.CheckActiveLicense();
                return Conflict("Лицензия уже активирована");
            }
            catch (Exception)
            {
                Logger.LogInformation("Начинаем активацию лицензии {@LicenseKey}", license);

                await _licensingService.UploadLicenseKey(license.LicenseKey);

                Logger.LogInformation("Лицензия {LicenseKey} успешно активирована", license);
                return Created();
            }
        }).ConfigureAwait(false);

    /// <summary> Метод для деактивации лицензии </summary>
    /// <param name="licenseId"> Уникальный идентификатор лицензии </param>
    /// <returns> Результат операции </returns>
    /// <response code="204"> Лицензия успешно деактивирована </response>
    /// <response code="400"> Ошибка в параметрах запроса </response>
    /// <response code="409"> Лицензия уже была удалена ранее </response>
    /// <response code="429"> Ошибка, если будет превышен лимит запросов </response>
    /// <response code="500"> Ошибка сервера </response>
    /// <response code="503"> Ошибка доступности сервисов </response>
    [HttpDelete, MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> DeactivateLicense([FromBody] long licenseId) =>
        await HandleRequest(async () =>
        {
            await DbManager.LicenseRepository.DeactivateLicenseAsync(licenseId).ConfigureAwait(false);
            return NoContent();
        }).ConfigureAwait(false);
}