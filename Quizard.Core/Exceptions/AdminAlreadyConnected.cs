namespace Quizard.Core.Exceptions;

/// <summary>
/// Кастомное исключение, выбрасываемое, если будет выполнена попытка подключить дополнительного администратора
/// </summary>
public class AdminAlreadyConnected : Exception;