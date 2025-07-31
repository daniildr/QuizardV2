using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Extensions;

namespace Quizard.Core.Exceptions;

/// <summary> Исключение, вызываемое при отсутствии записи секретных данных </summary>
public class LicenseSecretDoesNotExistException : Exception
{
    /// <summary> Исключение вызываемое при отсутствии секретных данных </summary>
    /// <param name="msg"> Описание ошибки </param>
    public LicenseSecretDoesNotExistException(string msg) : base(msg) { }

    /// <summary> Исключение вызываемое при отсутствии секретных данных </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    public LicenseSecretDoesNotExistException(Expression<Func<LicenseSecret, bool>> predicate)
        : base($"Секретные данные лицензии не найдены {predicate.ToReadableString()}") { }
}
