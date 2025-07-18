using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RitualOS.Views
{
    public partial class ThemeSwitcher : UserControl
    {
        public ThemeSwitcher()
        {
            InitializeComponent();
        }

        private void OpenPicker_Click(object? sender, RoutedEventArgs e)
        {
            var picker = new ThemePickerWindow();
            picker.Show();
        }
    }
}