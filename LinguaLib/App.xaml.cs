using LinguaLib.Services;

namespace LinguaLib;

public partial class App : Application
{
    public App(ThemeService themeService, SettingsService settingsService)
    {
        InitializeComponent();
        settingsService.Load();
        themeService.ApplySavedTheme();
        LinguaLib.Resources.Localization.LocalizationManager.Instance
            .SetCulture(settingsService.Current.Language);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
