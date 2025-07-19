using Avalonia.Controls;
using RitualOS.ViewModels;

namespace RitualOS.Views
{
    public partial class SymbolWikiView : Window
    {
        public SymbolWikiView()
        {
            InitializeComponent();
            
            // Set up the view model
            DataContext = new SymbolWikiViewModel(new Services.SymbolWikiService());
            
            // Initialize the view model when the window is loaded
            this.Loaded += async (sender, e) =>
            {
                if (DataContext is SymbolWikiViewModel viewModel)
                {
                    await viewModel.InitializeAsync();
                }
            };
        }
    }
} 