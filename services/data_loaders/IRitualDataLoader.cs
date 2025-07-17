using System.Collections.Generic;
using System.Threading.Tasks;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Interface for loading and saving ritual data.
    /// </summary>
    public interface IRitualDataLoader
    {
        RitualEntry LoadRitualFromJson(string filePath);
        void SaveRitualToJson(RitualEntry entry, string filePath);
        List<RitualEntry> LoadRitualsForClient(string directory, string clientId);
        List<RitualEntry> LoadAllRituals(string directory);

        Task<List<RitualTemplate>> LoadRitualTemplatesAsync();
    }
}
