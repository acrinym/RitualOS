using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RitualOS.ViewModels;
using RitualOS.Models;
using System.Collections.Generic;
using RitualOS.Services;
using RitualOS.Helpers;

namespace RitualOS.ViewModels
{
    public class MainShellViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private int _selectedTabIndex;
        public RelayCommand SetTabCommand { get; }
        public ObservableCollection<string> RecentTemplateNames { get; } = new();

        public MainShellViewModel()
        {
            // Initialize services
            var userSettingsService = new UserSettingsService();
            var exportService = new ExportService(userSettingsService);
            var analyticsService = new AnalyticsService(userSettingsService);
            var dreamParserService = new DreamParserService();
            var ritualDataLoader = new RitualDataLoader();

            // Initialize all available view models
            InventoryViewModel = new InventoryViewModel();
            DreamDictionaryViewModel = new DreamDictionaryViewModel();
            RitualLibraryViewModel = new RitualLibraryViewModel();
            ClientViewModel = new ClientViewModel(new ClientProfile(), new List<RitualEntry>());
            SymbolSearchViewModel = new SymbolSearchViewModel();
            DocumentViewerViewModel = new DocumentViewerViewModel();
            RitualTimelineViewModel = new RitualTimelineViewModel();
            ThemeViewModel = new ThemeViewModel();
            ExportViewModel = new ExportViewModel(exportService, ritualDataLoader, userSettingsService);
            AnalyticsViewModel = new AnalyticsViewModel(analyticsService, userSettingsService);
            DreamParserViewModel = new DreamParserViewModel(dreamParserService, userSettingsService);
            TarotViewModel = new TarotViewModel();
            MagicSchoolsViewModel = new MagicSchoolsViewModel(path =>
            {
                DocumentViewerViewModel.DocumentPath = path;
                DocumentViewerViewModel.LoadDocumentCommand.Execute(null);
                SelectedTabIndex = 5; // Switch to Documents tab
            });

            LoadRecentTemplates();

            SetTabCommand = new RelayCommand(param =>
            {
                if (param is int index)
                    SelectedTabIndex = index;
            });

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
                        8 => ExportViewModel,
                        9 => AnalyticsViewModel,
                        10 => DreamParserViewModel,
                        11 => TarotViewModel,
                        12 => MagicSchoolsViewModel,
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
        public ExportViewModel ExportViewModel { get; }
        public AnalyticsViewModel AnalyticsViewModel { get; }
        public DreamParserViewModel DreamParserViewModel { get; }
        public TarotViewModel TarotViewModel { get; }
        public MagicSchoolsViewModel MagicSchoolsViewModel { get; }
        private void LoadRecentTemplates()
        {
            try
            {
                var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ritual_templates");
                if (!Directory.Exists(dir)) return;

                var files = Directory.GetFiles(dir, "*.json");
                foreach (var file in files.OrderByDescending(File.GetCreationTime).Take(3))
                {
                    var name = Path.GetFileNameWithoutExtension(file).Replace("_", " ");
                    RecentTemplateNames.Add(name);
                }
            }
            catch
            {
                // ignore errors for now
            }
        }
    }
}