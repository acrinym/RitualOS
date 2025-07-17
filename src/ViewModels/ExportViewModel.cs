using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RitualOS.Models;
using RitualOS.Services;
using RitualOS.Helpers;

namespace RitualOS.ViewModels
{
    public class ExportViewModel : ViewModelBase
    {
        private readonly IExportService _exportService;
        private readonly IRitualDataLoader _ritualDataLoader;
        private readonly IUserSettingsService _settingsService;

        private RitualTemplate? _selectedRitual;
        private string _selectedFormat = "markdown";
        private string _outputPath = string.Empty;
        private bool _isExporting;
        private string _exportStatus = string.Empty;
        private bool _exportSuccess;

        public ExportViewModel(IExportService exportService, IRitualDataLoader ritualDataLoader, IUserSettingsService settingsService)
        {
            _exportService = exportService;
            _ritualDataLoader = ritualDataLoader;
            _settingsService = settingsService;

            ExportRitualCommand = new RelayCommand(async () => await ExportRitualAsync(), () => CanExportRitual());
            ExportLibraryCommand = new RelayCommand(async () => await ExportLibraryAsync(), () => CanExportLibrary());
            BrowseOutputPathCommand = new RelayCommand(BrowseOutputPath);
            LoadRitualsCommand = new RelayCommand(async () => await LoadRitualsAsync());

            AvailableFormats = new ObservableCollection<string>
            {
                "markdown",
                "html",
                "json",
                "pdf",
                "epub",
                "website"
            };

            LoadRitualsAsync().ConfigureAwait(false);
        }

        public ObservableCollection<RitualTemplate> AvailableRituals { get; } = new();
        public ObservableCollection<string> AvailableFormats { get; }

        public RitualTemplate? SelectedRitual
        {
            get => _selectedRitual;
            set
            {
                SetProperty(ref _selectedRitual, value);
                ExportRitualCommand.RaiseCanExecuteChanged();
            }
        }

        public string SelectedFormat
        {
            get => _selectedFormat;
            set => SetProperty(ref _selectedFormat, value);
        }

        public string OutputPath
        {
            get => _outputPath;
            set
            {
                SetProperty(ref _outputPath, value);
                ExportRitualCommand.RaiseCanExecuteChanged();
                ExportLibraryCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsExporting
        {
            get => _isExporting;
            set
            {
                SetProperty(ref _isExporting, value);
                ExportRitualCommand.RaiseCanExecuteChanged();
                ExportLibraryCommand.RaiseCanExecuteChanged();
            }
        }

        public string ExportStatus
        {
            get => _exportStatus;
            set => SetProperty(ref _exportStatus, value);
        }

        public bool ExportSuccess
        {
            get => _exportSuccess;
            set => SetProperty(ref _exportSuccess, value);
        }

        public ICommand ExportRitualCommand { get; }
        public ICommand ExportLibraryCommand { get; }
        public ICommand BrowseOutputPathCommand { get; }
        public ICommand LoadRitualsCommand { get; }

        private async Task LoadRitualsAsync()
        {
            try
            {
                IsExporting = true;
                ExportStatus = "Loading rituals...";

                var rituals = await _ritualDataLoader.LoadRitualTemplatesAsync();
                AvailableRituals.Clear();
                
                foreach (var ritual in rituals)
                {
                    AvailableRituals.Add(ritual);
                }

                ExportStatus = $"Loaded {AvailableRituals.Count} rituals";
            }
            catch (Exception ex)
            {
                ExportStatus = $"Error loading rituals: {ex.Message}";
            }
            finally
            {
                IsExporting = false;
            }
        }

        private async Task ExportRitualAsync()
        {
            if (SelectedRitual == null) return;

            try
            {
                IsExporting = true;
                ExportSuccess = false;
                ExportStatus = $"Exporting {SelectedRitual.Name} to {SelectedFormat}...";

                string result;
                switch (SelectedFormat.ToLower())
                {
                    case "markdown":
                        result = await _exportService.ExportToMarkdownAsync(SelectedRitual);
                        break;
                    case "html":
                        result = await _exportService.ExportToHtmlAsync(SelectedRitual);
                        break;
                    case "json":
                        result = await _exportService.ExportToJsonAsync(SelectedRitual);
                        break;
                    case "pdf":
                        result = await _exportService.ExportToPdfAsync(SelectedRitual);
                        break;
                    case "epub":
                        result = await _exportService.ExportToEpubAsync(SelectedRitual);
                        break;
                    case "website":
                        result = await _exportService.ExportToWebsiteAsync(SelectedRitual);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported format: {SelectedFormat}");
                }

                // Save to file if output path is specified
                if (!string.IsNullOrEmpty(OutputPath))
                {
                    await System.IO.File.WriteAllTextAsync(OutputPath, result);
                    ExportStatus = $"Successfully exported to {OutputPath}";
                }
                else
                {
                    ExportStatus = "Export completed successfully";
                }

                ExportSuccess = true;
            }
            catch (Exception ex)
            {
                ExportStatus = $"Export failed: {ex.Message}";
                ExportSuccess = false;
            }
            finally
            {
                IsExporting = false;
            }
        }

        private async Task ExportLibraryAsync()
        {
            try
            {
                IsExporting = true;
                ExportSuccess = false;
                ExportStatus = $"Exporting library to {SelectedFormat}...";

                await _exportService.ExportRitualLibraryAsync(AvailableRituals, SelectedFormat, OutputPath);

                ExportStatus = $"Successfully exported library to {OutputPath}";
                ExportSuccess = true;
            }
            catch (Exception ex)
            {
                ExportStatus = $"Library export failed: {ex.Message}";
                ExportSuccess = false;
            }
            finally
            {
                IsExporting = false;
            }
        }

        private void BrowseOutputPath()
        {
            // In a real implementation, this would open a file dialog
            // For now, we'll set a default path
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            OutputPath = $"export_{SelectedFormat}_{timestamp}.{GetFileExtension(SelectedFormat)}";
        }

        private string GetFileExtension(string format)
        {
            return format.ToLower() switch
            {
                "markdown" => "md",
                "html" => "html",
                "json" => "json",
                "pdf" => "pdf",
                "epub" => "epub",
                "website" => "html",
                _ => "txt"
            };
        }

        private bool CanExportRitual()
        {
            return SelectedRitual != null && !IsExporting;
        }

        private bool CanExportLibrary()
        {
            return AvailableRituals.Count > 0 && !string.IsNullOrEmpty(OutputPath) && !IsExporting;
        }
    }
} 