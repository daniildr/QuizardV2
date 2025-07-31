using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUlid;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;
using Quizard.Core.Interfaces.Repositories;
using Quizard.Infrastructure.DataProvider.Abstractions;
using Quizard.Infrastructure.DataProvider.DbContext;
using Quizard.Infrastructure.DataProvider.Options;

namespace Quizard.Infrastructure.DataProvider.Repositories;

/// <inheritdoc cref="IPlayerRepository"/>
public class PlayerRepository : BaseDbService, IPlayerRepository
{
    public PlayerRepository(
        IDbContextFactory<QuizardDbContext> dbContextFactory,
        IOptions<DataProviderOptions> options, 
        ILogger<PlayerRepository> logger) : base(dbContextFactory, options, logger)
    {
        Logger.LogInformation("Сервис для работы с таблицей игроков проинициализирован");
    }

    /// <inheritdoc/>
    public async Task<Player> GetPlayerAsync(Ulid playerId) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется получение данных игрока - {Id}.", playerId);
            var query = dbContext.Players.AsQueryable();
            
            return await query.AsSplitQuery()
                       .FirstOrDefaultAsync(player => player.Id == playerId)
                       .ConfigureAwait(false) 
                   ?? throw new PlayerDoesNotExistException(playerId);
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Player> GetPlayerAsync(string nickname) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется получение данных игрока - {Nickname}.", nickname);
            var query = dbContext.Players.AsQueryable();
            
            return await query.AsSplitQuery()
                       .FirstOrDefaultAsync(player => player.Nickname == nickname)
                       .ConfigureAwait(false) 
                   ?? throw new PlayerDoesNotExistException(nickname);
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Player> AddPlayerAsync(Player newPlayer) =>
        await ExecuteAsync(async dbContext =>
        {
            if (await dbContext.Players.AnyAsync(player => player.Nickname == newPlayer.Nickname).ConfigureAwait(false))
            {
                Logger.LogDebug("Пользователь уже существует. Пользователь не будет создан");
                return await GetPlayerAsync(newPlayer.Nickname).ConfigureAwait(false);
            }
            
            Logger.LogInformation("Выполняется запись нового игрока в БД - {@Player}.", newPlayer);
            await dbContext.Players.AddAsync(newPlayer).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return newPlayer;
        }).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<bool> IsNicknameTakenAsync(string nickname) =>
        await ExecuteAsync(async dbContext =>
        {
            Logger.LogInformation("Выполняется проверка доступности никнейма - {Nickname}.", nickname);
            return await dbContext.Players.AnyAsync(player => player.Nickname == nickname).ConfigureAwait(false);
        }).ConfigureAwait(false);
}