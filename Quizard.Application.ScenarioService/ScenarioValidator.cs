using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quizard.Core.Entities;
using Quizard.Core.Interfaces;

namespace Quizard.Application.ScenarioService;

/// <inheritdoc />
public class ScenarioValidator : IScenarioValidator
{
    private readonly ILogger<ScenarioValidator> _logger;
    
    /// <summary> Конструктор для ручного создания объекта без DI инверсии </summary>
    public ScenarioValidator()
        : this(NullLogger<ScenarioValidator>.Instance) { }

    /// <summary> Конструктор валидатора </summary>
    /// <param name="logger"> Логгер </param>
    public ScenarioValidator(ILogger<ScenarioValidator> logger)
    {
        _logger = logger;
        
        _logger.LogInformation("Валидатор консистентности сценария проинициализирован");
    }
    
    /// <inheritdoc />
    public void Validate(Scenario inputScenario)
    {
        return;
        // _logger.LogDebug("Будет выполнена проверка загруженного сценария");
        //
        // var scenario = inputScenario.DeepClone();
        // _logger.LogTrace("Загруженный сценарий: {@Scenario}", scenario);
        //
        // var errors = new List<string>();
        //
        // _logger.LogTrace("Проверка имени сценария");
        // if (string.IsNullOrWhiteSpace(scenario.Name)) errors.Add("Сценарий должен иметь имя.");
        //
        // _logger.LogTrace("Проверка общих требований к сценарию");
        // if (scenario.Rounds.Length == 0) errors.Add("Сценарий должен содержать хотя бы один раунд.");
        // if (scenario.Questions.Length == 0) errors.Add("Сценарий должен содержать хотя бы один вопрос.");
        //
        // _logger.LogTrace("Проверка уникальность раундов по имени");
        // var dupRoundNames = scenario.Rounds
        //     .GroupBy(round => round.Name.Trim())
        //     .Where(grouping => grouping.Count() > 1)
        //     .Select(grouping => $"Дублирующее имя раунда: '{grouping.Key}'");
        // errors.AddRange(dupRoundNames);
        //
        // _logger.LogTrace("Проверка уникальность вопросов по тексту (только для текстовых вопросов");
        // var dupQuestionTexts = scenario.Questions.Where(q => q.Media.Type == MediaType.Text)
        //     .GroupBy(question => question.QuestionText.Trim())
        //     .Where(grouping => grouping.Count() > 1)
        //     .Select(grouping => $"Дублирующий текст вопроса: '{grouping.Key}'");
        // errors.AddRange(dupQuestionTexts);
        //
        // _logger.LogTrace("Проверка каждого Question");
        // for (var i = 0; i < scenario.Questions.Length; i++)
        // {
        //     var idx = i + 1;
        //
        //     if (string.IsNullOrWhiteSpace(scenario.Questions[i].Theme))
        //         errors.Add($"Вопрос[{idx}]: Тема вопроса не должен быть пустой.");
        //
        //     if (scenario.Questions[i].Difficulty < 1)
        //         errors.Add($"Вопрос[{idx}]: Сложность должен быть >= 1.");
        //
        //     if (string.IsNullOrWhiteSpace(scenario.Questions[i].Media.Url))
        //         if (scenario.Questions[i].Media.Type != MediaType.Text)
        //             errors.Add($"Вопрос[{idx}]: Media.Url не должен быть пустым.");
        //
        //     if (scenario.Questions[i].Answers.Length == 0)
        //         if (scenario.Questions[i].Media.Type == MediaType.Text)
        //             errors.Add($"Вопрос[{idx}]: Должен содержать хотя бы один Ответ (Answer).");
        //
        //     _logger.LogTrace("Проверка вопросов с вариантами проверяем флаг IsCorrect");
        //     if (scenario.Questions[i].Answers.Length != 0 
        //         && scenario.Questions[i].Answers.Any(answer => answer.IsCorrect == true) == false)
        //         if (scenario.Questions[i].Answers.All(answerDto => answerDto.Order == null))
        //             errors.Add($"Вопрос[{idx}]: Должен быть хотя бы один ответ с IsCorrect == true. " +
        //                        $"Или должен быть порядок ответов");
        //     
        //     _logger.LogTrace("Проверка дубликатов ответов");
        //     var dupAns = scenario.Questions[i].Answers.GroupBy(answer => answer.Text.Trim())
        //         .Where(grouping => grouping.Count() > 1)
        //         .Select(grouping => $"Вопрос[{idx}]: дублирующий текст ответа '{grouping.Key}'")
        //         .ToArray();
        //     if (dupAns.Length != 0) 
        //         errors.AddRange(dupAns);
        // }
        //
        // _logger.LogTrace("Проверка каждого Round");
        // var firstRoundExists = false;
        // for (var i = 0; i < scenario.Rounds.Length; i++)
        // {
        //     var idx = i + 1;
        //
        //     if (string.IsNullOrWhiteSpace(scenario.Rounds[i].Name))
        //         errors.Add($"Раунд[{idx}]: Name не должен быть пустым.");
        //     
        //     if (string.IsNullOrWhiteSpace(scenario.Rounds[i].PreviewUrl))
        //         errors.Add($"Раунд[{idx}]: PreviewUrl не должен быть пустым.");
        //
        //     List<string> roundsTypes = ["single_answer", "order", "interactive", "queue"];
        //     if (!roundsTypes.Contains(scenario.Rounds[i].RoundType))
        //         errors.Add($"Раунд[{idx}]: Недопустимый тип раунда.");
        //
        //     List<string> timeTypes = ["unlimited", "countdown"];
        //     if (!timeTypes.Contains(scenario.Rounds[i].TimeType))
        //         errors.Add($"Раунд[{idx}]: Недопустимый тип временных ограничений.");
        //
        //     List<string> additionalTypes = ["stepped", "hints", "auction"];
        //     if (scenario.Rounds[i].AdditionalTypes.Length > 0)
        //         errors.AddRange(
        //             from additionalType in scenario.Rounds[i].AdditionalTypes 
        //             where !additionalTypes.Contains(additionalType) 
        //             select $"Раунд[{idx}]: Недопустимый тип дополнительных механик.");
        //
        //     List<string> scoreTypes = ["fixed", "speed", "penalties"];
        //     if (!scoreTypes.Contains(scenario.Rounds[i].ScoringType))
        //         errors.Add($"Раунд[{idx}]: Недопустимый тип начисления очков.");
        //
        //     if (!firstRoundExists)
        //     {
        //         if (scenario.Rounds[i].FirstRound)
        //             firstRoundExists = true;
        //     }
        //     else
        //     {
        //         if (scenario.Rounds[i].FirstRound)
        //             errors.Add($"Раунд[{idx}]: Указан как первый раунд, но первый раунд уже определен.");
        //     }
        //
        //     if (scenario.Rounds[i].Questions < 1)
        //         errors.Add($"Раунд[{idx}]: Questions должен быть >= 1.");
        //
        //     if (scenario.Rounds[i].MinDifficulty < 1)
        //         errors.Add($"Раунд[{idx}]: MinDifficulty должен быть >= 1.");
        //
        //     if (scenario.Rounds[i].MaxDifficulty < scenario.Rounds[i].MinDifficulty)
        //         errors.Add($"Раунд[{idx}]: MaxDifficulty ({scenario.Rounds[i].MaxDifficulty}) " +
        //                    $"< MinDifficulty ({scenario.Rounds[i].MinDifficulty}).");
        //
        //     _logger.LogTrace("Проверяем, что pool вопросов позволяет заполнить раунд");
        //     var matchCount = scenario.Questions.Count(question =>
        //         question.Difficulty >= scenario.Rounds[i].MinDifficulty &&
        //         question.Difficulty <= scenario.Rounds[i].MaxDifficulty &&
        //         question.Media.Type == scenario.Rounds[i].MediaType &&
        //         question.Answers.Length == scenario.Rounds[i].Options
        //     );
        //
        //     if (matchCount < scenario.Rounds[i].Questions)
        //         errors.Add(
        //             $"Раунд[{idx}] '{scenario.Rounds[i].Name}': требует {scenario.Rounds[i].Questions} вопросов, " +
        //             $"но подходящих только {matchCount}."
        //         );
        // }
        //
        // _logger.LogTrace("Проверка лицензионный контента");
        // if (scenario.IsLicensedContent && string.IsNullOrWhiteSpace(scenario.ContentChecksum))
        //     errors.Add("При IsLicensedContent=true ContentChecksum не может быть пустым.");
        //
        // if (errors.Count == 0) return;
        //
        // _logger.LogCritical("Загруженный сценарий имеет {Count} ошибок", errors.Count);
        // throw new ScenarioValidationException(errors.ToArray());
    }
}