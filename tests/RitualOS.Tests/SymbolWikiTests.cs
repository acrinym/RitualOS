using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RitualOS.Models;
using RitualOS.Services;
using Xunit;
using System.Collections.Generic;

namespace RitualOS.Tests
{
    public class SymbolWikiTests : IDisposable
    {
        private readonly string _testSymbolsDirectory;
        private readonly SymbolWikiService _symbolWikiService;

        public SymbolWikiTests()
        {
            _testSymbolsDirectory = Path.Combine(Path.GetTempPath(), "RitualOS_SymbolWiki_Test");
            _symbolWikiService = new SymbolWikiService();
        }

        [Fact]
        public async Task InitializeAsync_ShouldCreateDirectories()
        {
            // Act
            await _symbolWikiService.InitializeAsync();

            // Assert
            Assert.True(Directory.Exists(Path.Combine(_testSymbolsDirectory, "symbols")));
            Assert.True(Directory.Exists(Path.Combine(_testSymbolsDirectory, "symbol_images")));
        }

        [Fact]
        public async Task AddSymbolAsync_ShouldAddSymbolToDatabase()
        {
            // Arrange
            await _symbolWikiService.InitializeAsync();
            var symbol = new Symbol
            {
                Name = "Test Symbol",
                Category = SymbolCategory.Unified,
                PowerLevel = SymbolPower.Moderate,
                Description = "A test symbol for unit testing",
                Tags = new List<string> { "test", "unified" }
            };

            // Act
            await _symbolWikiService.AddSymbolAsync(symbol);

            // Assert
            var allSymbols = _symbolWikiService.GetAllSymbols();
            Assert.Contains(allSymbols, s => s.Name == "Test Symbol");
        }

        [Fact]
        public async Task SearchSymbols_ShouldReturnMatchingSymbols()
        {
            // Arrange
            await _symbolWikiService.InitializeAsync();
            var symbol = new Symbol
            {
                Name = "Search Test Symbol",
                Category = SymbolCategory.Egyptian,
                PowerLevel = SymbolPower.Major,
                Description = "A symbol for search testing",
                Tags = new List<string> { "search", "egyptian" }
            };
            await _symbolWikiService.AddSymbolAsync(symbol);

            // Act
            var results = _symbolWikiService.SearchSymbols("Search Test");

            // Assert
            Assert.Single(results);
            Assert.Equal("Search Test Symbol", results.First().Name);
        }

        [Fact]
        public async Task GetSymbolsByCategory_ShouldReturnCorrectSymbols()
        {
            // Arrange
            await _symbolWikiService.InitializeAsync();
            var symbol1 = new Symbol
            {
                Name = "Egyptian Symbol",
                Category = SymbolCategory.Egyptian,
                PowerLevel = SymbolPower.Major
            };
            var symbol2 = new Symbol
            {
                Name = "Celtic Symbol",
                Category = SymbolCategory.Celtic,
                PowerLevel = SymbolPower.Moderate
            };
            await _symbolWikiService.AddSymbolAsync(symbol1);
            await _symbolWikiService.AddSymbolAsync(symbol2);

            // Act
            var egyptianSymbols = _symbolWikiService.GetSymbolsByCategory(SymbolCategory.Egyptian);

            // Assert
            Assert.Single(egyptianSymbols);
            Assert.Equal("Egyptian Symbol", egyptianSymbols.First().Name);
        }

        [Fact]
        public async Task GetGoeticSpirits_ShouldReturnGoeticSpirits()
        {
            // Arrange
            await _symbolWikiService.InitializeAsync();

            // Act
            var goeticSpirits = _symbolWikiService.GetGoeticSpirits();

            // Assert
            Assert.NotEmpty(goeticSpirits);
            Assert.All(goeticSpirits, spirit => Assert.Equal(SymbolCategory.Goetic, spirit.Category));
        }

        [Fact]
        public async Task UpdateSymbolAsync_ShouldUpdateExistingSymbol()
        {
            // Arrange
            await _symbolWikiService.InitializeAsync();
            var symbol = new Symbol
            {
                Name = "Update Test Symbol",
                Category = SymbolCategory.Unified,
                PowerLevel = SymbolPower.Minor,
                Description = "Original description"
            };
            await _symbolWikiService.AddSymbolAsync(symbol);

            // Act
            symbol.Description = "Updated description";
            symbol.PowerLevel = SymbolPower.Major;
            await _symbolWikiService.UpdateSymbolAsync(symbol);

            // Assert
            var updatedSymbol = _symbolWikiService.GetAllSymbols().First(s => s.Name == "Update Test Symbol");
            Assert.Equal("Updated description", updatedSymbol.Description);
            Assert.Equal(SymbolPower.Major, updatedSymbol.PowerLevel);
        }

        [Fact]
        public async Task GetAllTags_ShouldReturnUniqueTags()
        {
            // Arrange
            await _symbolWikiService.InitializeAsync();
            var symbol1 = new Symbol
            {
                Name = "Symbol 1",
                Category = SymbolCategory.Unified,
                Tags = new List<string> { "tag1", "tag2" }
            };
            var symbol2 = new Symbol
            {
                Name = "Symbol 2",
                Category = SymbolCategory.Egyptian,
                Tags = new List<string> { "tag2", "tag3" }
            };
            await _symbolWikiService.AddSymbolAsync(symbol1);
            await _symbolWikiService.AddSymbolAsync(symbol2);

            // Act
            var allTags = _symbolWikiService.GetAllTags();

            // Assert
            Assert.Contains("tag1", allTags);
            Assert.Contains("tag2", allTags);
            Assert.Contains("tag3", allTags);
            Assert.Equal(3, allTags.Count);
        }

        public void Dispose()
        {
            // Clean up test directories
            if (Directory.Exists(_testSymbolsDirectory))
            {
                Directory.Delete(_testSymbolsDirectory, true);
            }
        }
    }
} 