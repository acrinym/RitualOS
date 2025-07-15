using System.Collections.ObjectModel;
using System.Linq;
using RitualOS.Models;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// ViewModel providing filtering and selection for the ritual library view.
    /// </summary>
    public class RitualLibraryViewModel : ViewModelBase
    {
        public ObservableCollection<RitualEntry> AllRituals { get; } = new();
        public ObservableCollection<string> ChakraFilters { get; } = new();
        public ObservableCollection<string> SpiritFilters { get; } = new();
        public ObservableCollection<RitualEntry> FilteredRituals { get; } = new();

        private RitualEntry? _selectedRitual;
        public RitualEntry? SelectedRitual
        {
            get => _selectedRitual!;
            set
            {
                if (_selectedRitual != value)
                {
                    _selectedRitual = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedChakra = string.Empty;
        public string SelectedChakra
        {
            get => _selectedChakra;
            set
            {
                if (_selectedChakra != value)
                {
                    _selectedChakra = value;
                    OnPropertyChanged();
                    UpdateFilter();
                }
            }
        }

        private string _selectedSpirit = string.Empty;
        public string SelectedSpirit
        {
            get => _selectedSpirit;
            set
            {
                if (_selectedSpirit != value)
                {
                    _selectedSpirit = value;
                    OnPropertyChanged();
                    UpdateFilter();
                }
            }
        }

        public RitualLibraryViewModel()
        {
            // placeholder data until wired to backend
            ChakraFilters.Add("All");
            ChakraFilters.Add("Root");
            ChakraFilters.Add("Sacral");
            ChakraFilters.Add("Solar Plexus");
            ChakraFilters.Add("Heart");
            ChakraFilters.Add("Throat");
            ChakraFilters.Add("Third Eye");
            ChakraFilters.Add("Crown");

            SpiritFilters.Add("All");
            SpiritFilters.Add("Seere");
            SpiritFilters.Add("Balam");
            SpiritFilters.Add("Zepar");
            SpiritFilters.Add("Lucifer");

            SelectedChakra = "All";
            SelectedSpirit = "All";
            UpdateFilter();
        }

        private void UpdateFilter()
        {
            var filtered = AllRituals.Where(r =>
                (SelectedChakra == "All" || r.AffectedChakras.Any(c => c.ToString().Replace("SolarPlexus", "Solar Plexus").Replace("ThirdEye", "Third Eye") == SelectedChakra)) &&
                (SelectedSpirit == "All" || r.SpiritsInvoked.Contains(SelectedSpirit)))
                .ToList();

            FilteredRituals.Clear();
            foreach (var r in filtered)
            {
                FilteredRituals.Add(r);
            }
        }
    }
}
