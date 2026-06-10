namespace LinguaLib.Models;

public class AppSettings
{
    public string Language { get; set; } = "en";
    public bool IsDarkTheme { get; set; } = false;
    public bool IsSoundEnabled { get; set; } = true;
    public float FontSize { get; set; } = 16f;
}
