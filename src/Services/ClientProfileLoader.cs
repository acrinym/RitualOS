using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Helper methods for loading and saving <see cref="ClientProfile"/> objects.
    /// </summary>
    public static class ClientProfileLoader
    {
        /// <summary>
        /// Loads all client profiles from the specified directory.
        /// </summary>
        /// <param name="directory">Directory path containing JSON client files.</param>
        public static List<ClientProfile> LoadClients(string directory)
        {
            var results = new List<ClientProfile>();
            if (!Directory.Exists(directory))
            {
                // create the directory so first time launches don't fail
                Directory.CreateDirectory(directory);
                return results;
            }

            foreach (var file in Directory.GetFiles(directory, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var profile = JsonSerializer.Deserialize<ClientProfile>(json);
                    if (profile != null)
                    {
                        results.Add(profile);
                    }
                }
                catch (Exception)
                {
                    // ignore invalid client files
                }
            }

            return results;
        }

        /// <summary>
        /// Saves the provided client profile to a JSON file in the directory.
        /// </summary>
        public static void SaveClient(ClientProfile profile, string directory)
        {
            Directory.CreateDirectory(directory);
            var filePath = Path.Combine(directory, $"{profile.Id}.json");

            try
            {
                var json = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception)
            {
                // swallow serialization issues for now
            }
        }
    }
}
