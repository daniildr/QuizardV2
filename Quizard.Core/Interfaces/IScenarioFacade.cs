using Quizard.Core.Entities;
using Quizard.Core.Exceptions;

namespace Quizard.Core.Interfaces;

/// <summary> Фасад для работы со сценариями </summary>
public interface IScenarioFacade
{
    /// <summary> Получает определенный сценарий </summary>
    /// <remarks> В рамках получения сценария проводится проверка контрольных сумм и разрешений на запуск </remarks>
    /// <param name="scenarioId"> Уникальный идентификатор сценария </param>
    /// <returns> <see cref="Scenario"/> </returns>
    /// <exception cref="ScenarioDoesNotExistException"> Если сценарий не найден </exception>
    /// <exception cref="LicensePermissionLevelException"> Если уровень лицензии недостаточный для операции </exception>
    /// <exception cref="ChecksumValidationException"> В случае несогласованности данных контрольной суммы </exception>
    public Task<Scenario> GetScenarioAsync(string scenarioId);
    
    /// <summary> Загрузить новый сценарий </summary>
    /// <remarks> 
    /// <list type="number">
    /// <listheader>
    /// <description> Пайп-лайн метода: </description>
    /// </listheader>
    /// <item><description> Валидация DTO (ScenarioDtoValidator) </description></item>
    /// <item><description> Сохранение через IScenarioRepository </description></item>
    /// <item><description> Проверка лицензии (CanUploadNonLicensedContentAsync) </description></item>
    /// <item><description> Вычисление checksum (IScenarioChecksumCalculator) </description></item>
    /// <item><description> Оценка вариативности (IScenarioVariabilityEvaluator) </description></item>
    /// <item><description> Возврат <see cref="Scenario"/> </description></item>
    /// </list>
    /// </remarks>
    /// <param name="uploadScenario"> Новый загруженный сценарий </param>
    /// <returns> Добавленный сценарий </returns>
    /// <exception cref="LicensePermissionLevelException"> Если уровень лицензии недостаточный для операции </exception>
    /// <exception cref="ScenarioValidationException"> В случае несогласованности данных сценария </exception>
    public Task<Scenario> CreateScenarioAsync(Scenario uploadScenario);

    /// <summary> Удалить сценарий </summary>
    /// <param name="scenarioId"> Уникальный идентификатор сценарий </param>
    /// <returns> Результат операции </returns>
    /// <exception cref="ScenarioDoesNotExistException"> Исключение, если сценарий не существует в БД </exception>
    /// <exception cref="DatabaseUnavailableException"> Исключение, если подключится к БД не получится </exception>
    Task DeleteScenarioAsync(string scenarioId);
}