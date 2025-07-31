namespace Quizard.Core.Exceptions;

/// <summary>
/// Кастомное исключение, на случай если запрашиваемый в кеше ключ будет null или пустой строкой
/// </summary>
/// <param name="key"> Ключ </param>
public class CacheKeyNullException(string key) 
    : ArgumentException("Ключ не может быть пустым или содержать только пробелы", key);