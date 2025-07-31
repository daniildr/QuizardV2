using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Extensions;

namespace Quizard.Core.Exceptions;

/// <summary> Исключение вызываемое при отсутствии медиа </summary>
public class MediaDoesNotExistException : Exception
{
    /// <summary> Исключение вызываемое при отсутствии медиа </summary>
    /// <param name="msg"> Описание ошибки </param>
    public MediaDoesNotExistException(string msg) : base(msg) { }

    /// <summary> Исключение вызываемое при отсутствии медиа </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    public MediaDoesNotExistException(Expression<Func<Media, bool>> predicate)
        : base($"Данные медиа не найдены {predicate.ToReadableString()}") { }
}