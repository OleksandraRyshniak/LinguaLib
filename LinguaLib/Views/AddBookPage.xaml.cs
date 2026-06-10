using LinguaLib.Converters;
using LinguaLib.Resources.Localization;
using LinguaLib.ViewModels;

namespace LinguaLib.Views;

public partial class AddBookPage : ContentPage
{
    private readonly AddBookViewModel _vm;
    private static readonly Color Primary = Color.FromArgb("#7C6AF7");

    public AddBookPage(AddBookViewModel vm)
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
        this.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#F5F5FF"), Color.FromArgb("#0F0F1A"));
        this.SetBinding(TitleProperty, new Binding("[AddBookTitle]", source: L));

        var scroll = new ScrollView();
        var stack = new VerticalStackLayout { Spacing = 16, Padding = 20 };

        var titleLbl = new Label { FontSize = 24, FontAttributes = FontAttributes.Bold };
        titleLbl.SetBinding(Label.TextProperty, new Binding("[NewBook]", source: L));
        titleLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));
        stack.Add(titleLbl);

        // Cover card
        stack.Add(MakeSectionCard(L["Cover"], content =>
        {
            var img = new Image { HeightRequest = 160, Aspect = Aspect.AspectFit };
            img.SetBinding(Image.SourceProperty, nameof(_vm.CoverUrl));
            img.SetBinding(IsVisibleProperty, nameof(_vm.HasCover));
            content.Add(img);

            var urlEntry = new Entry();
            urlEntry.SetBinding(Entry.PlaceholderProperty, new Binding("[CoverUrlPlaceholder]", source: L));
            urlEntry.SetBinding(Entry.TextProperty, nameof(_vm.CoverUrl));
            urlEntry.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#F0F0FF"), Color.FromArgb("#2A2A3E"));
            urlEntry.SetAppTheme(Entry.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));
            content.Add(urlEntry);

            var pickBtn = new Button { BackgroundColor = Primary, TextColor = Colors.White, CornerRadius = 8, HeightRequest = 44 };
            pickBtn.SetBinding(Button.TextProperty, new Binding("[PickGallery]", source: L));
            pickBtn.SetBinding(Button.CommandProperty, nameof(_vm.PickCoverCommand));
            content.Add(pickBtn);
        }));

        // Book info card
        stack.Add(MakeSectionCard(L["BookInfo"], content =>
        {
            var titleEntry = new Entry();
            titleEntry.SetBinding(Entry.PlaceholderProperty, new Binding("[Title]", source: L));
            titleEntry.SetBinding(Entry.TextProperty, nameof(_vm.Title));
            titleEntry.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#F0F0FF"), Color.FromArgb("#2A2A3E"));
            titleEntry.SetAppTheme(Entry.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));

            var authorEntry = new Entry();
            authorEntry.SetBinding(Entry.PlaceholderProperty, new Binding("[Author]", source: L));
            authorEntry.SetBinding(Entry.TextProperty, nameof(_vm.Author));
            authorEntry.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#F0F0FF"), Color.FromArgb("#2A2A3E"));
            authorEntry.SetAppTheme(Entry.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));

            var picker = new Picker();
            picker.SetBinding(Picker.TitleProperty, new Binding("[Language]", source: L));
            picker.SetBinding(Picker.SelectedItemProperty, nameof(_vm.Language));
            picker.Items.Add("en"); picker.Items.Add("ru"); picker.Items.Add("et");
            picker.Items.Add("de"); picker.Items.Add("fr");
            picker.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#F0F0FF"), Color.FromArgb("#2A2A3E"));
            picker.SetAppTheme(Picker.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));

            content.Add(titleEntry); content.Add(authorEntry); content.Add(picker);
        }));

        // File card
        stack.Add(MakeSectionCard(L["BookFile"], content =>
        {
            var fileLbl = new Label { FontSize = 13 };
            fileLbl.SetBinding(Label.TextProperty, nameof(_vm.FileName));
            fileLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#555555"), Color.FromArgb("#AAAAAA"));

            var pickBtn = new Button { BackgroundColor = Primary, TextColor = Colors.White, CornerRadius = 8, HeightRequest = 44 };
            pickBtn.SetBinding(Button.TextProperty, new Binding("[PickFile]", source: L));
            pickBtn.SetBinding(Button.CommandProperty, nameof(_vm.PickFileCommand));

            content.Add(fileLbl); content.Add(pickBtn);
        }));

        var saveBtn = new Button
        {
            BackgroundColor = Color.FromArgb("#4CAF50"), TextColor = Colors.White,
            CornerRadius = 12, HeightRequest = 52, FontSize = 17, FontAttributes = FontAttributes.Bold
        };
        saveBtn.SetBinding(Button.TextProperty, new Binding("[SaveBook]", source: L));
        saveBtn.SetBinding(Button.CommandProperty, nameof(_vm.SaveCommand));
        saveBtn.SetBinding(IsEnabledProperty, new Binding(nameof(_vm.IsBusy), converter: inv));

        var spinner = new ActivityIndicator { Color = Primary, HorizontalOptions = LayoutOptions.Center };
        spinner.SetBinding(ActivityIndicator.IsRunningProperty, nameof(_vm.IsBusy));
        spinner.SetBinding(IsVisibleProperty, nameof(_vm.IsBusy));

        stack.Add(saveBtn); stack.Add(spinner);
        scroll.Content = stack;
        Content = scroll;
    }

    private View MakeSectionCard(string title, Action<VerticalStackLayout> addContent)
    {
        var border = new Border
        {
            Padding = 16, StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 }
        };
        border.SetAppTheme(BackgroundColorProperty, Colors.White, Color.FromArgb("#1E1E2E"));

        var stack = new VerticalStackLayout { Spacing = 10 };
        var lbl = new Label { Text = title, FontSize = 14, FontAttributes = FontAttributes.Bold };
        lbl.SetAppTheme(Label.TextColorProperty, Primary, Color.FromArgb("#9E8FFF"));
        stack.Add(lbl);
        addContent(stack);
        border.Content = stack;
        return border;
    }
}
