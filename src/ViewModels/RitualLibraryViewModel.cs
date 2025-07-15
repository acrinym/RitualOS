using System.Collections.ObjectModel;
using RitualOS.Models;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// Provides filtering of available rituals.
    /// </summary>
    public class RitualLibraryViewModel : ViewModelBase
    {
        private Chakra? _selectedChakra;
        private string? _selectedSpirit;

        public ObservableCollection<string> SpiritFilters { get; } = new();
        public ObservableCollection<Chakra> ChakraFilters { get; } = new();
        public ObservableCollection<RitualEntry> FilteredRituals { get; } = new();

        public Chakra? SelectedChakra
        {
            get => _selectedChakra;
            set
            {
                if (_selectedChakra != value)
                {
                    _selectedChakra = value;
                    OnPropertyChanged();
                    ApplyFilter();
                }
            }
        }

        public string? SelectedSpirit
        {
            get => _selectedSpirit;
            set
            {
                if (_selectedSpirit != value)
                {
                    _selectedSpirit = value;
                    OnPropertyChanged();
                    ApplyFilter();
                }
            }
        }

        public RitualLibraryViewModel()
        {
            // seed
            ChakraFilters.Add(Chakra.Root);
            ChakraFilters.Add(Chakra.Heart);
            SpiritFilters.Add("Gabriel");
            SpiritFilters.Add("Uriel");
            // sample ritual
            FilteredRituals.Add(new RitualEntry { Title = "Sample", Intention = "Demo" });
        }

        private void ApplyFilter()
        {
            // Filtering logic placeholder
            // Here we would refresh FilteredRituals from a data source
        }
    }
}
