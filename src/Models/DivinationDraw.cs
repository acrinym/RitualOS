using System;
using System.Collections.Generic;

namespace RitualOS.Models
{
    public enum DivinationType { Tarot, Rune }
    public enum DivinationSpread { Single, ThreeCard }

    public class DivinationDraw
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; } = DateTime.Now;
        public DivinationType Type { get; set; } = DivinationType.Tarot;
        public DivinationSpread Spread { get; set; } = DivinationSpread.Single;
        public List<string> CardNames { get; set; } = new(); // Card or rune names
        public string Journal { get; set; } = string.Empty;
    }
} 