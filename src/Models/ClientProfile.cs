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
        public List<Chakra> AssociatedChakras { get; set; } = new();
        public List<string> RitualIds { get; set; } = new();
        public string EnergyNotes { get; set; }
    }
}