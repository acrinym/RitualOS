namespace RitualOS.Models
{
    public class InventoryAlert
    {
        public InventoryAlert()
        {
            ItemName = string.Empty;
            AlertType = string.Empty;
            Message = string.Empty;
        }

        public string ItemName { get; set; }
        public string AlertType { get; set; } // e.g., "LowQuantity" or "Expired"
        public string Message { get; set; }
    }
}