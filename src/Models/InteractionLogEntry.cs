using System;

namespace RitualOS.Models
{
    public class InteractionLogEntry
    {
        public InteractionLogEntry()
        {
            Id = Guid.NewGuid().ToString();
            Type = string.Empty;
            Summary = string.Empty;
            Details = string.Empty;
        }

        public string Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Type { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
    }
}