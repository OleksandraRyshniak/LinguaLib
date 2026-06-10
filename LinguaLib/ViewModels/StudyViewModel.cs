using System.Collections.ObjectModel;
using System.Windows.Input;
using LinguaLib.Models;
using LinguaLib.Services;

namespace LinguaLib.ViewModels;

public class StudyViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private List<UserWord> _deck = new();
    private int _currentIndex;

    private UserWord? _currentCard;
    public UserWord? CurrentCard { get => _currentCard; set => SetProperty(ref _currentCard, value); }

    private bool _isFlipped;
    public bool IsFlipped { get => _isFlipped; set => SetProperty(ref _isFlipped, value); }

    private string _frontText = string.Empty;
    public string FrontText { get => _frontText; set => SetProperty(ref _frontText, value); }

    private string _backText = string.Empty;
    public string BackText { get => _backText; set => SetProperty(ref _backText, value); }

    private int _totalCards;
    public int TotalCards { get => _totalCards; set => SetProperty(ref _totalCards, value); }

    private int _currentCardNumber;
    public int CurrentCardNumber { get => _currentCardNumber; set => SetProperty(ref _currentCardNumber, value); }

    private bool _hasCards;
    public bool HasCards { get => _hasCards; set => SetProperty(ref _hasCards, value); }

    private bool _isFinished;
    public bool IsFinished { get => _isFinished; set => SetProperty(ref _isFinished, value); }

    private bool _isEmpty;
    public bool IsEmpty { get => _isEmpty; set => SetProperty(ref _isEmpty, value); }

    public ObservableCollection<UserWord> KnownWords { get; } = new();
    public ObservableCollection<UserWord> UnknownWords { get; } = new();

    public ICommand LoadCommand { get; }
    public ICommand FlipCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand MarkKnownCommand { get; }
    public ICommand MarkUnknownCommand { get; }
    public ICommand RestartCommand { get; }
    public ICommand ShuffleCommand { get; }

    public StudyViewModel(DatabaseService db)
    {
        _db = db;
        LoadCommand = new RelayCommand(async () => await LoadAsync());
        FlipCommand = new RelayCommand(Flip);
        NextCommand = new RelayCommand(Next);
        MarkKnownCommand = new RelayCommand(async () => await MarkAsync(true));
        MarkUnknownCommand = new RelayCommand(async () => await MarkAsync(false));
        RestartCommand = new RelayCommand(async () => await LoadAsync());
        ShuffleCommand = new RelayCommand(Shuffle);
    }

    public async Task LoadAsync()
    {
        _deck = await _db.GetUserWordsAsync();
        _deck = _deck.OrderBy(_ => Guid.NewGuid()).ToList();
        _currentIndex = 0;
        TotalCards = _deck.Count;
        IsFinished = false;
        IsEmpty = TotalCards == 0;
        HasCards = TotalCards > 0;
        KnownWords.Clear();
        UnknownWords.Clear();
        if (HasCards) ShowCard();
    }

    private void ShowCard()
    {
        if (_currentIndex >= _deck.Count)
        {
            CurrentCard = null;
            HasCards = false;
            IsFinished = true;
            return;
        }
        CurrentCard = _deck[_currentIndex];
        FrontText = CurrentCard.Original;
        BackText = CurrentCard.Translation;
        IsFlipped = false;
        CurrentCardNumber = _currentIndex + 1;
    }

    private void Flip() => IsFlipped = !IsFlipped;

    private void Shuffle()
    {
        if (_deck.Count == 0) return;
        _deck = _deck.OrderBy(_ => Guid.NewGuid()).ToList();
        _currentIndex = 0;
        IsFinished = false;
        HasCards = true;
        KnownWords.Clear();
        UnknownWords.Clear();
        ShowCard();
    }

    private void Next()
    {
        _currentIndex++;
        ShowCard();
    }

    private async Task MarkAsync(bool known)
    {
        if (CurrentCard is null) return;
        CurrentCard.KnowledgeLevel = known ? 1 : 0;
        await _db.SaveUserWordAsync(CurrentCard);
        if (known) KnownWords.Add(CurrentCard);
        else UnknownWords.Add(CurrentCard);
        Next();
    }
}
