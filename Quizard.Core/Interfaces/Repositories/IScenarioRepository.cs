using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces.Repositories;

/// <summary> Репозиторий сценариев </summary>
public interface IScenarioRepository
{
    /// <summary> Метод для получения данных всех сценариев в БД </summary>
    /// <returns> Коллекция <see cref="Scenario"/> </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Scenario[]> GetScenariosAsync();
    
    /// <summary> Метод для получения данных определенных сценариев в БД </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    /// <returns> Коллекция <see cref="Scenario"/> </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Scenario[]> GetScenariosAsync(Expression<Func<Scenario, bool>> predicate);

    /// <summary> Метод для получения данных сценария из БД </summary>
    /// <param name="scenarioId"> Уникальный идентификатор сценария </param>
    /// <returns> Сущность <see cref="Scenario"/> </returns>
    /// <exception cref="ScenarioDoesNotExistException"> Исключение, если сценарий не существует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Scenario> GetScenarioAsync(string scenarioId);

    /// <summary> Метод для добавления нового сценария в БД </summary>
    /// <param name="newScenario"> Новый сценарий </param>
    /// <returns> Новый сценарий</returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Scenario> AddScenarioAsync(Scenario newScenario);
    
    /// <summary> Метод для пакетного добавления новых остатков в магазине в БД </summary>
    /// <param name="newStocks"></param>
    /// <returns></returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<List<Stock>> AddShopStocksAsync(List<Stock> newStocks);
    
    /// <summary> Метод для пакетного добавления новых этапов сценария в БД </summary>
    /// <param name="newStages"></param>
    /// <returns></returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<List<Stage>> AddStagesAsync(List<Stage> newStages);
    
    /// <summary> Метод для добавления нового файла локализации для сценария </summary>
    /// <param name="localization"> Локализация сценария </param>
    /// <returns></returns>
    public Task<Localization> AddLocalizationAsync(Localization localization);

    /// <summary> Метод для удаления сценария в БД </summary>
    /// <param name="scenarioId"> Уникальный идентификатор сценария </param>
    /// <returns> Результат операции </returns>
    /// <exception cref="ScenarioDoesNotExistException"> Исключение, если сценарий не существует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task RemoveScenarioAsync(string scenarioId);
}