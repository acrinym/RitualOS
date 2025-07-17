using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RitualOS.Services;
using RitualOS.Helpers;

namespace RitualOS.ViewModels
{
    public class AnalyticsViewModel : ViewModelBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IUserSettingsService _settingsService;

        private AnalyticsData? _analyticsData;
        private bool _isLoading;
        private string _statusMessage = string.Empty;
        private string _selectedExportFormat = "html";
        private string _exportPath = string.Empty;

        public AnalyticsViewModel(IAnalyticsService analyticsService, IUserSettingsService settingsService)
        {
            _analyticsService = analyticsService;
            _settingsService = settingsService;

            RefreshCommand = new RelayCommand(async () => await LoadAnalyticsAsync());
            ExportCommand = new RelayCommand(async () => await ExportAnalyticsAsync(), () => CanExport());
            BrowseExportPathCommand = new RelayCommand(BrowseExportPath);

            AvailableExportFormats = new ObservableCollection<string>
            {
                "html",
                "markdown",
                "json"
            };

            LoadAnalyticsAsync().ConfigureAwait(false);
        }

        public ObservableCollection<string> AvailableExportFormats { get; }

        public AnalyticsData? AnalyticsData
        {
            get => _analyticsData;
            set => SetProperty(ref _analyticsData, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
                RefreshCommand.RaiseCanExecuteChanged();
                ExportCommand.RaiseCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string SelectedExportFormat
        {
            get => _selectedExportFormat;
            set => SetProperty(ref _selectedExportFormat, value);
        }

        public string ExportPath
        {
            get => _exportPath;
            set
            {
                SetProperty(ref _exportPath, value);
                ExportCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand BrowseExportPathCommand { get; }

        // Computed properties for UI binding
        public string TotalRitualsText => AnalyticsData?.TotalRituals.ToString() ?? "0";
        public string TotalSessionsText => AnalyticsData?.TotalSessions.ToString() ?? "0";
        public string TotalUsageTimeText => AnalyticsData?.TotalUsageTime.TotalHours.ToString("F1") ?? "0.0";
        public string GeneratedDateText => AnalyticsData?.Generated.ToString("yyyy-MM-dd HH:mm") ?? "Never";

        public ObservableCollection<KeyValuePair<string, int>> MostUsedRituals { get; } = new();
        public ObservableCollection<KeyValuePair<string, int>> ElementUsage { get; } = new();
        public ObservableCollection<KeyValuePair<string, int>> ChakraUsage { get; } = new();
        public ObservableCollection<KeyValuePair<string, int>> ThemePreferences { get; } = new();
        public ObservableCollection<KeyValuePair<string, int>> FeatureUsage { get; } = new();

        private async Task LoadAnalyticsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading analytics data...";

                AnalyticsData = await _analyticsService.GetAnalyticsAsync();

                // Update collections for UI binding
                UpdateMostUsedRituals();
                UpdateElementUsage();
                UpdateChakraUsage();
                UpdateThemePreferences();
                UpdateFeatureUsage();

                StatusMessage = "Analytics data loaded successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading analytics: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateMostUsedRituals()
        {
            MostUsedRituals.Clear();
            if (AnalyticsData?.MostUsedRituals != null)
            {
                foreach (var ritual in AnalyticsData.MostUsedRituals.OrderByDescending(x => x.Value).Take(10))
                {
                    MostUsedRituals.Add(ritual);
                }
            }
        }

        private void UpdateElementUsage()
        {
            ElementUsage.Clear();
            if (AnalyticsData?.ElementUsage != null)
            {
                foreach (var element in AnalyticsData.ElementUsage.OrderByDescending(x => x.Value))
                {
                    ElementUsage.Add(element);
                }
            }
        }

        private void UpdateChakraUsage()
        {
            ChakraUsage.Clear();
            if (AnalyticsData?.ChakraUsage != null)
            {
                foreach (var chakra in AnalyticsData.ChakraUsage.OrderByDescending(x => x.Value))
                {
                    ChakraUsage.Add(chakra);
                }
            }
        }

        private void UpdateThemePreferences()
        {
            ThemePreferences.Clear();
            if (AnalyticsData?.ThemePreferences != null)
            {
                foreach (var theme in AnalyticsData.ThemePreferences.OrderByDescending(x => x.Value))
                {
                    ThemePreferences.Add(theme);
                }
            }
        }

        private void UpdateFeatureUsage()
        {
            FeatureUsage.Clear();
            if (AnalyticsData?.FeatureUsage != null)
            {
                foreach (var feature in AnalyticsData.FeatureUsage.OrderByDescending(x => x.Value))
                {
                    FeatureUsage.Add(feature);
                }
            }
        }

        private async Task ExportAnalyticsAsync()
        {
            if (string.IsNullOrEmpty(ExportPath)) return;

            try
            {
                IsLoading = true;
                StatusMessage = $"Exporting analytics to {SelectedExportFormat}...";

                await _analyticsService.ExportAnalyticsAsync(SelectedExportFormat, ExportPath);

                StatusMessage = $"Analytics exported successfully to {ExportPath}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Export failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void BrowseExportPath()
        {
            // In a real implementation, this would open a file dialog
            // For now, we'll set a default path
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            ExportPath = $"analytics_report_{timestamp}.{GetFileExtension(SelectedExportFormat)}";
        }

        private string GetFileExtension(string format)
        {
            return format.ToLower() switch
            {
                "html" => "html",
                "markdown" => "md",
                "json" => "json",
                _ => "txt"
            };
        }

        private bool CanExport()
        {
            return !IsLoading && !string.IsNullOrEmpty(ExportPath);
        }

        // Helper methods for UI formatting
        public string GetElementColor(string element)
        {
            return element.ToLower() switch
            {
                "fire" => "#E53E3E",
                "water" => "#3182CE",
                "earth" => "#38A169",
                "air" => "#A0AEC0",
                "spirit" => "#9F7AEA",
                _ => "#718096"
            };
        }

        public string GetChakraColor(string chakra)
        {
            return chakra.ToLower() switch
            {
                "root" => "#E53E3E",
                "sacral" => "#F6AD55",
                "solar_plexus" => "#F6E05E",
                "heart" => "#68D391",
                "throat" => "#63B3ED",
                "third_eye" => "#9F7AEA",
                "crown" => "#F7FAFC",
                _ => "#718096"
            };
        }

        public string FormatPercentage(int value, int total)
        {
            if (total == 0) return "0%";
            return $"{(double)value / total * 100:F1}%";
        }

        public string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalHours >= 1)
                return $"{duration.TotalHours:F1} hours";
            if (duration.TotalMinutes >= 1)
                return $"{duration.TotalMinutes:F0} minutes";
            return $"{duration.TotalSeconds:F0} seconds";
        }
    }
} 