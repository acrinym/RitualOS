using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RitualOS.Helpers;
using RitualOS.Models;
using RitualOS.Services;

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

        public RitualTemplate Template { get; } = new();

        public ICommand NextCommand { get; }
        public ICommand PrevCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddSpiritCommand { get; }
        public ICommand AddToolCommand { get; }

        public ObservableCollection<string> Spirits { get; } = new();
        public ObservableCollection<string> Tools { get; } = new();

        public RitualTemplateBuilderViewModel()
        {
            NextCommand = new RelayCommand(_ => MoveNext(), _ => Step != RitualWizardStep.Review);
            PrevCommand = new RelayCommand(_ => MovePrev(), _ => Step != RitualWizardStep.Tools);
            SaveCommand = new RelayCommand(_ => Save());
            AddSpiritCommand = new RelayCommand(_ => Spirits.Add(string.Empty));
            AddToolCommand = new RelayCommand(_ => Tools.Add(string.Empty));
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

        private void Save()
        {
            Template.SpiritsInvoked = Spirits.ToList();
            Template.Tools = Tools.ToList();
            RitualTemplateSerializer.Save(Template, $"{Template.Name}.json");
        }
    }
}