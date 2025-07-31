using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quizard.Core.Exceptions;
using Quizard.Core.Interfaces;
using Quizard.WebApi.Options;

namespace Quizard.WebApi.Controllers.BaseControllers;

/// <summary> Базовый API контроллер взаимодействующий с БД </summary>[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
public abstract class BaseDbController : BaseController
{ 
    /// <summary> Менеджер для работы с БД </summary>
    protected readonly IQuizardDbManager DbManager;

    /// <summary> Конструктор контроллера </summary>
    /// <param name="dbManager"></param>
    /// <param name="apiOptions"> Конфигурация API </param>
    /// <param name="logger"> Логгер </param>
    protected BaseDbController(
        IQuizardDbManager dbManager,
        IOptions<ApiOptions> apiOptions,
        ILogger<BaseDbController> logger) : base(apiOptions, logger)
    {
        DbManager = dbManager;
    }

    /// <summary> Обработчик запросов контроллера, работающего с менеджером БД </summary>
    /// <param name="action"> Функция </param>
    /// <returns> Результат выполнения функции </returns>
    protected async Task<IActionResult> HandleRequest(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (FormatException formatException)
        {
            return BadRequest(formatException);
        }
        catch (LicenseValidationException licenseValidationException)
        {
            return BadRequest(licenseValidationException);
        }
        catch (ChecksumValidationException checksumValidationException)
        {
            return BadRequest(checksumValidationException);
        }
        catch (ScenarioValidationException scenarioValidationException)
        {
            return BadRequest(scenarioValidationException);
        }
        catch (ActiveLicenseNotFoundException activeLicenseNotFound)
        {
            return Unauthorized(activeLicenseNotFound);
        }
        catch (LicensePermissionLevelException licensePermissionLevelException)
        {
            return Forbidden(licensePermissionLevelException);
        }
        catch (AnswerDoesNotExistException answerDoesNotExistException)
        {
            return NotFound(answerDoesNotExistException);
        }
        catch (LicenseSecretDoesNotExistException licenseSecretDoesNotExistException)
        {
            return NotFound(licenseSecretDoesNotExistException);
        }
        catch (ScenarioDoesNotExistException scenarioDoesNotExistException)
        {
            return NotFound(scenarioDoesNotExistException);
        }
        catch (ContentChecksumDoesNotExistException contentChecksumDoesNotExistException)
        {
            return NotFound(contentChecksumDoesNotExistException);
        }
        catch (LicenseDoesNotExistException licenseDoesNotExistException)
        {
            return NotFound(licenseDoesNotExistException);
        }
        catch (MediaDoesNotExistException mediaDoesNotExistException)
        {
            return NotFound(mediaDoesNotExistException);
        }
        catch (PlayerDoesNotExistException playerDoesNotExistException)
        {
            return NotFound(playerDoesNotExistException);
        }
        catch (QuestionDoesNotExistException questionDoesNotExistException)
        {
            return NotFound(questionDoesNotExistException);
        }
        catch (RoundDoesNotExistException roundDoesNotExistException)
        {
            return NotFound(roundDoesNotExistException);
        }
        catch (GameSessionDoesNotExistException)
        {
            return NotFound("Игровая сессия еще не начата");
        }
        catch (LicenseAlreadyExistException)
        {
            return Conflict("Лицензия уже была активирована ранее");
        }
        catch (ContentChecksumAlreadyExistException contentChecksumAlreadyExistException)
        {
            return Conflict(contentChecksumAlreadyExistException);
        }
        catch (LicenseSecretAlreadyExistException licenseSecretAlreadyExistException)
        {
            return Conflict(licenseSecretAlreadyExistException);
        }
        catch (GameAlreadyRunning gameAlreadyRunning)
        {
            return Conflict(gameAlreadyRunning);
        }
        catch (DatabaseUnavailableException databaseUnavailableException)
        {
            return ServiceUnavailable(databaseUnavailableException);
        }
        catch (Exception e)
        {
            return InternalServerError(e);
        }
    }
}