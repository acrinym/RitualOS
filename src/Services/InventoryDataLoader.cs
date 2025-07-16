using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.Services
{
    public static class InventoryDataLoader
    {
        public static List<InventoryItem> LoadAllItems(string directory)
        {
            var items = new List<InventoryItem>();
            if (string.IsNullOrEmpty(directory))
            {
                return items;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            foreach (var file in Directory.GetFiles(directory, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var item = JsonSerializer.Deserialize<InventoryItem>(json);
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading item from {file}: {ex.Message}");
                }
            }
            return items;
        }

        public static void SaveItemToJson(InventoryItem item, string path)
        {
            try
            {
                var json = JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save item to {path}: {ex.Message}", ex);
            }
        }
    }
}