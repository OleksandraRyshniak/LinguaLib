using System.Collections.ObjectModel;
using System.Windows.Input;
using LinguaLib.Models;
using LinguaLib.Services;
using LinguaLib.Views;
using Microsoft.Maui.Controls;

namespace LinguaLib.ViewModels;

public class LibraryViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    public ObservableCollection<Book> Books { get; } = new();

    public ICommand DeleteBookCommand { get; }
    public ICommand NavigateToAddCommand { get; }

    public LibraryViewModel(DatabaseService db)
    {
        _db = db;
        Title = "Library";
        DeleteBookCommand = new RelayCommand<Book>(async b => await DeleteAsync(b));
        NavigateToAddCommand = new RelayCommand(async () => await Shell.Current.GoToAsync(nameof(AddBookPage)));
    }

    public async Task LoadAsync()
    {
        IsBusy = true;
        var books = await _db.GetBooksAsync();
        Books.Clear();
        foreach (var b in books) Books.Add(b);
        IsBusy = false;
    }

    private async Task DeleteAsync(Book? book)
    {
        if (book is null) return;
        await _db.DeleteBookAsync(book);
        Books.Remove(book);
    }
}
