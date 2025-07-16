namespace RitualOS.Models
{
    public class IngredientItem
    {
        public IngredientItem()
        {
            Name = string.Empty;
            Category = string.Empty;
            Location = string.Empty;
            MarketplaceLink = string.Empty;
            Notes = string.Empty;
        }

        public string Name { get; set; }
        public string Category { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Location { get; set; }
        public List<Chakra> Chakras { get; set; } = new();
        public string MarketplaceLink { get; set; }
        public string Notes { get; set; }
    }
}