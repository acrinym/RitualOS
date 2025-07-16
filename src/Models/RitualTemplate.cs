using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RitualOS.Models
{
    /// <summary>
    /// The sacred blueprint for rituals! 😄
    /// </summary>
    public class RitualTemplate
    {
        /// <summary>
        /// Unique identifier for the template.
        /// </summary>
        public Guid TemplateId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// When this template was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Intention { get; set; } = string.Empty;

        public List<string> Tools { get; set; } = new();

        public List<string> SpiritsInvoked { get; set; } = new();

        public List<string> ChakrasAffected { get; set; } = new();

        public List<string> Elements { get; set; } = new();

        public string MoonPhase { get; set; } = string.Empty;

        public List<string> Steps { get; set; } = new();

        public string OutcomeField { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Schema version for forward compatibility.
        /// </summary>
        public int Version { get; set; } = 1;
    }
}