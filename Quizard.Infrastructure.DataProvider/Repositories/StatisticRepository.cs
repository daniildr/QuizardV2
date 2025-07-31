using System.Linq.Expressions;
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

/// <inheritdoc cref="IStatisticRepository"/>
public class StatisticRepository : BaseDbService, IStatisticRepository
{
    public StatisticRepository(
        IDbContextFactory<QuizardDbContext> dbContextFactory, 
        IOptions<DataProviderOptions> options, 
        ILogger<BaseDbService> logger) : base(dbContextFactory, options, logger)
    {
        Logger.LogInformation("Сервис для работы с таблицей статистики раундов и сценария");
    }

    /// <inheritdoc/>
    public async Task<RoundStatistic[]> GetRoundStatistic(Expression<Func<RoundStatistic, bool>> predicate) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation(
                "Выполняется получение данных статистики раунда - {Predicate}.", predicate.ToReadableString());
            
            return await dbContext.RoundStatistics
                       .Include(stat => stat.Player)
                       .Where(predicate)
                       .ToArrayAsync()
                       .ConfigureAwait(false) 
                   ?? throw new StatisticDoesNotExistException<RoundStatistic>(predicate);
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<RoundStatistic> SubmitRoundStatistic(RoundStatistic roundStatistic) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записи статистики раунда - {@Statistic}.", roundStatistic);
            
            await dbContext.RoundStatistics.AddAsync(roundStatistic).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return roundStatistic;
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<ScenarioStatistic[]> GetScenarioStatistic(Expression<Func<ScenarioStatistic, bool>> predicate) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation(
                "Выполняется получение данных статистики сценария - {Predicate}.", predicate.ToReadableString());
            
            return await dbContext.ScenarioStatistics
                       .Include(stat => stat.Player)
                       .Where(predicate)
                       .ToArrayAsync()
                       .ConfigureAwait(false) 
                   ?? throw new StatisticDoesNotExistException<ScenarioStatistic>(predicate);
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<ScenarioStatistic> SubmitScenarioStatistic(ScenarioStatistic scenarioStatistic) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записи статистики сценария - {@Statistic}.", scenarioStatistic);
            
            await dbContext.ScenarioStatistics.AddAsync(scenarioStatistic).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return scenarioStatistic;
        }).ConfigureAwait(false);
}