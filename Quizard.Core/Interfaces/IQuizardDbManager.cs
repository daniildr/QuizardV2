using Quizard.Core.Interfaces.Repositories;

namespace Quizard.Core.Interfaces;

/// <summary> Менеджер для работы с БД </summary>
public interface IQuizardDbManager
{
    /// <inheritdoc cref="IAnswerRepository"/>
    public IAnswerRepository AnswerRepository { get; }
    
    /// <inheritdoc cref="IGameRepository"/>
    public IGameRepository GameRepository { get; }
    
    /// <inheritdoc cref="ILicenseRepository"/>
    public ILicenseRepository LicenseRepository { get; }
    
    /// <inheritdoc cref="ILicenseSecretRepository"/>
    public ILicenseSecretRepository LicenseSecretRepository { get; }
    
    /// <inheritdoc cref="IMediaRepository"/>
    public IMediaRepository MediaRepository { get; }
    
    /// <inheritdoc cref="IPlayerRepository"/>
    public IPlayerRepository PlayerRepository { get; }
    
    /// <inheritdoc cref="IQuestionRepository"/>
    public IQuestionRepository QuestionRepository { get; }
    
    /// <inheritdoc cref="IRoundRepository"/>
    public IRoundRepository RoundRepository { get; }
    
    /// <inheritdoc cref="IStatisticRepository"/>
    public IStatisticRepository StatisticRepository { get; }
    
    /// <inheritdoc cref="IRoundTypeRepository"/>
    public IRoundTypeRepository RoundTypeRepository { get; }
    
    /// <inheritdoc cref="IScenarioRepository"/>
    public IScenarioRepository ScenarioRepository { get; }
}