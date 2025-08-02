using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Quizard.SignalR.Abstractions;

/// <summary> Базовый хаб SignalR </summary>
public class BaseHub : Hub
{
    /// <summary> Логгер </summary>
    protected readonly ILogger<BaseHub> Logger;
    
    /// <summary> Конструктор базового хаба </summary>
    /// <param name="logger"> Логгер </param>
    protected BaseHub(ILogger<BaseHub> logger)
    {
        Logger = logger;
    }
}