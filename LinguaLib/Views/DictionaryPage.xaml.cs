using LinguaLib.Models;
using LinguaLib.Resources.Localization;
using LinguaLib.ViewModels;

namespace LinguaLib.Views;

public partial class DictionaryPage : ContentPage
{
    private readonly DictionaryViewModel _vm;
    private static readonly Color Primary = Color.FromArgb("#7C6AF7");

    public DictionaryPage(DictionaryViewModel vm)
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
        this.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#F5F5FF"), Color.FromArgb("#0F0F1A"));
        this.SetBinding(TitleProperty, new Binding("[Dictionary]", source: L));

        var search = new SearchBar { Margin = new Thickness(12, 8) };
        search.SetBinding(SearchBar.TextProperty, nameof(_vm.SearchQuery));
        search.SetBinding(SearchBar.PlaceholderProperty, new Binding("[Search]", source: L));
        search.SetAppTheme(BackgroundColorProperty, Colors.White, Color.FromArgb("#1E1E2E"));
        search.SetAppTheme(SearchBar.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));

        var statsLbl = new Label { FontSize = 13, Margin = new Thickness(16, 0, 16, 8) };
        statsLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#666666"), Color.FromArgb("#AAAAAA"));
        var fs = new FormattedString();
        var span1 = new Span { Text = "📝 " };
        var span2 = new Span { FontAttributes = FontAttributes.Bold };
        span2.SetBinding(Span.TextProperty, new Binding("Words.Count", source: _vm));
        fs.Spans.Add(span1);
        fs.Spans.Add(span2);
        statsLbl.FormattedText = fs;

        var cv = new CollectionView();
        cv.SetBinding(ItemsView.ItemsSourceProperty, nameof(_vm.Words));
        cv.ItemTemplate = new DataTemplate(() =>
        {
            var swipe = new SwipeView();
            var del = new SwipeItem { Text = L["Delete"], BackgroundColor = Colors.Red };
            del.SetBinding(MenuItem.CommandProperty, new Binding("DeleteCommand", source: _vm));
            del.SetBinding(MenuItem.CommandParameterProperty, ".");
            swipe.RightItems = new SwipeItems { del };

            var grid = new Grid
            {
                Padding = new Thickness(16, 10), ColumnSpacing = 8,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };
            grid.SetAppTheme(BackgroundColorProperty, Colors.White, Color.FromArgb("#1E1E2E"));

            var orig = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
            orig.SetBinding(Label.TextProperty, nameof(UserWord.Original));
            orig.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));

            var trans = new Label { FontSize = 15, VerticalOptions = LayoutOptions.Center };
            trans.SetBinding(Label.TextProperty, nameof(UserWord.Translation));
            trans.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#555555"), Color.FromArgb("#CCCCCC"));

            var icon = new Label { Text = "📖", FontSize = 18, VerticalOptions = LayoutOptions.Center };
            icon.Triggers.Add(new DataTrigger(typeof(Label))
            {
                Binding = new Binding(nameof(UserWord.KnowledgeLevel)),
                Value = 2,
                Setters = { new Setter { Property = Label.TextProperty, Value = "⭐" } }
            });

            Grid.SetColumn(orig, 0); Grid.SetColumn(trans, 1); Grid.SetColumn(icon, 2);
            grid.Add(orig); grid.Add(trans); grid.Add(icon);
            swipe.Content = grid;
            return swipe;
        });

        var emptyLbl = new Label { Text = L["NoWords"], HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, FontSize = 15, Margin = 20 };
        emptyLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#888888"), Color.FromArgb("#666666"));
        cv.EmptyView = emptyLbl;

        var addPanel = new Border
        {
            Padding = 12, StrokeThickness = 1,
            Stroke = new SolidColorBrush(Primary),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(12, 12, 0, 0) }
        };
        addPanel.SetAppTheme(BackgroundColorProperty, Colors.White, Color.FromArgb("#1E1E2E"));

        var addStack = new VerticalStackLayout { Spacing = 8 };
        var addTitle = new Label { Text = L["AddWord"], FontSize = 14, FontAttributes = FontAttributes.Bold };
        addTitle.SetAppTheme(Label.TextColorProperty, Primary, Color.FromArgb("#9E8FFF"));

        var wordEntry = MakeEntry(L["Word"], nameof(_vm.NewOriginal));
        var translEntry = MakeEntry(L["Translation"], nameof(_vm.NewTranslation));

        var addBtn = new Button { BackgroundColor = Primary, TextColor = Colors.White, CornerRadius = 8, HeightRequest = 44 };
        addBtn.SetBinding(Button.TextProperty, new Binding("[Add]", source: L));
        addBtn.SetBinding(Button.CommandProperty, nameof(_vm.AddCommand));

        addStack.Add(addTitle); addStack.Add(wordEntry); addStack.Add(translEntry); addStack.Add(addBtn);
        addPanel.Content = addStack;

        var root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto }
            }
        };
        root.Add(search, 0, 0); root.Add(statsLbl, 0, 1);
        root.Add(cv, 0, 2); root.Add(addPanel, 0, 3);
        Content = root;
    }

    private static Entry MakeEntry(string placeholder, string bindingPath)
    {
        var e = new Entry { Placeholder = placeholder };
        e.SetBinding(Entry.TextProperty, bindingPath);
        e.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#F0F0FF"), Color.FromArgb("#2A2A3E"));
        e.SetAppTheme(Entry.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));
        return e;
    }
}
