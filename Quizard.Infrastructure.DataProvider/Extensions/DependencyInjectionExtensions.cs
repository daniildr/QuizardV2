using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quizard.Core.Interfaces;
using Quizard.Core.Interfaces.Repositories;
using Quizard.Infrastructure.DataProvider.DbContext;
using Quizard.Infrastructure.DataProvider.Options;
using Quizard.Infrastructure.DataProvider.Repositories;

namespace Quizard.Infrastructure.DataProvider.Extensions;

/// <summary> Расширения для введения DI зависимостей DataProvider-а (менеджера для работы с БД) </summary>
public static class DependencyInjectionExtensions
{
    /// <summary> Расширение для введения зависимостей менеджера БД </summary>
    /// <param name="services"> Коллекция сервисов </param>
    /// <param name="configuration"> Конфигурация сервисов </param>
    public static void AddDataProviderServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<DataProviderOptions>(opt =>
                configuration.GetSection(DataProviderOptions.ConfigurationSection).Bind(opt))
            .AddPooledDbContextFactory<QuizardDbContext>(opt => 
                opt.UseNpgsql(configuration.GetConnectionString("Postgres")))
            .AddTransient<IDbInitializer, DbInitializer>()
            .AddTransient<ILicenseSecretRepository, LicenseSecretRepository>()
            .AddTransient<ILicenseRepository, LicenseRepository>()
            .AddTransient<IRoundRepository, RoundRepository>()
            .AddTransient<IMediaRepository, MediaRepository>()
            .AddTransient<IRoundTypeRepository, RoundTypeRepository>()
            .AddTransient<IStatisticRepository, StatisticRepository>()
            .AddTransient<IGameRepository, GameRepository>()
            .AddTransient<IPlayerRepository, PlayerRepository>()
            .AddTransient<IAnswerRepository, AnswerRepository>()
            .AddTransient<IQuestionRepository, QuestionRepository>()
            .AddTransient<IScenarioRepository, ScenarioRepository>()
            .AddTransient<IQuizardDbManager, QuizardDbManager>();
    }
}