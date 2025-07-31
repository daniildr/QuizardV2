using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizard.Core.Entities;
using Quizard.Core.Interfaces.Repositories;
using Quizard.Infrastructure.DataProvider.Abstractions;
using Quizard.Infrastructure.DataProvider.DbContext;
using Quizard.Infrastructure.DataProvider.Options;

namespace Quizard.Infrastructure.DataProvider.Repositories;

/// <inheritdoc cref="IRoundTypeRepository"/>
public class RoundTypeRepository : BaseDbService, IRoundTypeRepository
{
    public RoundTypeRepository(
        IDbContextFactory<QuizardDbContext> dbContextFactory,
        IOptions<DataProviderOptions> options,
        ILogger<RoundTypeRepository> logger) : base(dbContextFactory, options, logger)
    {
        Logger.LogInformation("Сервис для работы с таблицей типов раундов проинициализирован");
    }

    /// <inheritdoc/>
    public async Task<RoundType[]> GetAllRoundTypes() => await ExecuteAsync(async dbContext =>
    {
        Logger.LogInformation("Выполняется получение коллекции всех типов раундов");
        return await dbContext.RoundTypes.ToArrayAsync().ConfigureAwait(false);
    }).ConfigureAwait(false);
}