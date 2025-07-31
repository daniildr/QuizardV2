using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;
using Quizard.Core.Extensions;
using Quizard.Core.Interfaces.Repositories;
using Quizard.Infrastructure.DataProvider.Abstractions;
using Quizard.Infrastructure.DataProvider.DbContext;
using Quizard.Infrastructure.DataProvider.Options;

namespace Quizard.Infrastructure.DataProvider.Repositories;

/// <inheritdoc cref="IMediaRepository"/>
public class MediaRepository : BaseDbService, IMediaRepository
{
    public MediaRepository(
        IDbContextFactory<QuizardDbContext> dbContextFactory,
        IOptions<DataProviderOptions> options,
        ILogger<MediaRepository> logger) : base(dbContextFactory, options, logger)
    {
        Logger.LogInformation("Сервис для работы с таблицей медиа-файлов проинициализирован");
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<Media> IterateMediaAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Выполняется итерация по списку медиа-файлов");
        await foreach (
            var media in ExecuteAsync<Media>(dbContext =>
            {
                var query = dbContext.Media.AsQueryable();
                
                return query.AsAsyncEnumerable();
            }, cancellationToken).ConfigureAwait(false))
        {
            Logger.LogTrace("Получен медиа-файл {@Media}", media);
            yield return media;
        }
    }

    /// <inheritdoc/>
    public async Task<Media> GetMediaAsync(Expression<Func<Media, bool>> predicate) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation(
                "Выполняется получение данных медиа-файлов - {Predicate}.", predicate.ToReadableString());
            
            IQueryable<Media> query = dbContext.Media;
            
            return await query.FirstOrDefaultAsync(predicate).ConfigureAwait(false) 
                   ?? throw new MediaDoesNotExistException(predicate);
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Media> AddMediaAsync(Media newMedia) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записи данных медиа-файла - {@Media}.", newMedia);
            
            await dbContext.Media.AddAsync(newMedia).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newMedia;
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<List<Media>> AddMediaAsync(List<Media> newMedia) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записей данных медиа-файлов - {@Media}.", newMedia);
            
            await dbContext.Media.AddRangeAsync(newMedia).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newMedia;
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task RemoveMediaAsync(string mediaId) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется удаления медиа-файла {Id}.", mediaId);
            
            var existingMedia = await dbContext.Media.FindAsync(mediaId).ConfigureAwait(false);
            if (existingMedia == null)
                throw new MediaDoesNotExistException($"Медиа-файл с Id={mediaId} не найден");

            dbContext.Media.Remove(existingMedia);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
}