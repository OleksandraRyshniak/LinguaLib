using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Resources;

namespace LinguaLib.Resources.Localization;

public class LocalizationManager : INotifyPropertyChanged
{
    public static readonly LocalizationManager Instance = new();
    private ResourceManager _rm = AppResources.ResourceManager;

    public string this[string key]
    {
        get
        {
            var val = _rm.GetString(key, CultureInfo.CurrentUICulture);
            return val ?? key;
        }
    }

    public void SetCulture(string code)
    {
        CultureInfo.CurrentUICulture = new CultureInfo(code);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
