using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using RitualOS.Models;

namespace RitualOS.Services
{
    public static class SymbolIndexService
    {
        public const string FileName = "SymbolIndex.json";

        public static List<Symbol> Load()
        {
            if (!File.Exists(FileName))
            {
                return new List<Symbol>();
            }

            // Let exceptions propagate up to the caller
            var json = File.ReadAllText(FileName);
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<List<Symbol>>(json, options) ?? new List<Symbol>();
        }

        public static void Save(IEnumerable<Symbol> symbols)
        {
            // Let exceptions propagate up to the caller
            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new JsonStringEnumConverter());
            var json = JsonSerializer.Serialize(symbols, options);
            File.WriteAllText(FileName, json);
        }
    }
}