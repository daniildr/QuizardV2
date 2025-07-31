using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using NUlid;
using Quizard.Application.LicensingApplication.Models;
using Quizard.Application.LicensingApplication.Services;
using Quizard.Application.LicensingService.Interfaces;
using Quizard.Application.ScenarioService;
using Quizard.Core.Interfaces;

namespace Quizard.Application.LicensingApplication;

/// <summary> Главное окно приложения </summary>
public partial class MainWindow
{
    private readonly IScenarioValidator _scenarioValidator;
    private readonly ILicenseKeyGenerator _licenseKeyGenerator;
    private readonly IScenarioChecksumCalculator _scenarioChecksumCalculator;
    private static readonly Regex UlidRegex = new(@"^[0-9A-HJKMNP-TV-Z]{26}$", RegexOptions.Compiled);

    public MainWindow()
    {
        _scenarioValidator = new ScenarioValidator();
        _licenseKeyGenerator = new LicenseKeyGenerator();
        _scenarioChecksumCalculator = new ScenarioChecksumCalculator();

        InitializeComponent();

        DaysTextBox.Text = "14";
        GamesTextBox.Text = "42";

        SaveButton.IsEnabled = false;
    }

    /// <summary> Вызов окна "О программе" </summary>
    private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var about = new AboutWindow { Owner = this };
        about.ShowDialog();
    }

    /// <summary> Генерация лицензии </summary>
    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        var salt = SaltTextBox.Text.Trim();

        if (!UlidRegex.IsMatch(salt))
        {
            SaltErrorTextBlock.Text = "Неверная строка (должно быть 26 символов Base32)";
            SaltErrorTextBlock.Visibility = Visibility.Visible;
            SaltTextBox.Focus();
            return;
        }

        if (string.IsNullOrEmpty(DaysTextBox.Text))
        {
            DaysErrorTextBlock.Visibility = Visibility.Visible;
            return;
        }

        if (string.IsNullOrEmpty(GamesTextBox.Text))
        {
            GamesErrorTextBlock.Visibility = Visibility.Visible;
            return;
        }

        SaltErrorTextBlock.Visibility = Visibility.Hidden;
        DaysErrorTextBlock.Visibility = Visibility.Hidden;
        GamesErrorTextBlock.Visibility = Visibility.Hidden;

        var ulid = Ulid.Parse(salt);
        OutputSaltTextBox.Text = ulid.ToString();
        OutputSaltTimeTextBox.Text = $"Время создания: [UTC-0] {ulid.Time}";
        OutputSaltTimeTextBox.Visibility = Visibility.Visible;

        var claims = new Dictionary<string, object>
        {
            { "ExpirationTime", DateTimeOffset.UtcNow.AddDays(int.Parse(DaysTextBox.Text)).ToUnixTimeSeconds() },
            { "GamesLeft", int.Parse(GamesTextBox.Text) }
        };
        var licenseKey = _licenseKeyGenerator.GenerateLicenseKey(ulid.ToString(), claims);
        OutputLicenseTextBox.Text = licenseKey;

        SaveButton.IsEnabled = true;
    }

    /// <summary> Валидация сценария </summary>
    private void ValidateButton_Click(object sender, RoutedEventArgs e)
    {
        // var scenarioText = InputScenarioTextBox.Text;
        //
        // try
        // {
        //     var dto = JsonConvert.DeserializeObject<UploadScenarioDto>(
        //                   scenarioText, 
        //                   _scenarioChecksumCalculator.JsonSerializerSettings) 
        //               ?? throw new InvalidOperationException("Некорректный DTO");
        //
        //     _scenarioValidator.Validate(dto);
        //     var result = _scenarioVariabilityEvaluator.Evaluate(dto);
        //     var grade =  "Ошибки отсутствуют. " +
        //                  $"Оценка: {result.score}%, " +
        //                  $"Уровень: {result.level.ToString()}, ";
        //     ResultLabel.Text = "Результаты:";
        //     OutputScenarioErrorsTextBox.Text = grade;
        // }
        // catch (JsonException jsonEx)
        // {
        //     MessageBox.Show($"Ошибка разбора JSON:\n{jsonEx.Message}", "Валидация сценария",
        //         MessageBoxButton.OK, MessageBoxImage.Error);
        // }
        // catch (ScenarioValidationException scenarioValidationException)
        // {
        //     OutputScenarioErrorsTextBox.Text = scenarioValidationException.Message;
        // }
        // catch (Exception ex)
        // {
        //     MessageBox.Show($"Не удалось создать DTO:\n{ex.Message}", "Валидация сценария",
        //         MessageBoxButton.OK, MessageBoxImage.Error);
        // }
    }

    /// <summary> Подсчет контрольной суммы (подпись) </summary>
    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        // OutputSumTextBox.Text = "Ожидайте...";
        // var scenarioText = InputScenarioForEvaluationsTextBox.Text;
        //
        // try
        // {
        //     var inputDto = JsonConvert.DeserializeObject<UploadScenarioDto>(
        //                   scenarioText, 
        //                   _scenarioChecksumCalculator.JsonSerializerSettings) 
        //               ?? throw new InvalidOperationException("Некорректный DTO");
        //     _scenarioValidator.Validate(inputDto);
        //
        //     var canonicalJson = _scenarioChecksumCalculator.GetCanonicalJson(inputDto);
        //     var result = _scenarioChecksumCalculator.CalculateChecksum(canonicalJson);
        //     OutputSumTextBox.Text = result;
        // }
        // catch (JsonException jsonEx)
        // {
        //     MessageBox.Show(
        //         $"Ошибка разбора JSON:\n{jsonEx.Message}", 
        //         "Валидация сценария",
        //         MessageBoxButton.OK, 
        //         MessageBoxImage.Error);
        //     OutputSumTextBox.Text = "";
        // }
        // catch (ScenarioValidationException scenarioValidationException)
        // {
        //     MessageBox.Show(
        //         $"Ошибка валидации:\n{scenarioValidationException.Message}",
        //         "Валидация сценария",
        //         MessageBoxButton.OK, 
        //         MessageBoxImage.Error);
        //     OutputSumTextBox.Text = "";
        // }
        // catch (Exception ex)
        // {
        //     MessageBox.Show($"Не удалось создать DTO:\n{ex.Message}", "Валидация сценария",
        //         MessageBoxButton.OK, MessageBoxImage.Error);
        //     OutputSumTextBox.Text = "";
        // }
    }
    
    /// <summary> Кнопка сохранения файла лицензии </summary>
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var dto = new LicenseAndSaltDto
        {
            Salt = OutputSaltTextBox.Text,
            License = OutputLicenseTextBox.Text
        };

        var dlg = new SaveFileDialog
        {
            FileName = $"{dto.Salt}.txt",
            DefaultExt = ".txt",
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            Title = "Сохранить лицензию"
        };

        var result = dlg.ShowDialog();
        if (result != true)
            return;

        try
        {
            var json = JsonConvert.SerializeObject(dto, _scenarioChecksumCalculator.JsonSerializerSettings);

            File.WriteAllText(dlg.FileName, json);
            MessageBox.Show("Файл успешно сохранён", "Генератор лицензий",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось сохранить файл:\n{ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    /// <summary> Вызов формы создания лицензии </summary>
    private void NewLicenseForm_Click(object sender, RoutedEventArgs e)
    {
        NewLicenseGrid.Visibility = Visibility.Visible;
        ValidationGrid.Visibility = Visibility.Collapsed;
        SignGrid.Visibility = Visibility.Collapsed;

        SaltErrorTextBlock.Visibility = Visibility.Hidden;

        SaltTextBox.Text = "";
        DaysTextBox.Text = "";
        GamesTextBox.Text = "";

        OutputSaltTextBox.Text = "";
        OutputSaltTimeTextBox.Text = "";
        OutputSaltTimeTextBox.Visibility = Visibility.Hidden;

        OutputLicenseTextBox.Text = "";
    }
    
    /// <summary> Вызов формы валидации сценария </summary>
    private void ValidForm_Click(object sender, RoutedEventArgs e)
    {
        NewLicenseGrid.Visibility    = Visibility.Collapsed;
        ValidationGrid.Visibility    = Visibility.Visible;
        SignGrid.Visibility          = Visibility.Collapsed;

        ResultLabel.Text = "Найденные ошибки:";
        OutputScenarioErrorsTextBox.Text = "";
        InputScenarioTextBox.Text = "";
    }

    /// <summary> Вызов формы подписи сценария </summary>
    private void SignForm_Click(object sender, RoutedEventArgs e)
    {
        NewLicenseGrid.Visibility    = Visibility.Collapsed;
        ValidationGrid.Visibility    = Visibility.Collapsed;
        SignGrid.Visibility          = Visibility.Visible;

        InputScenarioForEvaluationsTextBox.Text = "";
        OutputSumTextBox.Text = "";
    }

    /// <summary> Кнопка выхода </summary>
    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
    }

    /// <summary>
    /// Перехватывает ввод символа и проверяет:
    /// - это цифра
    /// - в результирующем тексте нет ведущих нулей (кроме одиночной "0")
    /// </summary>
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var tb = sender as TextBox;
        if (tb == null)
        {
            e.Handled = true;
            return;
        }

        var newText = tb.Text
            .Remove(tb.SelectionStart, tb.SelectionLength)
            .Insert(tb.CaretIndex, e.Text);

        var isValidFormat = Regex.IsMatch(newText, @"^(0|[1-9]\d*)$");
        e.Handled = !isValidFormat;
    }

    /// <summary>
    /// Перехватывает вставку из буфера и проверяет аналогично:
    /// - все символы — цифры
    /// - нет ведущих нулей
    /// </summary>
    private void OnPasteNumbersOnly(object sender, DataObjectPastingEventArgs e)
    {
        if (sender is not TextBox tb || !e.DataObject.GetDataPresent(typeof(string)))
        {
            e.CancelCommand();
            return;
        }

        var pastedText = (string?)e.DataObject.GetData(typeof(string));

        var newText = tb.Text
            .Remove(tb.SelectionStart, tb.SelectionLength)
            .Insert(tb.SelectionStart, pastedText!);

        var isValidFormat = Regex.IsMatch(newText, @"^(0|[1-9]\d*)$");
        if (!isValidFormat)
            e.CancelCommand();
    }
}