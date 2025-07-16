using System;
using System.Collections.Generic;

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
        public string Category { get; set; }  // Herb, Oil, etc.
        public int Quantity { get; set; }
        public bool ChakraTrackingEnabled { get; set; } = false;
        public List<Chakra> ChakraTags { get; set; } = new();
        public DateTimeOffset? ExpirationDate { get; set; }
        public string StorageLocation { get; set; }
    }
}