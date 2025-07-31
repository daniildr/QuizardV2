using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizard.Application.GameEngine.Lifecycle;
using Quizard.Application.GameEngine.Options;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class WaitingForPlayersConfig : IGameStateConfiguration
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<GameOptions> _gameOptions;
    private readonly ILogger<WaitingForPlayersConfig> _logger;
    
    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    public WaitingForPlayersConfig(
        IOptions<GameOptions> options,
        IServiceProvider serviceProvider,
        ILogger<WaitingForPlayersConfig> logger)
    {
        _logger = logger;
        _gameOptions = options;
        _serviceProvider = serviceProvider;
        
        _logger.LogInformation("Конфигурация FSM - WaitingForPlayers");
    }
    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        _logger.LogTrace("Конфигурируем поведение FSM в состоянии WaitingForPlayers");
        stateMachine.Configure(GameState.WaitingForPlayers)
            .PermitReentry(GameTrigger.PlayerIdentified)
            .PermitDynamicAsync(GameTrigger.AllPlayersReady, async () =>
            {
                _logger.LogInformation("Все игроки подключились. Начинаем игру");
                var session = GameSessionService.GetSession(true);
                session.CurrentStateIndex++;
                await GameSessionService.SaveChangesAsync();
                var nextState = session.Scenario.Stages
                    .FirstOrDefault(stage => stage.Index == 1);

                return context.SwitchStateByStages(nextState, stateMachine);
            })
            .PermitDynamic(GameTrigger.PauseRequested, () => context.ProcessPause(stateMachine))
            .OnExit(() =>
            {
                _logger.LogInformation("Запускаем задачу-таймер для контроля времени игровой сессии");
                var session = GameSessionService.GetSession();
                _ = Task.Run(async () =>
                {
                    var timerProperties = new GameTimerProperties(_serviceProvider, gameId: session.GameId);
                    
                    if (session.Scenario.GameDuration is not null)
                        await Task.Delay(TimeSpan.FromMinutes((int)session.Scenario.GameDuration));
                    else
                        await Task.Delay(TimeSpan.FromMinutes(_gameOptions.Value.DefaultGameDuration));

                    var currentGameSession = timerProperties.GameSessionService.GetSession();
                    if (timerProperties.GameId != null)
                    {
                        if (currentGameSession.GameId == timerProperties.GameId)
                        {
                            _logger.LogInformation("Таймаут сессии истёк — форсим завершение");
                            await stateMachine.FireAsync(GameTrigger.EndRequested).ConfigureAwait(false);
                        }
                    }
                });
            });
    }
}