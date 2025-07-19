using System.Text.Json.Serialization;

namespace RitualOS.Models
{
    public class SymbolWikiEntry
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("tradition")]
        public string Tradition { get; set; } = string.Empty;
        [JsonPropertyName("wikimedia_url")]
        public string WikimediaUrl { get; set; } = string.Empty;
        [JsonPropertyName("wikipedia_url")]
        public string WikipediaUrl { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
} 