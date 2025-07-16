using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RitualOS.Helpers;
using RitualOS.Models;
using RitualOS.Services;
using RitualOS.ViewModels;

namespace RitualOS.ViewModels.Wizards
{
    /// <summary>
    /// Dashboard presenting multiple client timelines.
    /// </summary>
    public class ClientProfileDashboardViewModel : ViewModelBase
    {
        public ObservableCollection<ClientViewModel> Clients { get; } = new();

        public ClientProfileDashboardViewModel()
        {
            const string clientDir = "clients";
            const string ritualDir = "rituals";

            foreach (var client in ClientProfileLoader.LoadClients(clientDir))
            {
                var rituals = RitualDataLoader.LoadRitualsForClient(ritualDir, client.Id);
                Clients.Add(new ClientViewModel(client, rituals));
            }
        }
    }
}