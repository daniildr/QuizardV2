using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUlid;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;
using Quizard.Core.Extensions;
using Quizard.Core.Interfaces.Repositories;
using Quizard.Infrastructure.DataProvider.Abstractions;
using Quizard.Infrastructure.DataProvider.DbContext;
using Quizard.Infrastructure.DataProvider.Options;

namespace Quizard.Infrastructure.DataProvider.Repositories;

/// <inheritdoc cref="IGameRepository"/>
public class GameRepository : BaseDbService, IGameRepository
{
    public GameRepository(
        IDbContextFactory<QuizardDbContext> dbContextFactory, 
        IOptions<DataProviderOptions> options, 
        ILogger<GameRepository> logger) : base(dbContextFactory, options, logger)
    { 
        Logger.LogInformation("Репозиторий игровых сессий проинициализирован");
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<Game> IterateGamesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Выполняется итерация по списку всех игровых сессий");
        await foreach (
            var games in ExecuteAsync<Game>(dbContext =>
            {
                var query = dbContext.Games.AsQueryable();
                
                return query.AsSplitQuery().AsAsyncEnumerable();
            }, cancellationToken).ConfigureAwait(false))
        {
            Logger.LogTrace("Получена игровая сессия {@Game}", games);
            yield return games;
        }
    }

    /// <inheritdoc/>
    public async Task<Game[]> GetGamesAsync(Expression<Func<Game, bool>> predicate) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation(
                "Выполняется получение данных игровых сессий - {Predicate}.", predicate.ToReadableString());
            
            var query = dbContext.Games.Where(predicate).AsQueryable();
            
            return await query.AsSplitQuery().ToArrayAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Game> GetGameAsync(Expression<Func<Game, bool>> predicate) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation(
                "Выполняется получение данных игровой сессии - {Predicate}.", predicate.ToReadableString());
            
            var query = dbContext.Games.AsQueryable();
            
            return await query.AsSplitQuery().FirstOrDefaultAsync(predicate).ConfigureAwait(false) 
                   ?? throw new GameDoesNotExistException(
                       $"Игровая сессии {predicate.ToReadableString()} не найдена");
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Game> AddGameAsync(Game game) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется создание записи игровой сессии - {@Game}.", game);
            
            await dbContext.Games.AddAsync(game).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return game;
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Game> UpdateGameAsync(Ulid gameId, Game game) =>
        await ExecuteAsync(async dbContext =>
        {
            var existingGame = await dbContext.Games.FindAsync(gameId).ConfigureAwait(false);

            if (existingGame == null)
                throw new GameDoesNotExistException($"Игровая сессии с Id={gameId} не найдена");
            
            existingGame.IsRunning = game.IsRunning;

            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return existingGame;
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task DeleteGameAsync(Ulid gameId) =>
        await ExecuteAsync(async dbContext =>
        {
            var existingGame = await dbContext.Games.FindAsync(gameId).ConfigureAwait(false);

            if (existingGame == null)
                throw new GameDoesNotExistException($"Игровая сессии с Id={gameId} не найдена");
            
            dbContext.Games.Remove(existingGame);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
}