using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quizard.Core.Enums;
using Quizard.Core.Interfaces;
using Stateless;

namespace Quizard.Application.GameEngine.StateMachine.Configurations;

public class RevealConfig : IGameStateConfiguration
{
    private readonly ILogger<RevealConfig> _logger;
    private readonly IServiceProvider _serviceProvider;

    private IGameSessionService GameSessionService => _serviceProvider.GetRequiredService<IGameSessionService>();

    private IGameNotificationService GameNotificationService
        => _serviceProvider.GetRequiredService<IGameNotificationService>();
    
    public RevealConfig(
        ILogger<RevealConfig> logger,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _logger.LogInformation("Конфигурация FSM - Reveal Showing");
    }
    
    public void Configure(StateMachine<GameState, GameTrigger> stateMachine, IStateMachineConfigurator context)
    {
        _logger.LogTrace("Конфигурируем поведение FSM в состоянии RevealShowing");
        stateMachine.Configure(GameState.RevealShowing)
            .OnEntryFromAsync(GameTrigger.RoundTimeout, async () =>
            {
                _logger.LogInformation("Вопрос завершен по триггеру RoundTimeout");
                _logger.LogDebug("Отправляем правильный ответ информатору");
                var session = GameSessionService.GetSession(true);
                var reveal = session.CurrentRound!.Questions
                    .First(question => question.QuestionNumber == session.CurrentQuestionIndex).Reveal;
                await GameNotificationService.ShowReveal(reveal);

                session.CurrentQuestionIndex = session.CurrentRound!.Questions.Count;
                await GameSessionService.SaveChangesAsync();
            })
            .OnEntryAsync(async () =>
            {
                _logger.LogDebug("Отправляем правильный ответ информатору");
                var session = GameSessionService.GetSession();
                var reveal = session.CurrentRound!.Questions
                    .First(question => question.QuestionNumber == session.CurrentQuestionIndex).Reveal;
                await GameNotificationService.ShowReveal(reveal);
            })
            .PermitDynamicAsync(GameTrigger.Skip, async () =>
            {
                _logger.LogDebug("Правильный ответ показан. Выполняется переход к следующему состоянию или вопросу");
                var session = GameSessionService.GetSession(true);
                session.PlayersWhoCompleteState.Clear();
                session.PlayersSpeedAnswers.Clear();
                session.PlayersAnswersOnInteractiveQuestion.Clear();
                await GameSessionService.SaveChangesAsync();

                if (session.CurrentQuestionIndex < session.CurrentRound!.Questions.Count)
                    return GameState.QuestionPlaying;

                _logger.LogDebug("Показан ответ на последний вопрос. Завершаем раунд");
                return GameState.WaitStats;
            })
            .PermitDynamicAsync(GameTrigger.RevealShowed, async () =>
            {
                _logger.LogDebug("Правильный ответ показан. Выполняется переход к следующему состоянию или вопросу");
                var session = GameSessionService.GetSession(true);
                session.PlayersWhoCompleteState.Clear();
                session.PlayersSpeedAnswers.Clear();
                session.PlayersAnswersOnInteractiveQuestion.Clear();
                await GameSessionService.SaveChangesAsync();

                if (session.CurrentQuestionIndex < session.CurrentRound!.Questions.Count) 
                    return session.CurrentRound!.RoundTypeId == RoundTypeId.Auction 
                        ? GameState.Auction 
                        : GameState.QuestionPlaying;

                _logger.LogDebug("Показан ответ на последний вопрос. Завершаем раунд");
                return GameState.WaitStats;
            })
            .Permit(GameTrigger.EndRequested, GameState.Finished)
            .PermitDynamic(GameTrigger.PauseRequested, () => context.ProcessPause(stateMachine))
            .OnExitAsync(async () =>
            {
                _logger.LogDebug("Поднимаем счетчик вопросов");
                var session = GameSessionService.GetSession(true);
                session.CurrentQuestionIndex++;
                await GameSessionService.SaveChangesAsync();
            });
    }
}