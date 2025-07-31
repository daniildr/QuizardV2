namespace Quizard.Core.Exceptions;

/// <summary> Кастомное исключение, которое выбрасывается при не валидной лицензии </summary>
public class LicenseValidationException : Exception
{
    /// <summary> Кастомное исключение, которое выбрасывается при не валидной лицензии </summary>
    /// <param name="msg"> Описание ошибки </param>
    public LicenseValidationException(string msg) : base(msg) { }
}