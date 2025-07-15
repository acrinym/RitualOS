using System.Collections.ObjectModel;
using RitualOS.ViewModels;

namespace RitualOS.ViewModels.Wizards
{
    /// <summary>
    /// Dashboard presenting multiple client timelines.
    /// </summary>
    public class ClientProfileDashboardViewModel : ViewModelBase
    {
        public ObservableCollection<ClientViewModel> Clients { get; } = new();
    }
}
