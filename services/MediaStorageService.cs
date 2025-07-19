using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace RitualOS.Services
{
    public class MediaStorageService
    {
        private const string MediaDirectory = "media";
        private const string EncryptedDirectory = "encrypted";
        private const string CompressedDirectory = "compressed";
        private const string ThumbnailsDirectory = "thumbnails";
        private const string MetadataFile = "media_metadata.json";
        private const string IndexFile = "media_index.json";
        private const string TempDirectory = "temp";
        
        private readonly string _masterKey;
        private readonly string _salt;
        private bool _isInitialized = false;

        public enum MediaType
        {
            [Display(Name = "Image")]
            Image,
            [Display(Name = "Audio")]
            Audio,
            [Display(Name = "Video")]
            Video,
            [Display(Name = "Document")]
            Document,
            [Display(Name = "SVG")]
            Svg,
            [Display(Name = "3D Model")]
            Model3D,
            [Display(Name = "Archive")]
            Archive,
            [Display(Name = "Other")]
            Other
        }

        public enum MediaCategory
        {
            [Display(Name = "Ritual Recording")]
            RitualRecording,
            [Display(Name = "Meditation Music")]
            MeditationMusic,
            [Display(Name = "Chants")]
            Chants,
            [Display(Name = "Dream Journal Audio")]
            DreamJournalAudio,
            [Display(Name = "Sigil Images")]
            SigilImages,
            [Display(Name = "Symbol Images")]
            SymbolImages,
            [Display(Name = "Ritual Photos")]
            RitualPhotos,
            [Display(Name = "Astral Travel Visuals")]
            AstralTravelVisuals,
            [Display(Name = "Divination Tools")]
            DivinationTools,
            [Display(Name = "Energy Work Visuals")]
            EnergyWorkVisuals,
            [Display(Name = "Spell Components")]
            SpellComponents,
            [Display(Name = "Other")]
            Other
        }

        public class MediaFile
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string OriginalName { get; set; } = string.Empty;
            public string StoredName { get; set; } = string.Empty;
            public MediaType Type { get; set; }
            public MediaCategory Category { get; set; }
            public string MimeType { get; set; } = string.Empty;
            public long OriginalSize { get; set; } = 0;
            public long CompressedSize { get; set; } = 0;
            public long EncryptedSize { get; set; } = 0;
            public DateTime UploadDate { get; set; } = DateTime.Now;
            public DateTime LastAccessed { get; set; } = DateTime.Now;
            public string Checksum { get; set; } = string.Empty;
            public bool IsEncrypted { get; set; } = true;
            public bool IsCompressed { get; set; } = false;
            public string EncryptionVersion { get; set; } = "1.0";
            public Dictionary<string, string> Metadata { get; set; } = new();
            public List<string> Tags { get; set; } = new();
            public List<string> RelatedEntries { get; set; } = new();
            public List<string> SymbolIds { get; set; } = new();
            public string Description { get; set; } = string.Empty;
            public string ThumbnailPath { get; set; } = string.Empty;
            public int Width { get; set; } = 0;  // For images/video
            public int Height { get; set; } = 0; // For images/video
            public TimeSpan Duration { get; set; } = TimeSpan.Zero; // For audio/video
            public int BitRate { get; set; } = 0; // For audio/video
            public string Format { get; set; } = string.Empty;
            public string Quality { get; set; } = string.Empty;
            public bool IsPublic { get; set; } = false;
            public string AccessLevel { get; set; } = "private";
            public long AccessCount { get; set; } = 0;
            public DateTime? ExpiryDate { get; set; } = null;
        }

        public class MediaMetadata
        {
            public string Version { get; set; } = "1.0";
            public DateTime CreatedDate { get; set; } = DateTime.Now;
            public DateTime LastModified { get; set; } = DateTime.Now;
            public int TotalFiles { get; set; } = 0;
            public long TotalOriginalSize { get; set; } = 0;
            public long TotalCompressedSize { get; set; } = 0;
            public long TotalEncryptedSize { get; set; } = 0;
            public Dictionary<MediaType, int> TypeCounts { get; set; } = new();
            public Dictionary<MediaCategory, int> CategoryCounts { get; set; } = new();
            public List<string> AllTags { get; set; } = new();
            public List<string> AllSymbolIds { get; set; } = new();
            public string LastBackupDate { get; set; } = string.Empty;
            public bool IsEncrypted { get; set; } = true;
            public long StorageEfficiency { get; set; } = 0; // Compression ratio
        }

        public class MediaIndex
        {
            public Dictionary<string, string> FileIdToPath { get; set; } = new();
            public Dictionary<string, List<string>> TagToFiles { get; set; } = new();
            public Dictionary<string, List<string>> SymbolToFiles { get; set; } = new();
            public Dictionary<string, List<string>> CategoryToFiles { get; set; } = new();
            public Dictionary<string, List<string>> TypeToFiles { get; set; } = new();
            public List<string> RecentlyAccessed { get; set; } = new();
            public List<string> MostAccessed { get; set; } = new();
            public DateTime LastIndexed { get; set; } = DateTime.Now;
        }

        public MediaStorageService(string masterKey, string salt = "")
        {
            _masterKey = masterKey;
            _salt = string.IsNullOrEmpty(salt) ? GenerateSalt() : salt;
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            // Create directory structure
            Directory.CreateDirectory(MediaDirectory);
            Directory.CreateDirectory(Path.Combine(MediaDirectory, EncryptedDirectory));
            Directory.CreateDirectory(Path.Combine(MediaDirectory, CompressedDirectory));
            Directory.CreateDirectory(Path.Combine(MediaDirectory, ThumbnailsDirectory));
            Directory.CreateDirectory(Path.Combine(MediaDirectory, TempDirectory));

            // Initialize metadata and index if they don't exist
            var metadataPath = Path.Combine(MediaDirectory, MetadataFile);
            if (!File.Exists(metadataPath))
            {
                var metadata = new MediaMetadata();
                await SaveMetadataAsync(metadata);
            }

            var indexPath = Path.Combine(MediaDirectory, IndexFile);
            if (!File.Exists(indexPath))
            {
                var index = new MediaIndex();
                await SaveIndexAsync(index);
            }

            _isInitialized = true;
        }

        public async Task<MediaFile> StoreMediaAsync(
            Stream mediaStream, 
            string originalName, 
            MediaType type, 
            MediaCategory category,
            string mimeType,
            Dictionary<string, string>? metadata = null,
            List<string>? tags = null,
            List<string>? symbolIds = null,
            bool compress = true,
            bool createThumbnail = true)
        {
            var mediaFile = new MediaFile
            {
                OriginalName = originalName,
                Type = type,
                Category = category,
                MimeType = mimeType,
                Tags = tags ?? new List<string>(),
                SymbolIds = symbolIds ?? new List<string>(),
                Metadata = metadata ?? new Dictionary<string, string>()
            };

            // Generate unique stored name
            mediaFile.StoredName = GenerateStoredFileName(originalName, type);
            
            // Read original data
            mediaStream.Position = 0;
            var originalData = new byte[mediaStream.Length];
            await mediaStream.ReadAsync(originalData, 0, originalData.Length);
            mediaFile.OriginalSize = originalData.Length;
            mediaFile.Checksum = CalculateChecksum(originalData);

            // Extract media properties
            await ExtractMediaPropertiesAsync(mediaFile, originalData);

            // Compress if requested and beneficial
            byte[] processedData = originalData;
            if (compress && ShouldCompress(type, originalData.Length))
            {
                processedData = await CompressDataAsync(originalData);
                mediaFile.IsCompressed = true;
                mediaFile.CompressedSize = processedData.Length;
            }
            else
            {
                mediaFile.CompressedSize = originalData.Length;
            }

            // Encrypt the data
            var encryptedData = EncryptData(processedData);
            mediaFile.EncryptedSize = encryptedData.Length;

            // Save encrypted file
            var encryptedPath = Path.Combine(MediaDirectory, EncryptedDirectory, mediaFile.StoredName);
            await File.WriteAllBytesAsync(encryptedPath, encryptedData);

            // Create thumbnail if requested
            if (createThumbnail && ShouldCreateThumbnail(type))
            {
                await CreateThumbnailAsync(mediaFile, originalData);
            }

            // Save media file metadata
            await SaveMediaFileAsync(mediaFile);
            await UpdateMetadataAsync();
            await UpdateIndexAsync(mediaFile);

            return mediaFile;
        }

        public async Task<Stream> RetrieveMediaAsync(string mediaId)
        {
            var mediaFile = await GetMediaFileAsync(mediaId);
            if (mediaFile == null)
                throw new FileNotFoundException($"Media file {mediaId} not found");

            // Update access statistics
            mediaFile.LastAccessed = DateTime.Now;
            mediaFile.AccessCount++;
            await SaveMediaFileAsync(mediaFile);

            // Read encrypted file
            var encryptedPath = Path.Combine(MediaDirectory, EncryptedDirectory, mediaFile.StoredName);
            var encryptedData = await File.ReadAllBytesAsync(encryptedPath);

            // Decrypt
            var decryptedData = DecryptData(encryptedData);

            // Decompress if necessary
            byte[] finalData;
            if (mediaFile.IsCompressed)
            {
                finalData = await DecompressDataAsync(decryptedData);
            }
            else
            {
                finalData = decryptedData;
            }

            return new MemoryStream(finalData);
        }

        public async Task<MediaFile?> GetMediaFileAsync(string mediaId)
        {
            var metadataPath = Path.Combine(MediaDirectory, "files", $"{mediaId}.json");
            
            if (!File.Exists(metadataPath))
                return null;

            try
            {
                var json = await File.ReadAllTextAsync(metadataPath);
                return JsonSerializer.Deserialize<MediaFile>(json);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<MediaFile>> GetAllMediaAsync()
        {
            var mediaFiles = new List<MediaFile>();
            var filesDir = Path.Combine(MediaDirectory, "files");
            
            if (!Directory.Exists(filesDir))
                return mediaFiles;

            var jsonFiles = Directory.GetFiles(filesDir, "*.json");
            
            foreach (var file in jsonFiles)
            {
                var mediaId = Path.GetFileNameWithoutExtension(file);
                var mediaFile = await GetMediaFileAsync(mediaId);
                if (mediaFile != null)
                {
                    mediaFiles.Add(mediaFile);
                }
            }

            return mediaFiles.OrderByDescending(m => m.LastAccessed).ToList();
        }

        public async Task<List<MediaFile>> GetMediaByTypeAsync(MediaType type)
        {
            var allMedia = await GetAllMediaAsync();
            return allMedia.Where(m => m.Type == type).ToList();
        }

        public async Task<List<MediaFile>> GetMediaByCategoryAsync(MediaCategory category)
        {
            var allMedia = await GetAllMediaAsync();
            return allMedia.Where(m => m.Category == category).ToList();
        }

        public async Task<List<MediaFile>> SearchMediaAsync(string searchTerm, MediaType? type = null)
        {
            var allMedia = await GetAllMediaAsync();
            var query = allMedia.AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(m => m.Type == type.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLowerInvariant();
                query = query.Where(m => 
                    m.OriginalName.ToLowerInvariant().Contains(searchTerm) ||
                    m.Description.ToLowerInvariant().Contains(searchTerm) ||
                    m.Tags.Any(tag => tag.ToLowerInvariant().Contains(searchTerm)) ||
                    m.Metadata.Any(meta => meta.Value.ToLowerInvariant().Contains(searchTerm))
                );
            }

            return query.ToList();
        }

        public async Task<List<MediaFile>> GetMediaByTagsAsync(List<string> tags)
        {
            var allMedia = await GetAllMediaAsync();
            return allMedia.Where(m => tags.Any(tag => m.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))).ToList();
        }

        public async Task<List<MediaFile>> GetMediaBySymbolAsync(string symbolId)
        {
            var allMedia = await GetAllMediaAsync();
            return allMedia.Where(m => m.SymbolIds.Contains(symbolId)).ToList();
        }

        public async Task UpdateMediaFileAsync(MediaFile mediaFile)
        {
            mediaFile.LastAccessed = DateTime.Now;
            await SaveMediaFileAsync(mediaFile);
            await UpdateIndexAsync(mediaFile);
        }

        public async Task DeleteMediaAsync(string mediaId)
        {
            var mediaFile = await GetMediaFileAsync(mediaId);
            if (mediaFile == null) return;

            // Delete encrypted file
            var encryptedPath = Path.Combine(MediaDirectory, EncryptedDirectory, mediaFile.StoredName);
            if (File.Exists(encryptedPath))
            {
                File.Delete(encryptedPath);
            }

            // Delete thumbnail
            if (!string.IsNullOrEmpty(mediaFile.ThumbnailPath) && File.Exists(mediaFile.ThumbnailPath))
            {
                File.Delete(mediaFile.ThumbnailPath);
            }

            // Delete metadata file
            var metadataPath = Path.Combine(MediaDirectory, "files", $"{mediaId}.json");
            if (File.Exists(metadataPath))
            {
                File.Delete(metadataPath);
            }

            await UpdateMetadataAsync();
            await UpdateIndexAsync(mediaFile, true); // Remove from index
        }

        public async Task<Stream?> GetThumbnailAsync(string mediaId)
        {
            var mediaFile = await GetMediaFileAsync(mediaId);
            if (mediaFile == null || string.IsNullOrEmpty(mediaFile.ThumbnailPath))
                return null;

            if (!File.Exists(mediaFile.ThumbnailPath))
                return null;

            return File.OpenRead(mediaFile.ThumbnailPath);
        }

        public async Task<MediaMetadata> GetMetadataAsync()
        {
            return await LoadMetadataAsync();
        }

        public async Task<MediaIndex> GetIndexAsync()
        {
            return await LoadIndexAsync();
        }

        public async Task CreateBackupAsync(string backupName = "")
        {
            if (string.IsNullOrEmpty(backupName))
            {
                backupName = $"media_backup_{DateTime.Now:yyyyMMdd_HHmmss}";
            }

            var backupPath = Path.Combine(MediaDirectory, "backups", backupName);
            Directory.CreateDirectory(backupPath);

            // Copy encrypted files
            var encryptedDir = Path.Combine(MediaDirectory, EncryptedDirectory);
            var backupEncryptedDir = Path.Combine(backupPath, EncryptedDirectory);
            Directory.CreateDirectory(backupEncryptedDir);

            if (Directory.Exists(encryptedDir))
            {
                foreach (var file in Directory.GetFiles(encryptedDir, "*.*"))
                {
                    var fileName = Path.GetFileName(file);
                    var destPath = Path.Combine(backupEncryptedDir, fileName);
                    File.Copy(file, destPath, true);
                }
            }

            // Copy thumbnails
            var thumbnailsDir = Path.Combine(MediaDirectory, ThumbnailsDirectory);
            var backupThumbnailsDir = Path.Combine(backupPath, ThumbnailsDirectory);
            Directory.CreateDirectory(backupThumbnailsDir);

            if (Directory.Exists(thumbnailsDir))
            {
                foreach (var file in Directory.GetFiles(thumbnailsDir, "*.*"))
                {
                    var fileName = Path.GetFileName(file);
                    var destPath = Path.Combine(backupThumbnailsDir, fileName);
                    File.Copy(file, destPath, true);
                }
            }

            // Copy metadata and index
            var metadataPath = Path.Combine(MediaDirectory, MetadataFile);
            var backupMetadataPath = Path.Combine(backupPath, MetadataFile);
            if (File.Exists(metadataPath))
            {
                File.Copy(metadataPath, backupMetadataPath, true);
            }

            var indexPath = Path.Combine(MediaDirectory, IndexFile);
            var backupIndexPath = Path.Combine(backupPath, IndexFile);
            if (File.Exists(indexPath))
            {
                File.Copy(indexPath, backupIndexPath, true);
            }

            // Copy file metadata
            var filesDir = Path.Combine(MediaDirectory, "files");
            var backupFilesDir = Path.Combine(backupPath, "files");
            Directory.CreateDirectory(backupFilesDir);

            if (Directory.Exists(filesDir))
            {
                foreach (var file in Directory.GetFiles(filesDir, "*.json"))
                {
                    var fileName = Path.GetFileName(file);
                    var destPath = Path.Combine(backupFilesDir, fileName);
                    File.Copy(file, destPath, true);
                }
            }

            // Update metadata
            var metadata = await LoadMetadataAsync();
            metadata.LastBackupDate = DateTime.Now.ToString("O");
            await SaveMetadataAsync(metadata);
        }

        private async Task SaveMediaFileAsync(MediaFile mediaFile)
        {
            var filesDir = Path.Combine(MediaDirectory, "files");
            Directory.CreateDirectory(filesDir);
            
            var filePath = Path.Combine(filesDir, $"{mediaFile.Id}.json");
            var json = JsonSerializer.Serialize(mediaFile, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        private async Task UpdateMetadataAsync()
        {
            var allMedia = await GetAllMediaAsync();
            var metadata = new MediaMetadata
            {
                LastModified = DateTime.Now,
                TotalFiles = allMedia.Count,
                TotalOriginalSize = allMedia.Sum(m => m.OriginalSize),
                TotalCompressedSize = allMedia.Sum(m => m.CompressedSize),
                TotalEncryptedSize = allMedia.Sum(m => m.EncryptedSize),
                AllTags = allMedia.SelectMany(m => m.Tags).Distinct().OrderBy(t => t).ToList(),
                AllSymbolIds = allMedia.SelectMany(m => m.SymbolIds).Distinct().OrderBy(s => s).ToList()
            };

            // Calculate storage efficiency
            if (metadata.TotalOriginalSize > 0)
            {
                metadata.StorageEfficiency = (long)((double)metadata.TotalCompressedSize / metadata.TotalOriginalSize * 100);
            }

            // Count by type and category
            foreach (var type in Enum.GetValues<MediaType>())
            {
                metadata.TypeCounts[type] = allMedia.Count(m => m.Type == type);
            }

            foreach (var category in Enum.GetValues<MediaCategory>())
            {
                metadata.CategoryCounts[category] = allMedia.Count(m => m.Category == category);
            }

            await SaveMetadataAsync(metadata);
        }

        private async Task UpdateIndexAsync(MediaFile mediaFile, bool remove = false)
        {
            var index = await LoadIndexAsync();

            if (remove)
            {
                // Remove from all indexes
                index.FileIdToPath.Remove(mediaFile.Id);
                foreach (var tag in mediaFile.Tags)
                {
                    if (index.TagToFiles.ContainsKey(tag))
                    {
                        index.TagToFiles[tag].Remove(mediaFile.Id);
                    }
                }
                foreach (var symbolId in mediaFile.SymbolIds)
                {
                    if (index.SymbolToFiles.ContainsKey(symbolId))
                    {
                        index.SymbolToFiles[symbolId].Remove(mediaFile.Id);
                    }
                }
                index.CategoryToFiles[mediaFile.Category.ToString()].Remove(mediaFile.Id);
                index.TypeToFiles[mediaFile.Type.ToString()].Remove(mediaFile.Id);
                index.RecentlyAccessed.Remove(mediaFile.Id);
                index.MostAccessed.Remove(mediaFile.Id);
            }
            else
            {
                // Add to indexes
                index.FileIdToPath[mediaFile.Id] = mediaFile.StoredName;
                
                foreach (var tag in mediaFile.Tags)
                {
                    if (!index.TagToFiles.ContainsKey(tag))
                        index.TagToFiles[tag] = new List<string>();
                    if (!index.TagToFiles[tag].Contains(mediaFile.Id))
                        index.TagToFiles[tag].Add(mediaFile.Id);
                }

                foreach (var symbolId in mediaFile.SymbolIds)
                {
                    if (!index.SymbolToFiles.ContainsKey(symbolId))
                        index.SymbolToFiles[symbolId] = new List<string>();
                    if (!index.SymbolToFiles[symbolId].Contains(mediaFile.Id))
                        index.SymbolToFiles[symbolId].Add(mediaFile.Id);
                }

                var categoryKey = mediaFile.Category.ToString();
                if (!index.CategoryToFiles.ContainsKey(categoryKey))
                    index.CategoryToFiles[categoryKey] = new List<string>();
                if (!index.CategoryToFiles[categoryKey].Contains(mediaFile.Id))
                    index.CategoryToFiles[categoryKey].Add(mediaFile.Id);

                var typeKey = mediaFile.Type.ToString();
                if (!index.TypeToFiles.ContainsKey(typeKey))
                    index.TypeToFiles[typeKey] = new List<string>();
                if (!index.TypeToFiles[typeKey].Contains(mediaFile.Id))
                    index.TypeToFiles[typeKey].Add(mediaFile.Id);

                // Update recently accessed
                index.RecentlyAccessed.Remove(mediaFile.Id);
                index.RecentlyAccessed.Insert(0, mediaFile.Id);
                if (index.RecentlyAccessed.Count > 100)
                    index.RecentlyAccessed.RemoveAt(index.RecentlyAccessed.Count - 1);
            }

            index.LastIndexed = DateTime.Now;
            await SaveIndexAsync(index);
        }

        private async Task<MediaMetadata> LoadMetadataAsync()
        {
            var metadataPath = Path.Combine(MediaDirectory, MetadataFile);
            
            if (!File.Exists(metadataPath))
            {
                return new MediaMetadata();
            }

            try
            {
                var json = await File.ReadAllTextAsync(metadataPath);
                return JsonSerializer.Deserialize<MediaMetadata>(json) ?? new MediaMetadata();
            }
            catch
            {
                return new MediaMetadata();
            }
        }

        private async Task SaveMetadataAsync(MediaMetadata metadata)
        {
            var metadataPath = Path.Combine(MediaDirectory, MetadataFile);
            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(metadataPath, json);
        }

        private async Task<MediaIndex> LoadIndexAsync()
        {
            var indexPath = Path.Combine(MediaDirectory, IndexFile);
            
            if (!File.Exists(indexPath))
            {
                return new MediaIndex();
            }

            try
            {
                var json = await File.ReadAllTextAsync(indexPath);
                return JsonSerializer.Deserialize<MediaIndex>(json) ?? new MediaIndex();
            }
            catch
            {
                return new MediaIndex();
            }
        }

        private async Task SaveIndexAsync(MediaIndex index)
        {
            var indexPath = Path.Combine(MediaDirectory, IndexFile);
            var json = JsonSerializer.Serialize(index, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(indexPath, json);
        }

        private string GenerateStoredFileName(string originalName, MediaType type)
        {
            var extension = Path.GetExtension(originalName);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var random = Guid.NewGuid().ToString("N").Substring(0, 8);
            return $"{type}_{timestamp}_{random}{extension}";
        }

        private async Task ExtractMediaPropertiesAsync(MediaFile mediaFile, byte[] data)
        {
            // TODO: Implement media property extraction
            // This would use libraries like ImageSharp for images, NAudio for audio, etc.
            // For now, we'll set basic properties
            
            mediaFile.Format = Path.GetExtension(mediaFile.OriginalName).ToLowerInvariant();
            
            // Placeholder for actual media analysis
            if (mediaFile.Type == MediaType.Image)
            {
                mediaFile.Width = 1920; // Placeholder
                mediaFile.Height = 1080; // Placeholder
            }
            else if (mediaFile.Type == MediaType.Audio || mediaFile.Type == MediaType.Video)
            {
                mediaFile.Duration = TimeSpan.FromMinutes(5); // Placeholder
                mediaFile.BitRate = 320000; // Placeholder
            }
        }

        private bool ShouldCompress(MediaType type, long size)
        {
            // Don't compress already compressed formats
            if (type == MediaType.Audio && (size > 10 * 1024 * 1024)) // > 10MB
                return true;
            if (type == MediaType.Video && (size > 50 * 1024 * 1024)) // > 50MB
                return true;
            if (type == MediaType.Document && (size > 1 * 1024 * 1024)) // > 1MB
                return true;
            
            return false;
        }

        private async Task<byte[]> CompressDataAsync(byte[] data)
        {
            using var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                await gzipStream.WriteAsync(data, 0, data.Length);
            }
            return outputStream.ToArray();
        }

        private async Task<byte[]> DecompressDataAsync(byte[] compressedData)
        {
            using var inputStream = new MemoryStream(compressedData);
            using var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                await gzipStream.CopyToAsync(outputStream);
            }
            return outputStream.ToArray();
        }

        private bool ShouldCreateThumbnail(MediaType type)
        {
            return type == MediaType.Image || type == MediaType.Video;
        }

        private async Task CreateThumbnailAsync(MediaFile mediaFile, byte[] originalData)
        {
            // TODO: Implement thumbnail creation
            // This would use ImageSharp or similar library to create thumbnails
            // For now, we'll just set a placeholder path
            
            var thumbnailName = $"thumb_{mediaFile.Id}.jpg";
            mediaFile.ThumbnailPath = Path.Combine(MediaDirectory, ThumbnailsDirectory, thumbnailName);
            
            // Placeholder: create a simple thumbnail file
            var thumbnailData = new byte[1024]; // Placeholder thumbnail data
            await File.WriteAllBytesAsync(mediaFile.ThumbnailPath, thumbnailData);
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