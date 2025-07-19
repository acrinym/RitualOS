using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RitualOS.Models
{
    public enum SymbolCategory
    {
        [Display(Name = "Alchemical")]
        Alchemical,
        [Display(Name = "Astrological")]
        Astrological,
        [Display(Name = "Celtic")]
        Celtic,
        [Display(Name = "Chinese")]
        Chinese,
        [Display(Name = "Egyptian")]
        Egyptian,
        [Display(Name = "Goetic")]
        Goetic,
        [Display(Name = "Hermetic")]
        Hermetic,
        [Display(Name = "Kabbalistic")]
        Kabbalistic,
        [Display(Name = "Nordic")]
        Nordic,
        [Display(Name = "Planetary")]
        Planetary,
        [Display(Name = "Protection")]
        Protection,
        [Display(Name = "Runic")]
        Runic,
        [Display(Name = "Sigil")]
        Sigil,
        [Display(Name = "Tibetan")]
        Tibetan,
        [Display(Name = "Unified")]
        Unified,
        [Display(Name = "Wiccan")]
        Wiccan,
        [Display(Name = "Other")]
        Other
    }

    public enum SymbolPower
    {
        [Display(Name = "Minor")]
        Minor = 1,
        [Display(Name = "Moderate")]
        Moderate = 2,
        [Display(Name = "Major")]
        Major = 3,
        [Display(Name = "Greater")]
        Greater = 4,
        [Display(Name = "Supreme")]
        Supreme = 5
    }

    public class Symbol
    {
        public Symbol()
        {
            Name = string.Empty;
            AlternativeNames = new List<string>();
            Original = string.Empty;
            Rewritten = string.Empty;
            RitualText = string.Empty;
            Description = string.Empty;
            HistoricalContext = string.Empty;
            UsageInstructions = string.Empty;
            Warnings = string.Empty;
            Source = string.Empty;
            Tags = new List<string>();
            AssociatedEntities = new List<string>();
            PlanetaryCorrespondences = new List<string>();
            ElementalCorrespondences = new List<string>();
            ColorCorrespondences = new List<string>();
            MaterialCorrespondences = new List<string>();
            TimeCorrespondences = new List<string>();
            ChakraTags = new List<Chakra>();
            ElementTags = new List<Element>();
            RelatedSymbols = new List<string>();
            DateAdded = DateTime.Now;
            LastModified = DateTime.Now;
        }

        // Basic Information
        public string Name { get; set; }
        public List<string> AlternativeNames { get; set; }
        public SymbolCategory Category { get; set; }
        public SymbolPower PowerLevel { get; set; }
        
        // Visual Representation
        public string Original { get; set; } // SVG or image path
        public string Rewritten { get; set; } // Modified version
        public string SvgData { get; set; } = string.Empty; // SVG content
        public string ImagePath { get; set; } = string.Empty; // Path to image file
        
        // Description and Context
        public string Description { get; set; }
        public string HistoricalContext { get; set; }
        public string UsageInstructions { get; set; }
        public string Warnings { get; set; }
        public string RitualText { get; set; }
        
        // Source and Attribution
        public string Source { get; set; }
        public string Author { get; set; } = string.Empty;
        public string License { get; set; } = string.Empty;
        
        // Categorization and Tags
        public List<string> Tags { get; set; }
        public List<string> AssociatedEntities { get; set; } // For Goetic spirits, deities, etc.
        
        // Correspondences
        public List<string> PlanetaryCorrespondences { get; set; }
        public List<string> ElementalCorrespondences { get; set; }
        public List<string> ColorCorrespondences { get; set; }
        public List<string> MaterialCorrespondences { get; set; }
        public List<string> TimeCorrespondences { get; set; }
        public List<Chakra> ChakraTags { get; set; }
        public List<Element> ElementTags { get; set; }
        
        // Relationships
        public List<string> RelatedSymbols { get; set; }
        
        // Metadata
        public DateTime DateAdded { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsPersonal { get; set; } = false; // User-created vs. system
        public bool IsVerified { get; set; } = false; // Verified by community/sources
        public int UsageCount { get; set; } = 0;
        public double Rating { get; set; } = 0.0;
        public int RatingCount { get; set; } = 0;
        
        // Goetic Spirit Specific (for Goetic category)
        public int? GoeticRank { get; set; }
        public string? GoeticLegion { get; set; }
        public string? GoeticAppearance { get; set; }
        public string? GoeticPowers { get; set; }
        public string? GoeticSeal { get; set; }
    }
}