using System;

namespace RitualOS.Models
{
    public class InteractionLogEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Type { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
    }
}
