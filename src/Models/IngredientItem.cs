using System;
using System.Collections.Generic;

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
        public int Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Location { get; set; }
        public bool ReorderFlag { get; set; }
        public string MarketplaceLink { get; set; }
        public string Notes { get; set; }
    }
}