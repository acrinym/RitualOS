using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RitualOS.Models;
using RitualOS.Helpers;
using RitualOS.Services;
using System.IO;
using System.Text.Json;

namespace RitualOS.ViewModels
{
    public class SymbolWikiViewModel : ViewModelBase
    {
        public ObservableCollection<SymbolWikiEntry> Symbols { get; } = new();
        private SymbolWikiEntry? _hoveredSymbol;
        public SymbolWikiEntry? HoveredSymbol
        {
            get => _hoveredSymbol;
            set { _hoveredSymbol = value; OnPropertyChanged(); }
        }
        public SymbolWikiViewModel()
        {
            var path = Path.Combine("symbols", "symbol_metadata.json");
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var entries = JsonSerializer.Deserialize<SymbolWikiEntry[]>(json);
                if (entries != null)
                    foreach (var s in entries) Symbols.Add(s);
            }
        }
    }
} 