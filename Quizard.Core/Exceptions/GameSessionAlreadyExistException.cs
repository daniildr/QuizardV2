namespace Quizard.Core.Exceptions;

/// <summary> Кастомное исключение, выбрасывается при попытке сохранить "вторую" игровую сессию </summary>
public class GameSessionAlreadyExistException : Exception;