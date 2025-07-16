namespace RitualOS.Models
{
    public class RitualEntry
    {
        public RitualEntry()
        {
            Id = string.Empty;
            Title = string.Empty;
            Intention = string.Empty;
            MoonPhase = string.Empty;
            Outcome = string.Empty;
            Notes = string.Empty;
            ClientId = string.Empty;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Intention { get; set; }
        public string MoonPhase { get; set; }
        public List<Chakra> Chakras { get; set; } = new();
        public List<IngredientItem> Ingredients { get; set; } = new();
        public List<string> Steps { get; set; } = new();
        public string Outcome { get; set; }
        public List<string> Symbols { get; set; } = new();
        public string Notes { get; set; }
        public string ClientId { get; set; }
    }
}