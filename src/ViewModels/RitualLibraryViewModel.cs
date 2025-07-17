using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using RitualOS.Models;
using RitualOS.Services;
using RitualOS.Helpers;

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
            // Load rituals from disk using the data loader 😄
            var dataDir = Path.Combine("samples");
            var loader = new RitualDataLoader();
            foreach (var ritual in loader.LoadAllRituals(dataDir))
            {
                AllRituals.Add(ritual);
            }

            ChakraFilters.Add("All");
            foreach (var chakra in AllRituals.SelectMany(r => r.AffectedChakras).Distinct())
            {
                ChakraFilters.Add(ChakraHelper.GetDisplayName(chakra));
            }

            SpiritFilters.Add("All");
            foreach (var spirit in AllRituals.SelectMany(r => r.SpiritsInvoked).Distinct())
            {
                SpiritFilters.Add(spirit);
            }

            SelectedChakra = "All";
            SelectedSpirit = "All";
            UpdateFilter();
        }

        private void UpdateFilter()
        {
            var filtered = AllRituals.Where(r =>
                (SelectedChakra == "All" || r.AffectedChakras.Any(c => ChakraHelper.GetDisplayName(c) == SelectedChakra)) &&
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
