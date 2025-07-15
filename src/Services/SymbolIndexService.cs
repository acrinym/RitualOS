using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Provides loading and saving functionality for SymbolIndex.json.
    /// </summary>
    public static class SymbolIndexService
    {
        public const string FileName = "SymbolIndex.json";

        /// <summary>
        /// Loads the symbol index from disk, returning an empty list if missing.
        /// </summary>
        public static List<Symbol> Load()
        {
            if (!File.Exists(FileName))
            {
                return new List<Symbol>();
            }

            var json = File.ReadAllText(FileName);
            return JsonSerializer.Deserialize<List<Symbol>>(json) ?? new List<Symbol>();
        }

        /// <summary>
        /// Saves the provided symbols to disk.
        /// </summary>
        public static void Save(IEnumerable<Symbol> symbols)
        {
            var json = JsonSerializer.Serialize(symbols, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
    }
}
