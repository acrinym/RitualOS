using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using RitualOS.Helpers;
using RitualOS.Models;

namespace RitualOS.ViewModels.Wizards
{
    /// <summary>
    /// View model for editing SymbolIndex.json.
    /// </summary>
    public class SymbolIndexBuilderViewModel : ViewModelBase
    {
        public ObservableCollection<Symbol> Symbols { get; } = new();

        public ICommand SaveCommand { get; }

        public SymbolIndexBuilderViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save());
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(Symbols);
            File.WriteAllText("SymbolIndex.json", json);
        }
    }
}
