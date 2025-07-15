using System;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

namespace RitualOS.Services
{
    /// <summary>
    /// Utility for loading theme ResourceDictionary files.
    /// </summary>
    public class ThemeLoader
    {
        public Styles Load(string themeFile)
        {
            var uri = new Uri($"avares://RitualOS/Styles/Themes/{themeFile}");
            return new Styles { new StyleInclude(uri) { Source = uri } };
        }
    }
}
