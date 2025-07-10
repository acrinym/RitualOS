using System;
using System.Collections.Generic;

namespace RitualOS.Models
{
    public class RitualEntry
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DatePerformed { get; set; }
        public string Intention { get; set; }
        public string MoonPhase { get; set; }
        public List<string> SpiritsInvoked { get; set; } = new();
        public List<string> Ingredients { get; set; } = new();
        public List<string> RitualSteps { get; set; } = new();
        public string OutcomeStatus { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Notes { get; set; }
        public string ClientId { get; set; }
    }
}
