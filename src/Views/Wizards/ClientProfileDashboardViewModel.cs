using RitualOS.Models;

namespace RitualOS.ViewModels.Wizards
{
    public class ClientProfileDashboardViewModel : ViewModelBase
    {
        public ClientProfile Client { get; }

        public ClientProfileDashboardViewModel(ClientProfile client)
        {
            Client = client;
        }
    }
}