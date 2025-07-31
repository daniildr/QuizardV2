using System.Windows;

namespace Quizard.Application.LicensingApplication;

public partial class AboutWindow
{
    public AboutWindow()
    {
        InitializeComponent();

        VerTextBlock.Text = $"Версия {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}