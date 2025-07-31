using Microsoft.Extensions.Logging;
using Quizard.Core.Interfaces;

namespace Quizard.Core.Utils;

/// <summary> Утилита подсчета очков в интерактивном раунде </summary>
public static class InteractivePointsCounter
{
    /// <summary> Метод для подсчета очков ведущего (показывающего игрока) в интерактивном раунде </summary>
    /// <param name="logger"> Логгер </param>
    /// <param name="gameSession"> Текущая игровая сессия </param>
    /// <returns> Количество баллов, которое набрал ведущий </returns>
    public static int CalculateShowPlayerScore(ILogger logger, IGameSession gameSession)
    {
        var expectedGuessersCount = gameSession.Players.Count - 1;

        var guessersAnswers = gameSession.PlayersAnswersOnInteractiveQuestion.Select(kvp => kvp.Value).ToList();
        
        logger.LogTrace("Считаем, сколько человек угадали");
        var correctCount = guessersAnswers.Count(isCorrect => isCorrect);

        logger.LogTrace("Вычисляем баллы для ведущего");
        int showPlayerPoints;

        if (correctCount == 0 || correctCount == expectedGuessersCount)
        {
            logger.LogTrace("Никто не угадал или угадали все");
            showPlayerPoints = -(gameSession.CurrentRound.CorrectMultiplier * gameSession.Scenario.BasePointPrice);
        }
        else
        {
            logger.LogTrace("Угадали не все (но есть хотя бы один)");
            showPlayerPoints = gameSession.CurrentRound.CorrectMultiplier * gameSession.Scenario.BasePointPrice;
        }

        logger.LogDebug("Показывающий игрок набрал - {ShowPlayerPoints}", showPlayerPoints);
        return showPlayerPoints;
    }
}