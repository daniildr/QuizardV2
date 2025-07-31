using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces;

/// <summary> Интерфейс валидатора сценариев </summary>
public interface IScenarioValidator
{
    /// <summary> Проверяет консистентность сценария </summary>
    /// <param name="inputScenario"> Полностью заполненный объект Scenario </param>
    /// <exception cref="ScenarioValidationException"> В случае несогласованности данных сценария </exception>
    public void Validate(Scenario inputScenario);
}