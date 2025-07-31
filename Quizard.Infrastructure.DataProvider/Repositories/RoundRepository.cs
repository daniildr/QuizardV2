using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizard.Core.Entities;
using Quizard.Core.Interfaces.Repositories;
using Quizard.Infrastructure.DataProvider.Abstractions;
using Quizard.Infrastructure.DataProvider.DbContext;
using Quizard.Infrastructure.DataProvider.Options;

namespace Quizard.Infrastructure.DataProvider.Repositories;

/// <inheritdoc cref="IRoundRepository"/>
public class RoundRepository : BaseDbService, IRoundRepository
{
    public RoundRepository(
        IDbContextFactory<QuizardDbContext> dbContextFactory, 
        IOptions<DataProviderOptions> options,
        ILogger<RoundRepository> logger) : base(dbContextFactory, options, logger)
    {
        Logger.LogInformation("Сервис для работы с таблицей раундов проинициализирован");
    }
    
    /// <inheritdoc/>
    public async Task<List<Round>> AddRoundsAsync(List<Round> newRounds) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется пакетная вставка {Count} записей раундов", newRounds.Count);

            await dbContext.Rounds.AddRangeAsync(newRounds).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newRounds;
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Round> AddRoundAsync(Round newRound) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется запись {@Round}", newRound);

            await dbContext.Rounds.AddAsync(newRound).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newRound;
        }).ConfigureAwait(false);
}