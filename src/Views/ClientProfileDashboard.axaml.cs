using Avalonia.Controls;
using RitualOS.Models;
using RitualOS.ViewModels.Wizards;
using System.Collections.Generic;

namespace RitualOS.Views
{
    public partial class ClientProfileDashboard : UserControl
    {
        public ClientProfileDashboard()
        {
            InitializeComponent();

            // This is for demonstration purposes. In the final app,
            // this data would be loaded from your ClientDatabase service.
            var demoClient = new ClientProfile
            {
                Name = "Faith Healer",
                Role = "Practitioner",
                Email = "faith@example.com",
                Phone = "555-123-4567",
                EnergyNotes = "Client reports feeling a sense of clarity and lightness after the last session. Focus on grounding exercises for the next ritual.",
                ChakraNotes = new Dictionary<Chakra, string>
                {
                    { Chakra.Root, "Feeling disconnected from physical self. Needs grounding." },
                    { Chakra.Heart, "Open and receptive, but guarded around new relationships." },
                    { Chakra.Crown, "Strong connection to the divine, sometimes leading to a feeling of being ungrounded." }
                }
            };

            this.DataContext = new ClientProfileDashboardViewModel(demoClient);
        }
    }
}