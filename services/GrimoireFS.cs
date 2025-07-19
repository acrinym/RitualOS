using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace RitualOS.Services
{
    public class GrimoireFS
    {
        private const string GrimoireDirectory = "grimoire";
        private const string EncryptedDirectory = "encrypted";
        private const string MetadataFile = "metadata.json";
        private const string BackupDirectory = "backups";
        private const string TempDirectory = "temp";
        
        private readonly string _masterKey;
        private readonly string _salt;
        private bool _isInitialized = false;

        public enum GrimoireEntryType
        {
            [Display(Name = "Ritual Log")]
            RitualLog,
            [Display(Name = "Dream Entry")]
            DreamEntry,
            [Display(Name = "Spell Template")]
            SpellTemplate,
            [Display(Name = "Personal Sigil")]
            PersonalSigil,
            [Display(Name = "Journal Entry")]
            JournalEntry,
            [Display(Name = "Meditation Session")]
            MeditationSession,
            [Display(Name = "Divination Reading")]
            DivinationReading,
            [Display(Name = "Energy Work")]
            EnergyWork,
            [Display(Name = "Astral Travel")]
            AstralTravel,
            [Display(Name = "Other")]
            Other
        }

        public class GrimoireEntry
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public GrimoireEntryType Type { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; } = DateTime.Now;
            public DateTime ModifiedDate { get; set; } = DateTime.Now;
            public DateTime? RitualDate { get; set; }
            public List<string> Tags { get; set; } = new();
            public Dictionary<string, string> Metadata { get; set; } = new();
            public List<string> RelatedEntries { get; set; } = new();
            public List<string> SymbolIds { get; set; } = new();
            public string MoonPhase { get; set; } = string.Empty;
            public string PlanetaryInfluences { get; set; } = string.Empty;
            public string Weather { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public string Participants { get; set; } = string.Empty;
            public bool IsEncrypted { get; set; } = true;
            public string EncryptionVersion { get; set; } = "1.0";
            public long FileSize { get; set; } = 0;
            public string Checksum { get; set; } = string.Empty;
        }

        public class GrimoireMetadata
        {
            public string Version { get; set; } = "1.0";
            public DateTime CreatedDate { get; set; } = DateTime.Now;
            public DateTime LastModified { get; set; } = DateTime.Now;
            public int TotalEntries { get; set; } = 0;
            public long TotalSize { get; set; } = 0;
            public Dictionary<GrimoireEntryType, int> EntryCounts { get; set; } = new();
            public List<string> AllTags { get; set; } = new();
            public List<string> AllSymbolIds { get; set; } = new();
            public string LastBackupDate { get; set; } = string.Empty;
            public bool IsEncrypted { get; set; } = true;
        }

        public GrimoireFS(string masterKey, string salt = "")
        {
            _masterKey = masterKey;
            _salt = string.IsNullOrEmpty(salt) ? GenerateSalt() : salt;
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            // Create directory structure
            Directory.CreateDirectory(GrimoireDirectory);
            Directory.CreateDirectory(Path.Combine(GrimoireDirectory, EncryptedDirectory));
            Directory.CreateDirectory(Path.Combine(GrimoireDirectory, BackupDirectory));
            Directory.CreateDirectory(Path.Combine(GrimoireDirectory, TempDirectory));

            // Initialize metadata if it doesn't exist
            var metadataPath = Path.Combine(GrimoireDirectory, MetadataFile);
            if (!File.Exists(metadataPath))
            {
                var metadata = new GrimoireMetadata();
                await SaveMetadataAsync(metadata);
            }

            _isInitialized = true;
        }

        public async Task<GrimoireEntry> CreateEntryAsync(GrimoireEntryType type, string title, string content, 
            Dictionary<string, string>? metadata = null, List<string>? tags = null, List<string>? symbolIds = null)
        {
            var entry = new GrimoireEntry
            {
                Type = type,
                Title = title,
                Content = content,
                Tags = tags ?? new List<string>(),
                SymbolIds = symbolIds ?? new List<string>(),
                Metadata = metadata ?? new Dictionary<string, string>()
            };

            // Add automatic metadata
            entry.Metadata["created_timestamp"] = DateTime.Now.ToString("O");
            entry.Metadata["entry_type"] = type.ToString();
            entry.Metadata["content_length"] = content.Length.ToString();

            await SaveEntryAsync(entry);
            await UpdateMetadataAsync();
            
            return entry;
        }

        public async Task<GrimoireEntry?> GetEntryAsync(string id)
        {
            var encryptedPath = Path.Combine(GrimoireDirectory, EncryptedDirectory, $"{id}.enc");
            
            if (!File.Exists(encryptedPath))
                return null;

            try
            {
                var encryptedData = await File.ReadAllBytesAsync(encryptedPath);
                var decryptedData = DecryptData(encryptedData);
                var entry = JsonSerializer.Deserialize<GrimoireEntry>(decryptedData);
                
                if (entry != null)
                {
                    entry.FileSize = encryptedData.Length;
                    entry.Checksum = CalculateChecksum(decryptedData);
                }
                
                return entry;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to decrypt entry {id}: {ex.Message}", ex);
            }
        }

        public async Task<List<GrimoireEntry>> GetAllEntriesAsync()
        {
            var entries = new List<GrimoireEntry>();
            var encryptedDir = Path.Combine(GrimoireDirectory, EncryptedDirectory);
            
            if (!Directory.Exists(encryptedDir))
                return entries;

            var encryptedFiles = Directory.GetFiles(encryptedDir, "*.enc");
            
            foreach (var file in encryptedFiles)
            {
                var id = Path.GetFileNameWithoutExtension(file);
                var entry = await GetEntryAsync(id);
                if (entry != null)
                {
                    entries.Add(entry);
                }
            }

            return entries.OrderByDescending(e => e.ModifiedDate).ToList();
        }

        public async Task<List<GrimoireEntry>> GetEntriesByTypeAsync(GrimoireEntryType type)
        {
            var allEntries = await GetAllEntriesAsync();
            return allEntries.Where(e => e.Type == type).ToList();
        }

        public async Task<List<GrimoireEntry>> SearchEntriesAsync(string searchTerm, GrimoireEntryType? type = null)
        {
            var allEntries = await GetAllEntriesAsync();
            var query = allEntries.AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(e => e.Type == type.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLowerInvariant();
                query = query.Where(e => 
                    e.Title.ToLowerInvariant().Contains(searchTerm) ||
                    e.Content.ToLowerInvariant().Contains(searchTerm) ||
                    e.Tags.Any(tag => tag.ToLowerInvariant().Contains(searchTerm)) ||
                    e.Metadata.Any(m => m.Value.ToLowerInvariant().Contains(searchTerm))
                );
            }

            return query.ToList();
        }

        public async Task<List<GrimoireEntry>> GetEntriesByTagsAsync(List<string> tags)
        {
            var allEntries = await GetAllEntriesAsync();
            return allEntries.Where(e => tags.Any(tag => e.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))).ToList();
        }

        public async Task<List<GrimoireEntry>> GetEntriesBySymbolAsync(string symbolId)
        {
            var allEntries = await GetAllEntriesAsync();
            return allEntries.Where(e => e.SymbolIds.Contains(symbolId)).ToList();
        }

        public async Task<List<GrimoireEntry>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var allEntries = await GetAllEntriesAsync();
            return allEntries.Where(e => e.CreatedDate >= startDate && e.CreatedDate <= endDate).ToList();
        }

        public async Task UpdateEntryAsync(GrimoireEntry entry)
        {
            entry.ModifiedDate = DateTime.Now;
            entry.Metadata["modified_timestamp"] = DateTime.Now.ToString("O");
            entry.Metadata["content_length"] = entry.Content.Length.ToString();

            await SaveEntryAsync(entry);
            await UpdateMetadataAsync();
        }

        public async Task DeleteEntryAsync(string id)
        {
            var encryptedPath = Path.Combine(GrimoireDirectory, EncryptedDirectory, $"{id}.enc");
            
            if (File.Exists(encryptedPath))
            {
                File.Delete(encryptedPath);
                await UpdateMetadataAsync();
            }
        }

        public async Task<List<string>> GetAllTagsAsync()
        {
            var metadata = await LoadMetadataAsync();
            return metadata.AllTags.OrderBy(t => t).ToList();
        }

        public async Task<List<string>> GetAllSymbolIdsAsync()
        {
            var metadata = await LoadMetadataAsync();
            return metadata.AllSymbolIds.OrderBy(s => s).ToList();
        }

        public async Task<GrimoireMetadata> GetMetadataAsync()
        {
            return await LoadMetadataAsync();
        }

        public async Task CreateBackupAsync(string backupName = "")
        {
            if (string.IsNullOrEmpty(backupName))
            {
                backupName = $"grimoire_backup_{DateTime.Now:yyyyMMdd_HHmmss}";
            }

            var backupPath = Path.Combine(GrimoireDirectory, BackupDirectory, backupName);
            Directory.CreateDirectory(backupPath);

            // Copy encrypted files
            var encryptedDir = Path.Combine(GrimoireDirectory, EncryptedDirectory);
            var backupEncryptedDir = Path.Combine(backupPath, EncryptedDirectory);
            Directory.CreateDirectory(backupEncryptedDir);

            if (Directory.Exists(encryptedDir))
            {
                foreach (var file in Directory.GetFiles(encryptedDir, "*.enc"))
                {
                    var fileName = Path.GetFileName(file);
                    var destPath = Path.Combine(backupEncryptedDir, fileName);
                    File.Copy(file, destPath, true);
                }
            }

            // Copy metadata
            var metadataPath = Path.Combine(GrimoireDirectory, MetadataFile);
            var backupMetadataPath = Path.Combine(backupPath, MetadataFile);
            if (File.Exists(metadataPath))
            {
                File.Copy(metadataPath, backupMetadataPath, true);
            }

            // Update metadata with backup info
            var metadata = await LoadMetadataAsync();
            metadata.LastBackupDate = DateTime.Now.ToString("O");
            await SaveMetadataAsync(metadata);
        }

        public async Task RestoreFromBackupAsync(string backupName)
        {
            var backupPath = Path.Combine(GrimoireDirectory, BackupDirectory, backupName);
            
            if (!Directory.Exists(backupPath))
                throw new DirectoryNotFoundException($"Backup {backupName} not found");

            // Restore encrypted files
            var backupEncryptedDir = Path.Combine(backupPath, EncryptedDirectory);
            var encryptedDir = Path.Combine(GrimoireDirectory, EncryptedDirectory);
            
            if (Directory.Exists(backupEncryptedDir))
            {
                // Clear existing encrypted files
                if (Directory.Exists(encryptedDir))
                {
                    Directory.Delete(encryptedDir, true);
                }
                Directory.CreateDirectory(encryptedDir);

                // Copy backup files
                foreach (var file in Directory.GetFiles(backupEncryptedDir, "*.enc"))
                {
                    var fileName = Path.GetFileName(file);
                    var destPath = Path.Combine(encryptedDir, fileName);
                    File.Copy(file, destPath, true);
                }
            }

            // Restore metadata
            var backupMetadataPath = Path.Combine(backupPath, MetadataFile);
            var metadataPath = Path.Combine(GrimoireDirectory, MetadataFile);
            if (File.Exists(backupMetadataPath))
            {
                File.Copy(backupMetadataPath, metadataPath, true);
            }
        }

        public async Task<List<string>> GetBackupListAsync()
        {
            var backupDir = Path.Combine(GrimoireDirectory, BackupDirectory);
            
            if (!Directory.Exists(backupDir))
                return new List<string>();

            return Directory.GetDirectories(backupDir)
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .OrderByDescending(name => name)
                .ToList();
        }

        private async Task SaveEntryAsync(GrimoireEntry entry)
        {
            var json = JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true });
            var data = Encoding.UTF8.GetBytes(json);
            var encryptedData = EncryptData(data);
            
            var encryptedPath = Path.Combine(GrimoireDirectory, EncryptedDirectory, $"{entry.Id}.enc");
            await File.WriteAllBytesAsync(encryptedPath, encryptedData);
        }

        private async Task UpdateMetadataAsync()
        {
            var allEntries = await GetAllEntriesAsync();
            var metadata = new GrimoireMetadata
            {
                LastModified = DateTime.Now,
                TotalEntries = allEntries.Count,
                TotalSize = allEntries.Sum(e => e.FileSize),
                AllTags = allEntries.SelectMany(e => e.Tags).Distinct().OrderBy(t => t).ToList(),
                AllSymbolIds = allEntries.SelectMany(e => e.SymbolIds).Distinct().OrderBy(s => s).ToList()
            };

            // Count entries by type
            foreach (var type in Enum.GetValues<GrimoireEntryType>())
            {
                metadata.EntryCounts[type] = allEntries.Count(e => e.Type == type);
            }

            await SaveMetadataAsync(metadata);
        }

        private async Task<GrimoireMetadata> LoadMetadataAsync()
        {
            var metadataPath = Path.Combine(GrimoireDirectory, MetadataFile);
            
            if (!File.Exists(metadataPath))
            {
                return new GrimoireMetadata();
            }

            try
            {
                var json = await File.ReadAllTextAsync(metadataPath);
                return JsonSerializer.Deserialize<GrimoireMetadata>(json) ?? new GrimoireMetadata();
            }
            catch
            {
                return new GrimoireMetadata();
            }
        }

        private async Task SaveMetadataAsync(GrimoireMetadata metadata)
        {
            var metadataPath = Path.Combine(GrimoireDirectory, MetadataFile);
            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(metadataPath, json);
        }

        private byte[] EncryptData(byte[] data)
        {
            using var aes = Aes.Create();
            aes.Key = DeriveKey();
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            // Write IV first
            msEncrypt.Write(aes.IV, 0, aes.IV.Length);
            
            // Write encrypted data
            csEncrypt.Write(data, 0, data.Length);
            csEncrypt.FlushFinalBlock();

            return msEncrypt.ToArray();
        }

        private byte[] DecryptData(byte[] encryptedData)
        {
            using var aes = Aes.Create();
            aes.Key = DeriveKey();

            // Read IV from the beginning
            var iv = new byte[16];
            Array.Copy(encryptedData, 0, iv, 0, 16);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(encryptedData, 16, encryptedData.Length - 16);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var resultStream = new MemoryStream();

            csDecrypt.CopyTo(resultStream);
            return resultStream.ToArray();
        }

        private byte[] DeriveKey()
        {
            using var deriveBytes = new Rfc2898DeriveBytes(_masterKey, Encoding.UTF8.GetBytes(_salt), 10000);
            return deriveBytes.GetBytes(32); // 256-bit key
        }

        private string GenerateSalt()
        {
            var salt = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        private string CalculateChecksum(byte[] data)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }
    }
} 