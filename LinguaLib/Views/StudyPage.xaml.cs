using LinguaLib.Converters;
using LinguaLib.Resources.Localization;
using LinguaLib.ViewModels;

namespace LinguaLib.Views;

public partial class StudyPage : ContentPage
{
    private readonly StudyViewModel _vm;
    private static readonly Color Primary = Color.FromArgb("#7C6AF7");
    private static readonly Color Green = Color.FromArgb("#4CAF50");
    private static readonly Color Red = Color.FromArgb("#F44336");

    public StudyPage(StudyViewModel vm)
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
        var inv = new InvertBoolConverter();
        this.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#F5F5FF"), Color.FromArgb("#0F0F1A"));
        this.SetBinding(TitleProperty, new Binding("[Study]", source: L));

        // Progress row
        var progressRow = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            }
        };
        progressRow.SetBinding(IsVisibleProperty, nameof(_vm.HasCards));

        var progStack = new HorizontalStackLayout { Padding = new Thickness(16, 12), Spacing = 10 };
        var progLbl = new Label { FontSize = 16 };
        progLbl.SetAppTheme(Label.TextColorProperty, Primary, Color.FromArgb("#9E8FFF"));
        var pfs = new FormattedString();
        var ps1 = new Span { FontAttributes = FontAttributes.Bold }; ps1.SetBinding(Span.TextProperty, nameof(_vm.CurrentCardNumber));
        var ps2 = new Span { Text = " / " };
        var ps3 = new Span { FontAttributes = FontAttributes.Bold }; ps3.SetBinding(Span.TextProperty, nameof(_vm.TotalCards));
        pfs.Spans.Add(ps1); pfs.Spans.Add(ps2); pfs.Spans.Add(ps3);
        progLbl.FormattedText = pfs;

        var knownCountLbl = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Green };
        knownCountLbl.SetBinding(Label.TextProperty, "KnownWords.Count");
        var unknownCountLbl = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Red };
        unknownCountLbl.SetBinding(Label.TextProperty, "UnknownWords.Count");

        progStack.Add(progLbl);
        progStack.Add(new Label { Text = "⭐", FontSize = 16 });
        progStack.Add(knownCountLbl);
        progStack.Add(new Label { Text = "  ❌", FontSize = 16 });
        progStack.Add(unknownCountLbl);

        Grid.SetColumn(progStack, 0);
        progressRow.Add(progStack);

        // Flashcard
        var card = new Border
        {
            Margin = new Thickness(20),
            Padding = new Thickness(24),
            Stroke = new SolidColorBrush(Primary),
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 }
        };
        card.SetBinding(IsVisibleProperty, nameof(_vm.HasCards));
        card.SetAppTheme(BackgroundColorProperty, Colors.White, Color.FromArgb("#1E1E2E"));
        card.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = _vm.FlipCommand
        });

        var cardGrid = new Grid();

        // Front
        var front = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, Spacing = 12 };
        front.SetBinding(IsVisibleProperty, new Binding(nameof(_vm.IsFlipped), converter: inv));
        var frontLbl = new Label { FontSize = 32, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
        frontLbl.SetBinding(Label.TextProperty, nameof(_vm.FrontText));
        frontLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));
        var flipHint = new Label { FontSize = 13, HorizontalOptions = LayoutOptions.Center };
        flipHint.SetBinding(Label.TextProperty, new Binding("[TapToFlip]", source: L));
        flipHint.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#AAAAAA"), Color.FromArgb("#555555"));
        front.Add(frontLbl); front.Add(flipHint);

        // Back
        var back = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, Spacing = 8 };
        back.SetBinding(IsVisibleProperty, nameof(_vm.IsFlipped));
        var backLbl = new Label { FontSize = 26, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
        backLbl.SetBinding(Label.TextProperty, nameof(_vm.BackText));
        backLbl.SetAppTheme(Label.TextColorProperty, Primary, Color.FromArgb("#9E8FFF"));
        back.Add(backLbl);

        cardGrid.Add(front); cardGrid.Add(back);
        card.Content = cardGrid;

        // Empty state
        var emptyState = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center,
            Spacing = 16, Padding = 40
        };
        emptyState.SetBinding(IsVisibleProperty, nameof(_vm.IsEmpty));
        var emptyTitle = new Label { FontSize = 22, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center };
        emptyTitle.SetBinding(Label.TextProperty, new Binding("[DictionaryEmpty]", source: L));
        emptyTitle.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));
        var emptyHint = new Label { FontSize = 15, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
        emptyHint.SetBinding(Label.TextProperty, new Binding("[NoWordsHint]", source: L));
        emptyHint.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#888888"), Color.FromArgb("#666666"));
        emptyState.Add(new Label { Text = "📖", FontSize = 64, HorizontalOptions = LayoutOptions.Center });
        emptyState.Add(emptyTitle); emptyState.Add(emptyHint);

        // Finished state
        var finishedState = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center,
            Spacing = 16, Padding = 40
        };
        finishedState.SetBinding(IsVisibleProperty, nameof(_vm.IsFinished));
        var doneTitle = new Label { FontSize = 28, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center };
        doneTitle.SetBinding(Label.TextProperty, new Binding("[Done]", source: L));
        doneTitle.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));

        var doneSummary = new Label { FontSize = 16, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
        doneSummary.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#666666"), Color.FromArgb("#AAAAAA"));
        var dfs = new FormattedString();
        var dk1 = new Span(); dk1.SetBinding(Span.TextProperty, new Binding("[KnownWords]", source: L));
        var dk2 = new Span { Text = ": " };
        var dk3 = new Span { FontAttributes = FontAttributes.Bold, TextColor = Green }; dk3.SetBinding(Span.TextProperty, "KnownWords.Count");
        var dk4 = new Span { Text = "   " };
        var dk5 = new Span(); dk5.SetBinding(Span.TextProperty, new Binding("[UnknownWords]", source: L));
        var dk6 = new Span { Text = ": " };
        var dk7 = new Span { FontAttributes = FontAttributes.Bold, TextColor = Red }; dk7.SetBinding(Span.TextProperty, "UnknownWords.Count");
        dfs.Spans.Add(dk1); dfs.Spans.Add(dk2); dfs.Spans.Add(dk3);
        dfs.Spans.Add(dk4); dfs.Spans.Add(dk5); dfs.Spans.Add(dk6); dfs.Spans.Add(dk7);
        doneSummary.FormattedText = dfs;

        var restartBtn = new Button { BackgroundColor = Primary, TextColor = Colors.White, CornerRadius = 12, HeightRequest = 52, FontSize = 17 };
        restartBtn.SetBinding(Button.TextProperty, new Binding("[RestartDeck]", source: L));
        restartBtn.SetBinding(Button.CommandProperty, nameof(_vm.RestartCommand));
        finishedState.Add(new Label { Text = "🎉", FontSize = 64, HorizontalOptions = LayoutOptions.Center });
        finishedState.Add(doneTitle); finishedState.Add(doneSummary); finishedState.Add(restartBtn);

        // Action buttons
        var btnRow = new Grid
        {
            Padding = new Thickness(16, 0), ColumnSpacing = 8,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };
        btnRow.SetBinding(IsVisibleProperty, nameof(_vm.HasCards));

        var unknownBtn = new Button { BackgroundColor = Red, TextColor = Colors.White, CornerRadius = 10, HeightRequest = 48 };
        unknownBtn.SetBinding(Button.TextProperty, new Binding("[Unknown]", source: L));
        unknownBtn.SetBinding(Button.CommandProperty, nameof(_vm.MarkUnknownCommand));

        var shuffleBtn = new Button { Text = "🔀", FontSize = 20, BackgroundColor = Primary, TextColor = Colors.White, CornerRadius = 10, HeightRequest = 48 };
        shuffleBtn.SetBinding(Button.CommandProperty, nameof(_vm.ShuffleCommand));

        var knownBtn = new Button { BackgroundColor = Green, TextColor = Colors.White, CornerRadius = 10, HeightRequest = 48 };
        knownBtn.SetBinding(Button.TextProperty, new Binding("[Known]", source: L));
        knownBtn.SetBinding(Button.CommandProperty, nameof(_vm.MarkKnownCommand));

        Grid.SetColumn(unknownBtn, 0); Grid.SetColumn(shuffleBtn, 1); Grid.SetColumn(knownBtn, 2);
        btnRow.Add(unknownBtn); btnRow.Add(shuffleBtn); btnRow.Add(knownBtn);

        // Root grid
        var root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto }
            }
        };

        // Overlay empty/finished states on row 1
        var contentLayer = new Grid();
        contentLayer.Add(card);
        contentLayer.Add(emptyState);
        contentLayer.Add(finishedState);

        root.Add(progressRow, 0, 0);
        root.Add(contentLayer, 0, 1);
        root.Add(btnRow, 0, 2);

        Content = root;
    }
}
