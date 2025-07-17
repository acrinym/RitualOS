using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Minimal export service used for tests and demos.
    /// </summary>
    public interface IExportService
    {
        Task<string> ExportToMarkdownAsync(RitualTemplate ritual);
        Task<string> ExportToHtmlAsync(RitualTemplate ritual);
        Task<string> ExportToJsonAsync(RitualTemplate ritual);
        Task ExportRitualLibraryAsync(IEnumerable<RitualTemplate> rituals, string format, string outputPath);
    }

    /// <summary>
    /// Basic implementation that serializes templates to JSON or simple text.
    /// </summary>
    public class ExportService : IExportService
    {
        private readonly IUserSettingsService _settings;

        public ExportService(IUserSettingsService settings)
        {
            _settings = settings;
        }

        public Task<string> ExportToMarkdownAsync(RitualTemplate ritual)
        {
            // For now just return pretty JSON as a placeholder.
            return ExportToJsonAsync(ritual);
        }

        public Task<string> ExportToHtmlAsync(RitualTemplate ritual)
        {
            return ExportToJsonAsync(ritual);
        }

        public Task<string> ExportToJsonAsync(RitualTemplate ritual)
        {
            var json = JsonSerializer.Serialize(ritual, new JsonSerializerOptions { WriteIndented = true });
            return Task.FromResult(json);
        }

        public async Task ExportRitualLibraryAsync(IEnumerable<RitualTemplate> rituals, string format, string outputPath)
        {
            var content = JsonSerializer.Serialize(rituals, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(outputPath, content);
            _settings.Save();
        }
    }
}
