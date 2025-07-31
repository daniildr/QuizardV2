using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Quizard.SignalR.Abstractions;

/// <summary> Базовый хаб SignalR </summary>
public class BaseHub : Hub
{
    protected readonly ILogger<BaseHub> Logger;
    
    public BaseHub(ILogger<BaseHub> logger)
    {
        Logger = logger;
    }
}