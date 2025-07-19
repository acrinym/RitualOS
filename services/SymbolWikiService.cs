using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RitualOS.Models;

namespace RitualOS.Services
{
    public class SymbolWikiService
    {
        private const string SymbolsDirectory = "symbols";
        private const string SymbolsDatabaseFile = "symbols_database.json";
        private const string GoeticSpiritsFile = "goetic_spirits.json";
        private const string SymbolImagesDirectory = "symbol_images";
        
        private List<Symbol> _allSymbols = new();
        private List<Symbol> _goeticSpirits = new();
        private bool _isInitialized = false;

        public event EventHandler? SymbolsLoaded;
        public event EventHandler? SymbolAdded;
        public event EventHandler? SymbolUpdated;

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            // Ensure directories exist
            Directory.CreateDirectory(SymbolsDirectory);
            Directory.CreateDirectory(SymbolImagesDirectory);

            // Load existing symbols
            await LoadSymbolsAsync();
            
            // Load Goetic spirits if not already loaded
            if (!_goeticSpirits.Any())
            {
                await LoadGoeticSpiritsAsync();
            }

            _isInitialized = true;
            SymbolsLoaded?.Invoke(this, EventArgs.Empty);
        }

        public async Task LoadSymbolsAsync()
        {
            var databasePath = Path.Combine(SymbolsDirectory, SymbolsDatabaseFile);
            
            if (File.Exists(databasePath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(databasePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true
                    };
                    options.Converters.Add(new JsonStringEnumConverter());
                    
                    _allSymbols = JsonSerializer.Deserialize<List<Symbol>>(json, options) ?? new List<Symbol>();
                }
                catch (Exception)
                {
                    // Log error and start with empty database
                    _allSymbols = new List<Symbol>();
                }
            }
            else
            {
                // Load initial symbols if database doesn't exist
                await LoadInitialSymbolsAsync();
            }
        }

        private async Task LoadInitialSymbolsAsync()
        {
            var initialSymbolsPath = Path.Combine(SymbolsDirectory, "initial_symbols.json");
            
            if (File.Exists(initialSymbolsPath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(initialSymbolsPath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true
                    };
                    options.Converters.Add(new JsonStringEnumConverter());
                    
                    _allSymbols = JsonSerializer.Deserialize<List<Symbol>>(json, options) ?? new List<Symbol>();
                    await SaveSymbolsAsync(); // Save to the main database
                }
                catch (Exception)
                {
                    _allSymbols = new List<Symbol>();
                }
            }
        }

        public async Task SaveSymbolsAsync()
        {
            var databasePath = Path.Combine(SymbolsDirectory, SymbolsDatabaseFile);
            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new JsonStringEnumConverter());
            
            var json = JsonSerializer.Serialize(_allSymbols, options);
            await File.WriteAllTextAsync(databasePath, json);
        }

        public async Task LoadGoeticSpiritsAsync()
        {
            var goeticPath = Path.Combine(SymbolsDirectory, GoeticSpiritsFile);
            
            if (File.Exists(goeticPath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(goeticPath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true
                    };
                    options.Converters.Add(new JsonStringEnumConverter());
                    
                    _goeticSpirits = JsonSerializer.Deserialize<List<Symbol>>(json, options) ?? new List<Symbol>();
                }
                catch (Exception)
                {
                    _goeticSpirits = new List<Symbol>();
                }
            }
            else
            {
                // Create initial Goetic spirits database
                await CreateInitialGoeticDatabaseAsync();
            }
        }

        public async Task CreateInitialGoeticDatabaseAsync()
        {
            _goeticSpirits = new List<Symbol>
            {
                new Symbol
                {
                    Name = "Bael",
                    Category = SymbolCategory.Goetic,
                    PowerLevel = SymbolPower.Major,
                    Description = "The first spirit of the Goetia, appears as a cat, toad, or man, or a combination of these forms.",
                    GoeticRank = 1,
                    GoeticLegion = "First",
                    GoeticAppearance = "Appears as a cat, toad, or man, or a combination of these forms",
                    GoeticPowers = "Makes men invisible, grants wisdom, and can cause love between man and woman",
                    Tags = new List<string> { "invisibility", "wisdom", "love", "goetic", "demon" },
                    AssociatedEntities = new List<string> { "Bael" },
                    PlanetaryCorrespondences = new List<string> { "Sun" },
                    ElementalCorrespondences = new List<string> { "Fire" },
                    Source = "The Lesser Key of Solomon",
                    IsVerified = true
                },
                new Symbol
                {
                    Name = "Agares",
                    Category = SymbolCategory.Goetic,
                    PowerLevel = SymbolPower.Major,
                    Description = "The second spirit, appears as a fair old man riding a crocodile with a hawk on his fist.",
                    GoeticRank = 2,
                    GoeticLegion = "First",
                    GoeticAppearance = "Fair old man riding a crocodile with a hawk on his fist",
                    GoeticPowers = "Teaches languages, causes earthquakes, returns runaways",
                    Tags = new List<string> { "languages", "earthquakes", "runaways", "goetic", "demon" },
                    AssociatedEntities = new List<string> { "Agares" },
                    PlanetaryCorrespondences = new List<string> { "Venus" },
                    ElementalCorrespondences = new List<string> { "Earth" },
                    Source = "The Lesser Key of Solomon",
                    IsVerified = true
                },
                new Symbol
                {
                    Name = "Vassago",
                    Category = SymbolCategory.Goetic,
                    PowerLevel = SymbolPower.Moderate,
                    Description = "The third spirit, appears as a beautiful angel riding a crocodile.",
                    GoeticRank = 3,
                    GoeticLegion = "First",
                    GoeticAppearance = "Beautiful angel riding a crocodile",
                    GoeticPowers = "Discovers hidden things, foretells the future, finds lost objects",
                    Tags = new List<string> { "divination", "future", "hidden", "lost", "goetic", "demon" },
                    AssociatedEntities = new List<string> { "Vassago" },
                    PlanetaryCorrespondences = new List<string> { "Mercury" },
                    ElementalCorrespondences = new List<string> { "Air" },
                    Source = "The Lesser Key of Solomon",
                    IsVerified = true
                }
            };

            await SaveGoeticSpiritsAsync();
        }

        public async Task SaveGoeticSpiritsAsync()
        {
            var goeticPath = Path.Combine(SymbolsDirectory, GoeticSpiritsFile);
            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new JsonStringEnumConverter());
            
            var json = JsonSerializer.Serialize(_goeticSpirits, options);
            await File.WriteAllTextAsync(goeticPath, json);
        }

        public List<Symbol> GetAllSymbols()
        {
            return _allSymbols.Concat(_goeticSpirits).ToList();
        }

        public List<Symbol> GetSymbolsByCategory(SymbolCategory category)
        {
            return GetAllSymbols().Where(s => s.Category == category).ToList();
        }

        public List<Symbol> GetGoeticSpirits()
        {
            return _goeticSpirits.ToList();
        }

        public List<Symbol> SearchSymbols(string searchTerm, SymbolCategory? category = null, SymbolPower? minPower = null)
        {
            var query = GetAllSymbols().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLowerInvariant();
                query = query.Where(s => 
                    s.Name.ToLowerInvariant().Contains(searchTerm) ||
                    s.AlternativeNames.Any(name => name.ToLowerInvariant().Contains(searchTerm)) ||
                    s.Description.ToLowerInvariant().Contains(searchTerm) ||
                    s.Tags.Any(tag => tag.ToLowerInvariant().Contains(searchTerm)) ||
                    s.AssociatedEntities.Any(entity => entity.ToLowerInvariant().Contains(searchTerm))
                );
            }

            if (category.HasValue)
            {
                query = query.Where(s => s.Category == category.Value);
            }

            if (minPower.HasValue)
            {
                query = query.Where(s => s.PowerLevel >= minPower.Value);
            }

            return query.OrderBy(s => s.Name).ToList();
        }

        public List<Symbol> GetSymbolsByTags(List<string> tags)
        {
            return GetAllSymbols()
                .Where(s => tags.Any(tag => s.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)))
                .OrderBy(s => s.Name)
                .ToList();
        }

        public List<Symbol> GetSymbolsByPlanet(string planet)
        {
            return GetAllSymbols()
                .Where(s => s.PlanetaryCorrespondences.Contains(planet, StringComparer.OrdinalIgnoreCase))
                .OrderBy(s => s.Name)
                .ToList();
        }

        public List<Symbol> GetSymbolsByElement(string element)
        {
            return GetAllSymbols()
                .Where(s => s.ElementalCorrespondences.Contains(element, StringComparer.OrdinalIgnoreCase))
                .OrderBy(s => s.Name)
                .ToList();
        }

        public async Task AddSymbolAsync(Symbol symbol)
        {
            symbol.DateAdded = DateTime.Now;
            symbol.LastModified = DateTime.Now;

            if (symbol.Category == SymbolCategory.Goetic)
            {
                _goeticSpirits.Add(symbol);
                await SaveGoeticSpiritsAsync();
            }
            else
            {
                _allSymbols.Add(symbol);
                await SaveSymbolsAsync();
            }

            SymbolAdded?.Invoke(this, EventArgs.Empty);
        }

        public async Task UpdateSymbolAsync(Symbol symbol)
        {
            symbol.LastModified = DateTime.Now;

            if (symbol.Category == SymbolCategory.Goetic)
            {
                var existingIndex = _goeticSpirits.FindIndex(s => s.Name == symbol.Name);
                if (existingIndex >= 0)
                {
                    _goeticSpirits[existingIndex] = symbol;
                    await SaveGoeticSpiritsAsync();
                }
            }
            else
            {
                var existingIndex = _allSymbols.FindIndex(s => s.Name == symbol.Name);
                if (existingIndex >= 0)
                {
                    _allSymbols[existingIndex] = symbol;
                    await SaveSymbolsAsync();
                }
            }

            SymbolUpdated?.Invoke(this, EventArgs.Empty);
        }

        public async Task DeleteSymbolAsync(string symbolName, SymbolCategory category)
        {
            if (category == SymbolCategory.Goetic)
            {
                _goeticSpirits.RemoveAll(s => s.Name == symbolName);
                await SaveGoeticSpiritsAsync();
            }
            else
            {
                _allSymbols.RemoveAll(s => s.Name == symbolName);
                await SaveSymbolsAsync();
            }
        }

        public List<string> GetAllTags()
        {
            return GetAllSymbols()
                .SelectMany(s => s.Tags)
                .Distinct()
                .OrderBy(tag => tag)
                .ToList();
        }

        public List<string> GetAllPlanets()
        {
            return GetAllSymbols()
                .SelectMany(s => s.PlanetaryCorrespondences)
                .Distinct()
                .OrderBy(planet => planet)
                .ToList();
        }

        public List<string> GetAllElements()
        {
            return GetAllSymbols()
                .SelectMany(s => s.ElementalCorrespondences)
                .Distinct()
                .OrderBy(element => element)
                .ToList();
        }

        public async Task ImportSymbolsFromJsonAsync(string jsonContent)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new JsonStringEnumConverter());
                
                var importedSymbols = JsonSerializer.Deserialize<List<Symbol>>(jsonContent, options);
                
                if (importedSymbols != null)
                {
                    foreach (var symbol in importedSymbols)
                    {
                        await AddSymbolAsync(symbol);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to import symbols from JSON", ex);
            }
        }

        public async Task ExportSymbolsToJsonAsync(string filePath, SymbolCategory? category = null)
        {
            var symbolsToExport = category.HasValue 
                ? GetSymbolsByCategory(category.Value) 
                : GetAllSymbols();

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new JsonStringEnumConverter());
            
            var json = JsonSerializer.Serialize(symbolsToExport, options);
            await File.WriteAllTextAsync(filePath, json);
        }
    }
} 