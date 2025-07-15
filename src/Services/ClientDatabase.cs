using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Simple file-backed database for storing and retrieving <see cref="ClientProfile"/> records.
    /// Clients can also be assigned as ritual beneficiaries.
    /// </summary>
    public class ClientDatabase
    {
        private readonly Dictionary<string, ClientProfile> _clients = new();
        private readonly string _storageDirectory;

        /// <summary>
        /// Initializes the database and loads any existing client JSON files from the directory.
        /// </summary>
        /// <param name="storageDirectory">Directory where client files are stored.</param>
        public ClientDatabase(string storageDirectory)
        {
            _storageDirectory = storageDirectory;
            LoadClients();
        }

        /// <summary>
        /// Returns a client profile by id, or <c>null</c> if it does not exist.
        /// </summary>
        public ClientProfile? GetClient(string id)
        {
            _clients.TryGetValue(id, out var client);
            return client;
        }

        /// <summary>
        /// Adds or updates a client record and writes it to disk.
        /// </summary>
        public void SaveClient(ClientProfile client)
        {
            if (string.IsNullOrWhiteSpace(client.Id))
            {
                client.Id = Guid.NewGuid().ToString();
            }

            client.LastUpdated = DateTime.Now;
            _clients[client.Id] = client;

            var path = Path.Combine(_storageDirectory, $"{client.Id}.json");
            var json = JsonSerializer.Serialize(client, new JsonSerializerOptions { WriteIndented = true });
            Directory.CreateDirectory(_storageDirectory);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Searches clients by case-insensitive name fragment.
        /// </summary>
        public IEnumerable<ClientProfile> FindByName(string nameFragment)
        {
            return _clients.Values.Where(c => c.Name.Contains(nameFragment, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Links the specified client as a beneficiary of the ritual and updates the client's history.
        /// </summary>
        public void AddClientAsBeneficiary(string clientId, RitualEntry ritual)
        {
            ritual.ClientId = clientId;

            if (_clients.TryGetValue(clientId, out var client))
            {
                client.RitualsPerformed.Add(ritual);
                client.LastUpdated = DateTime.Now;
                SaveClient(client);
            }
        }

        private void LoadClients()
        {
            if (!Directory.Exists(_storageDirectory))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(_storageDirectory, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var client = JsonSerializer.Deserialize<ClientProfile>(json);
                    if (client?.Id != null)
                    {
                        _clients[client.Id] = client;
                    }
                }
                catch
                {
                    // ignore malformed files
                }
            }
        }
    }
}
