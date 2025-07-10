using System.Collections.Generic;

namespace RitualOS.Models
{
    public class ClientProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public List<string> Flags { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public List<string> RitualIds { get; set; } = new();
        public string EnergyNotes { get; set; }
    }
}
