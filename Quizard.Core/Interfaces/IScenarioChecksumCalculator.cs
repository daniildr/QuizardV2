using Newtonsoft.Json;
using Quizard.Core.Entities;

namespace Quizard.Core.Interfaces;

/// <summary> Интерфейс калькулятора контрольных сумм </summary>
public interface IScenarioChecksumCalculator
{
    /// <summary> Опции сериализация объектов </summary>
    public JsonSerializerSettings JsonSerializerSettings { get; }

    /// <summary> Метод для создания каноничного Json для подсчета контрольной суммы </summary>
    /// <param name="baseScenarioDto"> DTO сценария </param>
    /// <returns> Каноничный Json</returns>
    public string GetCanonicalJson(Scenario baseScenarioDto);
    
    /// <summary> Метод выполняет расчет контрольной суммы сценария </summary>
    /// <param name="canonicalScenarioJson"> Каноничный json Scenario </param>
    /// <returns> Контрольная сумма </returns>
    public string CalculateChecksum(string canonicalScenarioJson);
    
    //TODO: Сделать внутренний подсчет контрольной суммы от имени (?)
    
    /// <summary> Метод для валидации контрольной суммы </summary>
    /// <param name="canonicalScenarioJson"> Каноничный json Scenario </param>
    /// <param name="preparedChecksum"> Контрольная сумма </param>
    /// <returns> Результат проверки </returns>
    public bool ValidateChecksum(string canonicalScenarioJson, string preparedChecksum);
}