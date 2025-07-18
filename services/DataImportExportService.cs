using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using RitualOS.Models;

namespace RitualOS.Services
{
    public interface IDataImportExportService
    {
        Task ImportRitualLogsAsync(string path);
        Task ExportAllDataAsync(string outputPath);
    }

    public class DataImportExportService : IDataImportExportService
    {
        private readonly string _ritualDir;
        private readonly string _inventoryPath;

        public DataImportExportService()
        {
            _ritualDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rituals");
            _inventoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventory.json");
        }

        public async Task ImportRitualLogsAsync(string path)
        {
            if (!File.Exists(path)) return;

            var json = await File.ReadAllTextAsync(path);
            var entries = JsonSerializer.Deserialize<List<RitualEntry>>(json) ?? new();

            if (!Directory.Exists(_ritualDir))
                Directory.CreateDirectory(_ritualDir);

            foreach (var entry in entries)
            {
                var fileName = $"{entry.Id}.json";
                var filePath = Path.Combine(_ritualDir, fileName);
                await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        public async Task ExportAllDataAsync(string outputPath)
        {
            var bundle = new
            {
                rituals = await LoadRitualsAsync(),
                inventory = File.Exists(_inventoryPath) ? await File.ReadAllTextAsync(_inventoryPath) : null
            };

            var json = JsonSerializer.Serialize(bundle, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(outputPath, json);
        }

        private async Task<List<RitualEntry>> LoadRitualsAsync()
        {
            var list = new List<RitualEntry>();
            if (!Directory.Exists(_ritualDir)) return list;

            foreach (var file in Directory.GetFiles(_ritualDir, "*.json"))
            {
                var text = await File.ReadAllTextAsync(file);
                var entry = JsonSerializer.Deserialize<RitualEntry>(text);
                if (entry != null) list.Add(entry);
            }

            return list;
        }
    }
}
