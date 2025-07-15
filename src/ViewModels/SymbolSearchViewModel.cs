using System.Collections.ObjectModel;
using System.Windows.Input;
using RitualOS.Models;
using RitualOS.Helpers;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// View model for searching codex symbols.
    /// </summary>
    public class SymbolSearchViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private Symbol? _selectedResult;

        public ObservableCollection<Symbol> SearchResults { get; } = new();

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

        public Symbol? SelectedResult
        {
            get => _selectedResult;
            set
            {
                if (_selectedResult != value)
                {
                    _selectedResult = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand GenerateRitualCommand { get; }

        public SymbolSearchViewModel()
        {
            GenerateRitualCommand = new RelayCommand(_ => {/* placeholder */});
        }

        private void PerformSearch()
        {
            // Dummy implementation; in real code this would query a data source
            SearchResults.Clear();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                SearchResults.Add(new Symbol { Name = SearchText, Description = $"Description for {SearchText}" });
            }
        }
    }
}
