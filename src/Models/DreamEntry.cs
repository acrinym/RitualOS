using System;
using System.Collections.Generic;

namespace RitualOS.Models
{
    public class DreamEntry
    {
        public DreamEntry()
        {
            Id = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            Interpretation = string.Empty;
        }

        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Symbols { get; set; } = new();
        public List<Chakra> ChakraTags { get; set; } = new();
        /// <summary>
        /// Elements associated with this dream entry.
        /// </summary>
        public List<Element> ElementTags { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public string Interpretation { get; set; }
    }
}