using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RitualOS.ViewModels;
using RitualOS.Models;
using System.Collections.Generic;

namespace RitualOS.ViewModels
{
    public class MainShellViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private int _selectedTabIndex;

        public MainShellViewModel()
        {
            // Initialize all available view models
            InventoryViewModel = new InventoryViewModel();
            DreamDictionaryViewModel = new DreamDictionaryViewModel();
            RitualLibraryViewModel = new RitualLibraryViewModel();
            ClientViewModel = new ClientViewModel(new ClientProfile(), new List<RitualEntry>());
            SymbolSearchViewModel = new SymbolSearchViewModel();
            DocumentViewerViewModel = new DocumentViewerViewModel();
            RitualTimelineViewModel = new RitualTimelineViewModel();
            ThemeViewModel = new ThemeViewModel();

            // Set default view
            CurrentViewModel = InventoryViewModel;
            SelectedTabIndex = 0;
        }

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    OnPropertyChanged();
                    
                    // Update current view model based on selected tab
                    CurrentViewModel = value switch
                    {
                        0 => InventoryViewModel,
                        1 => DreamDictionaryViewModel,
                        2 => RitualLibraryViewModel,
                        3 => ClientViewModel,
                        4 => SymbolSearchViewModel,
                        5 => DocumentViewerViewModel,
                        6 => RitualTimelineViewModel,
                        7 => ThemeViewModel,
                        _ => InventoryViewModel
                    };
                }
            }
        }

        // All available view models
        public InventoryViewModel InventoryViewModel { get; }
        public DreamDictionaryViewModel DreamDictionaryViewModel { get; }
        public RitualLibraryViewModel RitualLibraryViewModel { get; }
        public ClientViewModel ClientViewModel { get; }
        public SymbolSearchViewModel SymbolSearchViewModel { get; }
        public DocumentViewerViewModel DocumentViewerViewModel { get; }
        public RitualTimelineViewModel RitualTimelineViewModel { get; }
        public ThemeViewModel ThemeViewModel { get; }
    }
} 