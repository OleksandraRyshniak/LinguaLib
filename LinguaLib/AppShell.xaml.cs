using LinguaLib.Resources.Localization;
using LinguaLib.Views;

namespace LinguaLib;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(ReaderPage), typeof(ReaderPage));
        Routing.RegisterRoute(nameof(AddBookPage), typeof(AddBookPage));

        UpdateTabTitles();
        LocalizationManager.Instance.PropertyChanged += (_, _) => UpdateTabTitles();
    }

    private void UpdateTabTitles()
    {
        var L = LocalizationManager.Instance;
        var tabs = Items.OfType<TabBar>().FirstOrDefault()?.Items;
        if (tabs is null) return;
        if (tabs.Count >= 1) tabs[0].Title = "📚";
        if (tabs.Count >= 2) tabs[1].Title = "📖";
        if (tabs.Count >= 3) tabs[2].Title = "💡";
        if (tabs.Count >= 4) tabs[3].Title = "⚙️";
    }
}
