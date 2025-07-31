using Quizard.Core.Enums;
using Stateless;

namespace Quizard.Core.Interfaces;

/// <summary> Интерфейс для конфигураторов состояний </summary>
public interface IGameStateConfiguration
{
    /// <summary> Метод для конфигурации состояния конечного автомата </summary>
    /// <param name="stateMachine"> Конечный автомат </param>
    /// <param name="context"> Контекст автомата </param>
    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context);
}