using System.Windows.Input;
using LinguaLib.Models;
using LinguaLib.Services;
using LinguaLib.Resources.Localization;
using LinguaLib.Views;
using Microsoft.Maui.Controls;

namespace LinguaLib.ViewModels;

public class AddBookViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    private string _title = string.Empty;
    public string Title { get => _title; set => SetProperty(ref _title, value); }

    private string _author = string.Empty;
    public string Author { get => _author; set => SetProperty(ref _author, value); }

    private string _coverUrl = string.Empty;
    public string CoverUrl
    {
        get => _coverUrl;
        set { SetProperty(ref _coverUrl, value); OnPropertyChanged(nameof(HasCover)); }
    }
    public bool HasCover => !string.IsNullOrWhiteSpace(_coverUrl);

    private string _language = "en";
    public string Language { get => _language; set => SetProperty(ref _language, value); }

    private string _content = string.Empty;
    public string Content { get => _content; set => SetProperty(ref _content, value); }

    private string _fileName = LocalizationManager.Instance["NoFileSelected"];
    public string FileName { get => _fileName; set => SetProperty(ref _fileName, value); }

    public ICommand PickFileCommand { get; }
    public ICommand PickCoverCommand { get; }
    public ICommand SaveCommand { get; }

    public AddBookViewModel(DatabaseService db)
    {
        _db = db;
        PickFileCommand = new RelayCommand(async () => await PickFileAsync());
        PickCoverCommand = new RelayCommand(async () => await PickCoverAsync());
        SaveCommand = new RelayCommand(async () => await SaveAsync());
    }

    private async Task PickFileAsync()
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "Выберите текстовый файл",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, ["text/plain"] },
                    { DevicePlatform.iOS, ["public.plain-text"] },
                    { DevicePlatform.WinUI, [".txt"] },
                    { DevicePlatform.macOS, ["public.plain-text"] },
                })
            };
            var result = await FilePicker.PickAsync(options);
            if (result is null) return;
            FileName = result.FileName;
            using var stream = await result.OpenReadAsync();
            using var reader = new StreamReader(stream);
            Content = await reader.ReadToEndAsync();
        }
        catch
        {
            await Shell.Current.DisplayAlert(
                LocalizationManager.Instance["ErrorTitle"],
                LocalizationManager.Instance["ErrorFileRead"], "OK");
        }
    }

    private async Task PickCoverAsync()
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync();
            if (result is null) return;
            CoverUrl = result.FullPath;
        }
        catch
        {
            await Shell.Current.DisplayAlert(
                LocalizationManager.Instance["ErrorTitle"],
                LocalizationManager.Instance["ErrorPickPhoto"], "OK");
        }
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            await Shell.Current.DisplayAlert(
                LocalizationManager.Instance["ErrorTitle"],
                LocalizationManager.Instance["ErrorTitleRequired"], "OK");
            return;
        }
        IsBusy = true;
        var book = new Book
        {
            Title = Title,
            Author = Author,
            Language = Language,
            CoverUrl = CoverUrl,
            Content = Content,
            CoverEmoji = "📖"
        };
        await _db.SaveBookAsync(book);
        IsBusy = false;
        await Shell.Current.GoToAsync("..");
    }
}
