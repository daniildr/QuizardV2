using Microsoft.Extensions.Logging;
using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine;

/// <inheritdoc/>
public class StateMachineConfigurator : IStateMachineConfigurator
{
    private readonly IEnumerable<IGameStateConfiguration> _configurations;
    private readonly ILogger<StateMachineConfigurator> _logger;
    
    public GameState? StateBeforePause { get; set; }

    /// <summary> Конструктор конфигуратора </summary>
    /// <param name="configurations"> Коллекция конфигурация состояний FSM </param>
    /// <param name="logger"> Логгер </param>
    public StateMachineConfigurator(
        IEnumerable<IGameStateConfiguration> configurations,
        ILogger<StateMachineConfigurator> logger)
    {
        _logger = logger;
        _configurations = configurations;

        _logger.LogDebug("Конфигуратор стейт машины проинициализирован");
    }

    /// <inheritdoc/>
    public void Configure(StateMachine<GameState, GameTrigger> stateMachine)
    {
        foreach (var config in _configurations)
        {
            config.Configure(stateMachine, this);
        }
    }

    /// <inheritdoc/>
    public GameState ProcessPause(StateMachine<GameState, GameTrigger> stateMachine)
    {
        if (stateMachine.IsInState(GameState.Pause))
        {
            _logger.LogInformation(
                "Сессия уже на паузе, предыдущее состояние - {State}. Действия не требуется",
                StateBeforePause);
            return GameState.Pause;
        }

        StateBeforePause = stateMachine.State;
        _logger.LogInformation("Сессия на паузе, предыдущее состояние - {State}", StateBeforePause);
        return GameState.Pause;
    }

    /// <inheritdoc/>
    public GameState SwitchStateByStages(Stage? stage, StateMachine<GameState, GameTrigger> stateMachine)
    {
        _logger.LogDebug("Будет выполнен переход к следующему состоянию игровой сессии");
        if (stage is null)
        {
            return GameState.Finished;
        }

        switch (stage.Type)
        {
            case ScenarioStage.Pause:
            {
                if (stage.StageDuration != null)
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromMinutes((int)stage.StageDuration));
                    
                        _logger.LogInformation("Таймаут запланированной паузы истёк — форсим возобновление");
                        await stateMachine.FireAsync(GameTrigger.ResumeRequested).ConfigureAwait(false);
                    });
                }
                
                return ProcessPause(stateMachine);
            }
                    
            case ScenarioStage.Media:
                return GameState.Media;
                    
            case ScenarioStage.Round:
                return GameState.RoundPlaying;
                
            case ScenarioStage.Vote:
                return GameState.Voting;

            case ScenarioStage.Finish:
            {
                if (stage.RoundId != null)
                {
                    return GameState.RoundPlaying;
                }
                else if (stage.MediaId != null)
                {
                    return GameState.Media;
                }
                else
                {
                    return GameState.Finished;
                }
            }
                
            case ScenarioStage.Shop:
                return GameState.Shop;
                    
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}