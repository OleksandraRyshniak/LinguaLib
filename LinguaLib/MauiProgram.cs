using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

using LinguaLib.Services;

using LinguaLib.ViewModels;
using LinguaLib.Views;

namespace LinguaLib;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
#if ANDROID
            .ConfigureMauiHandlers(h =>
                h.AddHandler<Shell, LinguaLib.Platforms.Android.BigTabShellRenderer>())
#endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<IAudioManager>(AudioManager.Current);
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<SettingsService>();
        builder.Services.AddSingleton<AudioService>();
        builder.Services.AddSingleton<TranslationService>();

        // ViewModels
        builder.Services.AddSingleton<LibraryViewModel>();
        builder.Services.AddSingleton<DictionaryViewModel>();
        builder.Services.AddSingleton<StudyViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddTransient<ReaderViewModel>();
        builder.Services.AddTransient<AddBookViewModel>();

        // Views
        builder.Services.AddSingleton<LibraryPage>();
        builder.Services.AddSingleton<DictionaryPage>();
        builder.Services.AddSingleton<StudyPage>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddTransient<ReaderPage>();
        builder.Services.AddTransient<AddBookPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
