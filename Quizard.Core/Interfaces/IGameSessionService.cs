using NUlid;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces;

/// <summary> Хранилище текущей игровой сессии </summary>
public interface IGameSessionService : IDisposable, IAsyncDisposable
{
    /// <summary> Создаёт новую игровую сессию и сохраняет её в хранилище </summary>
    /// <param name="gameId"> Уникальный идентификатор игры (ULID) </param>
    /// <param name="scenario"> Модель сценария </param>
    /// <param name="players"> Список игроков, участвующих в сессии </param>
    /// <param name="cancellationTokenSource"> Источник отмены для управления сессией </param>
    public Task<IGameSession> CreateSessionAsync(
        Ulid gameId,
        Scenario scenario,
        IReadOnlyList<Player> players,
        CancellationTokenSource cancellationTokenSource);
    
    /// <summary> Возвращает текущую игровую сессию </summary>
    /// <param name="forModification">
    /// Если <c>true</c> — сессия будет взята на эксклюзивное изменение (lock/транзакция).
    /// Если <c>false</c> — только для чтения.
    /// </param>
    /// <returns> Объект <see cref="IGameSession"/> </returns>
    /// <exception cref="GameSessionDoesNotExistException"> Если сессии нет </exception>
    public IGameSession GetSession(bool forModification = false);

    /// <summary> Сохраняет изменения в текущей игровой сессии в персистентное хранилище </summary>
    public Task SaveChangesAsync();

    /// <summary> Удаляет (сбрасывает) текущую игровую сессию </summary>
    public Task ResetSession();
}