using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quizard.WebApi.Options;

namespace Quizard.WebApi.Controllers.BaseControllers;

/// <summary> Базовый API контроллер </summary>
[ApiController]
public class BaseController : ControllerBase
{
    /// <summary> Логгер </summary>
    protected readonly ILogger<BaseDbController> Logger;
    
    /// <summary> Конфигурация API </summary>
    protected readonly ApiOptions Options;

    /// <summary> Конструктор контроллера </summary>
    /// <param name="options"> Конфигурация API </param>
    /// <param name="logger"> Логгер </param>
    public BaseController(IOptions<ApiOptions> options, ILogger<BaseDbController> logger)
    {
        Options = options.Value;
        Logger = logger;
    }

    /// <summary> Создает <see cref="ObjectResult"/> с 500 ошибкой (InternalServerError) </summary>
    /// <param name="exception"> Исключение, которое будет передано пользователю </param>
    /// <returns> InternalServerError <see cref="ObjectResult"/> </returns>
    protected ObjectResult InternalServerError(Exception exception) => 
        LogAndReturnError(exception, "Внутренняя ошибка сервер", StatusCodes.Status500InternalServerError);

    /// <summary> Создает <see cref="ObjectResult"/> с 503 ошибкой (ServiceUnavailable) </summary>
    /// <param name="exception"> Исключение, которое будет передано пользователю </param>
    /// <returns> ServiceUnavailable <see cref="ObjectResult"/> </returns>
    protected ObjectResult ServiceUnavailable(Exception exception) => 
        LogAndReturnError(exception, "Ошибка доступности сервера", StatusCodes.Status503ServiceUnavailable);

    /// <summary> Создает <see cref="ObjectResult"/> с 403 ошибкой (Forbidden) </summary>
    /// <param name="exception"> Исключение, которое будет передано пользователю </param>
    /// <returns> Forbidden <see cref="ObjectResult"/> </returns>
    protected ObjectResult Forbidden(Exception exception) => 
        LogAndReturnError(exception, "Ошибка уровня доступа пользователя", StatusCodes.Status403Forbidden);

    /// <summary> Метод получения данных маршрута для запроса получения сущности, после ее создания </summary>
    /// <param name="objectId"> Id созданной сущности</param>
    /// <returns> Анонимный тип <code>new { id = objectId }</code></returns>
    protected static object GetRouteValues(string objectId) => new { id = objectId };

    private ObjectResult LogAndReturnError(Exception exception, string errorMessage, int statusCode)
    {
        Logger.LogError("{Message}. Ошибка {@Exception}", errorMessage, exception);
        return StatusCode(statusCode, exception.Message);
    }
    
}