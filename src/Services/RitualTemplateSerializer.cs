using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Helper methods for loading and saving ritual templates to disk.
    /// </summary>
    public static class RitualTemplateSerializer
    {
        private static readonly string DirectoryPath = Path.Combine(AppContext.BaseDirectory, "ritual_templates");
        private const int CurrentVersion = 1;

        public static async Task SaveAsync(RitualTemplate template, string? directory = null)
        {
            Validate(template);

            var dir = directory ?? DirectoryPath;
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, $"template_{template.TemplateId}.json");

            // backup existing file to prevent data loss
            if (File.Exists(path))
            {
                var backup = Path.Combine(dir, $"template_{template.TemplateId}_{DateTime.Now:yyyyMMdd_HHmmss}.bak");
                File.Copy(path, backup, true);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new JsonStringEnumConverter());
            var json = JsonSerializer.Serialize(template, options);
            await File.WriteAllTextAsync(path, json);

            UserSettingsService.Current.LastTemplatePath = path;
            UserSettingsService.Save();
        }

        public static void Save(RitualTemplate template, string? directory = null)
        {
            SaveAsync(template, directory).GetAwaiter().GetResult();
        }
        public static async Task<RitualTemplate> LoadAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Template file not found: {path}");

            var json = await File.ReadAllTextAsync(path);
            ValidateJsonSchema(json);

            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            var template = JsonSerializer.Deserialize<RitualTemplate>(json, options);
            if (template == null)
                throw new Exception($"Failed to deserialize template from {path}");

            Validate(template);

            UserSettingsService.Current.LastTemplatePath = path;
            UserSettingsService.Save();

            return template;
        }

        public static RitualTemplate Load(string path)
        {
            return LoadAsync(path).GetAwaiter().GetResult();
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
                    list.Add(Load(file));
                }
                catch
                {
                    // ignore invalid files
                }
            }
            return list;
        }

        private static void Validate(RitualTemplate template)
        {
            var context = new ValidationContext(template);
            Validator.ValidateObject(template, context, true);

            if (template.Version != CurrentVersion)
                throw new Exception($"Unsupported template version {template.Version}. Expected {CurrentVersion}.");
        }

        private static void ValidateJsonSchema(string json)
        {
            // very minimal validation to ensure critical fields exist before deserialization
            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("TemplateId", out _))
                throw new Exception("Invalid template schema: missing TemplateId");
            if (!doc.RootElement.TryGetProperty("Version", out _))
                throw new Exception("Invalid template schema: missing Version");
        }
    }
}
