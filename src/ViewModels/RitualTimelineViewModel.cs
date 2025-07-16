using System;
using System.Collections.ObjectModel;
using System.Linq;
using RitualOS.Models;
using RitualOS.Services;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// View model providing a simple chronological list of rituals.
    /// </summary>
    public class RitualTimelineViewModel : ViewModelBase
    {
        public ObservableCollection<RitualEntry> Rituals { get; } = new();

        public RitualTimelineViewModel()
        {
            var dataDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples");
            var rituals = RitualDataLoader.LoadAllRituals(dataDir)
                .OrderByDescending(r => r.DatePerformed);
            foreach (var r in rituals)
            {
                Rituals.Add(r);
            }
        }
    }
}
