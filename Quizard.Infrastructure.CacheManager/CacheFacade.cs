using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Quizard.Core.Exceptions;
using Quizard.Core.Interfaces;

namespace Quizard.Infrastructure.CacheManager;

/// <inheritdoc/>
public class CacheFacade : ICacheManager
{
    private readonly TimeSpan _cacheLifeTime;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheFacade> _logger;
    private readonly HashSet<string> _usedKeys = [];
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    

    /// <summary> Конструктор менеджера </summary>
    /// <param name="memoryCache"> Кеш в памяти </param>
    /// <param name="logger"> Логгер </param>
    public CacheFacade(IMemoryCache memoryCache, ILogger<CacheFacade> logger)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _cacheLifeTime = TimeSpan.FromHours(3);
        
        _logger.LogInformation("Менеджер кеширования проинициализирован");
    }
    
    /// <inheritdoc/>
    public async Task<(bool result, T? value)> TryGetValueAsync<T>(string key)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new CacheKeyNullException(nameof(key));
        
            var exists = _memoryCache.TryGetValue(key, out T? value);
            
            _logger.LogTrace(
                exists 
                    ? "{Key} найден в кеше" 
                    : "{Key} отсутствует в кеше", 
                key);
            
            return (exists, value);

        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateAsync<T>(string key, T value)
    {
        await _semaphore.WaitAsync();
        try
        {
            ValidateKeyAndValue(key, value);
            
            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = _cacheLifeTime,
                Size = CalculateSize(value)
            };

            _memoryCache.Set(key, value, options);
            _usedKeys.Add(key);
            
            _logger.LogDebug("Значение с ключом '{Key}' добавлено в кеш.", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении значения с ключом '{Key}' в кеш.", key);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc/>
    public async Task<string[]> GetAllCachedKeysAsync()
    {
        _logger.LogDebug("Выполняется получение коллекции всех использованных ключей");
        await _semaphore.WaitAsync();
        try
        {
            return _usedKeys.ToArray();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    /// <inheritdoc/>
    public async Task ClearAllCacheAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            _logger.LogDebug("Полный сброс кеша");
            
            foreach (var key in _usedKeys.ToArray())
            {
                await RemoveKeyAsync(key);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc/>
    public async Task ClearAllCacheAsync(string key)
    {
        _logger.LogTrace("Сбрасываем кеш по ключу {Key}", key);
        await _semaphore.WaitAsync();
        try
        {
            await RemoveKeyAsync(key);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private Task RemoveKeyAsync(string key)
    {
        try
        {
            _memoryCache.Remove(key);
            _usedKeys.Remove(key);
            _logger.LogDebug("Кеш {Key} удалён", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка удаления кеша {Key}", key);
            throw;
        }

        return Task.CompletedTask;
    }
    
    private static void ValidateKeyAndValue<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new CacheKeyNullException(nameof(key));

        ArgumentNullException.ThrowIfNull(value, nameof(value));
    }
    
    private static long CalculateSize<T>(T value)
    {
        try
        {
            return JsonSerializer.Serialize(value).Length;
        }
        catch
        {
            return 1;
        }
    }
}