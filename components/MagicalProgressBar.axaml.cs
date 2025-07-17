using Avalonia.Controls;
using Avalonia;

namespace RitualOS.Components
{
    public partial class MagicalProgressBar : UserControl
    {
        public static readonly StyledProperty<double> ProgressProperty =
            AvaloniaProperty.Register<MagicalProgressBar, double>(nameof(Progress));

        public double Progress
        {
            get => GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public MagicalProgressBar()
        {
            InitializeComponent();
        }
    }
}
