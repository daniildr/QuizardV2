using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Extensions;

namespace Quizard.Core.Exceptions;

/// <summary> Исключение вызываемое при отсутствии вопроса </summary>
public class QuestionDoesNotExistException : Exception
{
    /// <summary> Исключение вызываемое при отсутствии вопроса </summary>
    /// <param name="msg"> Описание ошибки </param>
    public QuestionDoesNotExistException(string msg) : base(msg) { }

    /// <summary> Исключение вызываемое при отсутствии вопроса </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    public QuestionDoesNotExistException(Expression<Func<Question, bool>> predicate)
        : base($"Данные вопроса не найдены {predicate.ToReadableString()}") { }
}