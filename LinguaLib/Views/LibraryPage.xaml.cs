using LinguaLib.Converters;
using LinguaLib.Models;
using LinguaLib.Resources.Localization;
using LinguaLib.ViewModels;

namespace LinguaLib.Views;

public partial class LibraryPage : ContentPage
{
    private readonly LibraryViewModel _vm;
    private static readonly Color Primary = Color.FromArgb("#7C6AF7");

    public LibraryPage(LibraryViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        BuildUI();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private void BuildUI()
    {
        var L = LocalizationManager.Instance;
        this.SetAppTheme(BackgroundColorProperty,
            Color.FromArgb("#F5F5FF"), Color.FromArgb("#0F0F1A"));

        // Header
        var header = new VerticalStackLayout { Padding = new Thickness(16, 20, 16, 8) };
        var titleLabel = new Label
        {
            FontSize = 28,
            FontAttributes = FontAttributes.Bold
        };
        titleLabel.SetBinding(Label.TextProperty, new Binding("[Library]", source: L));
        titleLabel.SetAppTheme(Label.TextColorProperty,
            Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));
        header.Add(titleLabel);

        // Book list
        var cv = new CollectionView { SelectionMode = SelectionMode.Single };
        cv.SetBinding(ItemsView.ItemsSourceProperty, nameof(_vm.Books));
        cv.SelectionChanged += OnBookSelected;

        cv.ItemTemplate = new DataTemplate(() =>
        {
            var swipe = new SwipeView();
            var deleteItem = new SwipeItem { Text = L["Delete"], BackgroundColor = Colors.Red };
            deleteItem.SetBinding(MenuItem.CommandProperty,
                new Binding("DeleteBookCommand", source: _vm));
            deleteItem.SetBinding(MenuItem.CommandParameterProperty, ".");
            swipe.RightItems = new SwipeItems { deleteItem };

            var grid = new Grid
            {
                Padding = new Thickness(12, 10),
                ColumnSpacing = 12,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 68 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };
            grid.SetAppTheme(BackgroundColorProperty, Colors.White, Color.FromArgb("#1E1E2E"));

            var coverBorder = new Border
            {
                StrokeThickness = 0,
                HeightRequest = 68,
                WidthRequest = 52,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 }
            };
            coverBorder.SetAppTheme(BackgroundColorProperty,
                Color.FromArgb("#EEE8FF"), Color.FromArgb("#2A2040"));

            var coverGrid = new Grid();
            var coverImage = new Image { Aspect = Aspect.AspectFill };
            coverImage.SetBinding(Image.SourceProperty, nameof(Book.CoverUrl));
            coverImage.SetBinding(IsVisibleProperty,
                new Binding(nameof(Book.CoverUrl), converter: new StringToBoolConverter()));

            var emojiLabel = new Label { FontSize = 32, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            emojiLabel.SetBinding(Label.TextProperty, nameof(Book.CoverEmoji));
            emojiLabel.SetBinding(IsVisibleProperty,
                new Binding(nameof(Book.CoverUrl), converter: new StringToBoolConverter(), converterParameter: "invert"));

            coverGrid.Add(coverImage);
            coverGrid.Add(emojiLabel);
            coverBorder.Content = coverGrid;

            var info = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 3 };
            var titleLbl = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
            titleLbl.SetBinding(Label.TextProperty, nameof(Book.Title));
            titleLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));

            var authorLbl = new Label { FontSize = 13 };
            authorLbl.SetBinding(Label.TextProperty, nameof(Book.Author));
            authorLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#666666"), Color.FromArgb("#AAAAAA"));

            var progress = new ProgressBar { ProgressColor = Primary, HeightRequest = 4, Margin = new Thickness(0, 4, 0, 0) };
            progress.SetBinding(ProgressBar.ProgressProperty,
                new Binding(nameof(Book.ReadProgress), converter: new IntToDoubleConverter()));

            info.Add(titleLbl);
            info.Add(authorLbl);
            info.Add(progress);

            var arrow = new Label { Text = "›", FontSize = 22, VerticalOptions = LayoutOptions.Center };
            arrow.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#7C6AF7"), Color.FromArgb("#9E8FFF"));

            Grid.SetColumn(coverBorder, 0);
            Grid.SetColumn(info, 1);
            Grid.SetColumn(arrow, 2);
            grid.Add(coverBorder);
            grid.Add(info);
            grid.Add(arrow);

            swipe.Content = grid;
            return swipe;
        });

        cv.EmptyView = BuildEmptyView(L);

        // FAB
        var fab = new Button
        {
            Text = "+",
            FontSize = 28,
            BackgroundColor = Primary,
            TextColor = Colors.White,
            CornerRadius = 32,
            WidthRequest = 64,
            HeightRequest = 64,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 20, 24)
        };
        fab.SetBinding(Button.CommandProperty, nameof(_vm.NavigateToAddCommand));

        var mainGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star }
            }
        };
        mainGrid.Add(header, 0, 0);
        mainGrid.Add(cv, 0, 1);
        mainGrid.Add(fab, 0, 1);

        Content = mainGrid;
    }

    private static View BuildEmptyView(LocalizationManager L)
    {
        var stack = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 12,
            Padding = new Thickness(40)
        };
        stack.Add(new Label { Text = "📖", FontSize = 60, HorizontalOptions = LayoutOptions.Center });

        var noBooksLbl = new Label
        {
            Text = L["NoBooksYet"],
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center
        };
        noBooksLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#888888"), Color.FromArgb("#666666"));

        var hintLbl = new Label
        {
            Text = L["AddFirstBook"],
            FontSize = 14,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center
        };
        hintLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#AAAAAA"), Color.FromArgb("#555555"));

        stack.Add(noBooksLbl);
        stack.Add(hintLbl);
        return stack;
    }

    private async void OnBookSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Book book) return;
        if (sender is CollectionView cv) cv.SelectedItem = null;
        await Shell.Current.GoToAsync($"{nameof(ReaderPage)}?bookId={book.Id}");
    }
}
