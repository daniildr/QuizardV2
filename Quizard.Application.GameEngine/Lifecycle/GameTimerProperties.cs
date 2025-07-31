using Microsoft.Extensions.DependencyInjection;
using NUlid;
using Quizard.Core.Interfaces;

namespace Quizard.Application.GameEngine.Lifecycle;

/// <summary> Хранилище свойств таймера </summary>
/// <remarks> Хранилище необходимо для определения принадлежности таймера, чтобы не завершить не нужный раунд </remarks>
public class GameTimerProperties
{
    private readonly IServiceProvider _serviceProvider;
    
    public Ulid? GameId { get; }
    
    public string? RoundId { get; }
    
    public Ulid? QuestionId { get; }
    
    public int? StateNumber { get; }
    
    public IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    /// <summary> Конструктор хранилища </summary>
    /// <param name="serviceProvider"> Сервис провайдер </param>
    /// <param name="gameId"> Идентификатор игры </param>
    /// <param name="roundId"> Идентификатор раунда </param>
    /// <param name="questionId"> Идентификатор вопроса </param>
    /// <param name="stateNumber"> Порядковый номер текущего этапа </param>
    public GameTimerProperties(
        IServiceProvider serviceProvider,
        Ulid? gameId = null,
        string? roundId = null,
        Ulid? questionId = null,
        int? stateNumber = null)
    {
        _serviceProvider = serviceProvider;
        GameId = gameId;
        RoundId = roundId;
        QuestionId = questionId;
        StateNumber = stateNumber;
    }
}