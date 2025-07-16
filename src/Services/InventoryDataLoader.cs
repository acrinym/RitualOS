using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.Services
{
    public static class InventoryDataLoader
    {
        public static InventoryItem LoadItemFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new RitualDataLoadException($"File not found: {filePath}");
            }

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<InventoryItem>(json) ?? throw new RitualDataLoadException("Failed to deserialize inventory item.");
            }
            catch (Exception ex) when (ex is IOException || ex is JsonException)
            {
                throw new RitualDataLoadException("Failed to deserialize inventory item.", ex);
            }
        }

        public static void SaveItemToJson(InventoryItem item, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            var json = JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public static List<InventoryItem> LoadAllItems(string directory)
        {
            var results = new List<InventoryItem>();
            if (!Directory.Exists(directory))
            {
                return results;
            }

            foreach (var file in Directory.GetFiles(directory, "*.json"))
            {
                try
                {
                    var item = LoadItemFromJson(file);
                    results.Add(item);
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