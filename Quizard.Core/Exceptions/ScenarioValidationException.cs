namespace Quizard.Core.Exceptions;

/// <summary> Исключение, если сценарий не пройдет проверку консистентности </summary>
public class ScenarioValidationException : Exception
{
    /// <summary> Коллекция ошибок </summary>
    public string[] Errors { get; }

    /// <summary> Конструктор исключения </summary>
    /// <param name="msg"> Описание ошибки </param>
    public ScenarioValidationException(string msg) : base(msg)
    {
        Errors = [msg];
    }
    
    /// <summary> Конструктор исключения </summary>
    /// <param name="errors"> Найденные ошибки </param>
    public ScenarioValidationException(string[] errors)
        : base("Проверка сценария не пройдена:\n" + string.Join("\n", errors))
    {
        Errors = errors;
    }
}