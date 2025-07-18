using System;
using System.Collections.Generic;

namespace RitualOS.Models
{
    /// <summary>
    /// Represents a single school of magic or occult practice.
    /// </summary>
    public class MagicSchool
    {
        public string Name { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string CommonPractices { get; set; } = string.Empty;
        public string HistoricalContext { get; set; } = string.Empty;
        public string WhoIsItFor { get; set; } = string.Empty;
        public string NotableFigures { get; set; } = string.Empty;
        public string RecommendedReadings { get; set; } = string.Empty;
        public string[] Keywords { get; set; } = Array.Empty<string>();
        public string? InfoPath { get; set; }
    }
}