using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.Services
{
    public static class DreamDataLoader
    {
        public static List<DreamEntry> LoadAllDreams(string directory)
        {
            var dreams = new List<DreamEntry>();
            if (string.IsNullOrEmpty(directory))
            {
                return dreams;
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
                    var dream = JsonSerializer.Deserialize<DreamEntry>(json);
                    if (dream != null)
                    {
                        dreams.Add(dream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading dream from {file}: {ex.Message}");
                }
            }
            return dreams;
        }

        public static void SaveDream(DreamEntry dream, string path)
        {
            try
            {
                var json = JsonSerializer.Serialize(dream, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save dream to {path}: {ex.Message}", ex);
            }
        }

        internal static void SaveDreamToJson(DreamEntry dream, string path)
        {
            SaveDream(dream, path);
        }
    }
}