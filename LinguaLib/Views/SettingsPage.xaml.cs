using LinguaLib.Resources.Localization;
using LinguaLib.ViewModels;

namespace LinguaLib.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _vm;
    private static readonly Color Primary = Color.FromArgb("#7C6AF7");

    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        BuildUI();
    }

    private void BuildUI()
    {
        var L = LocalizationManager.Instance;
        this.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#F5F5FF"), Color.FromArgb("#0F0F1A"));
        this.SetBinding(TitleProperty, new Binding("[Settings]", source: L));

        var scroll = new ScrollView();
        var stack = new VerticalStackLayout { Padding = 16, Spacing = 16 };

        // Language
        stack.Add(MakeCard(L["Language"], content =>
        {
            var picker = new Picker();
            picker.SetBinding(Picker.ItemsSourceProperty, nameof(_vm.Languages));
            picker.SetBinding(Picker.SelectedIndexProperty, nameof(_vm.SelectedLanguageIndex));
            picker.SetAppTheme(BackgroundColorProperty, Color.FromArgb("#F0F0FF"), Color.FromArgb("#2A2A3E"));
            picker.SetAppTheme(Picker.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));
            content.Add(picker);
        }));

        // Theme
        stack.Add(MakeToggleCard(L, "[Theme]", "[ThemeIcon]", nameof(_vm.ThemeIcon), nameof(_vm.IsDark)));

        // Sound
        stack.Add(MakeToggleCard(L, "[SoundEnabled]", null, "🔊", nameof(_vm.IsSoundEnabled)));

        // Music
        stack.Add(MakeCard(L["Music"], content =>
        {
            var btn = new Button { BackgroundColor = Primary, TextColor = Colors.White, CornerRadius = 8, HeightRequest = 44 };
            btn.SetBinding(Button.TextProperty, nameof(_vm.MusicButtonText));
            btn.SetBinding(Button.CommandProperty, nameof(_vm.ToggleMusicCommand));
            content.Add(btn);
        }));

        scroll.Content = stack;
        Content = scroll;
    }

    private View MakeCard(string title, Action<VerticalStackLayout> addContent)
    {
        var border = new Border
        {
            Padding = 16, StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 }
        };
        border.SetAppTheme(BackgroundColorProperty, Colors.White, Color.FromArgb("#1E1E2E"));

        var stack = new VerticalStackLayout { Spacing = 10 };
        var lbl = new Label { Text = title, FontSize = 16, FontAttributes = FontAttributes.Bold };
        lbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));
        stack.Add(lbl);
        addContent(stack);
        border.Content = stack;
        return border;
    }

    private View MakeToggleCard(LocalizationManager L, string titleKey, string? iconBindingKey, string iconStaticOrBinding, string toggleBinding)
    {
        var border = new Border
        {
            Padding = 16, StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 }
        };
        border.SetAppTheme(BackgroundColorProperty, Colors.White, Color.FromArgb("#1E1E2E"));

        var grid = new Grid { ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Auto } } };

        var labelStack = new VerticalStackLayout();
        var titleLbl = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
        titleLbl.SetBinding(Label.TextProperty, new Binding(titleKey, source: L));
        titleLbl.SetAppTheme(Label.TextColorProperty, Color.FromArgb("#1A1A2E"), Color.FromArgb("#E0E0FF"));

        var iconLbl = new Label { FontSize = 20 };
        if (iconBindingKey != null)
            iconLbl.SetBinding(Label.TextProperty, iconStaticOrBinding);
        else
            iconLbl.Text = iconStaticOrBinding;

        labelStack.Add(titleLbl); labelStack.Add(iconLbl);

        var sw = new Switch { ThumbColor = Color.FromArgb("#7C6AF7"), OnColor = Color.FromArgb("#9E8FFF"), VerticalOptions = LayoutOptions.Center };
        sw.SetBinding(Switch.IsToggledProperty, toggleBinding);

        Grid.SetColumn(labelStack, 0); Grid.SetColumn(sw, 1);
        grid.Add(labelStack); grid.Add(sw);
        border.Content = grid;
        return border;
    }
}
