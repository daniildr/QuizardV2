using Microsoft.Extensions.Logging;
using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;

namespace Quizard.Application.ScenarioService;

/// <summary> Фасад для работы со сценариями </summary>
public class ScenarioFacade : IScenarioFacade
{
    private readonly ILogger<ScenarioFacade> _logger;
    private readonly IAesEncryptor _aesEncryptor;
    private readonly IQuizardDbManager _dbManager;
    private readonly ILicensingService _licensingService;
    private readonly IScenarioValidator _scenarioValidator;
    private readonly IScenarioChecksumCalculator _checksumCalculator;

    /// <summary> Конструктор фасада </summary>
    /// <param name="checksumCalculator"> Калькулятор контрольных сумм</param>
    /// <param name="scenarioValidator"> Валидатор </param>
    /// <param name="licensingService"> Сервис лицензирования </param>
    /// <param name="dbManager"> Менеджер БД </param>
    /// <param name="aesEncryptor"> Шифровальщик </param>
    /// <param name="logger"> Логгер </param>
    public ScenarioFacade(
        IScenarioChecksumCalculator checksumCalculator,
        IScenarioValidator scenarioValidator,
        ILicensingService licensingService,
        IQuizardDbManager dbManager,
        IAesEncryptor aesEncryptor,
        ILogger<ScenarioFacade> logger)
    {
        _logger = logger;
        _dbManager = dbManager;
        _aesEncryptor = aesEncryptor;
        _licensingService = licensingService;
        _scenarioValidator = scenarioValidator;
        _checksumCalculator = checksumCalculator;

        _logger.LogInformation("Фасад для работы со сценариями проинициализирован");
    }

    /// <inheritdoc />
    public async Task<Scenario> GetScenarioAsync(string scenarioId)
    {
        _logger.LogInformation("Получение сценария с ID: {ScenarioId}", scenarioId);
        var scenario = await _dbManager.ScenarioRepository.GetScenarioAsync(scenarioId).ConfigureAwait(false);

        // _logger.LogTrace("Проверяем возможность запуска сценария с ID: {ScenarioId}", scenarioId);
        // var isLicensed = bool.Parse(await _aesEncryptor.DecryptAsync(scenario.IsLicensedContent).ConfigureAwait(false));
        // if (!isLicensed)
        //     if (await _licensingService.CanUploadNonLicensedContentAsync() == false)
        //         throw new LicensePermissionLevelException("Невозможно запустить нелицензионный контент");

        // TODO: Включить проверку контрольной суммы
        // _logger.LogTrace("Проверяем контрольную сумму сценария с ID: {ScenarioId}", scenarioId);
        // var canonicalJson = _checksumCalculator.GetCanonicalJson(new ScenarioDto(scenario));
        // if (!_checksumCalculator.ValidateChecksum(canonicalJson, scenario.ContentChecksum.Checksum))
        // {
        //     _logger.LogCritical("Ошибка валидации контрольной суммы для сценария {ScenarioId}", scenarioId);
        //     throw new ChecksumValidationException($"Ошибка валидации контрольной суммы для сценария {scenarioId}");
        // }

        _logger.LogDebug("Сценарий {ScenarioId} успешно получен", scenarioId);
        return scenario;
    }

    /// <inheritdoc />
    public async Task<Scenario> CreateScenarioAsync(Scenario uploadScenario)
    {
        _logger.LogDebug("Валидацию DTO сценария");
        _scenarioValidator.Validate(uploadScenario);

        _logger.LogDebug("Выполняем маппинг и сохранение сценария");
        var scenario = await InternalCreateScenarioAsync(uploadScenario).ConfigureAwait(false);

        // _logger.LogTrace("Сериализация каноничного Json");
        // var canonicalJson = _checksumCalculator.GetCanonicalJson(uploadScenario);

        // if (uploadScenario.IsLicensedContent)
        // {
        //     if (!_checksumCalculator.ValidateChecksum(canonicalJson, uploadScenario.ContentChecksum))
        //     {
        //         await _dbManager.ScenarioRepository
        //             .RemoveScenarioAsync(Ulid.Parse(scenarioDto.Id))
        //             .ConfigureAwait(false);
        //
        //         throw new ScenarioValidationException("Контрольные суммы сценариев не совпадают");
        //     }
        // }
        // else
        // {
        //     if (await _licensingService.CanUploadNonLicensedContentAsync() == false)
        //     {
        //         await _dbManager.ScenarioRepository
        //             .RemoveScenarioAsync(Ulid.Parse(scenarioDto.Id))
        //             .ConfigureAwait(false);
        //
        //         throw new LicensePermissionLevelException(
        //             "Уровень лицензии позволяет загружать только лицензионный контент");
        //     }
        // }
        //
        // _logger.LogDebug("Выполняем подсчет и сохранение контрольной суммы сценария");
        // var scenario = await _dbManager.ScenarioRepository
        //     .GetScenarioAsync(Ulid.Parse(scenarioDto.Id))
        //     .ConfigureAwait(false);
        // var contentChecksum = new ContentChecksum
        // {
        //     ScenarioId = scenario.Id,
        //     Checksum = _checksumCalculator.CalculateChecksum(
        //         _checksumCalculator.GetCanonicalJson(new ScenarioDto(scenario))),
        //     CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        // };
        // await _dbManager.ContentChecksumRepository.AddChecksumAsync(contentChecksum).ConfigureAwait(false);

        return scenario;
    }

    /// <inheritdoc />
    public async Task DeleteScenarioAsync(string scenarioId)
    {
        _logger.LogDebug("Будет выполнено удаление сценария {Id}", scenarioId);

        await _dbManager.ScenarioRepository.RemoveScenarioAsync(scenarioId).ConfigureAwait(false);
    }

    private async Task<Scenario> InternalCreateScenarioAsync(Scenario uploadScenario)
    {
        _logger.LogInformation("Выполняется маппинг и сохранение сущностей");
        var newScenario = await _dbManager.ScenarioRepository.AddScenarioAsync(new Scenario
        {
            Id = uploadScenario.Id,
            Name = uploadScenario.Name,
            Description = uploadScenario.Description,
            GameDuration = uploadScenario.GameDuration,
            BasePointPrice = uploadScenario.BasePointPrice,
            StartPlayerScore = uploadScenario.StartPlayerScore,
            RoundPresentationDuration = uploadScenario.RoundPresentationDuration,
            ShowScenarioStatsOnFinish = uploadScenario.ShowScenarioStatsOnFinish,
            FinishPlaceholder = uploadScenario.FinishPlaceholder,
            Placeholder = uploadScenario.Placeholder
        }).ConfigureAwait(false);

        await _dbManager.ScenarioRepository.AddShopStocksAsync(uploadScenario.ShopStocks.Select(shopStock => new Stock
        {
            ScenarioId = newScenario.Id,
            ModifierType = shopStock.ModifierType,
            Name = shopStock.Name,
            Description = shopStock.Description,
            IconUrl = shopStock.IconUrl,
            Quantity = shopStock.Quantity,
            CostMultiplier = shopStock.CostMultiplier,
            UniqForPlayer = shopStock.UniqForPlayer
        }).ToList()).ConfigureAwait(false);

        await _dbManager.ScenarioRepository.AddLocalizationAsync(new Localization
        {
            ScenarioId = newScenario.Id,
            ButtonUp = uploadScenario.Localization.ButtonUp,
            ButtonDown = uploadScenario.Localization.ButtonDown,
            ButtonConfirm = uploadScenario.Localization.ButtonConfirm,
            ButtonCancel = uploadScenario.Localization.ButtonCancel,
            ButtonContinue = uploadScenario.Localization.ButtonContinue,
            ButtonBuy = uploadScenario.Localization.ButtonBuy,
            ButtonFinish = uploadScenario.Localization.ButtonFinish,
            ButtonStub = uploadScenario.Localization.ButtonStub,
            BidButtonLabel = uploadScenario.Localization.BidButtonLabel,
            
            AttentionLabel = uploadScenario.Localization.AttentionLabel,
            WaitOtherPlayersLabel = uploadScenario.Localization.WaitOtherPlayersLabel,
            WaitYourTurnLabel = uploadScenario.Localization.WaitYourTurnLabel,
            HintLabel = uploadScenario.Localization.HintLabel,
            SpeedWinnerLabel = uploadScenario.Localization.SpeedWinnerLabel,
            PantomimeLabel = uploadScenario.Localization.PantomimeLabel,
            VotingLabel = uploadScenario.Localization.VotingLabel,
            PlayerLabel = uploadScenario.Localization.PlayerLabel,
            ScoreLabel = uploadScenario.Localization.ScoreLabel,
            BetTime = uploadScenario.Localization.BetTime,
            MakeBetLabelPart1 = uploadScenario.Localization.MakeBetLabelPart1,
            MakeBetLabelPart2 = uploadScenario.Localization.MakeBetLabelPart2,
            BidAcceptedLabelPart1 = uploadScenario.Localization.BidAcceptedLabelPart1,
            BidAcceptedLabelPart2 = uploadScenario.Localization.BidAcceptedLabelPart2,
            PriceLabel = uploadScenario.Localization.PriceLabel,
            
            LoginLabel = uploadScenario.Localization.LoginLabel,
            RoundLabel = uploadScenario.Localization.RoundLabel,
            RoundResultsLabel = uploadScenario.Localization.RoundResultsLabel,
            RoundChoiceLabel = uploadScenario.Localization.RoundChoiceLabel,
            ShopLabel = uploadScenario.Localization.ShopLabel,
            InsufficientFunds = uploadScenario.Localization.InsufficientFunds,
            ProductHasBeenPurchased = uploadScenario.Localization.ProductHasBeenPurchased,
            ProductAlreadyPurchased = uploadScenario.Localization.ProductAlreadyPurchased,
            ShopWaitLabel = uploadScenario.Localization.ShopWaitLabel,
            ApplyingModifiersLabel = uploadScenario.Localization.ApplyingModifiersLabel,
            GameFinishLabel = uploadScenario.Localization.GameFinishLabel,
        }).ConfigureAwait(false);

        var stages = new List<Stage>();

        foreach (var stage in uploadScenario.Stages)
        {
            stages.Add(new Stage
            {
                ScenarioId = newScenario.Id,
                Index = stage.Index,
                Type = stage.Type,
                MediaId = stage.MediaId,
                RoundId = stage.RoundId,
                StageDuration = stage.StageDuration
            });

            if (stage is { Type: ScenarioStage.Media, MediaId: not null })
                await ProcessMediaAsync(stage.MediaId!, stage.Media!).ConfigureAwait(false);
            else if (stage is { Type: ScenarioStage.Finish, MediaId: not null })
                await ProcessMediaAsync(stage.MediaId!, stage.Media!).ConfigureAwait(false);
            else if (stage is { Type: ScenarioStage.Round, RoundId: not null })
                await ProcessRoundAsync(stage.RoundId, newScenario.Id, stage.Round!).ConfigureAwait(false);
            else if (stage is { Type: ScenarioStage.Finish, RoundId: not null })
                await ProcessRoundAsync(stage.RoundId, newScenario.Id, stage.Round!).ConfigureAwait(false);
        }

        await _dbManager.ScenarioRepository.AddStagesAsync(stages).ConfigureAwait(false);

        foreach (var roundDefinition in uploadScenario.RoundDefinitions)
            await ProcessRoundAsync(roundDefinition.RoundId, newScenario.Id, roundDefinition).ConfigureAwait(false);

        return await _dbManager.ScenarioRepository.GetScenarioAsync(newScenario.Id).ConfigureAwait(false);
    }

    private async Task ProcessMediaAsync(string mediaId, Media media)
    {
        try
        {
            var oldMedia = await _dbManager.MediaRepository
                .GetMediaAsync(m => m.MediaId == mediaId)
                .ConfigureAwait(false);
            await _dbManager.MediaRepository.RemoveMediaAsync(oldMedia.MediaId).ConfigureAwait(false);
        }
        catch
        {
            // ignore
        }

        var newMedia = new Media
        {
            MediaId = mediaId,
            Type = media.Type,
            Url = media.Url!,
            Content = media.Content!,
            DelaySeconds = media.DelaySeconds,
            Duration = media.Duration,
            ShowOnPlayer = media.ShowOnPlayer
        };

        await _dbManager.MediaRepository.AddMediaAsync(newMedia).ConfigureAwait(false);
    }

    private async Task ProcessRoundAsync(string roundId, string scenarioId, Round round)
    {
        var newRound = await _dbManager.RoundRepository.AddRoundAsync(new Round
        {
            RoundId = roundId,
            ScenarioId = scenarioId,
            RoundTypeId = round.RoundTypeId,
            Name = round.Name,
            Description = round.Description!,
            PreviewUrl = round.PreviewUrl!,
            RoundDuration = round.RoundDuration,
            CorrectMultiplier = round.CorrectMultiplier,
            MissedMultiplier = round.MissedMultiplier,
            IncorrectMultiplier = round.IncorrectMultiplier,
        }).ConfigureAwait(false);

        foreach (var question in round.Questions)
        {
            if (question.MediaId != null)
                await ProcessMediaAsync(question.MediaId, question.Media!).ConfigureAwait(false);

            var newQuestion = await _dbManager.QuestionRepository
                .AddQuestionAsync(new Question
                {
                    RoundId = newRound.RoundId!,
                    QuestionNumber = question.QuestionNumber,
                    QuestionText = question.QuestionText,
                    MediaId = question.MediaId,
                    AnswerDelay = question.AnswerDelay,
                    QuestionTimeout = question.QuestionTimeout,
                }).ConfigureAwait(false);

            foreach (var answer in question.Answers)
            {
                await _dbManager.AnswerRepository
                    .AddAnswerAsync(new Answer
                    {
                        QuestionId = newQuestion.Id,
                        Text = answer.Text,
                        Button = answer.Button,
                        IsCorrect = answer.IsCorrect,
                        Order = answer.Order
                    }).ConfigureAwait(false);
            }

            if (question.Reveal.MediaId != null)
                await ProcessMediaAsync(question.Reveal.MediaId, question.Reveal.Media!).ConfigureAwait(false);

            await _dbManager.QuestionRepository
                .AddRevealAsync(new Reveal
                {
                    QuestionId = newQuestion.Id,
                    MediaId = question.Reveal.MediaId!,
                    Text = question.Reveal.Text,
                    Duration = question.Reveal.Duration
                }).ConfigureAwait(false);

            if (question.Hints.Count > 0)
            {
                foreach (var hint in question.Hints)
                {
                    await _dbManager.QuestionRepository.AddHintAsync(new Hint
                    {
                        QuestionId = newQuestion.Id,
                        Text = hint.Text
                    }).ConfigureAwait(false);
                }
            }
        }
    }
}