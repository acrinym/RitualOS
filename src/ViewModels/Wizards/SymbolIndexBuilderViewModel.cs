using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RitualOS.Helpers;
using RitualOS.Models;
using RitualOS.Services;

namespace RitualOS.ViewModels.Wizards
{
    /// <summary>
    /// View model for editing SymbolIndex.json.
    /// </summary>
    public class SymbolIndexBuilderViewModel : ViewModelBase
    {
        public ObservableCollection<Symbol> Symbols { get; } = new();

        public ICommand SaveCommand { get; }
        public ICommand AddCommand { get; }

        public SymbolIndexBuilderViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save());
            AddCommand = new RelayCommand(_ => AddSymbol());

            // Load existing symbols from disk when the editor opens
            foreach (var symbol in SymbolIndexService.Load())
            {
                Symbols.Add(symbol);
            }
        }

        private void Save()
        {
            SymbolIndexService.Save(Symbols.ToList());
        }

        private void AddSymbol()
        {
            Symbols.Add(new Symbol());
        }
    }
}
