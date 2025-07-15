using System.Windows.Input;
using RitualOS.Helpers;

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
            SaveCommand = new RelayCommand(_ => {/* save codex entry */});
        }
    }
}
