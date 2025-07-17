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
    public class DreamParserViewModel : ViewModelBase
    {
        private readonly IDreamParserService _dreamParserService;
        private readonly IUserSettingsService _settingsService;

        private string _dreamContent = string.Empty;
        private DreamAnalysis? _currentAnalysis;
        private bool _isAnalyzing;
        private string _statusMessage = string.Empty;
        private bool _analysisComplete;
        private RitualSuggestion? _selectedSuggestion;

        public DreamParserViewModel(IDreamParserService dreamParserService, IUserSettingsService settingsService)
        {
            _dreamParserService = dreamParserService;
            _settingsService = settingsService;

            AnalyzeDreamCommand = new RelayCommand(async () => await AnalyzeDreamAsync(), () => CanAnalyzeDream());
            SaveAnalysisCommand = new RelayCommand(async () => await SaveAnalysisAsync(), () => CanSaveAnalysis());
            ClearAnalysisCommand = new RelayCommand(ClearAnalysis);
            LoadDreamHistoryCommand = new RelayCommand(async () => await LoadDreamHistoryAsync());

            DreamHistory = new ObservableCollection<DreamAnalysis>();
            SuggestedRituals = new ObservableCollection<RitualSuggestion>();
            ExtractedSymbols = new ObservableCollection<string>();
            ExtractedEmotions = new ObservableCollection<string>();

            LoadDreamHistoryAsync().ConfigureAwait(false);
        }

        public string DreamContent
        {
            get => _dreamContent;
            set
            {
                SetProperty(ref _dreamContent, value);
                AnalyzeDreamCommand.RaiseCanExecuteChanged();
            }
        }

        public DreamAnalysis? CurrentAnalysis
        {
            get => _currentAnalysis;
            set
            {
                SetProperty(ref _currentAnalysis, value);
                SaveAnalysisCommand.RaiseCanExecuteChanged();
                UpdateAnalysisDisplay();
            }
        }

        public bool IsAnalyzing
        {
            get => _isAnalyzing;
            set
            {
                SetProperty(ref _isAnalyzing, value);
                AnalyzeDreamCommand.RaiseCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool AnalysisComplete
        {
            get => _analysisComplete;
            set => SetProperty(ref _analysisComplete, value);
        }

        public RitualSuggestion? SelectedSuggestion
        {
            get => _selectedSuggestion;
            set => SetProperty(ref _selectedSuggestion, value);
        }

        public ObservableCollection<DreamAnalysis> DreamHistory { get; }
        public ObservableCollection<RitualSuggestion> SuggestedRituals { get; }
        public ObservableCollection<string> ExtractedSymbols { get; }
        public ObservableCollection<string> ExtractedEmotions { get; }

        public RelayCommand AnalyzeDreamCommand { get; }
        public RelayCommand SaveAnalysisCommand { get; }
        public RelayCommand ClearAnalysisCommand { get; }
        public RelayCommand LoadDreamHistoryCommand { get; }

        // Computed properties for UI binding
        public string ConfidenceText => CurrentAnalysis?.Confidence.ToString("P1") ?? "0%";
        public string SymbolCountText => CurrentAnalysis?.ExtractedSymbols.Count.ToString() ?? "0";
        public string EmotionCountText => CurrentAnalysis?.Emotions.Count.ToString() ?? "0";
        public string AnalysisDateText => CurrentAnalysis?.Analyzed.ToString("yyyy-MM-dd HH:mm") ?? "Never";

        private async Task AnalyzeDreamAsync()
        {
            if (string.IsNullOrWhiteSpace(DreamContent)) return;

            try
            {
                IsAnalyzing = true;
                AnalysisComplete = false;
                StatusMessage = "Analyzing dream content...";

                CurrentAnalysis = await _dreamParserService.AnalyzeDreamAsync(DreamContent);

                // Get ritual suggestions
                var suggestions = await _dreamParserService.SuggestRitualsAsync(CurrentAnalysis);
                SuggestedRituals.Clear();
                foreach (var suggestion in suggestions)
                {
                    SuggestedRituals.Add(suggestion);
                }

                StatusMessage = "Dream analysis completed successfully";
                AnalysisComplete = true;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Analysis failed: {ex.Message}";
                AnalysisComplete = false;
            }
            finally
            {
                IsAnalyzing = false;
            }
        }

        private async Task SaveAnalysisAsync()
        {
            if (CurrentAnalysis == null) return;

            try
            {
                StatusMessage = "Saving dream analysis...";
                await _dreamParserService.SaveDreamAnalysisAsync(CurrentAnalysis);
                
                // Reload dream history
                await LoadDreamHistoryAsync();
                
                StatusMessage = "Dream analysis saved successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to save analysis: {ex.Message}";
            }
        }

        private void ClearAnalysis()
        {
            DreamContent = string.Empty;
            CurrentAnalysis = null;
            AnalysisComplete = false;
            StatusMessage = "Analysis cleared";
        }

        private async Task LoadDreamHistoryAsync()
        {
            try
            {
                var history = await _dreamParserService.LoadDreamHistoryAsync();
                DreamHistory.Clear();
                
                foreach (var analysis in history.OrderByDescending(x => x.Analyzed))
                {
                    DreamHistory.Add(analysis);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load dream history: {ex.Message}";
            }
        }

        private void UpdateAnalysisDisplay()
        {
            if (CurrentAnalysis == null)
            {
                ExtractedSymbols.Clear();
                ExtractedEmotions.Clear();
                return;
            }

            // Update symbols
            ExtractedSymbols.Clear();
            foreach (var symbol in CurrentAnalysis.ExtractedSymbols)
            {
                ExtractedSymbols.Add(symbol);
            }

            // Update emotions
            ExtractedEmotions.Clear();
            foreach (var emotion in CurrentAnalysis.Emotions)
            {
                ExtractedEmotions.Add(emotion);
            }
        }

        private bool CanAnalyzeDream()
        {
            return !string.IsNullOrWhiteSpace(DreamContent) && !IsAnalyzing;
        }

        private bool CanSaveAnalysis()
        {
            return CurrentAnalysis != null && AnalysisComplete;
        }

        // Helper methods for UI formatting
        public string GetElementAffinityText(string element)
        {
            if (CurrentAnalysis?.ElementalAffinities == null) return "0%";
            
            var affinity = CurrentAnalysis.ElementalAffinities.GetValueOrDefault(element, 0.0);
            return $"{affinity:P1}";
        }

        public string GetChakraAffinityText(string chakra)
        {
            if (CurrentAnalysis?.ChakraAffinities == null) return "0%";
            
            var affinity = CurrentAnalysis.ChakraAffinities.GetValueOrDefault(chakra, 0.0);
            return $"{affinity:P1}";
        }

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

        public string GetEmotionColor(string emotion)
        {
            return emotion.ToLower() switch
            {
                "fear" => "#E53E3E",
                "joy" => "#38A169",
                "sadness" => "#3182CE",
                "anger" => "#E53E3E",
                "love" => "#E91E63",
                "surprise" => "#F6AD55",
                "disgust" => "#805AD5",
                _ => "#718096"
            };
        }

        public string GetDifficultyColor(string difficulty)
        {
            return difficulty.ToLower() switch
            {
                "beginner" => "#38A169",
                "intermediate" => "#F6AD55",
                "advanced" => "#E53E3E",
                _ => "#718096"
            };
        }

        public string FormatDuration(int minutes)
        {
            if (minutes >= 60)
            {
                var hours = minutes / 60;
                var remainingMinutes = minutes % 60;
                return remainingMinutes > 0 ? $"{hours}h {remainingMinutes}m" : $"{hours}h";
            }
            return $"{minutes}m";
        }

        public string GetRelevanceText(double relevance)
        {
            return relevance switch
            {
                >= 0.9 => "Very High",
                >= 0.8 => "High",
                >= 0.7 => "Medium-High",
                >= 0.6 => "Medium",
                >= 0.5 => "Medium-Low",
                _ => "Low"
            };
        }

        public string GetRelevanceColor(double relevance)
        {
            return relevance switch
            {
                >= 0.9 => "#38A169",
                >= 0.8 => "#68D391",
                >= 0.7 => "#F6AD55",
                >= 0.6 => "#F6E05E",
                >= 0.5 => "#E53E3E",
                _ => "#718096"
            };
        }
    }
} 