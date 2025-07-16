using System.Collections.Generic;

namespace RitualOS.Models
{
    public class Symbol
    {
        public Symbol()
        {
            Name = string.Empty;
            Original = string.Empty;
            Rewritten = string.Empty;
            RitualText = string.Empty;
            Description = string.Empty;
        }

        public string Name { get; set; }
        public string Original { get; set; }
        public string Rewritten { get; set; }
        public string RitualText { get; set; }
        public string Description { get; set; }
        public List<Chakra> ChakraTags { get; set; } = new();
        public List<Element> ElementTags { get; set; } = new();
    }
}