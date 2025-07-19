using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace RitualOS.Services
{
    public class SymbolImageService
    {
        private const string SymbolImagesDirectory = "symbol_images";
        private const string SvgDirectory = "svg";
        private const string PngDirectory = "png";
        private const string ThumbnailDirectory = "thumbnails";
        private const string ImageMetadataFile = "image_metadata.json";

        public class ImageMetadata
        {
            public string SymbolName { get; set; } = string.Empty;
            public string FileName { get; set; } = string.Empty;
            public string FilePath { get; set; } = string.Empty;
            public string FileType { get; set; } = string.Empty; // "svg", "png"
            public long FileSize { get; set; }
            public DateTime DateAdded { get; set; }
            public string Source { get; set; } = string.Empty; // Book name, page number, etc.
            public string Description { get; set; } = string.Empty;
            public bool IsPrimary { get; set; } = false; // Primary image for the symbol
            public int Width { get; set; }
            public int Height { get; set; }
            public string ColorScheme { get; set; } = string.Empty; // "monochrome", "colored", "gold", etc.
            public string Style { get; set; } = string.Empty; // "traditional", "modern", "artistic", etc.
        }

        public async Task InitializeAsync()
        {
            // Create directory structure
            Directory.CreateDirectory(SymbolImagesDirectory);
            Directory.CreateDirectory(Path.Combine(SymbolImagesDirectory, SvgDirectory));
            Directory.CreateDirectory(Path.Combine(SymbolImagesDirectory, PngDirectory));
            Directory.CreateDirectory(Path.Combine(SymbolImagesDirectory, ThumbnailDirectory));
        }

        public async Task<string> SaveSvgAsync(string symbolName, string svgContent, string source, string description = "")
        {
            var fileName = $"{symbolName}_{DateTime.Now:yyyyMMdd_HHmmss}.svg";
            var filePath = Path.Combine(SymbolImagesDirectory, SvgDirectory, fileName);
            
            // Clean the SVG content and ensure it's valid
            var cleanedSvg = CleanSvgContent(svgContent);
            
            await File.WriteAllTextAsync(filePath, cleanedSvg);
            
            var metadata = new ImageMetadata
            {
                SymbolName = symbolName,
                FileName = fileName,
                FilePath = filePath,
                FileType = "svg",
                FileSize = new FileInfo(filePath).Length,
                DateAdded = DateTime.Now,
                Source = source,
                Description = description,
                IsPrimary = !await HasPrimaryImageAsync(symbolName)
            };

            await SaveImageMetadataAsync(metadata);
            
            return filePath;
        }

        public async Task<string> SavePngAsync(string symbolName, byte[] pngData, string source, string description = "")
        {
            var fileName = $"{symbolName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var filePath = Path.Combine(SymbolImagesDirectory, PngDirectory, fileName);
            
            await File.WriteAllBytesAsync(filePath, pngData);
            
            var metadata = new ImageMetadata
            {
                SymbolName = symbolName,
                FileName = fileName,
                FilePath = filePath,
                FileType = "png",
                FileSize = pngData.Length,
                DateAdded = DateTime.Now,
                Source = source,
                Description = description,
                IsPrimary = !await HasPrimaryImageAsync(symbolName)
            };

            await SaveImageMetadataAsync(metadata);
            
            return filePath;
        }

        public async Task<string> ConvertPngToSvgAsync(string pngFilePath, string symbolName, string source)
        {
            // This would integrate with a PNG to SVG conversion service
            // For now, we'll create a placeholder SVG
            var svgContent = CreatePlaceholderSvg(symbolName);
            return await SaveSvgAsync(symbolName, svgContent, source, "Converted from PNG");
        }

        public async Task<List<ImageMetadata>> GetSymbolImagesAsync(string symbolName)
        {
            var metadataList = await LoadAllImageMetadataAsync();
            return metadataList.Where(m => m.SymbolName.Equals(symbolName, StringComparison.OrdinalIgnoreCase))
                              .OrderByDescending(m => m.IsPrimary)
                              .ThenBy(m => m.DateAdded)
                              .ToList();
        }

        public async Task<ImageMetadata?> GetPrimaryImageAsync(string symbolName)
        {
            var images = await GetSymbolImagesAsync(symbolName);
            return images.FirstOrDefault(i => i.IsPrimary) ?? images.FirstOrDefault();
        }

        public async Task<string?> GetSvgContentAsync(string symbolName)
        {
            var primaryImage = await GetPrimaryImageAsync(symbolName);
            if (primaryImage?.FileType == "svg" && File.Exists(primaryImage.FilePath))
            {
                return await File.ReadAllTextAsync(primaryImage.FilePath);
            }
            return null;
        }

        public async Task SetPrimaryImageAsync(string symbolName, string fileName)
        {
            var metadataList = await LoadAllImageMetadataAsync();
            
            // Remove primary flag from all images for this symbol
            foreach (var metadata in metadataList.Where(m => m.SymbolName.Equals(symbolName, StringComparison.OrdinalIgnoreCase)))
            {
                metadata.IsPrimary = false;
            }
            
            // Set the specified image as primary
            var targetImage = metadataList.FirstOrDefault(m => m.FileName == fileName);
            if (targetImage != null)
            {
                targetImage.IsPrimary = true;
            }
            
            await SaveAllImageMetadataAsync(metadataList);
        }

        public async Task DeleteImageAsync(string fileName)
        {
            var metadataList = await LoadAllImageMetadataAsync();
            var imageToDelete = metadataList.FirstOrDefault(m => m.FileName == fileName);
            
            if (imageToDelete != null)
            {
                // Delete the file
                if (File.Exists(imageToDelete.FilePath))
                {
                    File.Delete(imageToDelete.FilePath);
                }
                
                // Remove from metadata
                metadataList.Remove(imageToDelete);
                await SaveAllImageMetadataAsync(metadataList);
            }
        }

        public async Task<List<ImageMetadata>> SearchImagesAsync(string searchTerm)
        {
            var metadataList = await LoadAllImageMetadataAsync();
            return metadataList.Where(m => 
                m.SymbolName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.Source.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        public async Task<long> GetTotalStorageSizeAsync()
        {
            var metadataList = await LoadAllImageMetadataAsync();
            return metadataList.Sum(m => m.FileSize);
        }

        public async Task<List<string>> GetImageSourcesAsync()
        {
            var metadataList = await LoadAllImageMetadataAsync();
            return metadataList.Select(m => m.Source)
                              .Distinct()
                              .OrderBy(s => s)
                              .ToList();
        }

        private string CleanSvgContent(string svgContent)
        {
            // Basic SVG cleaning - remove unnecessary whitespace and comments
            var cleaned = svgContent.Trim();
            
            // Ensure it starts with <?xml or <svg
            if (!cleaned.StartsWith("<?xml") && !cleaned.StartsWith("<svg"))
            {
                // Add basic SVG wrapper if needed
                cleaned = $@"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 100 100"">{cleaned}</svg>";
            }
            
            return cleaned;
        }

        private string CreatePlaceholderSvg(string symbolName)
        {
            return $@"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 100 100"">
  <rect width=""100"" height=""100"" fill=""none"" stroke=""#666"" stroke-width=""1""/>
  <text x=""50"" y=""50"" text-anchor=""middle"" dominant-baseline=""middle"" font-family=""Arial"" font-size=""8"" fill=""#666"">
    {symbolName}
  </text>
</svg>";
        }

        private async Task<bool> HasPrimaryImageAsync(string symbolName)
        {
            var metadataList = await LoadAllImageMetadataAsync();
            return metadataList.Any(m => m.SymbolName.Equals(symbolName, StringComparison.OrdinalIgnoreCase) && m.IsPrimary);
        }

        private async Task SaveImageMetadataAsync(ImageMetadata metadata)
        {
            var metadataList = await LoadAllImageMetadataAsync();
            
            // Remove existing metadata for the same file if it exists
            metadataList.RemoveAll(m => m.FileName == metadata.FileName);
            
            // Add new metadata
            metadataList.Add(metadata);
            
            await SaveAllImageMetadataAsync(metadataList);
        }

        private async Task<List<ImageMetadata>> LoadAllImageMetadataAsync()
        {
            var metadataPath = Path.Combine(SymbolImagesDirectory, ImageMetadataFile);
            
            if (!File.Exists(metadataPath))
            {
                return new List<ImageMetadata>();
            }
            
            try
            {
                var json = await File.ReadAllTextAsync(metadataPath);
                return JsonSerializer.Deserialize<List<ImageMetadata>>(json) ?? new List<ImageMetadata>();
            }
            catch
            {
                return new List<ImageMetadata>();
            }
        }

        private async Task SaveAllImageMetadataAsync(List<ImageMetadata> metadataList)
        {
            var metadataPath = Path.Combine(SymbolImagesDirectory, ImageMetadataFile);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(metadataList, options);
            await File.WriteAllTextAsync(metadataPath, json);
        }

        // Utility methods for batch operations
        public async Task ImportImagesFromDirectoryAsync(string sourceDirectory, string sourceName)
        {
            var svgFiles = Directory.GetFiles(sourceDirectory, "*.svg", SearchOption.AllDirectories);
            var pngFiles = Directory.GetFiles(sourceDirectory, "*.png", SearchOption.AllDirectories);
            
            foreach (var svgFile in svgFiles)
            {
                var symbolName = Path.GetFileNameWithoutExtension(svgFile);
                var svgContent = await File.ReadAllTextAsync(svgFile);
                await SaveSvgAsync(symbolName, svgContent, sourceName, $"Imported from {sourceName}");
            }
            
            foreach (var pngFile in pngFiles)
            {
                var symbolName = Path.GetFileNameWithoutExtension(pngFile);
                var pngData = await File.ReadAllBytesAsync(pngFile);
                await SavePngAsync(symbolName, pngData, sourceName, $"Imported from {sourceName}");
            }
        }

        public async Task ExportImagesAsync(string exportDirectory, string symbolName = "")
        {
            Directory.CreateDirectory(exportDirectory);
            
            var metadataList = await LoadAllImageMetadataAsync();
            var imagesToExport = string.IsNullOrEmpty(symbolName) 
                ? metadataList 
                : metadataList.Where(m => m.SymbolName.Equals(symbolName, StringComparison.OrdinalIgnoreCase));
            
            foreach (var metadata in imagesToExport)
            {
                if (File.Exists(metadata.FilePath))
                {
                    var exportPath = Path.Combine(exportDirectory, metadata.FileName);
                    File.Copy(metadata.FilePath, exportPath, true);
                }
            }
        }
    }
} 