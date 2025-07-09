using System.IO;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.Services
{
    public static class RitualDataLoader
    {
        public static RitualEntry LoadRitualFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<RitualEntry>(json);
        }

        public static void SaveRitualToJson(RitualEntry entry, string filePath)
        {
            var json = JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}
