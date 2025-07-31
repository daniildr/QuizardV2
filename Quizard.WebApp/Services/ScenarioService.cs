using Quizard.Core.Entities;

namespace Quizard.WebApp.Services;

public class ScenarioService
{
    public event Action? OnChange;
    
    private Scenario? _scenario;
    public Scenario? Scenario
    {
        get => _scenario;
        set
        {
            _scenario = value;
            OnChange?.Invoke();
        }
    }
    
    private void NotifyStateChanged() => OnChange?.Invoke();
}