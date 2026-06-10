using LinguaLib.Models;

namespace LinguaLib.Services;

public class SettingsService
{
    private const string LangKey = "app_language";
    private const string SoundKey = "app_sound";
    private const string FontKey = "app_font";

    public AppSettings Current { get; } = new();

    public void Load()
    {
        Current.Language = Preferences.Get(LangKey, "en");
        Current.IsSoundEnabled = Preferences.Get(SoundKey, true);
        Current.FontSize = Preferences.Get(FontKey, 16f);
    }

    public void Save()
    {
        Preferences.Set(LangKey, Current.Language);
        Preferences.Set(SoundKey, Current.IsSoundEnabled);
        Preferences.Set(FontKey, Current.FontSize);
    }

    public void SetLanguage(string lang)
    {
        Current.Language = lang;
        Preferences.Set(LangKey, lang);
    }

    public void SetSound(bool enabled)
    {
        Current.IsSoundEnabled = enabled;
        Preferences.Set(SoundKey, enabled);
    }
}
