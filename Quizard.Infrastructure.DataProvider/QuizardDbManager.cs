using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quizard.Core.Interfaces;
using Quizard.Core.Interfaces.Repositories;

namespace Quizard.Infrastructure.DataProvider;

/// <inheritdoc />
public class QuizardDbManager : IQuizardDbManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QuizardDbManager> _logger;

    /// <inheritdoc /> 
    public ILicenseSecretRepository LicenseSecretRepository
    {
        get
        {
            _logger.LogTrace("Вызван сервис для работы с данными секретной солью лицензий");
            return _serviceProvider.GetRequiredService<ILicenseSecretRepository>();
        }
    }

    /// <inheritdoc />
    public IMediaRepository MediaRepository
    {
        get
        {
            _logger.LogTrace("Вызван сервис для работы с данными медиа-файлов");
            return _serviceProvider.GetRequiredService<IMediaRepository>();
        }
    }

    /// <inheritdoc />
    public ILicenseRepository LicenseRepository
    {
        get
        {
            _logger.LogTrace("Вызван сервис для работы с данными лицензий");
            return _serviceProvider.GetRequiredService<ILicenseRepository>();
        }
    }

    /// <inheritdoc />
    public IRoundRepository RoundRepository
    {
        get
        {
            _logger.LogTrace("Вызван сервис для работы с раундами");
            return _serviceProvider.GetRequiredService<IRoundRepository>();
        }
    }

    /// <inheritdoc />
    public IStatisticRepository StatisticRepository
    {
        get
        {
            _logger.LogTrace("Вызван сервис для работы со статистикой раундов");
            return _serviceProvider.GetRequiredService<IStatisticRepository>();
        }
    }
    
    /// <inheritdoc />
    public IRoundTypeRepository RoundTypeRepository
    {
        get
        {
            _logger.LogTrace("Вызван сервис для работы со статистикой cсценариев");
            return _serviceProvider.GetRequiredService<IRoundTypeRepository>();
        }
    }

    /// <inheritdoc />
    public IScenarioRepository ScenarioRepository
    {
        get
        {
            _logger.LogTrace("Вызван сервис для работы с данными сценариев");
            return _serviceProvider.GetRequiredService<IScenarioRepository>();
        }
    }

    /// <inheritdoc />
    public IAnswerRepository AnswerRepository
    {
        get
        {
            _logger.LogTrace("Вызван сервис для работы с данными ответов");
            return _serviceProvider.GetRequiredService<IAnswerRepository>();
        }
    }

    /// <inheritdoc />
    public IGameRepository GameRepository
    {
        get
        {
            _logger.LogTrace("Вызван сервис для работы с данными сыгранных игр");
            return _serviceProvider.GetRequiredService<IGameRepository>();
        }
    }

    /// <inheritdoc />
    public IPlayerRepository PlayerRepository
    {
        get
        {
            _logger.LogTrace("Вызван сервис для работы с данными игроков");
            return _serviceProvider.GetRequiredService<IPlayerRepository>();
        }
    }

    /// <inheritdoc />
    public IQuestionRepository QuestionRepository
    {
        get
        {
            _logger.LogTrace("Вызван сервис для работы с данными вопросов");
            return _serviceProvider.GetRequiredService<IQuestionRepository>();
        }
    }

    /// <summary> Менеджер для работы с БД </summary>
    /// <param name="serviceProvider"> Сервис провайдер </param>
    /// <param name="logger"> Логгер </param>
    public QuizardDbManager(IServiceProvider serviceProvider, ILogger<QuizardDbManager> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _logger.LogInformation("Менеджер для работы с БД проинициализирован");
    }
}