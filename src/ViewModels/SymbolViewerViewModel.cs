using System.Collections.ObjectModel;
using System.Linq;
using RitualOS.Models;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// View model for browsing codex symbols.
    /// </summary>
    public class SymbolViewerViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private Symbol? _selectedSymbol;

        public ObservableCollection<Symbol> Symbols { get; } = new();
        public ObservableCollection<Symbol> FilteredSymbols { get; } = new();

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

        public Symbol? SelectedSymbol
        {
            get => _selectedSymbol;
            set
            {
                if (_selectedSymbol != value)
                {
                    _selectedSymbol = value;
                    OnPropertyChanged();
                }
            }
        }

        public SymbolViewerViewModel()
        {
            // Example seed data for now
            Symbols.Add(new Symbol { Name = "Sun", Original = "Sol", Rewritten = "Sun", RitualText = "Invoke the power of the sun", Description = "Symbol of light." });
            Symbols.Add(new Symbol { Name = "Moon", Original = "Luna", Rewritten = "Moon", RitualText = "Harness lunar energy", Description = "Symbol of intuition." });
            ApplyFilter();
            SelectedSymbol = FilteredSymbols.FirstOrDefault();
        }

        private void ApplyFilter()
        {
            FilteredSymbols.Clear();
            foreach (var symbol in Symbols.Where(s => string.IsNullOrWhiteSpace(SearchText) || s.Name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)))
            {
                FilteredSymbols.Add(symbol);
            }
        }
    }
}
