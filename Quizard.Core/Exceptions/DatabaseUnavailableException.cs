namespace Quizard.Core.Exceptions;

/// <summary> Кастомное исключение на случай отсутствия подключения к БД </summary>
public class DatabaseUnavailableException : Exception
{
    /// <summary> Кастомное исключение на случай отсутствия подключения к БД </summary>
    public DatabaseUnavailableException()
        : base("База данных недоступна") { }
    
    /// <summary> Кастомное исключение на случай отсутствия подключения к БД </summary>
    /// <param name="innerException"> Вложенное исключение </param>
    public DatabaseUnavailableException(Exception innerException)
        : base("База данных недоступна", innerException) { }

    /// <summary> Кастомное исключение на случай отсутствия подключения к БД </summary>
    /// <param name="message"> Кастомное сообщение </param>
    public DatabaseUnavailableException(string message)
        : base($"База данных недоступна. {message}") { }

    /// <summary> Кастомное исключение на случай отсутствия подключения к БД </summary>
    /// <param name="message"> Кастомное сообщение </param>
    /// <param name="innerException"> Вложенное исключение </param>
    public DatabaseUnavailableException(string message, Exception innerException)
        : base($"База данных недоступна. {message}", innerException) { }
}