using System.Collections.ObjectModel;
using System.Windows.Input;
using LinguaLib.Models;
using LinguaLib.Services;

namespace LinguaLib.ViewModels;

public class DictionaryViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    public ObservableCollection<UserWord> Words { get; } = new();

    private string _newOriginal = string.Empty;
    public string NewOriginal { get => _newOriginal; set => SetProperty(ref _newOriginal, value); }

    private string _newTranslation = string.Empty;
    public string NewTranslation { get => _newTranslation; set => SetProperty(ref _newTranslation, value); }

    private string _newTranscription = string.Empty;
    public string NewTranscription { get => _newTranscription; set => SetProperty(ref _newTranscription, value); }

    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set { SetProperty(ref _searchQuery, value); FilterWords(); }
    }

    private List<UserWord> _allWords = new();

    public ICommand LoadCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }

    public DictionaryViewModel(DatabaseService db)
    {
        _db = db;
        LoadCommand = new RelayCommand(async () => await LoadAsync());
        AddCommand = new RelayCommand(async () => await AddAsync());
        DeleteCommand = new RelayCommand<UserWord>(async w => await DeleteAsync(w));
    }

    public async Task LoadAsync()
    {
        _allWords = await _db.GetUserWordsAsync();
        FilterWords();
    }

    private void FilterWords()
    {
        var filtered = string.IsNullOrWhiteSpace(_searchQuery)
            ? _allWords
            : _allWords.Where(w =>
                w.Original.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                w.Translation.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();

        Words.Clear();
        foreach (var w in filtered) Words.Add(w);
    }

    private async Task AddAsync()
    {
        if (string.IsNullOrWhiteSpace(NewOriginal) || string.IsNullOrWhiteSpace(NewTranslation)) return;
        var word = new UserWord { Original = NewOriginal, Translation = NewTranslation, Transcription = NewTranscription };
        await _db.SaveUserWordAsync(word);
        _allWords.Add(word);
        FilterWords();
        NewOriginal = string.Empty;
        NewTranslation = string.Empty;
        NewTranscription = string.Empty;
    }

    private async Task DeleteAsync(UserWord? word)
    {
        if (word is null) return;
        await _db.DeleteUserWordAsync(word);
        _allWords.Remove(word);
        Words.Remove(word);
    }
}
