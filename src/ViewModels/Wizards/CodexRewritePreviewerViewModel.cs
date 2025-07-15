using System.Linq;
using System.Windows.Input;
using RitualOS.Helpers;
using RitualOS.Models;
using RitualOS.Services;

namespace RitualOS.ViewModels.Wizards
{
    /// <summary>
    /// View model for rewriting codex entries with preview.
    /// </summary>
    public class CodexRewritePreviewerViewModel : ViewModelBase
    {
        private string _original = string.Empty;
        private string _rewrite = string.Empty;
        private string _analysis = string.Empty;
        private Symbol? _symbol;

        public Symbol? Symbol
        {
            get => _symbol;
            set
            {
                if (_symbol != value)
                {
                    _symbol = value;
                    if (_symbol != null)
                    {
                        Original = _symbol.Original;
                        Rewrite = _symbol.Rewritten;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public string Original
        {
            get => _original;
            set
            {
                if (_original != value)
                {
                    _original = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Rewrite
        {
            get => _rewrite;
            set
            {
                if (_rewrite != value)
                {
                    _rewrite = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ChakraAnalysis
        {
            get => _analysis;
            set
            {
                if (_analysis != value)
                {
                    _analysis = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }

        public CodexRewritePreviewerViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save(), _ => Symbol != null);
        }

        private void Save()
        {
            if (Symbol == null)
                return;

            Symbol.Original = Original;
            Symbol.Rewritten = Rewrite;
            Symbol.RitualText = ChakraAnalysis;

            var symbols = SymbolIndexService.Load();
            var existing = symbols.FirstOrDefault(s => s.Name == Symbol.Name);
            if (existing != null)
            {
                symbols.Remove(existing);
            }
            symbols.Add(Symbol);
            SymbolIndexService.Save(symbols);
        }
    }
}