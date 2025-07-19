using Avalonia.Controls;
using RitualOS.ViewModels;
using RitualOS.Services;

namespace RitualOS.Views
{
    public partial class SymbolSvgViewer : UserControl
    {
        public SymbolSvgViewer()
        {
            InitializeComponent();
            
            // Set up the view model
            DataContext = new SymbolSvgViewerViewModel(new SymbolImageService());
        }

        public SymbolSvgViewer(SymbolImageService imageService)
        {
            InitializeComponent();
            
            // Set up the view model with the provided service
            DataContext = new SymbolSvgViewerViewModel(imageService);
        }

        public void SetSymbol(string symbolName)
        {
            if (DataContext is SymbolSvgViewerViewModel viewModel)
            {
                viewModel.SymbolName = symbolName;
            }
        }
    }
} 