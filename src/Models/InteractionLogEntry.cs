namespace RitualOS.Models
{
    public class InteractionLogEntry
    {
        public InteractionLogEntry()
        {
            Type = string.Empty;
            Summary = string.Empty;
            Details = string.Empty;
        }

        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
        public string ClientId { get; set; } = string.Empty;
    }
}