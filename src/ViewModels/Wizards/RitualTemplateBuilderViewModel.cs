using System.Collections.ObjectModel;
using System.Windows.Input;
using RitualOS.Helpers;
using RitualOS.Models;

namespace RitualOS.ViewModels.Wizards
{
    /// <summary>
    /// View model for the ritual creation wizard.
    /// </summary>
    public class RitualTemplateBuilderViewModel : ViewModelBase
    {
        private RitualWizardStep _step = RitualWizardStep.Tools;
        public RitualWizardStep Step
        {
            get => _step;
            set
            {
                if (_step != value)
                {
                    _step = value;
                    OnPropertyChanged();
                }
            }
        }

        public RitualEntry Ritual { get; } = new();

        public ICommand NextCommand { get; }
        public ICommand PrevCommand { get; }
        public ICommand SaveCommand { get; }

        public ObservableCollection<string> Spirits { get; } = new();
        public ObservableCollection<string> Ingredients { get; } = new();

        public RitualTemplateBuilderViewModel()
        {
            NextCommand = new RelayCommand(_ => MoveNext(), _ => Step != RitualWizardStep.Review);
            PrevCommand = new RelayCommand(_ => MovePrev(), _ => Step != RitualWizardStep.Tools);
            SaveCommand = new RelayCommand(_ => {/* hook up persistence */});
        }

        private void MoveNext()
        {
            if (Step < RitualWizardStep.Review)
            {
                Step++;
            }
        }

        private void MovePrev()
        {
            if (Step > RitualWizardStep.Tools)
            {
                Step--;
            }
        }
    }
}
