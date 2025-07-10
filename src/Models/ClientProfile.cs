using System;
using System.Collections.Generic;

namespace RitualOS.Models
{
    public class ClientProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }

        public List<RitualEntry> RitualHistory { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public string EnergyNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
