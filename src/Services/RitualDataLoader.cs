using System;
using System.IO;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Provides helper methods for loading and saving ritual data to disk.
    /// </summary>
    public static class RitualDataLoader
    {
        /// <summary>
        /// Loads a ritual entry from a JSON file on disk.
        /// </summary>
        /// <param name="filePath">Path to the JSON file.</param>
        /// <returns>The deserialized <see cref="RitualEntry"/>.</returns>
        /// <exception cref="RitualDataLoadException">
        /// Thrown when the file cannot be read or the JSON is invalid.
        /// </exception>
    public static RitualEntry LoadRitualFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new RitualDataLoadException($"File not found: {filePath}");
            }

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<RitualEntry>(json);
            }
            catch (Exception ex) when (ex is IOException || ex is JsonException)
            {
                throw new RitualDataLoadException("Failed to deserialize ritual data.", ex);
            }
        }

        /// <summary>
        /// Serializes a <see cref="RitualEntry"/> to the specified path in a pretty printed JSON format.
        /// </summary>
        /// <param name="entry">The ritual entry to save.</param>
        /// <param name="filePath">File path where the JSON should be written.</param>
        public static void SaveRitualToJson(RitualEntry entry, string filePath)
        {
            var json = JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}
