using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;
using Quizard.Core.Extensions;
using Quizard.Core.Interfaces;
using Quizard.Core.Interfaces.Repositories;
using Quizard.Infrastructure.DataProvider.Abstractions;
using Quizard.Infrastructure.DataProvider.DbContext;
using Quizard.Infrastructure.DataProvider.Options;

namespace Quizard.Infrastructure.DataProvider.Repositories;

/// <inheritdoc cref="ILicenseRepository"/>
public class LicenseRepository : BaseDbService, ILicenseRepository
{
    private readonly IAesEncryptor _encryptor;
    
    public LicenseRepository(
        IDbContextFactory<QuizardDbContext> dbContextFactory,
        IOptions<DataProviderOptions> options,
        ILogger<LicenseRepository> logger,
        IAesEncryptor encryptor) : base(dbContextFactory, options, logger)
    {
        _encryptor = encryptor;
        
        Logger.LogInformation("Сервис для работы с таблицей лицензий проинициализирован");
    }
    
    /// <inheritdoc/>
    public async IAsyncEnumerable<License> IterateLicenseAsync(
        bool withSalt = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Выполняется итерация по списку лицензий");
        await foreach (
            var license in ExecuteAsync<License>(dbContext =>
            {
                var query = dbContext.Licenses.AsQueryable();
                
                if (withSalt)
                    query = query.Include(license => license.Salt);
                
                return query.AsAsyncEnumerable();
            }, cancellationToken).ConfigureAwait(false))
        {
            Logger.LogTrace("Получена лицензия {License}", license.LicenseKey);
            yield return license;
        }
    }

    /// <inheritdoc/>
    public async Task<License> GetLicenseAsync(Expression<Func<License, bool>> predicate, bool withSalt) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation(
                "Выполняется получение данных лицензии - {Predicate}.", predicate.ToReadableString());
            
            var query = dbContext.Licenses.AsQueryable();
            
            if (withSalt)
                query = query.Include(contact => contact.Salt);
            
            var license = await query.FirstOrDefaultAsync(predicate).ConfigureAwait(false) 
                   ?? throw new LicenseDoesNotExistException(predicate);

            license.ExpirationTime = await _encryptor.DecryptAsync(license.ExpirationTime);
            license.GamesLeft = await _encryptor.DecryptAsync(license.GamesLeft);
            
            return license;
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<License> UpdateLicenseAsync(long licenseId, License updatedLicense) =>
        await ExecuteAsync(async dbContext =>
        {
            var existingLicense = await dbContext.Licenses.FindAsync(licenseId).ConfigureAwait(false);

            if (existingLicense == null)
                throw new LicenseDoesNotExistException($"Лицензия с Id={licenseId} не найдена");
            
            existingLicense.GamesLeft = updatedLicense.GamesLeft;
            existingLicense.Active = updatedLicense.Active;

            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return existingLicense;
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<License> AddLicenseAsync(License license) =>
        await ExecuteAsync(async dbContext =>
        {
            if (await dbContext.Licenses.AnyAsync(l => l.Active == true))
            {
                Logger.LogWarning("В системе есть активные лицензии. Они будут деактивированы");
                foreach (var activeLicense in dbContext.Licenses.Where(existLicense => existLicense.Active == true))
                {
                    activeLicense.Active = false;
                }
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            
            Logger.LogInformation("Выполняется создание записи данных лицензии - {@License}.", license);
            if (await dbContext.Licenses
                    .AnyAsync(licenseInDb => licenseInDb.LicenseKey == license.LicenseKey)
                    .ConfigureAwait(false))
                throw new LicenseAlreadyExistException($"Лицензия уже существует. {license.LicenseKey}");
            
            await dbContext.Licenses.AddAsync(license).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return license;
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task DeactivateLicenseAsync(long licenseId) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется деактивации лицензии {Id}.", licenseId);
            
            var existingLicense = await dbContext.Licenses.FindAsync(licenseId).ConfigureAwait(false);
            if (existingLicense == null)
                throw new LicenseDoesNotExistException($"Лицензия с Id={licenseId} не найдена");
            
            existingLicense.Active = false;
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
}