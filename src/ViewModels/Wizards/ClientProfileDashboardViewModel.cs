using System.Collections.ObjectModel;
using RitualOS.Services;
using RitualOS.Models;

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
            foreach (var client in ClientProfileLoader.LoadClients("clients"))
            {
                var rituals = RitualDataLoader.LoadRitualsForClient("rituals", client.Id);
                Clients.Add(new ClientViewModel(client, rituals));
            }
        }
    }
}
