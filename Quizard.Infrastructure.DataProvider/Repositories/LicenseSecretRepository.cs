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

/// <inheritdoc cref="ILicenseSecretRepository"/>
public class LicenseSecretRepository : BaseDbService, ILicenseSecretRepository
{
    public LicenseSecretRepository(
        IDbContextFactory<QuizardDbContext> dbContextFactory,
        IOptions<DataProviderOptions> options,
        ILogger<LicenseSecretRepository> logger) : base(dbContextFactory, options, logger)
    {
        Logger.LogInformation("Сервис для работы с таблицей секретных данных лицензий проинициализирован");
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<LicenseSecret> IterateLicenseSecretsAsync(
        bool withLicense = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Выполняется итерация по списку секретных данных лицензий");
        await foreach (
            var licenseSecret in ExecuteAsync<LicenseSecret>(dbContext =>
            {
                var query = dbContext.LicenseSecrets.AsQueryable();
                
                if (withLicense)
                    query = query.Include(secret => secret.License);
                
                return query.AsAsyncEnumerable();
            }, cancellationToken).ConfigureAwait(false))
        {
            Logger.LogTrace("Получена секретная соль {Salt}", licenseSecret.Salt);
            yield return licenseSecret;
        }
    }

    /// <inheritdoc/>
    public async Task<LicenseSecret> GetLicenseSecretAsync(
        Expression<Func<LicenseSecret, bool>> predicate, bool withLicense = false) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation(
                "Выполняется получение секретных данных лицензии - {Predicate}.", predicate.ToReadableString());
            
            IQueryable<LicenseSecret> query = dbContext.LicenseSecrets;
            
            if (withLicense)
                query = query.Include(contact => contact.License);
            
            return await query.FirstOrDefaultAsync(predicate).ConfigureAwait(false) 
                   ?? throw new LicenseSecretDoesNotExistException(predicate);
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<LicenseSecret[]> GetLicenseSecretsAsync(
        Expression<Func<LicenseSecret, bool>> predicate, bool withLicense)
    {
        return await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation(
                "Выполняется получение коллекции секретных данных лицензии - {Predicate}.", 
                predicate.ToReadableString());
            
            IQueryable<LicenseSecret> query = dbContext.LicenseSecrets;
            
            if (withLicense)
                query = query.Include(contact => contact.License);
            
            return await query.Where(predicate).ToArrayAsync().ConfigureAwait(false) 
                   ?? throw new LicenseSecretDoesNotExistException(predicate);
        }).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<LicenseSecret> AddLicenseSecretAsync(LicenseSecret secret) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записи секретных данных лицензии - {@Salt}.", secret);

            if (await dbContext.LicenseSecrets
                    .AnyAsync(secretLicense => secretLicense.Salt == secret.Salt)
                    .ConfigureAwait(false))
                throw new LicenseSecretAlreadyExistException($"Секретная запись уже существует. {secret.Salt}");
            
            await dbContext.LicenseSecrets.AddAsync(secret).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return secret;
        }).ConfigureAwait(false);
}