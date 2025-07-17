using System.Collections.ObjectModel;
using Avalonia;
using RitualOS.Services;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// View model for theme switching.
    /// </summary>
    public class ThemeViewModel : ViewModelBase
    {
        private readonly ThemeLoader _loader = new();
        private string _selectedTheme = "Theme.Dark.xaml";

        public ObservableCollection<string> AvailableThemes { get; } = new()
        {
            "Theme.Dark.xaml",
            "Theme.Light.xaml",
            "Theme.Flame.xaml",
            "Theme.Amanda.xaml",
            "Theme.Material.xaml",
            "Theme.Magical.xaml",
            "Theme.Parchment.xaml",
            "Theme.HighContrast.xaml"
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
            _loader.ApplyTheme(SelectedTheme);
        }
    }
}
