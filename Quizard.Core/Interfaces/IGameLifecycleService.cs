using NUlid;
using Quizard.Core.Exceptions;
using Quizard.Core.Models.Requests;

namespace Quizard.Core.Interfaces;

/// <summary> Интерфейс управления жизненным циклом игры (игровой сессией) </summary>
public interface IGameLifecycleService
{
    /// <summary> Запускает новую игровую сессию </summary>
    /// <remarks>
    /// <list type="number">
    /// <listheader>
    /// <description> Пайп-лайн метода: </description>
    /// </listheader>
    /// <item><description> Проверяет лицензию </description></item>
    /// <item><description> Создаёт и сохраняет GameSession </description></item>
    /// <item><description> Стартует FSM (StartRequested) и уведомляет клиентов </description></item>
    /// </list>
    /// </remarks>
    /// <param name="request"> Параметры старта </param>
    /// <returns> ULID новой игровой сессии </returns>
    /// <exception cref="GameAlreadyRunning"> Если сессия уже запущена </exception>
    public Task<Ulid> StartGameAsync(GameStartRequestDto request);
    
    /// <summary> Приостанавливает игру </summary>
    /// <remarks>
    /// FSM переходит в Pause, клиенты получают событие GamePaused.
    /// Может вызываться администратором или автоматически при ошибках.
    /// </remarks>
    /// <returns> Асинхронная операция </returns>
    public Task PauseGameAsync();

    /// <summary> Пропускает текущий этап игры </summary>
    /// <returns> Асинхронная операция </returns>
    public Task SkipStage();
    
    /// <summary> Возобновляет игру из паузы </summary>
    /// <remarks> FSM возвращается в предыдущее состояние, клиенты получают событие GameResumed </remarks>
    /// <returns> Асинхронная операция </returns>
    public Task ResumeGameAsync();
    
    /// <summary>
    /// Завершает текущую игровую сессию:
    /// сбрасывает FSM, помечает в БД IsRunning=false, очищает кеш.
    /// Клиенты получают событие ForceDisconnect.
    /// </summary>
    /// <returns> Асинхронная операция </returns>
    public Task ForceStopGameAsync();
}