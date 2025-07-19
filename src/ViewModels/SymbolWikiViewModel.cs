using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RitualOS.Models;
using RitualOS.Helpers;
using RitualOS.Services;

namespace RitualOS.ViewModels
{
    public class SymbolWikiViewModel : ViewModelBase
    {
        private readonly SymbolWikiService _symbolWikiService;
        
        // Search and Filter Properties
        private string _searchText = string.Empty;
        private SymbolCategory _selectedCategory = SymbolCategory.Other;
        private SymbolPower _selectedMinPower = SymbolPower.Minor;
        private string _selectedTag = string.Empty;
        private string _selectedPlanet = string.Empty;
        private string _selectedElement = string.Empty;
        
        // Display Properties
        private Symbol? _selectedSymbol;
        private bool _showAdvancedFilters = false;
        private bool _isLoading = false;
        
        // Collections
        public ObservableCollection<Symbol> AllSymbols { get; } = new();
        public ObservableCollection<Symbol> FilteredSymbols { get; } = new();
        public ObservableCollection<string> AvailableTags { get; } = new();
        public ObservableCollection<string> AvailablePlanets { get; } = new();
        public ObservableCollection<string> AvailableElements { get; } = new();
        public ObservableCollection<SymbolCategory> AvailableCategories { get; } = new();
        public ObservableCollection<SymbolPower> AvailablePowerLevels { get; } = new();

        // Commands
        public ICommand SearchCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand AddSymbolCommand { get; }
        public ICommand EditSymbolCommand { get; }
        public ICommand DeleteSymbolCommand { get; }
        public ICommand ExportSymbolsCommand { get; }
        public ICommand ImportSymbolsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleAdvancedFiltersCommand { get; }

        public SymbolWikiViewModel(SymbolWikiService symbolWikiService)
        {
            _symbolWikiService = symbolWikiService;
            
            // Initialize commands
            SearchCommand = new RelayCommand(_ => PerformSearch());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            AddSymbolCommand = new RelayCommand(_ => AddNewSymbol());
            EditSymbolCommand = new RelayCommand(_ => EditSelectedSymbol(), _ => SelectedSymbol != null);
            DeleteSymbolCommand = new RelayCommand(_ => DeleteSelectedSymbol(), _ => SelectedSymbol != null);
            ExportSymbolsCommand = new RelayCommand(_ => ExportSymbols());
            ImportSymbolsCommand = new RelayCommand(_ => ImportSymbols());
            RefreshCommand = new RelayCommand(_ => RefreshData());
            ToggleAdvancedFiltersCommand = new RelayCommand(_ => ShowAdvancedFilters = !ShowAdvancedFilters);

            // Initialize collections
            InitializeCollections();
            
            // Subscribe to service events
            _symbolWikiService.SymbolsLoaded += OnSymbolsLoaded;
            _symbolWikiService.SymbolAdded += OnSymbolAdded;
            _symbolWikiService.SymbolUpdated += OnSymbolUpdated;
        }

        #region Properties

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public SymbolCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public SymbolPower SelectedMinPower
        {
            get => _selectedMinPower;
            set
            {
                if (_selectedMinPower != value)
                {
                    _selectedMinPower = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public string SelectedTag
        {
            get => _selectedTag;
            set
            {
                if (_selectedTag != value)
                {
                    _selectedTag = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public string SelectedPlanet
        {
            get => _selectedPlanet;
            set
            {
                if (_selectedPlanet != value)
                {
                    _selectedPlanet = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public string SelectedElement
        {
            get => _selectedElement;
            set
            {
                if (_selectedElement != value)
                {
                    _selectedElement = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public Symbol? SelectedSymbol
        {
            get => _selectedSymbol;
            set
            {
                if (_selectedSymbol != value)
                {
                    _selectedSymbol = value;
                    OnPropertyChanged();
                    // Update command states
                    (EditSymbolCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (DeleteSymbolCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool ShowAdvancedFilters
        {
            get => _showAdvancedFilters;
            set
            {
                if (_showAdvancedFilters != value)
                {
                    _showAdvancedFilters = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Initialization

        private void InitializeCollections()
        {
            // Initialize categories
            AvailableCategories.Clear();
            foreach (SymbolCategory category in Enum.GetValues(typeof(SymbolCategory)))
            {
                AvailableCategories.Add(category);
            }

            // Initialize power levels
            AvailablePowerLevels.Clear();
            foreach (SymbolPower power in Enum.GetValues(typeof(SymbolPower)))
            {
                AvailablePowerLevels.Add(power);
            }
        }

        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                await _symbolWikiService.InitializeAsync();
                await LoadAllDataAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadAllDataAsync()
        {
            // Load all symbols
            var allSymbols = _symbolWikiService.GetAllSymbols();
            AllSymbols.Clear();
            foreach (var symbol in allSymbols)
            {
                AllSymbols.Add(symbol);
            }

            // Load available tags, planets, and elements
            await LoadAvailableFiltersAsync();

            // Perform initial search
            PerformSearch();
        }

        private async Task LoadAvailableFiltersAsync()
        {
            var tags = _symbolWikiService.GetAllTags();
            AvailableTags.Clear();
            AvailableTags.Add(string.Empty); // Empty option
            foreach (var tag in tags)
            {
                AvailableTags.Add(tag);
            }

            var planets = _symbolWikiService.GetAllPlanets();
            AvailablePlanets.Clear();
            AvailablePlanets.Add(string.Empty); // Empty option
            foreach (var planet in planets)
            {
                AvailablePlanets.Add(planet);
            }

            var elements = _symbolWikiService.GetAllElements();
            AvailableElements.Clear();
            AvailableElements.Add(string.Empty); // Empty option
            foreach (var element in elements)
            {
                AvailableElements.Add(element);
            }
        }

        #endregion

        #region Search and Filtering

        private void PerformSearch()
        {
            var results = _symbolWikiService.SearchSymbols(SearchText, SelectedCategory, SelectedMinPower);

            // Apply additional filters
            if (!string.IsNullOrWhiteSpace(SelectedTag))
            {
                results = results.Where(s => s.Tags.Contains(SelectedTag, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(SelectedPlanet))
            {
                results = results.Where(s => s.PlanetaryCorrespondences.Contains(SelectedPlanet, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(SelectedElement))
            {
                results = results.Where(s => s.ElementalCorrespondences.Contains(SelectedElement, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            // Update filtered results
            FilteredSymbols.Clear();
            foreach (var symbol in results)
            {
                FilteredSymbols.Add(symbol);
            }
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedCategory = SymbolCategory.Other;
            SelectedMinPower = SymbolPower.Minor;
            SelectedTag = string.Empty;
            SelectedPlanet = string.Empty;
            SelectedElement = string.Empty;
            ShowAdvancedFilters = false;
        }

        #endregion

        #region Symbol Management

        private void AddNewSymbol()
        {
            // TODO: Open symbol editor dialog
            // This would typically open a dialog or navigate to a symbol editor view
        }

        private void EditSelectedSymbol()
        {
            if (SelectedSymbol != null)
            {
                // TODO: Open symbol editor dialog with selected symbol
                // This would typically open a dialog or navigate to a symbol editor view
            }
        }

        private async void DeleteSelectedSymbol()
        {
            if (SelectedSymbol != null)
            {
                // TODO: Show confirmation dialog
                // For now, just delete directly
                await _symbolWikiService.DeleteSymbolAsync(SelectedSymbol.Name, SelectedSymbol.Category);
                await LoadAllDataAsync();
            }
        }

        private void ExportSymbols()
        {
            // TODO: Implement export functionality
            // This would typically open a file dialog and call the service export method
        }

        private void ImportSymbols()
        {
            // TODO: Implement import functionality
            // This would typically open a file dialog and call the service import method
        }

        private async void RefreshData()
        {
            await LoadAllDataAsync();
        }

        #endregion

        #region Event Handlers

        private void OnSymbolsLoaded(object? sender, EventArgs e)
        {
            // Symbols loaded event - could trigger UI updates
        }

        private void OnSymbolAdded(object? sender, EventArgs e)
        {
            // Symbol added event - refresh the data
            _ = LoadAllDataAsync();
        }

        private void OnSymbolUpdated(object? sender, EventArgs e)
        {
            // Symbol updated event - refresh the data
            _ = LoadAllDataAsync();
        }

        #endregion

        #region Helper Methods

        public string GetCategoryDisplayName(SymbolCategory category)
        {
            var field = category.GetType().GetField(category.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
            return attribute?.DisplayName ?? category.ToString();
        }

        public string GetPowerLevelDisplayName(SymbolPower power)
        {
            var field = power.GetType().GetField(power.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
            return attribute?.DisplayName ?? power.ToString();
        }

        #endregion
    }
} 