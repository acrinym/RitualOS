using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using RitualOS.Models;
using RitualOS.Services;

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
            var index = SymbolIndexService.Load();
            if (index.Count == 0)
            {
                var docPath = Path.Combine(AppContext.BaseDirectory, "docs", "DreamDictionary", "RitualOS_Dream_Dictionary.md");
                if (File.Exists(docPath))
                {
                    index = CodexRewriteEngine.ParseFile(docPath);
                    SymbolIndexService.Save(index);
                }
            }

            foreach (var symbol in index)
            {
                Symbols.Add(symbol);
            }

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
