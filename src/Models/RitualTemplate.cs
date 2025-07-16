using System;
using System.Collections.Generic;

namespace RitualOS.Models
{
    /// <summary>
    /// Represents a reusable ritual template with common metadata.
    /// </summary>
    public class RitualTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string Intention { get; set; } = string.Empty;
        public List<string> Tools { get; set; } = new();
        public List<string> SpiritsInvoked { get; set; } = new();
        public List<string> ChakrasAffected { get; set; } = new();
        public List<string> Elements { get; set; } = new();
        public string MoonPhase { get; set; } = string.Empty;
        public List<string> Steps { get; set; } = new();
        public string OutcomeField { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
