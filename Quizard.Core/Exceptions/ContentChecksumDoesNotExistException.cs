namespace Quizard.Core.Exceptions;

/// <summary> Исключение вызываемое при отсутствии контрольной суммы </summary>
public class ContentChecksumDoesNotExistException : Exception
{
    /// <summary> Исключение вызываемое при отсутствии контрольной суммы </summary>
    /// <param name="msg"> Описание ошибки </param>
    public ContentChecksumDoesNotExistException(string msg) : base(msg) { }
}