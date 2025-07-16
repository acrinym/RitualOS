using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RitualOS.Models
{
    /// <summary>
    /// Represents a reusable ritual template with common metadata.
    /// </summary>
    public class RitualTemplate
    {
        /// <summary>
        /// Unique identifier used when saving templates to disk.
        /// </summary>
        [Required]
        public string TemplateId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Intention { get; set; } = string.Empty;

        public List<string> Tools { get; set; } = new();

        public List<string> SpiritsInvoked { get; set; } = new();

        public List<Chakra> ChakrasAffected { get; set; } = new();

        public List<Element> Elements { get; set; } = new();

        public MoonPhase MoonPhase { get; set; } = MoonPhase.New;

        public List<string> Steps { get; set; } = new();

        public string OutcomeField { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Schema version for forward compatibility.
        /// </summary>
        public int Version { get; set; } = 1;
    }
}