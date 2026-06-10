using System.Windows.Input;
using LinguaLib.Services;
using LinguaLib.Resources.Localization;

namespace LinguaLib.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly ThemeService _theme;
    private readonly SettingsService _settings;
    private readonly AudioService _audio;

    private bool _isDark;
    public bool IsDark
    {
        get => _isDark;
        set
        {
            SetProperty(ref _isDark, value);
            OnPropertyChanged(nameof(ThemeIcon));
            _theme.SetTheme(value);
        }
    }

    // Dark on → show ☀️ (switch to light); Dark off → show 🌙 (switch to dark)
    public string ThemeIcon => _isDark ? "☀️" : "🌙";

    private bool _isSoundEnabled;
    public bool IsSoundEnabled
    {
        get => _isSoundEnabled;
        set
        {
            SetProperty(ref _isSoundEnabled, value);
            _settings.SetSound(value);
            if (!value) _audio.StopBackground();
        }
    }

    private int _selectedLanguageIndex;
    public int SelectedLanguageIndex
    {
        get => _selectedLanguageIndex;
        set
        {
            SetProperty(ref _selectedLanguageIndex, value);
            ApplyLanguage(value);
        }
    }

    public List<string> Languages { get; } = new() { "English", "Eesti", "Русский" };
    private static readonly string[] LangCodes = { "en", "et", "ru" };

    public ICommand ToggleMusicCommand { get; }

    public string MusicButtonText => LocalizationManager.Instance[
        _audio.IsBackgroundPlaying ? "StopMusic" : "PlayMusic"];

    public SettingsViewModel(ThemeService theme, SettingsService settings, AudioService audio)
    {
        _theme = theme;
        _settings = settings;
        _audio = audio;

        _isDark = theme.IsDark;
        _isSoundEnabled = settings.Current.IsSoundEnabled;
        _selectedLanguageIndex = Array.IndexOf(LangCodes, settings.Current.Language);
        if (_selectedLanguageIndex < 0) _selectedLanguageIndex = 0;

        ToggleMusicCommand = new RelayCommand(async () => await ToggleMusicAsync());
    }

    private void ApplyLanguage(int index)
    {
        if (index < 0 || index >= LangCodes.Length) return;
        var code = LangCodes[index];
        _settings.SetLanguage(code);
        LocalizationManager.Instance.SetCulture(code);
        OnPropertyChanged(nameof(MusicButtonText));
    }

    private async Task ToggleMusicAsync()
    {
        await _audio.ToggleBackgroundMusicAsync("background.mp3");
        OnPropertyChanged(nameof(MusicButtonText));
    }

    private void UpdateMusicButtonText() => OnPropertyChanged(nameof(MusicButtonText));
}
