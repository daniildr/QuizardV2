using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Extensions;

namespace Quizard.Core.Exceptions;

/// <summary> Исключение, вызываемое при отсутствии сценария </summary>
public class ScenarioDoesNotExistException : Exception
{
    /// <summary> Исключение вызываемое при отсутствии сценария </summary>
    /// <param name="msg"> Описание ошибки </param>
    public ScenarioDoesNotExistException(string msg) : base(msg) { }

    /// <summary> Исключение вызываемое при отсутствии сценария </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    public ScenarioDoesNotExistException(Expression<Func<Scenario, bool>> predicate)
        : base($"Данные сценария не найдены {predicate.ToReadableString()}") { }
}