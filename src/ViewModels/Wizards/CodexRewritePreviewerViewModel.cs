using System;
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
        private bool _showOriginal = true;

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
                    OnPropertyChanged(nameof(EditText));
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
                    OnPropertyChanged(nameof(EditText));
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
        public ICommand ExportCommand { get; }

        /// <summary>
        /// True if editing the original text, false for the rewritten version.
        /// </summary>
        public bool ShowOriginal
        {
            get => _showOriginal;
            set
            {
                if (_showOriginal != value)
                {
                    _showOriginal = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EditText));
                }
            }
        }

        /// <summary>
        /// The text currently being edited based on <see cref="ShowOriginal"/>.
        /// </summary>
        public string EditText
        {
            get => ShowOriginal ? Original : Rewrite;
            set
            {
                if (ShowOriginal)
                    Original = value;
                else
                    Rewrite = value;
            }
        }

        /// <summary>
        /// Whether the current user can modify codex entries.
        /// </summary>
        public bool CanEdit => SigilLock.HasAccess(UserContext.CurrentRole, "CodexRewrite");

        public CodexRewritePreviewerViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save(), _ => Symbol != null && CanEdit);
            ExportCommand = new RelayCommand(_ => Export(), _ => Symbol != null);
        }

        private void Save()
        {
            if (Symbol == null)
                return;

            Symbol.Original = Original;
            Symbol.Rewritten = Rewrite;
            Symbol.RitualText = ChakraAnalysis;

            try
            {
                var symbols = SymbolIndexService.Load();
                var existing = symbols.FirstOrDefault(s => s.Name == Symbol.Name);
                if (existing != null)
                {
                    symbols.Remove(existing);
                }
                symbols.Add(Symbol);
                SymbolIndexService.Save(symbols);
            }
            catch
            {
                // We should add user-facing error handling here later.
            }
        }

        private void Export()
        {
            if (Symbol == null)
                return;

            var fileName = $"{Symbol.Name}_rewritten.md";
            CodexRewriteEngine.ExportMarkdown(new[] { Symbol }, fileName);
        }
    }
}