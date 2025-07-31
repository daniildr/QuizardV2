namespace Quizard.Core.Interfaces;

/// <summary> Интерфейс для работы с менеджером кеширования </summary>
public interface ICacheManager
{
    /// <summary> Метод для получения значения по ключу </summary>
    /// <param name="key"> Ключ </param>
    /// <typeparam name="T"> Тип значения </typeparam>
    /// <returns> Кортеж <see cref="Tuple"/>. Результат получения объекта </returns>
    /// <remarks>
    /// Если ключ существует, то <c>true</c>, если нет - <c>false</c>.
    /// Полученное из кеша значение может быть <c>null</c>
    /// </remarks>
    /// <exception cref="ArgumentException"> Исключение, если key равен null или пустой строке </exception>
    public Task<(bool result, T? value)> TryGetValueAsync<T>(string key);

    /// <summary> Метод для сохранения пары ключ-значение </summary>
    /// <remarks> Если ключ уже существует, то значение будет перезаписано </remarks>
    /// <param name="key"> Ключ </param>
    /// <param name="value"> Значение </param>
    /// <typeparam name="T"> Тип значения </typeparam>
    /// <returns> Асинхронная операция </returns>
    /// <exception cref="ArgumentException"> Исключение, если key равен null, или value равно null </exception>
    public Task AddOrUpdateAsync<T>(string key, T value);

    /// <summary> Метод для получения всех закешированных ключей </summary>
    /// <returns> Массив ключей </returns>
    public Task<string[]> GetAllCachedKeysAsync();

    /// <summary> Метод для сброса всех за кешированных данных </summary>
    /// <returns> Асинхронная операция </returns>
    public Task ClearAllCacheAsync();

    /// <summary> Метод для сброса всех за кешированных данных по определенному ключу </summary>
    /// <param name="key"> Ключ </param>
    /// <returns> Асинхронная операция </returns>
    public Task ClearAllCacheAsync(string key);
}