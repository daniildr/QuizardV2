using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizard.Core.Entities;
using Quizard.Core.Interfaces.Repositories;
using Quizard.Infrastructure.DataProvider.Abstractions;
using Quizard.Infrastructure.DataProvider.DbContext;
using Quizard.Infrastructure.DataProvider.Options;

namespace Quizard.Infrastructure.DataProvider.Repositories;

/// <inheritdoc cref="IAnswerRepository"/>
public class AnswerRepository : BaseDbService, IAnswerRepository
{
    public AnswerRepository(
        IDbContextFactory<QuizardDbContext> dbContextFactory, 
        IOptions<DataProviderOptions> options,
        ILogger<AnswerRepository> logger) : base(dbContextFactory, options, logger)
    {
        Logger.LogInformation("Сервис для работы с таблицей ответов проинициализирован");
    }

    /// <inheritdoc/>
    public async Task<List<Answer>> AddAnswersAsync(List<Answer> newAnswers) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записей данных ответов - {@Answer}.", newAnswers);
            
            await dbContext.Answers.AddRangeAsync(newAnswers).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newAnswers;
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Answer> AddAnswerAsync(Answer newAnswer) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записей данных ответов - {@Answer}.", newAnswer);
            
            await dbContext.Answers.AddAsync(newAnswer).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newAnswer;
        }).ConfigureAwait(false);
}