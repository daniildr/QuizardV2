using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Extensions;

namespace Quizard.Core.Exceptions;

/// <summary> Исключение вызываемое при отсутствии игры </summary>
public class GameDoesNotExistException : Exception
{
    /// <summary> Исключение вызываемое при отсутствии игры </summary>
    /// <param name="msg"> Описание ошибки </param>
    public GameDoesNotExistException(string msg) : base(msg) { }

    /// <summary> Исключение вызываемое при отсутствии игры </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    public GameDoesNotExistException(Expression<Func<Game, bool>> predicate)
        : base($"Данные игровой сессии не найдены {predicate.ToReadableString()}") { }
}