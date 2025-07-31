namespace Quizard.Core.Exceptions;

/// <summary> Выбрасывается, если уровень доступа лицензии недостаточный для операции </summary>
/// <param name="msg"></param>
public class LicensePermissionLevelException(string msg) : Exception(msg) { }