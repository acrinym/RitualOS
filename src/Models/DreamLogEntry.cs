using System;
using System.Collections.Generic;

namespace RitualOS.Models
{
    public class DreamLogEntry
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Symbols { get; set; }
        public List<string> Tags { get; set; }
        public string Interpretations { get; set; }
    }
}
