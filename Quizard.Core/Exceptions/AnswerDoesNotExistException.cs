using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Extensions;

namespace Quizard.Core.Exceptions;

/// <summary> Исключение, вызываемое при отсутствии ответа </summary>
public class AnswerDoesNotExistException : Exception
{
    /// <summary> Исключение, вызываемое при отсутствии ответа </summary>
    /// <param name="msg"> Описание ошибки </param>
    public AnswerDoesNotExistException(string msg) : base(msg) { }

    /// <summary> Исключение вызываемое при отсутствии сценария </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    public AnswerDoesNotExistException(Expression<Func<Answer, bool>> predicate)
        : base($"Данные ответа не найдены {predicate.ToReadableString()}") { }
}