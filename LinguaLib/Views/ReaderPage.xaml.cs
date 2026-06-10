using LinguaLib.Converters;
using LinguaLib.Resources.Localization;
using LinguaLib.ViewModels;

namespace LinguaLib.Views;

[QueryProperty(nameof(BookId), "bookId")]
public partial class ReaderPage : ContentPage
{
    private readonly ReaderViewModel _vm;
    private static readonly Color Primary = Color.FromArgb("#7C6AF7");

    public string BookId
    {
        set { if (int.TryParse(value, out var id)) _ = _vm.LoadBookAsync(id); }
    }

    public ReaderPage(ReaderViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        BuildUI();
    }

    private void BuildUI()
    {
        var L = LocalizationManager.Instance;
        var inv = new InvertBoolConverter();
        this.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#FAFAFA"), Color.FromArgb("#0F0F1A"));
        this.SetBinding(TitleProperty, nameof(_vm.BookTitle));

        // Toolbar
        var toolbar = new HorizontalStackLayout { Padding = new Thickness(12, 8), Spacing = 8 };
        toolbar.SetAppTheme(BackgroundColorProperty, Colors.White, Color.FromArgb("#1E1E2E"));

        var decBtn = new Button { BackgroundColor = Primary, TextColor = Colors.White, WidthRequest = 48, HeightRequest = 36, CornerRadius = 8 };
        decBtn.SetBinding(Button.TextProperty, new Binding("[Decrease]", source: L));
        decBtn.SetBinding(Button.CommandProperty, nameof(_vm.DecreaseFontCommand));

        var fontLbl = new Label { VerticalOptions = LayoutOptions.Center, FontSize = 14 };
        fontLbl.SetBinding(Label.TextProperty, new Binding(nameof(_vm.FontSize), stringFormat: "{0:F0}px"));
        fontLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#444444"), Color.FromArgb("#CCCCCC"));

        var incBtn = new Button { BackgroundColor = Primary, TextColor = Colors.White, WidthRequest = 48, HeightRequest = 36, CornerRadius = 8 };
        incBtn.SetBinding(Button.TextProperty, new Binding("[Increase]", source: L));
        incBtn.SetBinding(Button.CommandProperty, nameof(_vm.IncreaseFontCommand));

        toolbar.Add(decBtn); toolbar.Add(fontLbl); toolbar.Add(incBtn);

        // Content scroll
        var scroll = new ScrollView();
        var contentLbl = new Label { Padding = new Thickness(20, 16, 20, 80), LineHeight = 1.6 };
        contentLbl.SetBinding(Label.TextProperty, nameof(_vm.Content));
        contentLbl.SetBinding(Label.FontSizeProperty, nameof(_vm.FontSize));
        contentLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));
        scroll.Content = contentLbl;

        // FAB lookup
        var fab = new Button
        {
            Text = "🔍", FontSize = 22,
            BackgroundColor = Primary, TextColor = Colors.White,
            CornerRadius = 28, WidthRequest = 56, HeightRequest = 56,
            HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 20, 24)
        };
        fab.SetBinding(IsVisibleProperty, new Binding(nameof(_vm.IsLookupVisible), converter: inv));
        fab.SetBinding(Button.CommandProperty, nameof(_vm.ShowLookupCommand));

        // Lookup panel
        var lookupPanel = BuildLookupPanel(L, inv);

        var contentGrid = new Grid();
        contentGrid.Add(scroll);
        contentGrid.Add(fab);
        contentGrid.Add(lookupPanel);

        var root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star }
            }
        };
        root.Add(toolbar, 0, 0);
        root.Add(contentGrid, 0, 1);
        Content = root;
    }

    private View BuildLookupPanel(LocalizationManager L, InvertBoolConverter inv)
    {
        var panel = new Border
        {
            VerticalOptions = LayoutOptions.End,
            Padding = new Thickness(20, 16, 20, 32),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(20, 20, 0, 0) }
        };
        panel.SetBinding(IsVisibleProperty, nameof(_vm.IsLookupVisible));
        panel.SetAppTheme(BackgroundColorProperty, Colors.White, Color.FromArgb("#1E1E2E"));

        var stack = new VerticalStackLayout { Spacing = 12 };

        // Header row
        var headerRow = new Grid { ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition { Width = GridLength.Auto } } };
        var titleLbl = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
        titleLbl.SetBinding(Label.TextProperty, new Binding("[TranslateWord]", source: L));
        titleLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));
        var closeBtn = new Button { Text = "✕", BackgroundColor = Colors.Transparent, TextColor = Color.FromArgb("#999999"), FontSize = 18, WidthRequest = 40, HeightRequest = 40 };
        closeBtn.SetBinding(Button.CommandProperty, nameof(_vm.CloseLookupCommand));
        Grid.SetColumn(titleLbl, 0); Grid.SetColumn(closeBtn, 1);
        headerRow.Add(titleLbl); headerRow.Add(closeBtn);

        // Input row
        var inputRow = new Grid { ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition { Width = GridLength.Auto } }, ColumnSpacing = 8 };
        var wordEntry = new Entry();
        wordEntry.SetBinding(Entry.TextProperty, nameof(_vm.LookupWord));
        wordEntry.SetBinding(Entry.PlaceholderProperty, new Binding("[EnterWord]", source: L));
        wordEntry.SetBinding(Entry.ReturnCommandProperty, nameof(_vm.TranslateCommand));
        wordEntry.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#F0F0FF"), Color.FromArgb("#2A2A3E"));
        wordEntry.SetAppTheme(Entry.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));

        var translateBtn = new Button { BackgroundColor = Primary, TextColor = Colors.White, CornerRadius = 8, HeightRequest = 44, FontSize = 13 };
        translateBtn.SetBinding(Button.TextProperty, new Binding("[Translate]", source: L));
        translateBtn.SetBinding(Button.CommandProperty, nameof(_vm.TranslateCommand));

        Grid.SetColumn(wordEntry, 0); Grid.SetColumn(translateBtn, 1);
        inputRow.Add(wordEntry); inputRow.Add(translateBtn);

        // Loading indicator
        var spinner = new ActivityIndicator { Color = Primary };
        spinner.SetBinding(ActivityIndicator.IsRunningProperty, nameof(_vm.IsTranslating));
        spinner.SetBinding(IsVisibleProperty, nameof(_vm.IsTranslating));

        // Result
        var resultBorder = new Border { Padding = new Thickness(14, 10), StrokeThickness = 0, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 } };
        resultBorder.SetBinding(IsVisibleProperty, nameof(_vm.HasTranslation));
        resultBorder.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#EEE8FF"), Color.FromArgb("#2A2040"));

        var resultStack = new VerticalStackLayout { Spacing = 4 };
        var origResultLbl = new Label { FontSize = 20, FontAttributes = FontAttributes.Bold };
        origResultLbl.SetBinding(Label.TextProperty, nameof(_vm.LookupWord));
        origResultLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));
        var transResultLbl = new Label { FontSize = 18 };
        transResultLbl.SetBinding(Label.TextProperty, nameof(_vm.LookupTranslation));
        transResultLbl.SetAppTheme(Label.TextColorProperty, Primary, Color.FromArgb("#9E8FFF"));
        resultStack.Add(origResultLbl); resultStack.Add(transResultLbl);
        resultBorder.Content = resultStack;

        // Add to dictionary button
        var addBtn = new Button { BackgroundColor = Color.FromArgb("#4CAF50"), TextColor = Colors.White, CornerRadius = 10, HeightRequest = 48, FontSize = 15 };
        addBtn.SetBinding(Button.TextProperty, new Binding("[AddToDictionary]", source: L));
        addBtn.SetBinding(Button.CommandProperty, nameof(_vm.SaveWordCommand));
        addBtn.SetBinding(IsVisibleProperty, nameof(_vm.HasTranslation));

        stack.Add(headerRow); stack.Add(inputRow); stack.Add(spinner);
        stack.Add(resultBorder); stack.Add(addBtn);
        panel.Content = stack;
        return panel;
    }
}
