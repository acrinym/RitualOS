using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
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

        /// <summary>
        /// Loads all ritual entries for a specific client from a directory of JSON files.
        /// </summary>
        /// <param name="directory">Directory containing ritual JSON files.</param>
        /// <param name="clientId">ID of the client to filter by.</param>
        /// <returns>List of rituals associated with the client.</returns>
        public static List<RitualEntry> LoadRitualsForClient(string directory, string clientId)
        {
            var results = new List<RitualEntry>();

            if (!Directory.Exists(directory))
            {
                return results;
            }

            foreach (var file in Directory.GetFiles(directory, "*.json"))
            {
                try
                {
                    var ritual = LoadRitualFromJson(file);
                    if (ritual?.ClientId == clientId)
                    {
                        results.Add(ritual);
                    }
                }
                catch (RitualDataLoadException)
                {
                    // ignore invalid files
                }
            }

            return results;
        }

        /// <summary>
        /// Loads all ritual entries from a directory of JSON files.
        /// </summary>
        /// <param name="directory">Directory containing ritual JSON files.</param>
        /// <returns>List of all successfully loaded rituals.</returns>
        public static List<RitualEntry> LoadAllRituals(string directory)
        {
            var results = new List<RitualEntry>();

            if (!Directory.Exists(directory))
            {
                return results;
            }

            foreach (var file in Directory.GetFiles(directory, "*.json"))
            {
                try
                {
                    var ritual = LoadRitualFromJson(file);
                    if (ritual != null)
                    {
                        results.Add(ritual);
                    }
                }
                catch (RitualDataLoadException)
                {
                    // ignore invalid files
                }
            }

            return results;
        }
    }
}
