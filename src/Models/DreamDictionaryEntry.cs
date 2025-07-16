namespace RitualOS.Models
{
    /// <summary>
    /// Represents a single dream dictionary entry parsed from markdown.
    /// </summary>
    public class DreamDictionaryEntry
    {
        public string Term { get; set; } = string.Empty;
        public string Emoji { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
