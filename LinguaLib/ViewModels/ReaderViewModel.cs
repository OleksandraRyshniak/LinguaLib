using System.Windows.Input;
using LinguaLib.Models;
using LinguaLib.Services;
using LinguaLib.Resources.Localization;
using Microsoft.Maui.Controls;

namespace LinguaLib.ViewModels;

public class ReaderViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private readonly TranslationService _translation;
    private readonly SettingsService _settings;

    private Book? _book;
    public Book? Book
    {
        get => _book;
        set
        {
            SetProperty(ref _book, value);
            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(BookTitle));
            OnPropertyChanged(nameof(BookLanguage));
        }
    }

    public string Content => _book?.Content ?? string.Empty;
    public string BookTitle => _book?.Title ?? string.Empty;
    public string BookLanguage => _book?.Language ?? "en";

    private double _scrollPosition;
    public double ScrollPosition { get => _scrollPosition; set => SetProperty(ref _scrollPosition, value); }

    private float _fontSize = 16f;
    public float FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }

    // Word lookup panel
    private bool _isLookupVisible;
    public bool IsLookupVisible { get => _isLookupVisible; set => SetProperty(ref _isLookupVisible, value); }

    private string _lookupWord = string.Empty;
    public string LookupWord
    {
        get => _lookupWord;
        set
        {
            SetProperty(ref _lookupWord, value);
            LookupTranslation = string.Empty;
            OnPropertyChanged(nameof(HasTranslation));
        }
    }

    private string _lookupTranslation = string.Empty;
    public string LookupTranslation { get => _lookupTranslation; set => SetProperty(ref _lookupTranslation, value); }

    private bool _isTranslating;
    public bool IsTranslating { get => _isTranslating; set => SetProperty(ref _isTranslating, value); }

    public bool HasTranslation => !string.IsNullOrEmpty(_lookupTranslation);

    public ICommand IncreaseFontCommand { get; }
    public ICommand DecreaseFontCommand { get; }
    public ICommand SaveProgressCommand { get; }
    public ICommand ShowLookupCommand { get; }
    public ICommand CloseLookupCommand { get; }
    public ICommand TranslateCommand { get; }
    public ICommand SaveWordCommand { get; }

    public ReaderViewModel(DatabaseService db, TranslationService translation, SettingsService settings)
    {
        _db = db;
        _translation = translation;
        _settings = settings;
        IncreaseFontCommand = new RelayCommand(() => { if (FontSize < 28) FontSize += 2; });
        DecreaseFontCommand = new RelayCommand(() => { if (FontSize > 12) FontSize -= 2; });
        SaveProgressCommand = new RelayCommand(async () => await SaveProgressAsync());
        ShowLookupCommand = new RelayCommand(OpenLookup);
        CloseLookupCommand = new RelayCommand(CloseLookup);
        TranslateCommand = new RelayCommand(async () => await TranslateAsync());
        SaveWordCommand = new RelayCommand(async () => await SaveWordAsync());
    }

    public async Task LoadBookAsync(int id)
    {
        Book = await _db.GetBookAsync(id);
    }

    private void OpenLookup()
    {
        LookupWord = string.Empty;
        LookupTranslation = string.Empty;
        OnPropertyChanged(nameof(HasTranslation));
        IsLookupVisible = true;
    }

    private void CloseLookup()
    {
        IsLookupVisible = false;
    }

    private async Task TranslateAsync()
    {
        if (string.IsNullOrWhiteSpace(LookupWord)) return;
        IsTranslating = true;
        LookupTranslation = string.Empty;
        OnPropertyChanged(nameof(HasTranslation));
        var targetLang = _settings.Current.Language;
        if (targetLang == BookLanguage)
            targetLang = targetLang == "ru" ? "en" : "ru";
        LookupTranslation = await _translation.TranslateAsync(LookupWord.Trim(), BookLanguage, targetLang);
        OnPropertyChanged(nameof(HasTranslation));
        IsTranslating = false;
    }

    private async Task SaveWordAsync()
    {
        if (string.IsNullOrWhiteSpace(LookupWord) || string.IsNullOrWhiteSpace(LookupTranslation)) return;
        var word = new UserWord
        {
            Original = LookupWord.Trim(),
            Translation = LookupTranslation.Trim()
        };
        await _db.SaveUserWordAsync(word);
        CloseLookup();
        await Shell.Current.DisplayAlertAsync(
            LocalizationManager.Instance["Saved"],
            $"«{word.Original}» {LocalizationManager.Instance["WordsSaved"]}", "OK");
    }

    private async Task SaveProgressAsync()
    {
        if (_book is null) return;
        _book.ReadProgress = (int)(_scrollPosition * 100);
        await _db.SaveBookAsync(_book);
    }
}
