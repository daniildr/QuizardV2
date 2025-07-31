using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUlid;
using Quizard.Core.Entities;
using Quizard.Infrastructure.DataProvider.DbContext.ValueConverters;

namespace Quizard.Infrastructure.DataProvider.DbContext;

public class QuizardDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly ILogger<QuizardDbContext> _logger;
    
    /// <summary> Таблица секретов лицензий </summary>
    public DbSet<LicenseSecret> LicenseSecrets { get; set; }
    
    /// <summary> Таблица данных лицензий </summary>
    public DbSet<License> Licenses { get; set; }
    
    /// <summary> Таблица сыгранных игр </summary>
    public DbSet<Game> Games { get; set; }
    
    /// <summary> Таблица подсказок </summary>
    public DbSet<Hint> Hints { get; set; }
    
    /// <summary> Таблица сценариев </summary>
    public DbSet<Scenario> Scenarios { get; set; }
    
    /// <summary> Таблица этапов сценариев </summary>
    public DbSet<Stage> Stages { get; set; }
    
    /// <summary> Таблица остатков в магазине </summary>
    public DbSet<Stock> Stocks { get; set; }
    
    /// <summary> Таблица правильных ответов </summary>
    public DbSet<Reveal> Reveals { get; set; }
    
    /// <summary> Таблица раундов </summary>
    public DbSet<Round> Rounds { get; set; }
    
    /// <summary> Таблица статистики раунда </summary>
    public DbSet<RoundStatistic> RoundStatistics { get; set; }
    
    /// <summary> Таблица статистики сценария </summary>
    public DbSet<ScenarioStatistic> ScenarioStatistics { get; set; }
    
    /// <summary> Таблица вопросов </summary>
    public DbSet<Question> Questions { get; set; }
    
    /// <summary> Таблица ответов </summary>
    public DbSet<Answer> Answers { get; set; }
    
    /// <summary> Таблица типов раундов </summary>
    public DbSet<RoundType> RoundTypes { get; set; }
    
    /// <summary> Таблица медиа файлов </summary>
    public DbSet<Media> Media { get; set; }
    
    /// <summary> Таблица игроков </summary>
    public DbSet<Player> Players { get; set; }
    
    /// <summary> Таблица переводов </summary>
    public DbSet<Localization> Localizations { get; set; }

    public QuizardDbContext(
        DbContextOptions<QuizardDbContext> options, 
        ILogger<QuizardDbContext>? logger = null) : base(options)
    {
        _logger = logger ?? NullLogger<QuizardDbContext>.Instance;
        
        _logger.LogInformation("Контекст БД проинициализирован");
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var ulidConverter = new UlidValueConverter();
        var entityTypes = modelBuilder.Model.GetEntityTypes();
        foreach (var entityType in entityTypes)
        {
            var clrType = entityType.ClrType;
            foreach (var prop in clrType.GetProperties()
                         .Where(p => p.PropertyType == typeof(Ulid)))
            {
                modelBuilder.Entity(clrType)
                    .Property(prop.Name)
                    .HasConversion(ulidConverter)
                    .HasColumnType("bytea");
            }
        }

        var assembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        
        // Глобальная настройка автоматической загрузки (AutoInclude) для всех вложенных зависимостей
        // Настройка автоматической загрузки навигации "Stages" для "Scenario"
        modelBuilder.Entity<Scenario>()
            .Navigation(scenario => scenario.Stages)
            .AutoInclude();
        
        // Настройка автоматической загрузки навигации "Localization" для "Scenario"
        modelBuilder.Entity<Scenario>()
            .Navigation(scenario => scenario.Localization)
            .AutoInclude();

        // Настройка автоматической загрузки навигации "Round" для Stage:
        modelBuilder.Entity<Stage>()
            .Navigation(stage => stage.Round)
            .AutoInclude();
        
        // Настройка автоматической загрузки навигации "Media" для Stage:
        modelBuilder.Entity<Stage>()
            .Navigation(stage => stage.Media)
            .AutoInclude();

        // Настройка автоматической загрузки навигации "RoundType" для Round:
        modelBuilder.Entity<Round>()
            .Navigation(round => round.RoundType)
            .AutoInclude();

        // Настройка автоматической загрузки навигации "Questions" для Round:
        modelBuilder.Entity<Round>()
            .Navigation(round => round.Questions)
            .AutoInclude();

        // Настройка автоматической загрузки навигаций "Answers", "Hints", "Reveal" и "Media" для Question:
        modelBuilder.Entity<Question>()
            .Navigation(question => question.Answers)
            .AutoInclude();
        modelBuilder.Entity<Question>()
            .Navigation(question => question.Hints)
            .AutoInclude();
        modelBuilder.Entity<Question>()
            .Navigation(question => question.Reveal)
            .AutoInclude();
        modelBuilder.Entity<Question>()
            .Navigation(question => question.Media)
            .AutoInclude();
        
        // Настройка автоматической загрузки навигации "Media" для Reveal (если есть):
        modelBuilder.Entity<Reveal>()
            .Navigation(reveal => reveal.Media)
            .AutoInclude();
        
        // Настройка автоматической загрузки навигации "RoundDefinitions" для Scenario (если есть):
        modelBuilder.Entity<Scenario>()
            .Navigation(scenario => scenario.RoundDefinitions)
            .AutoInclude();

        // Настройка автоматической загрузки навигации "Stocks" для Scenario (если есть):
        modelBuilder.Entity<Scenario>()
            .Navigation(scenario => scenario.ShopStocks)
            .AutoInclude();
        
        _logger.LogTrace("Конфигурация сущностей загружена из сборки {Assembly}", assembly.FullName);
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        
        var configuration = Core.Utils.ConfigurationBuilder.BuildConfiguration();
        var connectionString = configuration.GetConnectionString("Postgres");
        
        optionsBuilder.UseNpgsql(connectionString);
    }
}