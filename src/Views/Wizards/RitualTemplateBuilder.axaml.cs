using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RitualOS.Views.Wizards
{
    public partial class RitualTemplateBuilder : UserControl
    {
        public RitualTemplateBuilder()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}