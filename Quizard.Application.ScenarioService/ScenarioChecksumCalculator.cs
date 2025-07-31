using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Quizard.Core.Entities;
using Quizard.Core.Interfaces;

namespace Quizard.Application.ScenarioService;

/// <inheritdoc />
public class ScenarioChecksumCalculator : IScenarioChecksumCalculator
{
    private readonly ILogger<ScenarioChecksumCalculator> _logger;

    /// <summary> Опции сериализации объектов </summary>
    public JsonSerializerSettings JsonSerializerSettings { get; }

    /// <summary> Конструктор для ручного создания объекта без DI инверсии </summary>
    public ScenarioChecksumCalculator() : this(NullLogger<ScenarioChecksumCalculator>.Instance) { }

    public ScenarioChecksumCalculator(ILogger<ScenarioChecksumCalculator> logger)
    {
        _logger = logger;
        JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = true,
                    OverrideSpecifiedNames = false
                }
            },
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        _logger.LogInformation("Калькулятора контрольных сумм проинициализирован");
    }

    public string GetCanonicalJson(Scenario baseScenarioDto)
    {
        throw new NotImplementedException();
        // var baseDto = new BaseScenarioDto
        // {
        //     Name = baseScenarioDto.Name,
        //     GameDuration = baseScenarioDto.GameDuration,
        //     RoundDuration = baseScenarioDto.RoundDuration,
        //     Questions = baseScenarioDto.Questions,
        //     Rounds = baseScenarioDto.Rounds
        // };
        // var canonicalJson = JsonConvert.SerializeObject(
        //     baseDto,
        //     JsonSerializerSettings
        // );
        //
        // return canonicalJson;
    }

    /// <inheritdoc />
    public string CalculateChecksum(string canonicalScenarioJson)
    {
        _logger.LogDebug("Калькулятор контрольных сумм выполняет вычисления");
        
        _logger.LogTrace("Каноничный JSON: {Json}", canonicalScenarioJson);
        var j = JObject.Parse(canonicalScenarioJson);
        j.Remove("isLicensedContent");
        j.Remove("contentChecksum");
        j.Remove("roundDuration");
        j.Remove("gameDuration");
        j.Remove("name");
        j.Remove("id");
        
        var cleanedJson = j.ToString(Formatting.None);

        _logger.LogTrace("Чистый JSON: {Json}", cleanedJson);
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(cleanedJson));
        return Convert.ToHexStringLower(hashBytes);
    }

    /// <inheritdoc />
    public bool ValidateChecksum(string canonicalScenarioJson, string preparedChecksum)
    {
        _logger.LogDebug("Калькулятор контрольных сумм выполняет проверку");
        var currentChecksum = CalculateChecksum(canonicalScenarioJson);
        return currentChecksum.Equals(preparedChecksum, StringComparison.OrdinalIgnoreCase);
    }
}