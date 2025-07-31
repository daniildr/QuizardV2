using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Stateless;

namespace Quizard.Core.Interfaces;

/// <summary> Интерфейс описывает логику конфигурации конечного автомата игры </summary>
public interface IStateMachineConfigurator
{
    /// <summary> Состояние игры перед паузой </summary>
    public GameState? StateBeforePause { get; set; }
    
    /// <summary> Настраивает конечный автомат </summary>
    /// <param name="stateMachine"> Машина состояний, в начальном состоянии </param>
    public void Configure(StateMachine<GameState, GameTrigger> stateMachine);

    /// <summary> Обработчик состояния паузы </summary>
    /// <param name="stateMachine"> Машина состояний </param>
    /// <returns> Следующее состояние стейт машины </returns>
    public GameState ProcessPause(StateMachine<GameState, GameTrigger> stateMachine);

    /// <summary> Метод для переключения состояния стейт машины на основе этапов сценария </summary>
    /// <param name="stage"> Следующий этап сценария </param>
    /// <param name="stateMachine"> Машина состояний </param>
    /// <returns> Следующее состояние стейт машины </returns>
    public GameState SwitchStateByStages(Stage? stage, StateMachine<GameState, GameTrigger> stateMachine);
}