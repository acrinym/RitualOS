using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RitualOS.Models;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// View model representing a client profile and their ritual history.
    /// </summary>
    public class ClientViewModel : ViewModelBase
    {
        public ClientProfile Client { get; }
        public ObservableCollection<RitualEntry> Rituals { get; }

        public ClientViewModel(ClientProfile client, IEnumerable<RitualEntry> rituals)
        {
            Client = client;
            Rituals = new ObservableCollection<RitualEntry>(rituals.OrderByDescending(r => r.DatePerformed));
        }

        /// <summary>
        /// Creates a new ritual pre-populated for this client and adds it to the history.
        /// </summary>
        public RitualEntry CreateNewRitual()
        {
            var ritual = new RitualEntry
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.Now,
                DatePerformed = DateTime.Now,
                ClientId = Client.Id
            };

            AddRitual(ritual);
            return ritual;
        }

        /// <summary>
        /// Adds a new ritual to this client and updates metadata.
        /// </summary>
        public void AddRitual(RitualEntry ritual)
        {
            ritual.ClientId = Client.Id;
            Rituals.Add(ritual);
            Client.RitualsPerformed.Add(ritual);
            Client.LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Updates the notes field of the client.
        /// </summary>
        public void UpdateNotes(string notes)
        {
            Client.Notes = notes;
            Client.LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Replace the client's tags with the provided set.
        /// </summary>
        public void UpdateTags(IEnumerable<string> tags)
        {
            Client.Tags = tags.ToList();
            Client.LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Returns rituals optionally filtered by outcome or moon phase.
        /// Results are sorted by performed date descending.
        /// </summary>
        public IEnumerable<RitualEntry> GetRituals(string? outcome = null, string? moonPhase = null)
        {
            var query = Rituals.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(outcome))
            {
                query = query.Where(r => string.Equals(r.Outcome, outcome, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(moonPhase))
            {
                query = query.Where(r => string.Equals(r.MoonPhase, moonPhase, StringComparison.OrdinalIgnoreCase));
            }

            return query.OrderByDescending(r => r.DatePerformed);
        }

        /// <summary>
        /// Returns rituals affecting the specified chakra.
        /// </summary>
        public IEnumerable<RitualEntry> GetRitualsByChakra(Chakra chakra)
        {
            return Rituals.Where(r => r.AffectedChakras.Contains(chakra))
                          .OrderByDescending(r => r.DatePerformed);
        }
    }
}
