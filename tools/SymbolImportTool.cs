using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using RitualOS.Services;

namespace RitualOS.Tools
{
    public class SymbolImportTool
    {
        private readonly SymbolWikiService _symbolWikiService;
        private readonly SymbolImageService _imageService;

        public SymbolImportTool(SymbolWikiService symbolWikiService, SymbolImageService imageService)
        {
            _symbolWikiService = symbolWikiService;
            _imageService = imageService;
        }

        public async Task ImportFromBookDirectoryAsync(string bookDirectory, string bookName, string category)
        {
            Console.WriteLine($"Starting import from: {bookDirectory}");
            Console.WriteLine($"Book: {bookName}");
            Console.WriteLine($"Category: {category}");

            // Initialize services
            await _symbolWikiService.InitializeAsync();
            await _imageService.InitializeAsync();

            // Import images
            await _imageService.ImportImagesFromDirectoryAsync(bookDirectory, bookName);

            // Create symbols from images
            await CreateSymbolsFromImagesAsync(bookName, category);

            Console.WriteLine("Import completed successfully!");
        }

        private async Task CreateSymbolsFromImagesAsync(string bookName, string category)
        {
            var imageSources = await _imageService.GetImageSourcesAsync();
            var bookImages = imageSources.Where(s => s.Equals(bookName, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var imageSource in bookImages)
            {
                var images = await _imageService.SearchImagesAsync(imageSource);
                
                foreach (var image in images)
                {
                    // Create symbol entry
                    var symbol = new Models.Symbol
                    {
                        Name = image.SymbolName,
                        Category = ParseCategory(category),
                        PowerLevel = Models.SymbolPower.Moderate, // Default, can be updated later
                        Description = $"Symbol from {bookName}",
                        Source = bookName,
                        ImagePath = image.FilePath,
                        SvgData = image.FileType == "svg" ? await File.ReadAllTextAsync(image.FilePath) : "",
                        Tags = new List<string> { bookName.ToLowerInvariant(), category.ToLowerInvariant() },
                        IsVerified = false, // Mark as needing verification
                        DateAdded = DateTime.Now,
                        LastModified = DateTime.Now
                    };

                    // Add to database
                    await _symbolWikiService.AddSymbolAsync(symbol);
                    
                    Console.WriteLine($"Created symbol: {symbol.Name}");
                }
            }
        }

        private Models.SymbolCategory ParseCategory(string category)
        {
            return category.ToLowerInvariant() switch
            {
                "alchemical" => Models.SymbolCategory.Alchemical,
                "astrological" => Models.SymbolCategory.Astrological,
                "celtic" => Models.SymbolCategory.Celtic,
                "chinese" => Models.SymbolCategory.Chinese,
                "egyptian" => Models.SymbolCategory.Egyptian,
                "goetic" => Models.SymbolCategory.Goetic,
                "hermetic" => Models.SymbolCategory.Hermetic,
                "kabbalistic" => Models.SymbolCategory.Kabbalistic,
                "nordic" => Models.SymbolCategory.Nordic,
                "planetary" => Models.SymbolCategory.Planetary,
                "protection" => Models.SymbolCategory.Protection,
                "runic" => Models.SymbolCategory.Runic,
                "sigil" => Models.SymbolCategory.Sigil,
                "tibetan" => Models.SymbolCategory.Tibetan,
                "unified" => Models.SymbolCategory.Unified,
                "wiccan" => Models.SymbolCategory.Wiccan,
                _ => Models.SymbolCategory.Other
            };
        }

        public async Task ConvertPngToSvgAsync(string pngDirectory, string outputDirectory)
        {
            Console.WriteLine($"Converting PNG files from: {pngDirectory}");
            Console.WriteLine($"Output directory: {outputDirectory}");

            Directory.CreateDirectory(outputDirectory);

            var pngFiles = Directory.GetFiles(pngDirectory, "*.png", SearchOption.AllDirectories);
            
            foreach (var pngFile in pngFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(pngFile);
                var outputPath = Path.Combine(outputDirectory, $"{fileName}.svg");
                
                // Convert PNG to SVG (this would use a proper conversion library)
                await ConvertSinglePngToSvgAsync(pngFile, outputPath);
                
                Console.WriteLine($"Converted: {fileName}");
            }
        }

        private async Task ConvertSinglePngToSvgAsync(string pngPath, string svgPath)
        {
            // This is a placeholder for PNG to SVG conversion
            // In a real implementation, you'd use a library like:
            // - ImageMagick
            // - Potrace
            // - Vector Magic API
            // - Custom OCR + vectorization

            // For now, create a simple SVG wrapper
            var svgContent = $@"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 200 200"">
  <image href=""{pngPath}"" width=""200"" height=""200""/>
  <text x=""100"" y=""190"" text-anchor=""middle"" font-family=""Arial"" font-size=""10"" fill=""#666"">
    Converted from PNG
  </text>
</svg>";

            await File.WriteAllTextAsync(svgPath, svgContent);
        }

        public async Task GenerateSymbolMetadataAsync(string symbolName, string description, string category, string tags)
        {
            var symbol = new Models.Symbol
            {
                Name = symbolName,
                Category = ParseCategory(category),
                PowerLevel = Models.SymbolPower.Moderate,
                Description = description,
                Tags = tags.Split(',').Select(t => t.Trim()).ToList(),
                IsVerified = false,
                DateAdded = DateTime.Now,
                LastModified = DateTime.Now
            };

            await _symbolWikiService.AddSymbolAsync(symbol);
            Console.WriteLine($"Created symbol metadata: {symbolName}");
        }

        public async Task ExportSymbolDatabaseAsync(string exportPath, string? category = null)
        {
            await _symbolWikiService.ExportSymbolsToJsonAsync(exportPath, category != null ? ParseCategory(category) : null);
            Console.WriteLine($"Exported symbols to: {exportPath}");
        }

        public async Task ImportSymbolDatabaseAsync(string importPath)
        {
            var jsonContent = await File.ReadAllTextAsync(importPath);
            await _symbolWikiService.ImportSymbolsFromJsonAsync(jsonContent);
            Console.WriteLine($"Imported symbols from: {importPath}");
        }

        public async Task ListAllSymbolsAsync()
        {
            var symbols = _symbolWikiService.GetAllSymbols();
            
            Console.WriteLine($"Total symbols: {symbols.Count}");
            Console.WriteLine();
            
            foreach (var category in Enum.GetValues<Models.SymbolCategory>())
            {
                var categorySymbols = symbols.Where(s => s.Category == category).ToList();
                if (categorySymbols.Any())
                {
                    Console.WriteLine($"{category}: {categorySymbols.Count} symbols");
                    foreach (var symbol in categorySymbols.Take(5)) // Show first 5
                    {
                        Console.WriteLine($"  - {symbol.Name}");
                    }
                    if (categorySymbols.Count > 5)
                    {
                        Console.WriteLine($"  ... and {categorySymbols.Count - 5} more");
                    }
                    Console.WriteLine();
                }
            }
        }

        public async Task SearchSymbolsAsync(string searchTerm)
        {
            var results = _symbolWikiService.SearchSymbols(searchTerm);
            
            Console.WriteLine($"Search results for '{searchTerm}': {results.Count} symbols");
            Console.WriteLine();
            
            foreach (var symbol in results)
            {
                Console.WriteLine($"{symbol.Name} ({symbol.Category})");
                Console.WriteLine($"  {symbol.Description}");
                Console.WriteLine($"  Tags: {string.Join(", ", symbol.Tags)}");
                Console.WriteLine();
            }
        }

        public async Task GetStorageStatsAsync()
        {
            var totalSize = await _imageService.GetTotalStorageSizeAsync();
            var symbols = _symbolWikiService.GetAllSymbols();
            var imageSources = await _imageService.GetImageSourcesAsync();
            
            Console.WriteLine("Storage Statistics:");
            Console.WriteLine($"Total symbols: {symbols.Count}");
            Console.WriteLine($"Total image size: {totalSize / 1024.0:F1} KB");
            Console.WriteLine($"Image sources: {imageSources.Count}");
            Console.WriteLine();
            
            foreach (var source in imageSources)
            {
                var sourceImages = await _imageService.SearchImagesAsync(source);
                var sourceSize = sourceImages.Sum(i => i.FileSize);
                Console.WriteLine($"{source}: {sourceImages.Count} images, {sourceSize / 1024.0:F1} KB");
            }
        }
    }
} 