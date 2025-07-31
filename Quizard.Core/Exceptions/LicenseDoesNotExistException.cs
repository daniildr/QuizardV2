using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Extensions;

namespace Quizard.Core.Exceptions;

/// <summary> Исключение, вызываемое при отсутствии записи лицензии </summary>
public class LicenseDoesNotExistException : Exception
{
    /// <summary> Исключение вызываемое при отсутствии лицензии </summary>
    /// <param name="msg"> Описание ошибки </param>
    public LicenseDoesNotExistException(string msg) : base(msg) { }

    /// <summary> Исключение вызываемое при отсутствии лицензии </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    public LicenseDoesNotExistException(Expression<Func<License, bool>> predicate)
        : base($"Данные лицензии не найдены {predicate.ToReadableString()}") { }
}