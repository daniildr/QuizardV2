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

/// <inheritdoc cref="IScenarioRepository"/>
public class ScenarioRepository : BaseDbService, IScenarioRepository
{
    public ScenarioRepository(
        IDbContextFactory<QuizardDbContext> dbContextFactory,
        IOptions<DataProviderOptions> options,
        ILogger<ScenarioRepository> logger) : base(dbContextFactory, options, logger)
    {
        Logger.LogInformation("Сервис для работы с таблицей сценариев проинициализирован");
    }

    /// <inheritdoc/>
    public async Task<Scenario[]> GetScenariosAsync() =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется получение данных всех сценариев");
            return await dbContext.Scenarios.AsSplitQuery().ToArrayAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Scenario[]> GetScenariosAsync(Expression<Func<Scenario, bool>> predicate) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется получение данных сценариев - {Predicate}", predicate.ToReadableString());
            return await dbContext.Scenarios.Where(predicate).AsSplitQuery().ToArrayAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Scenario> GetScenarioAsync(string scenarioId) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется получение данных сценария - {Id}", scenarioId);
            return await dbContext.Scenarios
                .AsSplitQuery()
                .FirstOrDefaultAsync(scenario => scenario.Id == scenarioId)
                .ConfigureAwait(false) 
                   ?? throw new ScenarioDoesNotExistException($"Сценарий ${scenarioId} не найден");
        }).ConfigureAwait(false);

    /// <inheritdoc/> 
    public async Task<Scenario> AddScenarioAsync(Scenario newScenario) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется запись сценария в БД - {@Scenario}.", newScenario);
            await dbContext.Scenarios.AddAsync(newScenario).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newScenario;
        }).ConfigureAwait(false);

    /// <inheritdoc/> 
    public async Task<List<Stock>> AddShopStocksAsync(List<Stock> newStocks) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется запись остатков в магазине в БД - {@Stocks}.", newStocks);
            await dbContext.Stocks.AddRangeAsync(newStocks).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newStocks;
        }).ConfigureAwait(false);

    /// <inheritdoc/> 
    public async Task<List<Stage>> AddStagesAsync(List<Stage> newStages) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется запись этапов сценария в БД - {@Stages}.", newStages);
            await dbContext.Stages.AddRangeAsync(newStages).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newStages;
        }).ConfigureAwait(false);

    public async Task<Localization> AddLocalizationAsync(Localization localization) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется запись локализации сценария в БД - {@Localization}.", localization);
            await dbContext.Localizations.AddAsync(localization).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return localization;
        }).ConfigureAwait(false);

    /// <inheritdoc/> 
    public async Task RemoveScenarioAsync(string scenarioId) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется удаление сценария {Id}.", scenarioId);
            var existingScenario = await dbContext.Scenarios.FindAsync(scenarioId).ConfigureAwait(false);
            if (existingScenario == null)
                throw new ScenarioDoesNotExistException($"Сценарий с Id={scenarioId} не найдена");

            dbContext.Scenarios.Remove(existingScenario);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
}