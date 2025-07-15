using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;

namespace RitualOS.Services
{
    /// <summary>
    /// Utility class for applying theme resource dictionaries at runtime.
    /// </summary>
    public class ThemeLoader
    {
        /// <summary>
        /// Clears existing styles and loads the given theme file.
        /// </summary>
        /// <param name="themeFile">Name of the theme XAML file.</param>
        public void ApplyTheme(string themeFile)
        {
            var uri = new Uri($"avares://RitualOS/Styles/Themes/{themeFile}");
            var rd = new StyleInclude(uri) { Source = uri };

            Application.Current?.Styles.Clear();
            Application.Current?.Styles.Add(rd);
        }
    }
}
