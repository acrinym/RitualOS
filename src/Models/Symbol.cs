using System.Collections.Generic;

namespace RitualOS.Models
{
    /// <summary>
    /// Represents a codex symbol and related interpretations.
    /// </summary>
    public class Symbol
    {
        public string Name { get; set; }
        public string Original { get; set; }
        public string Rewritten { get; set; }
        public string RitualText { get; set; }
        public string Description { get; set; }
        public List<Chakra> ChakraTags { get; set; } = new();
        public List<string> ElementTags { get; set; } = new();
    }
}
