using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RitualOS.Services;
using RitualOS.Helpers;
using System.Collections.Generic; // Added missing import for List

namespace RitualOS.ViewModels
{
    public class GrimoireFSViewModel : ViewModelBase
    {
        private readonly GrimoireFS _grimoireFS;
        
        // Search and Filter Properties
        private string _searchText = string.Empty;
        private GrimoireFS.GrimoireEntryType _selectedType = GrimoireFS.GrimoireEntryType.Other;
        private string _selectedTag = string.Empty;
        private DateTime _startDate = DateTime.Now.AddDays(-30);
        private DateTime _endDate = DateTime.Now;
        
        // Display Properties
        private GrimoireFS.GrimoireEntry? _selectedEntry;
        private bool _isLoading = false;
        private bool _showAdvancedFilters = false;
        
        // Collections
        public ObservableCollection<GrimoireFS.GrimoireEntry> AllEntries { get; } = new();
        public ObservableCollection<GrimoireFS.GrimoireEntry> FilteredEntries { get; } = new();
        public ObservableCollection<string> AvailableTags { get; } = new();
        public ObservableCollection<GrimoireFS.GrimoireEntryType> AvailableTypes { get; } = new();
        public ObservableCollection<string> AvailableSymbolIds { get; } = new();

        // Commands
        public ICommand SearchCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand AddEntryCommand { get; }
        public ICommand EditEntryCommand { get; }
        public ICommand DeleteEntryCommand { get; }
        public ICommand CreateBackupCommand { get; }
        public ICommand RestoreBackupCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleAdvancedFiltersCommand { get; }
        public ICommand ExportEntriesCommand { get; }

        public GrimoireFSViewModel(GrimoireFS grimoireFS)
        {
            _grimoireFS = grimoireFS;
            
            // Initialize commands
            SearchCommand = new RelayCommand(_ => PerformSearch());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            AddEntryCommand = new RelayCommand(_ => AddNewEntry());
            EditEntryCommand = new RelayCommand(_ => EditSelectedEntry(), _ => SelectedEntry != null);
            DeleteEntryCommand = new RelayCommand(_ => DeleteSelectedEntry(), _ => SelectedEntry != null);
            CreateBackupCommand = new RelayCommand(_ => CreateBackup());
            RestoreBackupCommand = new RelayCommand(_ => RestoreBackup());
            RefreshCommand = new RelayCommand(_ => RefreshData());
            ToggleAdvancedFiltersCommand = new RelayCommand(_ => ShowAdvancedFilters = !ShowAdvancedFilters);
            ExportEntriesCommand = new RelayCommand(_ => ExportEntries());

            // Initialize collections
            InitializeCollections();
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

        public GrimoireFS.GrimoireEntryType SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
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

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public GrimoireFS.GrimoireEntry? SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                if (_selectedEntry != value)
                {
                    _selectedEntry = value;
                    OnPropertyChanged();
                    // Update command states
                    (EditEntryCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (DeleteEntryCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
            // Initialize types
            AvailableTypes.Clear();
            foreach (GrimoireFS.GrimoireEntryType type in Enum.GetValues(typeof(GrimoireFS.GrimoireEntryType)))
            {
                AvailableTypes.Add(type);
            }
        }

        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                await _grimoireFS.InitializeAsync();
                await LoadAllDataAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadAllDataAsync()
        {
            // Load all entries
            var allEntries = await _grimoireFS.GetAllEntriesAsync();
            AllEntries.Clear();
            foreach (var entry in allEntries)
            {
                AllEntries.Add(entry);
            }

            // Load available tags
            await LoadAvailableFiltersAsync();

            // Perform initial search
            PerformSearch();
        }

        private async Task LoadAvailableFiltersAsync()
        {
            var tags = await _grimoireFS.GetAllTagsAsync();
            AvailableTags.Clear();
            AvailableTags.Add(string.Empty); // Empty option
            foreach (var tag in tags)
            {
                AvailableTags.Add(tag);
            }

            var symbolIds = await _grimoireFS.GetAllSymbolIdsAsync();
            AvailableSymbolIds.Clear();
            AvailableSymbolIds.Add(string.Empty); // Empty option
            foreach (var symbolId in symbolIds)
            {
                AvailableSymbolIds.Add(symbolId);
            }
        }

        #endregion

        #region Search and Filtering

        private void PerformSearch()
        {
            var results = AllEntries.AsQueryable();

            // Apply type filter
            if (SelectedType != GrimoireFS.GrimoireEntryType.Other)
            {
                results = results.Where(e => e.Type == SelectedType);
            }

            // Apply search term
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchTerm = SearchText.ToLowerInvariant();
                results = results.Where(e => 
                    e.Title.ToLowerInvariant().Contains(searchTerm) ||
                    e.Content.ToLowerInvariant().Contains(searchTerm) ||
                    e.Tags.Any(tag => tag.ToLowerInvariant().Contains(searchTerm))
                );
            }

            // Apply tag filter
            if (!string.IsNullOrWhiteSpace(SelectedTag))
            {
                results = results.Where(e => e.Tags.Contains(SelectedTag, StringComparer.OrdinalIgnoreCase));
            }

            // Apply date range filter
            results = results.Where(e => e.CreatedDate >= StartDate && e.CreatedDate <= EndDate);

            // Update filtered results
            FilteredEntries.Clear();
            foreach (var entry in results.OrderByDescending(e => e.ModifiedDate))
            {
                FilteredEntries.Add(entry);
            }
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedType = GrimoireFS.GrimoireEntryType.Other;
            SelectedTag = string.Empty;
            StartDate = DateTime.Now.AddDays(-30);
            EndDate = DateTime.Now;
            ShowAdvancedFilters = false;
        }

        #endregion

        #region Entry Management

        private void AddNewEntry()
        {
            // TODO: Open entry editor dialog
            // This would typically open a dialog or navigate to an entry editor view
        }

        private void EditSelectedEntry()
        {
            if (SelectedEntry != null)
            {
                // TODO: Open entry editor dialog with selected entry
                // This would typically open a dialog or navigate to an entry editor view
            }
        }

        private async void DeleteSelectedEntry()
        {
            if (SelectedEntry != null)
            {
                // TODO: Show confirmation dialog
                // For now, just delete directly
                await _grimoireFS.DeleteEntryAsync(SelectedEntry.Id);
                await LoadAllDataAsync();
            }
        }

        private async void CreateBackup()
        {
            try
            {
                await _grimoireFS.CreateBackupAsync();
                // TODO: Show success message
            }
            catch (Exception ex)
            {
                // TODO: Show error message
            }
        }

        private async void RestoreBackup()
        {
            // TODO: Show backup selection dialog
            // For now, just use the most recent backup
            var backups = await _grimoireFS.GetBackupListAsync();
            if (backups.Any())
            {
                try
                {
                    await _grimoireFS.RestoreFromBackupAsync(backups.First());
                    await LoadAllDataAsync();
                    // TODO: Show success message
                }
                catch (Exception ex)
                {
                    // TODO: Show error message
                }
            }
        }

        private void ExportEntries()
        {
            // TODO: Implement export functionality
            // This would typically open a file dialog and export the filtered entries
        }

        private async void RefreshData()
        {
            await LoadAllDataAsync();
        }

        #endregion

        #region Helper Methods

        public string GetTypeDisplayName(GrimoireFS.GrimoireEntryType type)
        {
            var field = type.GetType().GetField(type.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
            return attribute?.DisplayName ?? type.ToString();
        }

        public async Task<GrimoireFS.GrimoireMetadata> GetMetadataAsync()
        {
            return await _grimoireFS.GetMetadataAsync();
        }

        public async Task<List<string>> GetBackupListAsync()
        {
            return await _grimoireFS.GetBackupListAsync();
        }

        #endregion
    }
} 