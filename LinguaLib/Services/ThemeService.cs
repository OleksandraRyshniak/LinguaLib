namespace LinguaLib.Services;

public class ThemeService
{
    private const string ThemeKey = "app_theme";

    public bool IsDark => Preferences.Get(ThemeKey, false);

    public void SetTheme(bool isDark)
    {
        Preferences.Set(ThemeKey, isDark);
        Application.Current!.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
    }

    public void ApplySavedTheme()
    {
        Application.Current!.UserAppTheme = IsDark ? AppTheme.Dark : AppTheme.Light;
    }
}
