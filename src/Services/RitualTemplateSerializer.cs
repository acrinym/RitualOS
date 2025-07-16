using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Helper methods for loading and saving ritual templates to disk.
    /// </summary>
    public static class RitualTemplateSerializer
    {
        private static readonly string DirectoryPath = Path.Combine(AppContext.BaseDirectory, "ritual_templates");

        public static void Save(RitualTemplate template, string fileName)
        {
            Directory.CreateDirectory(DirectoryPath);
            var path = Path.Combine(DirectoryPath, fileName);
            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            var json = JsonSerializer.Serialize(template, options);
            File.WriteAllText(path, json);
        }

        public static RitualTemplate Load(string fileName)
        {
            var path = Path.Combine(DirectoryPath, fileName);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Template file not found: {path}");

            var json = File.ReadAllText(path);
            var options = new JsonSerializerOptions();
            options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            var template = JsonSerializer.Deserialize<RitualTemplate>(json, options);
            if (template == null)
                throw new Exception($"Failed to deserialize template from {fileName}");
            return template;
        }

        public static List<RitualTemplate> LoadAll()
        {
            var list = new List<RitualTemplate>();
            if (!Directory.Exists(DirectoryPath))
                return list;

            foreach (var file in Directory.GetFiles(DirectoryPath, "*.json"))
            {
                try
                {
                    list.Add(Load(Path.GetFileName(file)));
                }
                catch
                {
                    // ignore invalid files
                }
            }
            return list;
        }
    }
}
