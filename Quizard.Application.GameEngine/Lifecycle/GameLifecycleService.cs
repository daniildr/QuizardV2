using Microsoft.Extensions.Logging;
using NUlid;
using Quizard.Core.Entities;
using Quizard.Core.Enums;
using Quizard.Core.Exceptions;
using Quizard.Core.Interfaces;
using Quizard.Core.Models.Requests;

namespace Quizard.Application.GameEngine.Lifecycle;

/// <inheritdoc/>
public class GameLifecycleService : IGameLifecycleService
{
    private readonly ILogger<GameLifecycleService> _logger;
    private readonly IScenarioFacade _scenarioFacade;
    private readonly IQuizardDbManager _quizardDbManager;
    private readonly ILicensingService _licensingService;
    private readonly IGameSessionService _gameSessionService;
    private readonly IStateMachineProvider _stateMachineProvider;
    private readonly IGameNotificationService _gameNotificationService;

    /// <summary> Конструктор сервиса </summary>
    /// <param name="logger"> Логгер </param>
    /// <param name="scenarioFacade"> Фасад для работы со сценариями </param>
    /// <param name="quizardDbManager"> Менеджер для работы с БД </param>
    /// <param name="licensingService"> Сервис лицензирования </param>
    /// <param name="gameSessionService"> Хранилище текущей игровой сессии </param>
    /// <param name="stateMachineProvider"> Поставщик машины состояний </param>
    /// <param name="gameNotificationService"> Сервис SignalR‑уведомлений </param>
    public GameLifecycleService(
        ILogger<GameLifecycleService> logger, 
        IScenarioFacade scenarioFacade, 
        IQuizardDbManager quizardDbManager, 
        ILicensingService licensingService, 
        IGameSessionService gameSessionService, 
        IStateMachineProvider stateMachineProvider, 
        IGameNotificationService gameNotificationService)
    {
        _logger = logger;
        _scenarioFacade = scenarioFacade;
        _quizardDbManager = quizardDbManager;
        _licensingService = licensingService;
        _gameSessionService = gameSessionService;
        _stateMachineProvider = stateMachineProvider;
        _gameNotificationService = gameNotificationService;

        _logger.LogInformation("Сервис управления жизненным циклом игры проинициализирован");
    }

    /// <inheritdoc/>
    public async Task<Ulid> StartGameAsync(GameStartRequestDto request)
    {
        var tokenSource = new CancellationTokenSource();
        
        _logger.LogInformation("Выполняем создание новой игровой сессии");
        try
        {
            _ = _gameSessionService.GetSession();
            
            _logger.LogDebug("Проверяем запущенные игры и все отключаем");
            await foreach (var pastGame in _quizardDbManager.GameRepository.IterateGamesAsync(tokenSource.Token))
            {
                if (!pastGame.IsRunning) continue;
            
                _logger.LogWarning("Обнаружена не завершенная игровая сессия");
                await _stateMachineProvider.Destroy();
                await _gameNotificationService.ForceDisconnect().ConfigureAwait(false);
                await _gameSessionService.ResetSession().ConfigureAwait(false);
                
                pastGame.IsRunning = false;
                await _quizardDbManager.GameRepository
                    .UpdateGameAsync((Ulid)pastGame.Id!, pastGame)
                    .ConfigureAwait(false);
            }
        }
        catch (GameSessionDoesNotExistException)
        {
            _logger.LogDebug("Игровая сессия отсутствует, продолжаем создание");
        }

        _logger.LogDebug("Выполняется проверка лицензии");
        if (!await _licensingService.CheckActiveLicense(true).ConfigureAwait(false))
            throw new LicenseValidationException("Неизвестная ошибка валидации лицензии");

        _logger.LogTrace("Получаем данные сценария");
        var scenario = await _scenarioFacade.GetScenarioAsync(request.ScenarioId).ConfigureAwait(false);

        _logger.LogTrace("Создаем новую игровую сессию");
        var game = await _quizardDbManager.GameRepository.AddGameAsync(new Game
        {
            IsRunning = true,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ScenarioId = scenario.Id
        }).ConfigureAwait(false);

        _logger.LogTrace("Сохраняем игроков");
        List<Player> players = [];
        foreach (var playerDto in request.Players)
        {
            var player = new Player { Nickname = playerDto.Nickname };
            var dbPlayer =  await _quizardDbManager.PlayerRepository.AddPlayerAsync(player).ConfigureAwait(false);
            players.Add(dbPlayer);
        }

        _logger.LogTrace("Создаем игровую сессию");
        var newSession = 
            await _gameSessionService
                .CreateSessionAsync((Ulid)game.Id!, scenario, players.AsReadOnly(), tokenSource)
                .ConfigureAwait(false);

        _logger.LogTrace("Обновляем состояние стейт машины");
        var stateMachine = _stateMachineProvider.Create(newSession);
        await stateMachine.FireAsync(GameTrigger.StartRequested);
        
        _logger.LogInformation("Игра запущена, текущее состояние: {State}", stateMachine.State);
        await _gameNotificationService.SendGameScenario(scenario);
        return (Ulid)game.Id!;
    }

    /// <inheritdoc/>
    public async Task PauseGameAsync()
    {
        if (!_stateMachineProvider.StateMachine.CanFire(GameTrigger.PauseRequested))
            return;

        _logger.LogInformation("Текущая игровая сессия будет приостановлена");
        await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.PauseRequested);
        await _gameNotificationService.GamePaused();
    }

    /// <inheritdoc/>
    public async Task SkipStage()
    {
        if (!_stateMachineProvider.StateMachine.CanFire(GameTrigger.Skip))
        {
            _logger.LogWarning("Текущий этап игры невозможно пропустить");
            return;
        }

        if (_stateMachineProvider.StateMachine.State == GameState.Pause)
        {
            _logger.LogWarning("Игра на паузе. Нельзя пропустить");
            return;
        }

        _logger.LogInformation("Текущий этап будет пропущен");
        await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.Skip);
        await _gameNotificationService.Skip();
    }

    /// <inheritdoc/>
    public async Task ResumeGameAsync()
    {
        
        if (!_stateMachineProvider.StateMachine.CanFire(GameTrigger.ResumeRequested))
            return;

        _logger.LogInformation("Будет выполнено возобновление игровой сессии");
        await _stateMachineProvider.StateMachine.FireAsync(GameTrigger.ResumeRequested);
        await _gameNotificationService.GameResumed();
    }

    /// <inheritdoc/>
    public async Task ForceStopGameAsync()
    {
        _logger.LogCritical("Будет выполнено принудительное завершения игровой сессии");
        await _stateMachineProvider.Destroy();

        _logger.LogTrace("Сбрасываем игровую сессию");
        var gameSession = _gameSessionService.GetSession();
        
        _logger.LogTrace("Завершаем игру в БД");
        var gameSessionInDb = await _quizardDbManager.GameRepository
            .GetGameAsync(game => game.Id == gameSession.GameId)
            .ConfigureAwait(false);
        gameSessionInDb.IsRunning = false;
        await _quizardDbManager.GameRepository
            .UpdateGameAsync(gameSession.GameId, gameSessionInDb)
            .ConfigureAwait(false);
        
        await gameSession.CancellationToken.CancelAsync().ConfigureAwait(false);
        await _gameSessionService.ResetSession();
        
        _logger.LogTrace("Отправляем событие об отключении клиентам");
        await _gameNotificationService.ForceDisconnect();
    }
}