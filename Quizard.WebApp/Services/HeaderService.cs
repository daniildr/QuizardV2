namespace Quizard.WebApp.Services;

public class HeaderService
{
    public event Action? OnChange;
    private string _header = "Quizard";
    public string Header
    {
        get => _header;
        set
        {
            if (_header == value) return;
            _header = value;
            OnChange?.Invoke();
        }
    }
}