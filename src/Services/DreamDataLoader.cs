using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.Services
{
    public static class DreamDataLoader
    {
        public static DreamEntry LoadDreamFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new RitualDataLoadException($"File not found: {filePath}");
            }

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<DreamEntry>(json) ?? throw new RitualDataLoadException("Failed to deserialize dream data.");
            }
            catch (Exception ex) when (ex is IOException || ex is JsonException)
            {
                throw new RitualDataLoadException("Failed to deserialize dream data.", ex);
            }
        }

        public static void SaveDreamToJson(DreamEntry entry, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            var json = JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public static List<DreamEntry> LoadAllDreams(string directory)
        {
            var results = new List<DreamEntry>();
            if (!Directory.Exists(directory))
            {
                return results;
            }

            foreach (var file in Directory.GetFiles(directory, "*.json"))
            {
                try
                {
                    var dream = LoadDreamFromJson(file);
                    results.Add(dream);
                }
                catch (RitualDataLoadException)
                {
                    // Ignore invalid files
                }
            }
            return results;
        }
    }
}
