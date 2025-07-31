using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Quizard.Infrastructure.DataProvider.DbContext;

namespace Quizard.Infrastructure.DataProvider;

/// <inheritdoc />
public class DbInitializer : IDbInitializer
{
    private readonly QuizardDbContext _dbContext;
    private readonly ILogger<DbInitializer> _logger;

    /// <summary> Конструктор инициализатора </summary>
    /// <param name="dbContextFactory"> Фабрика БД контекста </param>
    /// <param name="logger"> Логгер </param>
    public DbInitializer(IDbContextFactory<QuizardDbContext> dbContextFactory, ILogger<DbInitializer> logger)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        _logger = logger;
        
        _logger.LogDebug("Сервис инициализации БД проинициализирован");
    }

    /// <inheritdoc />
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await InitializeRoundTypesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task InitializeRoundTypesAsync(CancellationToken cancellationToken = default)
    {
        if (await _dbContext.RoundTypes.AnyAsync(cancellationToken: cancellationToken))
        {
            _logger.LogTrace("Инициализация типов раундов не требуется");
            return;
        }

        await _dbContext.RoundTypes.AddRangeAsync(
            new RoundType
            {
                RoundTypeId = RoundTypeId.Warmup,
                Name = "Разминка",
                Description = "Игроки выбирают один правильный ответ из предложенных вариантов",
                RoundClass = RoundClass.Base,
                WaitingRoundTimeout = false,
                DisplayQuestionOnInformator = true,
                DisplayQuestionOnPlayers = false,
            },
            new RoundType
            {
                RoundTypeId = RoundTypeId.TrueFalse,
                Name = "Верю не Верю",
                Description = "Игроки выбирают один правильный ответ из предложенных вариантов",
                RoundClass = RoundClass.Base,
                WaitingRoundTimeout = false,
                DisplayQuestionOnInformator = true,
                DisplayQuestionOnPlayers = true,
            },
            new RoundType
            {
                RoundTypeId = RoundTypeId.Blitz,
                Name = "Супер-блиц",
                Description = "Игрок должен дать ответ быстрее всех",
                RoundClass = RoundClass.Speed,
                WaitingRoundTimeout = true,
                DisplayQuestionOnInformator = true,
                DisplayQuestionOnPlayers = false,
            },
            new RoundType
            {
                RoundTypeId = RoundTypeId.HotPotato,
                Name = "Горячая картошка",
                Description = "Игроки отвечают по очереди",
                RoundClass = RoundClass.Sequential,
                WaitingRoundTimeout = false,
                DisplayQuestionOnInformator = false,
                DisplayQuestionOnPlayers = true,
            },
            new RoundType
            {
                RoundTypeId = RoundTypeId.Pantomime,
                Name = "Пантомима",
                Description = "Игроки должны угадать, что показывает им другой игрок",
                RoundClass = RoundClass.Interactive,
                WaitingRoundTimeout = false,
                DisplayQuestionOnInformator = false,
                DisplayQuestionOnPlayers = true,
            },
            new RoundType
            {
                RoundTypeId = RoundTypeId.GuessMelody,
                Name = "Угадай мелодию",
                Description = "Игроки угадывают мелодию",
                RoundClass = RoundClass.Speed,
                WaitingRoundTimeout = false,
                DisplayQuestionOnInformator = true,
                DisplayQuestionOnPlayers = false,
            },
            new RoundType
            {
                RoundTypeId = RoundTypeId.Steps,
                Name = "Ступеньки",
                Description = "Игроки выбирают один правильный ответ, получая подсказки",
                RoundClass = RoundClass.Steps,
                WaitingRoundTimeout = false,
                DisplayQuestionOnInformator = true,
                DisplayQuestionOnPlayers = true,
            },
            new RoundType
            {
                RoundTypeId = RoundTypeId.Ordering,
                Name = "Вопрос-расположение",
                Description = "Игроки должны расставить варианты в правильном порядке",
                RoundClass = RoundClass.Ordering,
                WaitingRoundTimeout = false,
                DisplayQuestionOnInformator = true,
                DisplayQuestionOnPlayers = true,
            },
            new RoundType
            {
                RoundTypeId = RoundTypeId.Auction,
                Name = "Вопрос-аукцион",
                Description = "Игроки должны сделать ставку, прежде чем дать свой ответ",
                RoundClass = RoundClass.Auction,
                WaitingRoundTimeout = false,
                DisplayQuestionOnInformator = true,
                DisplayQuestionOnPlayers = true,
            }).ConfigureAwait(false);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}