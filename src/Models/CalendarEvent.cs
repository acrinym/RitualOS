using System;

namespace RitualOS.Models
{
    public enum CalendarEventType
    {
        Ritual,
        Sabbat,
        Reminder,
        AstrologicalTransit,
        Other
    }

    public class CalendarEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public CalendarEventType Type { get; set; } = CalendarEventType.Ritual;
        public string Notes { get; set; } = string.Empty;
        public string? Icon { get; set; } // Optional: for moon phase, sabbat, etc.
    }
} 