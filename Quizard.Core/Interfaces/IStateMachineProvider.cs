using Quizard.Core.Enums;
using Stateless;

namespace Quizard.Core.Interfaces;

/// <summary> Интерфейс поставщика машины состояний для игрового движка </summary>
/// <remarks>
/// Читает текущее состояние из сессии <see cref="IGameSession"/>,
/// создаёт экземпляр <see cref="StateMachine{TState,TTrigger}"/>
/// и передаёт его в <see cref="IStateMachineConfigurator"/> для настройки
/// </remarks>
public interface IStateMachineProvider
{
    /// <summary> Экземпляр машины состояний </summary>
    public StateMachine<GameState, GameTrigger> StateMachine { get; }
    
    /// <summary> Создаёт машину состояний, синхронизированную с текущим состоянием игровой сессии </summary>
    /// <param name="session"> Текущая игровая сессия </param>
    /// <returns> Экземпляр <see cref="StateMachine{GameState,GameTrigger}"/> </returns>
    public StateMachine<GameState, GameTrigger> Create(IGameSession session);

    /// <summary> Уничтожает текущую машину состояний и создает новую в базовом состоянии </summary>
    /// <returns> Асинхронная операция </returns>
    public Task Destroy();
}