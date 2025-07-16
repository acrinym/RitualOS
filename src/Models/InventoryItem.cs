namespace RitualOS.Models
{
    public class InventoryItem
    {
        public InventoryItem()
        {
            Name = string.Empty;
            Category = string.Empty;
            StorageLocation = string.Empty;
        }

        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<Chakra> AssociatedChakras { get; set; } = new();
        public string StorageLocation { get; set; }
    }
}