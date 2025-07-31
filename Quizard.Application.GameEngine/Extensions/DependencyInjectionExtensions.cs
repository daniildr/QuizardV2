using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quizard.Application.GameEngine.Connections;
using Quizard.Application.GameEngine.Interfaces;
using Quizard.Application.GameEngine.Lifecycle;
using Quizard.Application.GameEngine.Notifications;
using Quizard.Application.GameEngine.Options;
using Quizard.Application.GameEngine.Session;
using Quizard.Application.GameEngine.StateMachine;
using Quizard.Application.GameEngine.StateMachine.Configurations;
using Quizard.Core.Interfaces;

namespace Quizard.Application.GameEngine.Extensions;

/// <summary> Расширения для введения DI зависимостей игрового движка </summary>
public static class DependencyInjectionExtensions
{
    /// <summary> Расширение для введения сервиса для работы со сценариями </summary>
    /// <param name="services"> Коллекция сервисов </param>
    /// <param name="configuration"> Конфигурация сервисов </param>
    public static void AddGameEngine(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GameOptions>(gameOptions => 
            configuration.GetSection(GameOptions.ConfigurationSection).Bind(gameOptions));

        services
            .AddSingleton<IGameStateConfiguration, NotStartedConfig>()
            .AddSingleton<IGameStateConfiguration, PauseConfig>()
            .AddSingleton<IGameStateConfiguration, WaitingForPlayersConfig>()
            .AddSingleton<IGameStateConfiguration, MediaConfig>()
            .AddSingleton<IGameStateConfiguration, RoundConfig>()
            .AddSingleton<IGameStateConfiguration, QuestionConfig>()
            .AddSingleton<IGameStateConfiguration, RevealConfig>()
            .AddSingleton<IGameStateConfiguration, AuctionConfig>()
            .AddSingleton<IGameStateConfiguration, StatsConfig>()
            .AddSingleton<IGameStateConfiguration, VotingConfig>()
            .AddSingleton<IGameStateConfiguration, ShopConfig>()
            .AddSingleton<IGameStateConfiguration, ModifiersConfig>()
            .AddSingleton<IGameStateConfiguration, FinishConfig>();
        
        services
            .AddSingleton<IStateMachineConfigurator, StateMachineConfigurator>()
            .AddSingleton<IStateMachineProvider, StateMachineProvider>();
        
        services
            .AddTransient<IGameSessionService, GameSessionService>()
            .AddTransient<IGameLifecycleService, GameLifecycleService>()
            .AddTransient<IGameNotificationService, GameNotificationService>()
            .AddTransient<IClientConnectionHandler, ClientConnectionHandler>()
            .AddTransient<ILightControllerNotificationService, LightControllerNotificationService>();
        
        services.AddSingleton<IGameManager, GameManager>();
    }
}