using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;
using RitualOS.Models;

namespace RitualOS.Services
{
    public interface IDreamParserService
    {
        Task<DreamAnalysis> AnalyzeDreamAsync(string dreamContent);
        Task<List<RitualSuggestion>> SuggestRitualsAsync(DreamAnalysis analysis);
        Task<List<string>> ExtractSymbolsAsync(string dreamContent);
        Task<Dictionary<string, double>> CalculateElementalAffinitiesAsync(List<string> symbols);
        Task<Dictionary<string, double>> CalculateChakraAffinitiesAsync(List<string> symbols);
        Task<string> GenerateRitualIntentAsync(DreamAnalysis analysis);
        Task<List<string>> ExtractEmotionsAsync(string dreamContent);
        Task<DreamPattern> IdentifyPatternsAsync(List<DreamAnalysis> dreamHistory);
        Task SaveDreamAnalysisAsync(DreamAnalysis analysis);
        Task<List<DreamAnalysis>> LoadDreamHistoryAsync();
    }

    public class DreamAnalysis
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Analyzed { get; set; } = DateTime.Now;
        public string OriginalContent { get; set; } = string.Empty;
        public List<string> ExtractedSymbols { get; set; } = new();
        public Dictionary<string, DreamDictionaryEntry> SymbolMeanings { get; set; } = new();
        public List<string> Emotions { get; set; } = new();
        public Dictionary<string, double> ElementalAffinities { get; set; } = new();
        public Dictionary<string, double> ChakraAffinities { get; set; } = new();
        public string GeneratedIntent { get; set; } = string.Empty;
        public List<RitualSuggestion> SuggestedRituals { get; set; } = new();
        public DreamPattern? IdentifiedPattern { get; set; }
        public double Confidence { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class RitualSuggestion
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Relevance { get; set; }
        public List<string> Elements { get; set; } = new();
        public List<string> Chakras { get; set; } = new();
        public string Intent { get; set; } = string.Empty;
        public List<string> KeySymbols { get; set; } = new();
        public string Difficulty { get; set; } = "beginner";
        public int EstimatedDuration { get; set; } = 30;
    }

    public class DreamPattern
    {
        public string PatternType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> RecurringSymbols { get; set; } = new();
        public List<string> RecurringEmotions { get; set; } = new();
        public Dictionary<string, double> AverageElementalAffinities { get; set; } = new();
        public Dictionary<string, double> AverageChakraAffinities { get; set; } = new();
        public int Frequency { get; set; }
        public DateTime FirstOccurrence { get; set; }
        public DateTime LastOccurrence { get; set; }
    }

    public class DreamParserService : IDreamParserService
    {
        private readonly string _dreamHistoryPath;
        private readonly Dictionary<string, List<string>> _symbolDictionary;
        private readonly Dictionary<string, List<string>> _emotionDictionary;
        private readonly Dictionary<string, List<string>> _elementalMappings;
        private readonly Dictionary<string, List<string>> _chakraMappings;
        private List<DreamDictionaryEntry> _dreamDictionary = new();

        public DreamParserService()
        {
            _dreamHistoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "dream_history.json");
            _symbolDictionary = InitializeSymbolDictionary();
            _emotionDictionary = InitializeEmotionDictionary();
            _elementalMappings = InitializeElementalMappings();
            _chakraMappings = InitializeChakraMappings();

            var searchPaths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs", "DreamDictionary", "RitualOS_Dream_Dictionary.md"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "docs", "DreamDictionary", "RitualOS_Dream_Dictionary.md"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "docs", "DreamDictionary", "RitualOS_Dream_Dictionary.md")
            };

            foreach (var path in searchPaths)
            {
                var full = Path.GetFullPath(path);
                if (File.Exists(full))
                {
                    _dreamDictionary = DreamDictionaryLoader.LoadFromMarkdown(full);
                    break;
                }
            }

            if (_dreamDictionary.Count == 0)
                _dreamDictionary = new List<DreamDictionaryEntry>();
       }

        public async Task<DreamAnalysis> AnalyzeDreamAsync(string dreamContent)
        {
            var analysis = new DreamAnalysis
            {
                OriginalContent = dreamContent
            };

            // Extract symbols
            analysis.ExtractedSymbols = await ExtractSymbolsAsync(dreamContent);

            // Lookup meanings
            foreach (var symbol in analysis.ExtractedSymbols)
            {
                var entry = _dreamDictionary.FirstOrDefault(e =>
                    string.Equals(e.Term, symbol, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(e.Term.Split(' ')[0], symbol, StringComparison.OrdinalIgnoreCase));
                if (entry != null)
                    analysis.SymbolMeanings[symbol] = entry;
            }
            
            // Extract emotions
            analysis.Emotions = await ExtractEmotionsAsync(dreamContent);
            
            // Calculate affinities
            analysis.ElementalAffinities = await CalculateElementalAffinitiesAsync(analysis.ExtractedSymbols);
            analysis.ChakraAffinities = await CalculateChakraAffinitiesAsync(analysis.ExtractedSymbols);
            
            // Generate intent
            analysis.GeneratedIntent = await GenerateRitualIntentAsync(analysis);
            
            // Calculate confidence
            analysis.Confidence = CalculateConfidence(analysis);
            
            // Add metadata
            analysis.Metadata["wordCount"] = dreamContent.Split(' ').Length;
            analysis.Metadata["symbolCount"] = analysis.ExtractedSymbols.Count;
            analysis.Metadata["emotionCount"] = analysis.Emotions.Count;
            
            return analysis;
        }

        public Task<List<RitualSuggestion>> SuggestRitualsAsync(DreamAnalysis analysis)
        {
            var suggestions = new List<RitualSuggestion>();
            
            // Determine primary elements and chakras
            var primaryElements = analysis.ElementalAffinities.OrderByDescending(x => x.Value).Take(2).Select(x => x.Key).ToList();
            var primaryChakras = analysis.ChakraAffinities.OrderByDescending(x => x.Value).Take(2).Select(x => x.Key).ToList();
            
            // Generate suggestions based on dream content
            if (analysis.Emotions.Contains("fear") || analysis.Emotions.Contains("anxiety"))
            {
                suggestions.Add(new RitualSuggestion
                {
                    Name = "Protection and Grounding Ritual",
                    Description = "A ritual to dispel fear and establish protective boundaries",
                    Relevance = 0.9,
                    Elements = new List<string> { "earth", "fire" },
                    Chakras = new List<string> { "root", "solar_plexus" },
                    Intent = "Protection and grounding",
                    KeySymbols = analysis.ExtractedSymbols.Take(3).ToList(),
                    Difficulty = "beginner",
                    EstimatedDuration = 45
                });
            }
            
            if (analysis.Emotions.Contains("sadness") || analysis.Emotions.Contains("grief"))
            {
                suggestions.Add(new RitualSuggestion
                {
                    Name = "Healing and Release Ritual",
                    Description = "A ritual to process emotions and facilitate healing",
                    Relevance = 0.85,
                    Elements = new List<string> { "water", "air" },
                    Chakras = new List<string> { "heart", "throat" },
                    Intent = "Healing and emotional release",
                    KeySymbols = analysis.ExtractedSymbols.Take(3).ToList(),
                    Difficulty = "intermediate",
                    EstimatedDuration = 60
                });
            }
            
            if (analysis.Emotions.Contains("joy") || analysis.Emotions.Contains("excitement"))
            {
                suggestions.Add(new RitualSuggestion
                {
                    Name = "Manifestation and Abundance Ritual",
                    Description = "A ritual to amplify positive energy and manifest desires",
                    Relevance = 0.8,
                    Elements = new List<string> { "fire", "air" },
                    Chakras = new List<string> { "sacral", "solar_plexus" },
                    Intent = "Manifestation and abundance",
                    KeySymbols = analysis.ExtractedSymbols.Take(3).ToList(),
                    Difficulty = "beginner",
                    EstimatedDuration = 30
                });
            }
            
            // Add element-specific suggestions
            foreach (var element in primaryElements)
            {
                suggestions.Add(GenerateElementalRitual(element, analysis));
            }
            
            // Add chakra-specific suggestions
            foreach (var chakra in primaryChakras)
            {
                suggestions.Add(GenerateChakraRitual(chakra, analysis));
            }
            
            // Sort by relevance and return top suggestions
            return Task.FromResult(suggestions.OrderByDescending(x => x.Relevance).Take(5).ToList());
        }

        public Task<List<string>> ExtractSymbolsAsync(string dreamContent)
        {
            var symbols = new List<string>();
            var content = dreamContent.ToLower();
            
            // Extract symbols from dictionary
            foreach (var entry in _symbolDictionary)
            {
                if (content.Contains(entry.Key.ToLower()))
                {
                    symbols.AddRange(entry.Value);
                }
            }
            
            // Extract common dream symbols using regex patterns
            var patterns = new Dictionary<string, string>
            {
                { "water", @"\b(water|ocean|sea|river|lake|rain|tears)\b" },
                { "fire", @"\b(fire|flame|burn|heat|lightning|sun)\b" },
                { "earth", @"\b(earth|ground|soil|mountain|rock|tree|forest)\b" },
                { "air", @"\b(air|wind|sky|cloud|bird|fly|wing)\b" },
                { "animals", @"\b(dog|cat|bird|snake|wolf|bear|deer|horse|fish)\b" },
                { "vehicles", @"\b(car|train|plane|boat|bike|bus)\b" },
                { "buildings", @"\b(house|building|room|door|window|stairs)\b" },
                { "people", @"\b(person|man|woman|child|family|friend)\b" }
            };
            
            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(content, pattern.Value);
                if (matches.Count > 0)
                {
                    symbols.Add(pattern.Key);
                }
            }
            
            return Task.FromResult(symbols.Distinct().ToList());
        }

        public Task<Dictionary<string, double>> CalculateElementalAffinitiesAsync(List<string> symbols)
        {
            var affinities = new Dictionary<string, double>
            {
                { "fire", 0.0 },
                { "water", 0.0 },
                { "earth", 0.0 },
                { "air", 0.0 },
                { "spirit", 0.0 }
            };
            
            foreach (var symbol in symbols)
            {
                foreach (var mapping in _elementalMappings)
                {
                    if (mapping.Value.Contains(symbol.ToLower()))
                    {
                        affinities[mapping.Key] += 1.0;
                    }
                }
            }
            
            // Normalize scores
            var maxScore = affinities.Values.Max();
            if (maxScore > 0)
            {
                foreach (var key in affinities.Keys.ToList())
                {
                    affinities[key] = affinities[key] / maxScore;
                }
            }
            
            return Task.FromResult(affinities);
        }

        public Task<Dictionary<string, double>> CalculateChakraAffinitiesAsync(List<string> symbols)
        {
            var affinities = new Dictionary<string, double>
            {
                { "root", 0.0 },
                { "sacral", 0.0 },
                { "solar_plexus", 0.0 },
                { "heart", 0.0 },
                { "throat", 0.0 },
                { "third_eye", 0.0 },
                { "crown", 0.0 }
            };
            
            foreach (var symbol in symbols)
            {
                foreach (var mapping in _chakraMappings)
                {
                    if (mapping.Value.Contains(symbol.ToLower()))
                    {
                        affinities[mapping.Key] += 1.0;
                    }
                }
            }
            
            // Normalize scores
            var maxScore = affinities.Values.Max();
            if (maxScore > 0)
            {
                foreach (var key in affinities.Keys.ToList())
                {
                    affinities[key] = affinities[key] / maxScore;
                }
            }
            
            return Task.FromResult(affinities);
        }

        public Task<string> GenerateRitualIntentAsync(DreamAnalysis analysis)
        {
            var intent = new List<string>();
            
            // Add emotional context
            if (analysis.Emotions.Contains("fear"))
                intent.Add("protection and courage");
            if (analysis.Emotions.Contains("sadness"))
                intent.Add("healing and comfort");
            if (analysis.Emotions.Contains("joy"))
                intent.Add("amplification and gratitude");
            if (analysis.Emotions.Contains("anger"))
                intent.Add("release and transformation");
            
            // Add elemental focus
            var primaryElement = analysis.ElementalAffinities.OrderByDescending(x => x.Value).FirstOrDefault();
            if (primaryElement.Value > 0.5)
            {
                intent.Add($"{primaryElement.Key} energy alignment");
            }
            
            // Add chakra focus
            var primaryChakra = analysis.ChakraAffinities.OrderByDescending(x => x.Value).FirstOrDefault();
            if (primaryChakra.Value > 0.5)
            {
                intent.Add($"{primaryChakra.Key} chakra activation");
            }
            
            // Add symbol-based intent
            if (analysis.ExtractedSymbols.Contains("water"))
                intent.Add("emotional flow and purification");
            if (analysis.ExtractedSymbols.Contains("fire"))
                intent.Add("transformation and passion");
            if (analysis.ExtractedSymbols.Contains("earth"))
                intent.Add("grounding and stability");
            if (analysis.ExtractedSymbols.Contains("air"))
                intent.Add("clarity and communication");
            
            return Task.FromResult(string.Join(", ", intent.Distinct()));
        }

        public Task<List<string>> ExtractEmotionsAsync(string dreamContent)
        {
            var emotions = new List<string>();
            var content = dreamContent.ToLower();
            
            // Extract emotions from dictionary
            foreach (var entry in _emotionDictionary)
            {
                if (content.Contains(entry.Key.ToLower()))
                {
                    emotions.AddRange(entry.Value);
                }
            }
            
            // Extract emotions using regex patterns
            var emotionPatterns = new Dictionary<string, string>
            {
                { "fear", @"\b(fear|afraid|scared|terrified|anxious|worried)\b" },
                { "joy", @"\b(joy|happy|excited|elated|thrilled|delighted)\b" },
                { "sadness", @"\b(sad|sorrow|grief|melancholy|depressed|lonely)\b" },
                { "anger", @"\b(angry|furious|rage|irritated|frustrated|mad)\b" },
                { "love", @"\b(love|affection|tenderness|warmth|caring|devotion)\b" },
                { "surprise", @"\b(surprised|shocked|amazed|astonished|startled)\b" },
                { "disgust", @"\b(disgust|repulsed|revolted|sickened|appalled)\b" }
            };
            
            foreach (var pattern in emotionPatterns)
            {
                var matches = Regex.Matches(content, pattern.Value);
                if (matches.Count > 0)
                {
                    emotions.Add(pattern.Key);
                }
            }
            
            return Task.FromResult(emotions.Distinct().ToList());
        }

        public Task<DreamPattern> IdentifyPatternsAsync(List<DreamAnalysis> dreamHistory)
        {
            if (dreamHistory.Count < 3)
                return Task.FromResult(new DreamPattern());
            
            var pattern = new DreamPattern();
            
            // Find recurring symbols
            var symbolFrequency = dreamHistory
                .SelectMany(d => d.ExtractedSymbols)
                .GroupBy(s => s)
                .Where(g => g.Count() > 1)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();
            
            pattern.RecurringSymbols = symbolFrequency;
            
            // Find recurring emotions
            var emotionFrequency = dreamHistory
                .SelectMany(d => d.Emotions)
                .GroupBy(e => e)
                .Where(g => g.Count() > 1)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();
            
            pattern.RecurringEmotions = emotionFrequency;
            
            // Calculate average affinities
            pattern.AverageElementalAffinities = CalculateAverageAffinities(
                dreamHistory.Select(d => d.ElementalAffinities).ToList());
            
            pattern.AverageChakraAffinities = CalculateAverageAffinities(
                dreamHistory.Select(d => d.ChakraAffinities).ToList());
            
            pattern.Frequency = dreamHistory.Count;
            pattern.FirstOccurrence = dreamHistory.Min(d => d.Analyzed);
            pattern.LastOccurrence = dreamHistory.Max(d => d.Analyzed);
            
            // Determine pattern type
            pattern.PatternType = DeterminePatternType(pattern);
            pattern.Description = GeneratePatternDescription(pattern);

            return Task.FromResult(pattern);
        }

        public async Task SaveDreamAnalysisAsync(DreamAnalysis analysis)
        {
            try
            {
                var history = await LoadDreamHistoryAsync();
                history.Add(analysis);
                
                var json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_dreamHistoryPath, json);
            }
            catch (Exception)
            {
                // Silently fail for dream analysis saving
            }
        }

        public async Task<List<DreamAnalysis>> LoadDreamHistoryAsync()
        {
            try
            {
                if (File.Exists(_dreamHistoryPath))
                {
                    var json = await File.ReadAllTextAsync(_dreamHistoryPath);
                    return JsonSerializer.Deserialize<List<DreamAnalysis>>(json) ?? new List<DreamAnalysis>();
                }
            }
            catch (Exception)
            {
                // Return empty list if loading fails
            }
            
            return new List<DreamAnalysis>();
        }

        private RitualSuggestion GenerateElementalRitual(string element, DreamAnalysis analysis)
        {
            var ritualTemplates = new Dictionary<string, RitualSuggestion>
            {
                ["fire"] = new RitualSuggestion
                {
                    Name = "Fire Transformation Ritual",
                    Description = "A ritual to harness fire energy for transformation and passion",
                    Relevance = 0.75,
                    Elements = new List<string> { "fire" },
                    Chakras = new List<string> { "solar_plexus" },
                    Intent = "Transformation and passion",
                    KeySymbols = analysis.ExtractedSymbols.Take(3).ToList(),
                    Difficulty = "intermediate",
                    EstimatedDuration = 45
                },
                ["water"] = new RitualSuggestion
                {
                    Name = "Water Purification Ritual",
                    Description = "A ritual to cleanse and purify through water energy",
                    Relevance = 0.75,
                    Elements = new List<string> { "water" },
                    Chakras = new List<string> { "sacral" },
                    Intent = "Purification and emotional flow",
                    KeySymbols = analysis.ExtractedSymbols.Take(3).ToList(),
                    Difficulty = "beginner",
                    EstimatedDuration = 30
                },
                ["earth"] = new RitualSuggestion
                {
                    Name = "Earth Grounding Ritual",
                    Description = "A ritual to establish stability and connection to the earth",
                    Relevance = 0.75,
                    Elements = new List<string> { "earth" },
                    Chakras = new List<string> { "root" },
                    Intent = "Grounding and stability",
                    KeySymbols = analysis.ExtractedSymbols.Take(3).ToList(),
                    Difficulty = "beginner",
                    EstimatedDuration = 30
                },
                ["air"] = new RitualSuggestion
                {
                    Name = "Air Clarity Ritual",
                    Description = "A ritual to bring clarity and enhance communication",
                    Relevance = 0.75,
                    Elements = new List<string> { "air" },
                    Chakras = new List<string> { "throat" },
                    Intent = "Clarity and communication",
                    KeySymbols = analysis.ExtractedSymbols.Take(3).ToList(),
                    Difficulty = "beginner",
                    EstimatedDuration = 25
                }
            };
            
            return ritualTemplates.GetValueOrDefault(element, new RitualSuggestion());
        }

        private RitualSuggestion GenerateChakraRitual(string chakra, DreamAnalysis analysis)
        {
            var ritualTemplates = new Dictionary<string, RitualSuggestion>
            {
                ["root"] = new RitualSuggestion
                {
                    Name = "Root Chakra Activation",
                    Description = "A ritual to activate and balance the root chakra",
                    Relevance = 0.7,
                    Elements = new List<string> { "earth" },
                    Chakras = new List<string> { "root" },
                    Intent = "Security and grounding",
                    KeySymbols = analysis.ExtractedSymbols.Take(3).ToList(),
                    Difficulty = "beginner",
                    EstimatedDuration = 30
                },
                ["heart"] = new RitualSuggestion
                {
                    Name = "Heart Chakra Healing",
                    Description = "A ritual to open and heal the heart chakra",
                    Relevance = 0.7,
                    Elements = new List<string> { "water", "air" },
                    Chakras = new List<string> { "heart" },
                    Intent = "Love and compassion",
                    KeySymbols = analysis.ExtractedSymbols.Take(3).ToList(),
                    Difficulty = "intermediate",
                    EstimatedDuration = 45
                }
            };
            
            return ritualTemplates.GetValueOrDefault(chakra, new RitualSuggestion());
        }

        private double CalculateConfidence(DreamAnalysis analysis)
        {
            var confidence = 0.0;
            
            // Base confidence on symbol count
            confidence += Math.Min(analysis.ExtractedSymbols.Count * 0.1, 0.3);
            
            // Add confidence for emotion detection
            confidence += Math.Min(analysis.Emotions.Count * 0.15, 0.3);
            
            // Add confidence for strong affinities
            confidence += analysis.ElementalAffinities.Values.Max() * 0.2;
            confidence += analysis.ChakraAffinities.Values.Max() * 0.2;
            
            return Math.Min(confidence, 1.0);
        }

        private Dictionary<string, double> CalculateAverageAffinities(List<Dictionary<string, double>> affinitiesList)
        {
            var result = new Dictionary<string, double>();
            
            if (affinitiesList.Count == 0) return result;
            
            var keys = affinitiesList[0].Keys;
            foreach (var key in keys)
            {
                var average = affinitiesList.Select(d => d.GetValueOrDefault(key, 0.0)).Average();
                result[key] = average;
            }
            
            return result;
        }

        private string DeterminePatternType(DreamPattern pattern)
        {
            if (pattern.RecurringSymbols.Count >= 3)
                return "Symbolic Recurrence";
            if (pattern.RecurringEmotions.Count >= 2)
                return "Emotional Pattern";
            if (pattern.AverageElementalAffinities.Values.Max() > 0.7)
                return "Elemental Focus";
            if (pattern.AverageChakraAffinities.Values.Max() > 0.7)
                return "Chakra Focus";
            
            return "General Pattern";
        }

        private string GeneratePatternDescription(DreamPattern pattern)
        {
            var descriptions = new List<string>();
            
            if (pattern.RecurringSymbols.Any())
                descriptions.Add($"Recurring symbols: {string.Join(", ", pattern.RecurringSymbols)}");
            
            if (pattern.RecurringEmotions.Any())
                descriptions.Add($"Recurring emotions: {string.Join(", ", pattern.RecurringEmotions)}");
            
            var primaryElement = pattern.AverageElementalAffinities.OrderByDescending(x => x.Value).FirstOrDefault();
            if (primaryElement.Value > 0.5)
                descriptions.Add($"Strong {primaryElement.Key} energy focus");
            
            var primaryChakra = pattern.AverageChakraAffinities.OrderByDescending(x => x.Value).FirstOrDefault();
            if (primaryChakra.Value > 0.5)
                descriptions.Add($"Strong {primaryChakra.Key} chakra focus");
            
            return string.Join("; ", descriptions);
        }

        private Dictionary<string, List<string>> InitializeSymbolDictionary()
        {
            return new Dictionary<string, List<string>>
            {
                ["moon"] = new List<string> { "moon", "lunar", "night", "silver" },
                ["sun"] = new List<string> { "sun", "solar", "day", "gold" },
                ["water"] = new List<string> { "water", "ocean", "sea", "river", "lake", "rain" },
                ["fire"] = new List<string> { "fire", "flame", "burn", "heat", "lightning" },
                ["earth"] = new List<string> { "earth", "ground", "soil", "mountain", "rock" },
                ["air"] = new List<string> { "air", "wind", "sky", "cloud", "bird" },
                ["tree"] = new List<string> { "tree", "forest", "wood", "leaf", "branch" },
                ["animal"] = new List<string> { "animal", "creature", "beast", "wild" }
            };
        }

        private Dictionary<string, List<string>> InitializeEmotionDictionary()
        {
            return new Dictionary<string, List<string>>
            {
                ["fear"] = new List<string> { "fear", "afraid", "scared", "terrified", "anxious" },
                ["joy"] = new List<string> { "joy", "happy", "excited", "elated", "thrilled" },
                ["sadness"] = new List<string> { "sad", "sorrow", "grief", "melancholy", "depressed" },
                ["anger"] = new List<string> { "angry", "furious", "rage", "irritated", "frustrated" },
                ["love"] = new List<string> { "love", "affection", "tenderness", "warmth", "caring" },
                ["surprise"] = new List<string> { "surprised", "shocked", "amazed", "astonished" },
                ["disgust"] = new List<string> { "disgust", "repulsed", "revolted", "sickened" }
            };
        }

        private Dictionary<string, List<string>> InitializeElementalMappings()
        {
            return new Dictionary<string, List<string>>
            {
                ["fire"] = new List<string> { "fire", "flame", "burn", "heat", "lightning", "sun", "red", "orange" },
                ["water"] = new List<string> { "water", "ocean", "sea", "river", "lake", "rain", "blue", "tears" },
                ["earth"] = new List<string> { "earth", "ground", "soil", "mountain", "rock", "tree", "brown", "green" },
                ["air"] = new List<string> { "air", "wind", "sky", "cloud", "bird", "fly", "white", "gray" },
                ["spirit"] = new List<string> { "spirit", "soul", "ghost", "angel", "divine", "sacred", "holy" }
            };
        }

        private Dictionary<string, List<string>> InitializeChakraMappings()
        {
            return new Dictionary<string, List<string>>
            {
                ["root"] = new List<string> { "earth", "ground", "soil", "mountain", "rock", "tree", "red" },
                ["sacral"] = new List<string> { "water", "ocean", "sea", "river", "lake", "orange", "flow" },
                ["solar_plexus"] = new List<string> { "fire", "sun", "light", "yellow", "power", "strength" },
                ["heart"] = new List<string> { "love", "heart", "green", "pink", "compassion", "healing" },
                ["throat"] = new List<string> { "air", "wind", "sky", "blue", "voice", "speech", "communication" },
                ["third_eye"] = new List<string> { "moon", "night", "indigo", "vision", "intuition", "psychic" },
                ["crown"] = new List<string> { "spirit", "soul", "divine", "sacred", "white", "gold", "enlightenment" }
            };
        }
    }
} 