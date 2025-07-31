using Quizard.Core.Entities;

namespace Quizard.Core.Interfaces.Repositories;

/// <summary> Репозиторий типов раундов </summary>
public interface IRoundTypeRepository
{
    /// <summary> Метод для получения данных всех существующих типов раундов </summary>
    /// <returns> Коллекция <see cref="RoundType"/> </returns>
    public Task<RoundType[]> GetAllRoundTypes();
}