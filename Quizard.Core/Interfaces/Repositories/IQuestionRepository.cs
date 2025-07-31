using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces.Repositories;

/// <summary> Репозиторий вопросов </summary>
public interface IQuestionRepository
{
    /// <summary> Метод для пакептного добавления новых вопросов в БД </summary>
    /// <param name="newQuestions"> Коллекция новых вопросов </param>
    /// <returns> Коллекция новых вопросов </returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<List<Question>> AddQuestionsAsync(List<Question> newQuestions);
    
    /// <summary> Метод для добавления нового вопроса в БД </summary>
    /// <param name="newQuestion"> Новый вопросов </param>
    /// <returns></returns>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    public Task<Question> AddQuestionAsync(Question newQuestion);
    
    /// <summary> Метод для добавления нового правильного ответа в БД </summary>
    /// <param name="newReveal"> Новый правильный ответ </param>
    /// <returns></returns>
    public Task<Reveal> AddRevealAsync(Reveal newReveal);
    
    /// <summary> Метод для добавления новой подсказки в БД </summary>
    /// <param name="newHint"> Новая подсказка </param>
    /// <returns></returns>
    public Task<Hint> AddHintAsync(Hint newHint);
}