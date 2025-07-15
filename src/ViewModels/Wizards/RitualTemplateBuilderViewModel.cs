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

        public RitualEntry Ritual { get; } = new();

        public ICommand NextCommand { get; }
        public ICommand PrevCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddSpiritCommand { get; }
        public ICommand AddIngredientCommand { get; }

        public ObservableCollection<string> Spirits { get; } = new();
        public ObservableCollection<string> Ingredients { get; } = new();

        public RitualTemplateBuilderViewModel()
        {
            NextCommand = new RelayCommand(_ => MoveNext(), _ => Step != RitualWizardStep.Review);
            PrevCommand = new RelayCommand(_ => MovePrev(), _ => Step != RitualWizardStep.Tools);
            SaveCommand = new RelayCommand(_ => Save());
            AddSpiritCommand = new RelayCommand(_ => Spirits.Add(string.Empty));
            AddIngredientCommand = new RelayCommand(_ => Ingredients.Add(string.Empty));
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
            Ritual.SpiritsInvoked = Spirits.ToList();
            Ritual.Ingredients = Ingredients.ToList();
            RitualDataLoader.SaveRitualToJson(Ritual, $"{Ritual.Id}.json");
        }
    }
}