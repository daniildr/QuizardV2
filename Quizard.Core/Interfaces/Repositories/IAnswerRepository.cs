using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces.Repositories;

/// <summary> Репозиторий ответов </summary>
public interface IAnswerRepository
{
    /// <summary> Метод для пакетного добавления новых ответов в БД </summary>
    /// <param name="newAnswers"> Коллекция новых ответов </param>
    /// <returns> Коллекция новых ответов </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<List<Answer>> AddAnswersAsync(List<Answer> newAnswers);
    
    /// <summary> Метод для добавления новых ответов в БД </summary>
    /// <param name="newAnswer"> Коллекция новых ответов </param>
    /// <returns></returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Answer> AddAnswerAsync(Answer newAnswer);
}