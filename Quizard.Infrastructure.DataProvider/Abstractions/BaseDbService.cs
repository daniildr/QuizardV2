using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizard.Core.Exceptions;
using Quizard.Infrastructure.DataProvider.DbContext;
using Quizard.Infrastructure.DataProvider.Options;

namespace Quizard.Infrastructure.DataProvider.Abstractions;

/// <summary> Абстрактный сервис контекста БД </summary>
public abstract class BaseDbService
{
    private readonly int _retryDelay;
    private readonly int _maxRetryCount;
    private readonly IDbContextFactory<QuizardDbContext> _dbContextFactory;
    
    protected readonly ILogger<BaseDbService> Logger;

    /// <summary> Конструктор абстрактного сервиса контекста БД </summary>
    /// <param name="dbContextFactory"> Фабрика контекста БД </param>
    /// <param name="options"> Опции провайдера данных </param>
    /// <param name="logger"> Логгер </param>
    protected BaseDbService(
        IDbContextFactory<QuizardDbContext> dbContextFactory, 
        IOptions<DataProviderOptions> options,
        ILogger<BaseDbService> logger)
    {
        Logger = logger;
        
        _dbContextFactory = dbContextFactory;
        _retryDelay = options.Value.RetryDelayMilliseconds;
        _maxRetryCount = options.Value.MaxRetries;
    }
    
    /// <summary> Выполняет асинхронные итерации к БД </summary>
    /// <param name="dbAction"> Асинхронная операция </param>
    /// <param name="cancellationToken"> Токен отмены </param>
    /// <typeparam name="T"> Тип итерируемой коллекции </typeparam>
    /// <returns> Асинхронное перечисление </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    protected async IAsyncEnumerable<T> ExecuteAsync<T>(
        Func<QuizardDbContext, IAsyncEnumerable<T>> dbAction,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var dbContext = await CreateDbContext();
            
        await foreach (var item in dbAction(dbContext).WithCancellation(cancellationToken))
        {
            yield return item;
        }
    }
    
    /// <summary> Выполняет асинхронный запрос к БД</summary>
    /// <param name="dbAction"> Асинхронная операция </param>
    /// <typeparam name="T"> Ожидаемый тип из асинхронной операции </typeparam>
    /// <returns> Асинхронная операция </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    protected async Task<T> ExecuteAsync<T>(Func<QuizardDbContext, Task<T>> dbAction)
    {
        await using var dbContext = await CreateDbContext();
        return await dbAction(dbContext);
    }
    
    /// <summary> Выполняет асинхронный запрос к БД</summary>
    /// <param name="dbAction"> Асинхронная операция </param>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    protected async Task ExecuteAsync(Func<QuizardDbContext, Task> dbAction)
    {
        await using var dbContext = await CreateDbContext();
        await dbAction(dbContext);
    }

    /// <summary> Метод, создающий DbContext с помощью фабрики </summary>
    /// <returns> DbContext <see cref="QuizardDbContext"/> </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    private async Task<QuizardDbContext> CreateDbContext()
    {
        Logger.LogInformation("Выполняется подключение к БД");
        for (var attempt = 1; attempt <= _maxRetryCount; attempt++)
        {
            try
            {
                var dbContext = await _dbContextFactory.CreateDbContextAsync();
                if (await dbContext.Database.CanConnectAsync())
                {
                    Logger.LogDebug("DbContext успешно создан и подключен на попытке {Attempt}", attempt);
                    return dbContext;
                }
            
                Logger.LogWarning("Создан DbContext, но подключение к БД не удалось на попытке {Attempt}", attempt);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Ошибка при попытке {Attempt} создания DbContext", attempt);
            }

            if (attempt >= _maxRetryCount) continue;
            await Task.Delay(_retryDelay);
        }
        
        Logger.LogCritical("Не удалось создать DbContext после {MaxRetries} попыток", _maxRetryCount);

        throw new DatabaseUnavailableException("Не удалось создать DbContext и подключиться к БД.");
    }
}