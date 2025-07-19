using Avalonia.Controls;
using RitualOS.ViewModels;
using RitualOS.Services;

namespace RitualOS.Views
{
    public partial class GrimoireFSView : Window
    {
        public GrimoireFSView()
        {
            InitializeComponent();
            
            // Set up the view model with a default master key
            // In a real application, this would come from user input or secure storage
            var masterKey = "your-secure-master-key-here"; // TODO: Get from secure input
            var grimoireFS = new GrimoireFS(masterKey);
            DataContext = new GrimoireFSViewModel(grimoireFS);
            
            // Initialize the view model when the window is loaded
            this.Loaded += async (sender, e) =>
            {
                if (DataContext is GrimoireFSViewModel viewModel)
                {
                    await viewModel.InitializeAsync();
                }
            };
        }

        public GrimoireFSView(string masterKey)
        {
            InitializeComponent();
            
            // Set up the view model with the provided master key
            var grimoireFS = new GrimoireFS(masterKey);
            DataContext = new GrimoireFSViewModel(grimoireFS);
            
            // Initialize the view model when the window is loaded
            this.Loaded += async (sender, e) =>
            {
                if (DataContext is GrimoireFSViewModel viewModel)
                {
                    await viewModel.InitializeAsync();
                }
            };
        }
    }
} 