using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RitualOS.Styles.Themes
{
    public partial class Theme_Material : ResourceDictionary
    {
        public Theme_Material()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 