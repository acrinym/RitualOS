using System;
using System.Collections.Generic;

namespace RitualOS.Models
{
    public class InventoryItem
    {
        public string Name { get; set; }
        public string Category { get; set; }  // Herb, Oil, etc.
        public int Quantity { get; set; }
        public bool ChakraTrackingEnabled { get; set; } = false;
        public List<Chakra> AssociatedChakras { get; set; } = new();
        public DateTime? ExpirationDate { get; set; }
        public string StorageLocation { get; set; }
    }
}
