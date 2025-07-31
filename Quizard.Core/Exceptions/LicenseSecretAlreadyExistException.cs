namespace Quizard.Core.Exceptions;

/// <summary> Исключение, вызываемое при попытке добавить записи секретных данных, которая уже существует </summary>
public class LicenseSecretAlreadyExistException : Exception
{
    /// <summary> Исключение, вызываемое при попытке добавить записи секретных данных, которая уже существует </summary>
    /// <param name="msg"> Описание ошибки </param>
    public LicenseSecretAlreadyExistException(string msg) : base(msg) { }
}