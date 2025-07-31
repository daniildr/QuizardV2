using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Extensions;

namespace Quizard.Core.Exceptions;

/// <summary> Исключение, вызываемое при отсутствии раунда </summary>
public class RoundDoesNotExistException : Exception
{
    /// <summary> Исключение вызываемое при отсутствии раунда </summary>
    /// <param name="msg"> Описание ошибки </param>
    public RoundDoesNotExistException(string msg) : base(msg) { }

    /// <summary> Исключение вызываемое при отсутствии раунда </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    public RoundDoesNotExistException(Expression<Func<Round, bool>> predicate)
        : base($"Данные раунда не найдены {predicate.ToReadableString()}") { }
}