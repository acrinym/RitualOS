using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using RitualOS.Models;
using RitualOS.Services;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// View model for browsing dream dictionary entries.
    /// </summary>
    public class DreamDictionaryViewModel : ViewModelBase
    {
        private readonly ObservableCollection<DreamDictionaryEntry> _allEntries = new();
        public ObservableCollection<DreamDictionaryEntry> FilteredEntries { get; } = new();

        private string _searchText = string.Empty;
        private DreamDictionaryEntry? _selectedEntry;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplyFilter();
                }
            }
        }

        public DreamDictionaryEntry? SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                if (_selectedEntry != value)
                {
                    _selectedEntry = value;
                    OnPropertyChanged();
                }
            }
        }

        public DreamDictionaryViewModel()
        {
            var path = Path.Combine("docs", "DreamDictionary", "RitualOS_Dream_Dictionary.md");
            foreach (var entry in DreamDictionaryLoader.LoadFromMarkdown(path))
            {
                _allEntries.Add(entry);
            }
            ApplyFilter();
            SelectedEntry = FilteredEntries.FirstOrDefault();
        }

        private void ApplyFilter()
        {
            FilteredEntries.Clear();
            foreach (var entry in _allEntries.Where(e => string.IsNullOrWhiteSpace(SearchText) || e.Term.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            {
                FilteredEntries.Add(entry);
            }
        }
    }
}
