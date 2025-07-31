namespace Quizard.Core.Exceptions;

/// <summary> Исключение, вызываемое при попытке добавить лицензию, которая уже существует </summary>
public class LicenseAlreadyExistException : Exception
{
    /// <summary> Исключение, вызываемое при попытке добавить лицензию, которая уже существует </summary>
    /// <param name="msg"> Описание ошибки </param>
    public LicenseAlreadyExistException(string msg) : base(msg) { }
}