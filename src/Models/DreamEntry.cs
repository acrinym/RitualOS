namespace RitualOS.Models
{
    public class DreamEntry
    {
        public DreamEntry()
        {
            Id = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            Interpretation = string.Empty;
        }

        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Chakra> Chakras { get; set; } = new();
        public List<string> Symbols { get; set; } = new();
        public string Interpretation { get; set; }
    }
}