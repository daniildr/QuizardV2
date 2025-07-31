using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces.Repositories;

/// <summary> Репозиторий раундов </summary>
public interface IRoundRepository
{
    /// <summary> Метод для пакетного добавления новых раундов в БД </summary>
    /// <param name="newRounds"> Коллекция новых раундов </param>
    /// <returns> Коллекция новых раундов  </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<List<Round>> AddRoundsAsync(List<Round> newRounds);
    
    /// <summary>  </summary>
    /// <param name="newRound">  </param>
    /// <returns>   </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Round> AddRoundAsync(Round newRound);
}