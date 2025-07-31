using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizard.Core.Entities;
using Quizard.Core.Interfaces.Repositories;
using Quizard.Infrastructure.DataProvider.Abstractions;
using Quizard.Infrastructure.DataProvider.DbContext;
using Quizard.Infrastructure.DataProvider.Options;

namespace Quizard.Infrastructure.DataProvider.Repositories;

/// <inheritdoc cref="IQuestionRepository"/>
public class QuestionRepository : BaseDbService, IQuestionRepository
{
    public QuestionRepository(
        IDbContextFactory<QuizardDbContext> dbContextFactory, 
        IOptions<DataProviderOptions> options, 
        ILogger<QuestionRepository> logger) : base(dbContextFactory, options, logger)
    {
        Logger.LogInformation("Сервис для работы с таблицей лицензий вопросов");
    }

    /// <inheritdoc/>  
    public async Task<List<Question>> AddQuestionsAsync(List<Question> newQuestions) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записей данных вопросов - {@Question}.", newQuestions);
            
            await dbContext.Questions.AddRangeAsync(newQuestions).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newQuestions;
        }).ConfigureAwait(false);

    public async Task<Question> AddQuestionAsync(Question newQuestion) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записей данных вопросов - {@Question}.", newQuestion);
            
            await dbContext.Questions.AddAsync(newQuestion).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newQuestion;
        }).ConfigureAwait(false);

    public async Task<Reveal> AddRevealAsync(Reveal newReveal) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записей данных вопросов - {@Reveal}.", newReveal);
            
            await dbContext.Reveals.AddAsync(newReveal).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newReveal;
        }).ConfigureAwait(false);

    public async Task<Hint> AddHintAsync(Hint newHint) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записей данных вопросов - {@Hint}.", newHint);
            
            await dbContext.Hints.AddAsync(newHint).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newHint;
        }).ConfigureAwait(false);
}