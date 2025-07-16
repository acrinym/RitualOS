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
        public ICommand LoadCommand { get; }
        public ICommand AddSpiritCommand { get; }
        public ICommand AddToolCommand { get; }
        public ICommand AddChakraCommand { get; }
        public ICommand AddElementCommand { get; }
        public ICommand AddStepCommand { get; }
        public bool CanEdit => SigilLock.HasAccess(UserContext.CurrentRole, "RitualBuilder");

        private string _preview = string.Empty;
        public string Preview
        {
            get => _preview;
            private set
            {
                if (_preview != value)
                {
                    _preview = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> Spirits { get; } = new();
        public ObservableCollection<string> Tools { get; } = new();
        public ObservableCollection<Chakra> Chakras { get; } = new();
        public ObservableCollection<Element> Elements { get; } = new();
        public ObservableCollection<string> Steps { get; } = new();

        public RitualTemplateBuilderViewModel()
        {
            NextCommand = new RelayCommand(_ => MoveNext(), _ => Step != RitualWizardStep.Review);
            PrevCommand = new RelayCommand(_ => MovePrev(), _ => Step != RitualWizardStep.Tools);
            SaveCommand = new RelayCommand(_ => Save(), _ => CanEdit);
            LoadCommand = new RelayCommand(_ => Load());
            AddSpiritCommand = new RelayCommand(_ => Spirits.Add(string.Empty));
            AddToolCommand = new RelayCommand(_ => Tools.Add(string.Empty));
            AddChakraCommand = new RelayCommand(_ => Chakras.Add(Chakra.Root));
            AddElementCommand = new RelayCommand(_ => Elements.Add(Element.Earth));
            AddStepCommand = new RelayCommand(_ => Steps.Add(string.Empty));
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
            Template.ChakrasAffected = Chakras.ToList();
            Template.Elements = Elements.ToList();
            Template.Steps = Steps.ToList();
            RitualTemplateSerializer.Save(Template, $"{Template.Name}.json");
            UpdatePreview();
        }

        private void Load()
        {
            try
            {
                var loaded = RitualTemplateSerializer.Load($"{Template.Name}.json");
                Template.Name = loaded.Name;
                Template.Intention = loaded.Intention;
                Template.MoonPhase = loaded.MoonPhase;
                Template.OutcomeField = loaded.OutcomeField;
                Template.Notes = loaded.Notes;
                Tools.Clear();
                foreach (var t in loaded.Tools)
                    Tools.Add(t);
                Spirits.Clear();
                foreach (var s in loaded.SpiritsInvoked)
                    Spirits.Add(s);
                Chakras.Clear();
                foreach (var c in loaded.ChakrasAffected)
                    Chakras.Add(c);
                Elements.Clear();
                foreach (var e in loaded.Elements)
                    Elements.Add(e);
                Steps.Clear();
                foreach (var step in loaded.Steps)
                    Steps.Add(step);
                UpdatePreview();
            }
            catch
            {
                // ignore load failures for now
            }
        }

        private void UpdatePreview()
        {
            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            Preview = System.Text.Json.JsonSerializer.Serialize(Template, options);
        }
    }
}