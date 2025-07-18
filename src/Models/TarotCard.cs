using System.Text.Json.Serialization;

namespace RitualOS.Models
{
    /// <summary>
    /// Represents a single Tarot card with basic meanings. 🔮
    /// </summary>
    public class TarotCard
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("arcana")]
        public string Arcana { get; set; } = string.Empty;

        [JsonPropertyName("suit")]
        public string Suit { get; set; } = string.Empty;

        [JsonPropertyName("meaning_up")]
        public string MeaningUp { get; set; } = string.Empty;

        [JsonPropertyName("meaning_rev")]
        public string MeaningRev { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}
