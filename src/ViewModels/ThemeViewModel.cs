using System.Collections.ObjectModel;
using Avalonia;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// View model for theme switching.
    /// </summary>
    public class ThemeViewModel : ViewModelBase
    {
        private string _selectedTheme = "Theme.Dark.xaml";

        public ObservableCollection<string> AvailableThemes { get; } = new()
        {
            "Theme.Dark.xaml",
            "Theme.Light.xaml",
            "Theme.Flame.xaml",
            "Theme.Amanda.xaml"
        };

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme != value)
                {
                    _selectedTheme = value;
                    OnPropertyChanged();
                    ApplyTheme();
                }
            }
        }

        private void ApplyTheme()
        {
            Application.Current?.Styles.Clear();
            var uri = $"avares://RitualOS/Styles/Themes/{SelectedTheme}";
            Application.Current?.Styles.Add(new Avalonia.Markup.Xaml.Styling.StyleInclude(new System.Uri(uri))
            {
                Source = new System.Uri(uri)
            });
        }
    }
}
