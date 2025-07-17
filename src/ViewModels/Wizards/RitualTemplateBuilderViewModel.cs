using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using RitualOS.Helpers;
using RitualOS.Models;
using RitualOS.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.IO;
using System.Collections;
using Avalonia;

namespace RitualOS.ViewModels.Wizards
{
    /// <summary>
    /// View model for the ritual template builder with validation and enhanced functionality.
    /// </summary>
    public class RitualTemplateBuilderViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();
        private RitualTemplate _template = new();
        private int _selectedStepIndex = -1;
        private string _preview = string.Empty;
        private bool _showRawMarkdown = false;
        private readonly IUserSettingsService _settingsService;

        public RitualTemplate Template
        {
            get => _template;
            set
            {
                if (_template != value)
                {
                    _template = value;
                    OnPropertyChanged();
                    ValidateTemplate();
                }
            }
        }

        public int SelectedStepIndex
        {
            get => _selectedStepIndex;
            set
            {
                if (_selectedStepIndex != value)
                {
                    _selectedStepIndex = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public bool ShowRawMarkdown
        {
            get => _showRawMarkdown;
            set
            {
                if (_showRawMarkdown != value)
                {
                    _showRawMarkdown = value;
                    OnPropertyChanged();
                    UpdatePreview();
                }
            }
        }

        public bool CanEdit => SigilLock.HasAccess(UserContext.CurrentRole, "RitualBuilder");

        // Observable Collections for UI binding
        public ObservableCollection<string> Tools { get; } = new();
        public ObservableCollection<string> SpiritsInvoked { get; } = new();
        public ObservableCollection<string> ChakrasAffected { get; } = new();
        public ObservableCollection<string> ElementsAligned { get; } = new();
        public ObservableCollection<string> RitualSteps { get; } = new();

        // Available options
        public Array ChakraOptions { get; } = Enum.GetValues(typeof(Chakra));
        public Array ElementOptions { get; } = Enum.GetValues(typeof(Element));
        public string[] MoonPhaseOptions { get; } = { "New Moon", "Waxing Crescent", "First Quarter", "Waxing Gibbous", "Full Moon", "Waning Gibbous", "Last Quarter", "Waning Crescent" };

        // Commands
        private ICommand _addStepCommand;
        public ICommand AddStepCommand { get => _addStepCommand; private set { _addStepCommand = value; OnPropertyChanged(); } }
        private ICommand _removeStepCommand;
        public ICommand RemoveStepCommand { get => _removeStepCommand; private set { _removeStepCommand = value; OnPropertyChanged(); } }
        private ICommand _moveStepUpCommand;
        public ICommand MoveStepUpCommand { get => _moveStepUpCommand; private set { _moveStepUpCommand = value; OnPropertyChanged(); } }
        private ICommand _moveStepDownCommand;
        public ICommand MoveStepDownCommand { get => _moveStepDownCommand; private set { _moveStepDownCommand = value; OnPropertyChanged(); } }
        private ICommand _saveTemplateCommand;
        public ICommand SaveTemplateCommand { get => _saveTemplateCommand; private set { _saveTemplateCommand = value; OnPropertyChanged(); } }
        private ICommand _loadTemplateCommand;
        public ICommand LoadTemplateCommand { get => _loadTemplateCommand; private set { _loadTemplateCommand = value; OnPropertyChanged(); } }
        private ICommand _clearTemplateCommand;
        public ICommand ClearTemplateCommand { get => _clearTemplateCommand; private set { _clearTemplateCommand = value; OnPropertyChanged(); } }
        private ICommand _addToolCommand;
        public ICommand AddToolCommand { get => _addToolCommand; private set { _addToolCommand = value; OnPropertyChanged(); } }
        private ICommand _removeToolCommand;
        public ICommand RemoveToolCommand { get => _removeToolCommand; private set { _removeToolCommand = value; OnPropertyChanged(); } }
        private ICommand _addSpiritCommand;
        public ICommand AddSpiritCommand { get => _addSpiritCommand; private set { _addSpiritCommand = value; OnPropertyChanged(); } }
        private ICommand _removeSpiritCommand;
        public ICommand RemoveSpiritCommand { get => _removeSpiritCommand; private set { _removeSpiritCommand = value; OnPropertyChanged(); } }
        private ICommand _addChakraCommand;
        public ICommand AddChakraCommand { get => _addChakraCommand; private set { _addChakraCommand = value; OnPropertyChanged(); } }
        private ICommand _removeChakraCommand;
        public ICommand RemoveChakraCommand { get => _removeChakraCommand; private set { _removeChakraCommand = value; OnPropertyChanged(); } }
        private ICommand _addElementCommand;
        public ICommand AddElementCommand { get => _addElementCommand; private set { _addElementCommand = value; OnPropertyChanged(); } }
        private ICommand _removeElementCommand;
        public ICommand RemoveElementCommand { get => _removeElementCommand; private set { _removeElementCommand = value; OnPropertyChanged(); } }

        // INotifyDataErrorInfo implementation
        public bool HasErrors => _errors.Any();
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public RitualTemplateBuilderViewModel() : this(new UserSettingsService())
        {
        }

        public RitualTemplateBuilderViewModel(IUserSettingsService settingsService)
        {
            _settingsService = settingsService;
            InitializeCommands();
            SetupPropertyChangedHandlers();
            UpdatePreview();
        }

        private void InitializeCommands()
        {
            AddStepCommand = new RelayCommand(_ => AddStep());
            RemoveStepCommand = new RelayCommand(_ => RemoveStep(), _ => SelectedStepIndex >= 0 && SelectedStepIndex < RitualSteps.Count);
            MoveStepUpCommand = new RelayCommand(_ => MoveStepUp(), _ => SelectedStepIndex > 0);
            MoveStepDownCommand = new RelayCommand(_ => MoveStepDown(), _ => SelectedStepIndex >= 0 && SelectedStepIndex < RitualSteps.Count - 1);
            
            SaveTemplateCommand = new RelayCommand(_ => SaveTemplate(), _ => CanEdit && !HasErrors);
            LoadTemplateCommand = new RelayCommand(_ => LoadTemplate());
            ClearTemplateCommand = new RelayCommand(_ => ClearTemplate());
            
            AddToolCommand = new RelayCommand(_ => AddTool());
            RemoveToolCommand = new RelayCommand(param => RemoveTool(param as string));
            AddSpiritCommand = new RelayCommand(_ => AddSpirit());
            RemoveSpiritCommand = new RelayCommand(param => RemoveSpirit(param as string));
            AddChakraCommand = new RelayCommand(_ => AddChakra());
            RemoveChakraCommand = new RelayCommand(param => RemoveChakra(param as string));
            AddElementCommand = new RelayCommand(_ => AddElement());
            RemoveElementCommand = new RelayCommand(param => RemoveElement(param as string));
        }

        private void SetupPropertyChangedHandlers()
        {
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Template))
                {
                    ValidateTemplate();
                    UpdatePreview();
                }
            };
        }

        // Step Management
        private void AddStep()
        {
            RitualSteps.Add($"Step {RitualSteps.Count + 1}");
            UpdateTemplateSteps();
        }

        private void RemoveStep()
        {
            if (SelectedStepIndex >= 0 && SelectedStepIndex < RitualSteps.Count)
            {
                RitualSteps.RemoveAt(SelectedStepIndex);
                SelectedStepIndex = -1;
                UpdateTemplateSteps();
            }
        }

        private void MoveStepUp()
        {
            if (SelectedStepIndex > 0)
            {
                var item = RitualSteps[SelectedStepIndex];
                RitualSteps.RemoveAt(SelectedStepIndex);
                RitualSteps.Insert(SelectedStepIndex - 1, item);
                SelectedStepIndex--;
                UpdateTemplateSteps();
            }
        }

        private void MoveStepDown()
        {
            if (SelectedStepIndex >= 0 && SelectedStepIndex < RitualSteps.Count - 1)
            {
                var item = RitualSteps[SelectedStepIndex];
                RitualSteps.RemoveAt(SelectedStepIndex);
                RitualSteps.Insert(SelectedStepIndex + 1, item);
                SelectedStepIndex++;
                UpdateTemplateSteps();
            }
        }

        // Tool Management
        private void AddTool()
        {
            Tools.Add("New Tool");
            UpdateTemplateTools();
        }

        private void RemoveTool(string? tool)
        {
            if (tool != null && Tools.Contains(tool))
            {
                Tools.Remove(tool);
                UpdateTemplateTools();
            }
        }

        // Spirit Management
        private void AddSpirit()
        {
            SpiritsInvoked.Add("New Spirit");
            UpdateTemplateSpirits();
        }

        private void RemoveSpirit(string? spirit)
        {
            if (spirit != null && SpiritsInvoked.Contains(spirit))
            {
                SpiritsInvoked.Remove(spirit);
                UpdateTemplateSpirits();
            }
        }

        // Chakra Management
        private void AddChakra()
        {
            ChakrasAffected.Add("Root");
            UpdateTemplateChakras();
        }

        private void RemoveChakra(string? chakra)
        {
            if (chakra != null && ChakrasAffected.Contains(chakra))
            {
                ChakrasAffected.Remove(chakra);
                UpdateTemplateChakras();
            }
        }

        // Element Management
        private void AddElement()
        {
            ElementsAligned.Add("Earth");
            UpdateTemplateElements();
        }

        private void RemoveElement(string? element)
        {
            if (element != null && ElementsAligned.Contains(element))
            {
                ElementsAligned.Remove(element);
                UpdateTemplateElements();
            }
        }

        // Template Updates
        private void UpdateTemplateSteps()
        {
            Template.Steps = RitualSteps.ToList();
            UpdatePreview();
        }

        private void UpdateTemplateTools()
        {
            Template.Tools = Tools.ToList();
            UpdatePreview();
        }

        private void UpdateTemplateSpirits()
        {
            Template.SpiritsInvoked = SpiritsInvoked.ToList();
            UpdatePreview();
        }

        private void UpdateTemplateChakras()
        {
            Template.ChakrasAffected = ChakrasAffected.ToList();
            UpdatePreview();
        }

        private void UpdateTemplateElements()
        {
            Template.Elements = ElementsAligned.ToList();
            UpdatePreview();
        }

        // File Operations
        private async void SaveTemplate()
        {
            try
            {
                var mainWindow = (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
                var topLevel = TopLevel.GetTopLevel(mainWindow);
                if (topLevel?.StorageProvider == null) return;

                var options = new FilePickerSaveOptions
                {
                    Title = "Save Ritual Template",
                    DefaultExtension = ".json",
                    SuggestedFileName = $"{Template.Name.Replace(" ", "_")}_template.json"
                };

                var file = await topLevel.StorageProvider.SaveFilePickerAsync(options);
                if (file != null)
                {
                    var path = file.Path.LocalPath;
                    await RitualTemplateSerializer.SaveAsync(Template, path, _settingsService);
                    _settingsService.Current.LastTemplatePath = path;
                    _settingsService.Save();
                }
            }
            catch (Exception ex)
            {
                // TODO: Show error dialog
                Console.WriteLine($"Error saving template: {ex.Message}");
            }
        }

        private async void LoadTemplate()
        {
            try
            {
                var mainWindow = (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
                var topLevel = TopLevel.GetTopLevel(mainWindow);
                if (topLevel?.StorageProvider == null) return;

                var options = new FilePickerOpenOptions
                {
                    Title = "Load Ritual Template",
                    AllowMultiple = false,
                    FileTypeFilter = new[] { new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } } }
                };

                var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);
                if (files.Count > 0)
                {
                    var file = files[0];
                    var path = file.Path.LocalPath;
                    var loaded = await RitualTemplateSerializer.LoadAsync(path, _settingsService);
                    LoadTemplateIntoUI(loaded);
                    _settingsService.Current.LastTemplatePath = path;
                    _settingsService.Save();
                }
            }
            catch (Exception ex)
            {
                // TODO: Show error dialog
                Console.WriteLine($"Error loading template: {ex.Message}");
            }
        }

        private void LoadTemplateIntoUI(RitualTemplate template)
        {
            Template = template;
            
            Tools.Clear();
            foreach (var tool in template.Tools)
                Tools.Add(tool);

            SpiritsInvoked.Clear();
            foreach (var spirit in template.SpiritsInvoked)
                SpiritsInvoked.Add(spirit);

            ChakrasAffected.Clear();
            foreach (var chakra in template.ChakrasAffected)
                ChakrasAffected.Add(chakra);

            ElementsAligned.Clear();
            foreach (var element in template.Elements)
                ElementsAligned.Add(element);

            RitualSteps.Clear();
            foreach (var step in template.Steps)
                RitualSteps.Add(step);

            UpdatePreview();
        }

        private void ClearTemplate()
        {
            Template = new RitualTemplate();
            Tools.Clear();
            SpiritsInvoked.Clear();
            ChakrasAffected.Clear();
            ElementsAligned.Clear();
            RitualSteps.Clear();
            SelectedStepIndex = -1;
            UpdatePreview();
        }

        // Validation
        private void ValidateTemplate()
        {
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(Template.Name))
                AddError(nameof(Template.Name), "Name is required");

            if (string.IsNullOrWhiteSpace(Template.Intention))
                AddError(nameof(Template.Intention), "Intention is required");

            if (RitualSteps.Count == 0)
                AddError("Steps", "At least one ritual step is required");
            if (RitualSteps.Any(s => string.IsNullOrWhiteSpace(s)))
                AddError("Steps", "Steps cannot be blank");

            OnErrorsChanged(nameof(Template));
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }

        public System.Collections.IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.Values.SelectMany(x => x);

            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : Enumerable.Empty<string>();
        }

        // Preview Generation
        private void UpdatePreview()
        {
            if (ShowRawMarkdown)
            {
                Preview = GenerateMarkdownPreview();
            }
            else
            {
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                };
                options.Converters.Add(new JsonStringEnumConverter());
                Preview = JsonSerializer.Serialize(Template, options);
            }
        }

        private string GenerateMarkdownPreview()
        {
            var markdown = $"# {Template.Name}\n\n";
            markdown += $"**Intention:** {Template.Intention}\n\n";
            
            if (Template.Tools.Any())
            {
                markdown += "**Tools:**\n";
                foreach (var tool in Template.Tools)
                    markdown += $"- {tool}\n";
                markdown += "\n";
            }

            if (Template.SpiritsInvoked.Any())
            {
                markdown += "**Spirits Invoked:**\n";
                foreach (var spirit in Template.SpiritsInvoked)
                    markdown += $"- {spirit}\n";
                markdown += "\n";
            }

            if (Template.ChakrasAffected.Any())
            {
                markdown += "**Chakras Affected:**\n";
                foreach (var chakra in Template.ChakrasAffected)
                    markdown += $"- {chakra}\n";
                markdown += "\n";
            }

            if (Template.Elements.Any())
            {
                markdown += "**Elements Aligned:**\n";
                foreach (var element in Template.Elements)
                    markdown += $"- {element}\n";
                markdown += "\n";
            }

            if (!string.IsNullOrEmpty(Template.MoonPhase))
            {
                markdown += $"**Moon Phase:** {Template.MoonPhase}\n\n";
            }

            if (Template.Steps.Any())
            {
                markdown += "**Ritual Steps:**\n";
                for (int i = 0; i < Template.Steps.Count; i++)
                    markdown += $"{i + 1}. {Template.Steps[i]}\n";
                markdown += "\n";
            }

            if (!string.IsNullOrEmpty(Template.OutcomeField))
            {
                markdown += $"**Expected Outcome:** {Template.OutcomeField}\n\n";
            }

            if (!string.IsNullOrEmpty(Template.Notes))
            {
                markdown += $"**Notes:** {Template.Notes}\n";
            }

            return markdown;
        }
    }
}