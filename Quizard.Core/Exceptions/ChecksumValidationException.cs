namespace Quizard.Core.Exceptions;

/// <summary>
/// Кастомное исключение, которое выбрасывается при не валидной контрольной сумме
/// </summary>
public class ChecksumValidationException : Exception
{
    /// <summary> Кастомное исключение, которое выбрасывается при не валидной контрольной сумме </summary>
    /// <param name="msg"> Описание ошибки </param>
    public ChecksumValidationException(string msg) : base(msg) { }
}