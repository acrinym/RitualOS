using Avalonia.Controls;
using RitualOS.ViewModels;
using RitualOS.Services;

namespace RitualOS.Views
{
    public partial class MediaStorageView : Window
    {
        public MediaStorageView()
        {
            InitializeComponent();
            
            // Set up the view model with a default master key
            // In a real application, this would come from user input or secure storage
            var masterKey = "your-secure-master-key-here"; // TODO: Get from secure input
            var mediaStorage = new MediaStorageService(masterKey);
            DataContext = new MediaStorageViewModel(mediaStorage);
            
            // Initialize the view model when the window is loaded
            this.Loaded += async (sender, e) =>
            {
                if (DataContext is MediaStorageViewModel viewModel)
                {
                    await viewModel.InitializeAsync();
                }
            };
        }

        public MediaStorageView(string masterKey)
        {
            InitializeComponent();
            
            // Set up the view model with the provided master key
            var mediaStorage = new MediaStorageService(masterKey);
            DataContext = new MediaStorageViewModel(mediaStorage);
            
            // Initialize the view model when the window is loaded
            this.Loaded += async (sender, e) =>
            {
                if (DataContext is MediaStorageViewModel viewModel)
                {
                    await viewModel.InitializeAsync();
                }
            };
        }
    }
} 