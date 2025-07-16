using System;
using System.Collections.Generic;

namespace RitualOS.Models
{
    public class RitualEntry
    {
        public RitualEntry()
        {
            Id = string.Empty;
            Title = string.Empty;
            Intention = string.Empty;
            MoonPhase = string.Empty;
            Outcome = string.Empty;
            Notes = string.Empty;
            ClientId = string.Empty;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DatePerformed { get; set; }
        public string Intention { get; set; }
        public string MoonPhase { get; set; }
        public List<string> SpiritsInvoked { get; set; } = new();
        public List<string> Ingredients { get; set; } = new();
        public List<Chakra> AffectedChakras { get; set; } = new();
        public List<string> RitualSteps { get; set; } = new();
        public string Outcome { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Notes { get; set; }
        public Dictionary<string, string> CustomMetadata { get; set; } = new();
        public string ClientId { get; set; }
    }
}