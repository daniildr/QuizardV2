namespace Quizard.Core.Exceptions;

/// <summary> Исключение, вызываемое при попытке добавить контрольную сумму, которая уже существует </summary>
public class ContentChecksumAlreadyExistException : Exception
{
    /// <summary> Исключение, вызываемое при попытке добавить контрольную сумму, которая уже существует </summary>
    /// <param name="msg"> Описание ошибки </param>
    public ContentChecksumAlreadyExistException(string msg) : base(msg) { }
}