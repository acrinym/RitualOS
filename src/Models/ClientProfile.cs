using System;
using System.Collections.Generic;

namespace RitualOS.Models
{
    public class ClientProfile
    {
        public ClientProfile()
        {
            Id = string.Empty;
            Name = string.Empty;
            Role = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Notes = string.Empty;
            EnergyNotes = string.Empty;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
        public List<RitualEntry> RitualsPerformed { get; set; } = new();
        public List<InteractionLogEntry> History { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public string EnergyNotes { get; set; }
        public Dictionary<Chakra, string> ChakraNotes { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}