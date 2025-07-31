using System.Linq.Expressions;
using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces.Repositories;

/// <summary> Репозиторий статистики раундов </summary>
public interface IStatisticRepository
{
    /// <summary> Метод для получения статистики раунда </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    /// <returns> <see cref="RoundStatistic"/> </returns>
    /// <exception cref="StatisticDoesNotExistException{RoundStatistic}">
    /// Исключение, если статистика не найдена в БД
    /// </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<RoundStatistic[]> GetRoundStatistic(Expression<Func<RoundStatistic, bool>> predicate);
    
    /// <summary> Метод для отправки новой записи статистики </summary>
    /// <param name="roundStatistic"> Новая запись статистики раунда </param>
    /// <returns> Новая запись <see cref="RoundStatistic"/> </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<RoundStatistic> SubmitRoundStatistic(RoundStatistic roundStatistic);
    
    /// <summary> Метод для получения статистики сценария </summary>
    /// <param name="predicate"> Функция - предикат поиска </param>
    /// <returns> <see cref="ScenarioStatistic"/> </returns>
    /// <exception cref="StatisticDoesNotExistException{ScenarioStatistic}">
    /// Исключение, если статистика не найдена в БД
    /// </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<ScenarioStatistic[]> GetScenarioStatistic(Expression<Func<ScenarioStatistic, bool>> predicate);
    
    /// <summary> Метод для отправки новой записи статистики сценария </summary>
    /// <param name="scenarioStatistic"> Новая запись статистики раунда </param>
    /// <returns> Новая запись <see cref="RoundStatistic"/> </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<ScenarioStatistic> SubmitScenarioStatistic(ScenarioStatistic scenarioStatistic);
}