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

            // Load dashboard data using the default view model behavior.
            this.DataContext = new ClientProfileDashboardViewModel();
        }
    }
}