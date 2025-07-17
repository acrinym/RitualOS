using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;

namespace RitualOS.Services
{
    /// <summary>
    /// Utility class for applying theme resource dictionaries at runtime.
    /// </summary>
    public class ThemeLoader
    {
        private StyleInclude? _currentTheme;

        /// <summary>
        /// Clears existing theme styles and loads the given theme file.
        /// </summary>
        /// <param name="themeFile">Name of the theme XAML file.</param>
        public void ApplyTheme(string themeFile)
        {
            if (Application.Current == null) return;

            var uri = new Uri($"avares://RitualOS/Styles/Themes/{themeFile}");
            var newTheme = new StyleInclude(uri) { Source = uri };

            // Remove the current theme if it exists
            if (_currentTheme != null)
            {
                Application.Current.Styles.Remove(_currentTheme);
            }

            // Add the new theme
            Application.Current.Styles.Add(newTheme);
            _currentTheme = newTheme;
        }
    }
}
