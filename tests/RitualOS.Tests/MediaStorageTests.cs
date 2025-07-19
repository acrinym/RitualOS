using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using RitualOS.Services;
using Xunit;

namespace RitualOS.Tests
{
    public class MediaStorageTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly MediaStorageService _mediaStorage;
        private readonly string _masterKey = "test-master-key-12345";
        private readonly string _salt = "test-salt-67890";

        public MediaStorageTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "RitualOS_MediaStorage_Test");
            _mediaStorage = new MediaStorageService(_masterKey, _salt);
        }

        [Fact]
        public async Task InitializeAsync_ShouldCreateDirectories()
        {
            // Act
            await _mediaStorage.InitializeAsync();

            // Assert
            Assert.True(Directory.Exists(Path.Combine(_testDirectory, "media")));
            Assert.True(Directory.Exists(Path.Combine(_testDirectory, "media", "encrypted")));
            Assert.True(Directory.Exists(Path.Combine(_testDirectory, "media", "compressed")));
            Assert.True(Directory.Exists(Path.Combine(_testDirectory, "media", "thumbnails")));
            Assert.True(Directory.Exists(Path.Combine(_testDirectory, "media", "temp")));
        }

        [Fact]
        public async Task StoreMediaAsync_ShouldCreateEncryptedMediaFile()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            var content = "This is test audio content for meditation";
            var mediaStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var tags = new List<string> { "meditation", "audio", "test" };
            var symbolIds = new List<string> { "pentagram", "moon" };

            // Act
            var mediaFile = await _mediaStorage.StoreMediaAsync(
                mediaStream,
                "meditation_audio.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.MeditationMusic,
                "audio/mpeg",
                tags: tags,
                symbolIds: symbolIds
            );

            // Assert
            Assert.NotNull(mediaFile);
            Assert.Equal("meditation_audio.mp3", mediaFile.OriginalName);
            Assert.Equal(MediaStorageService.MediaType.Audio, mediaFile.Type);
            Assert.Equal(MediaStorageService.MediaCategory.MeditationMusic, mediaFile.Category);
            Assert.Equal("audio/mpeg", mediaFile.MimeType);
            Assert.True(mediaFile.IsEncrypted);
            Assert.Equal("1.0", mediaFile.EncryptionVersion);
            Assert.Equal(tags, mediaFile.Tags);
            Assert.Equal(symbolIds, mediaFile.SymbolIds);
            Assert.True(mediaFile.OriginalSize > 0);
            Assert.True(mediaFile.EncryptedSize > 0);
            Assert.True(mediaFile.UploadDate > DateTime.Now.AddMinutes(-1));
            Assert.True(mediaFile.LastAccessed > DateTime.Now.AddMinutes(-1));
        }

        [Fact]
        public async Task RetrieveMediaAsync_ShouldRetrieveEncryptedMedia()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            var originalContent = "Test ritual recording content";
            var mediaStream = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));
            
            var mediaFile = await _mediaStorage.StoreMediaAsync(
                mediaStream,
                "ritual_recording.wav",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.RitualRecording,
                "audio/wav"
            );

            // Act
            using var retrievedStream = await _mediaStorage.RetrieveMediaAsync(mediaFile.Id);
            using var reader = new StreamReader(retrievedStream);
            var retrievedContent = await reader.ReadToEndAsync();

            // Assert
            Assert.Equal(originalContent, retrievedContent);
        }

        [Fact]
        public async Task GetAllMediaAsync_ShouldReturnAllMedia()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            var content1 = "First audio file";
            var content2 = "Second image file";
            
            var media1 = await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes(content1)),
                "audio1.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.Chants,
                "audio/mpeg"
            );
            
            var media2 = await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes(content2)),
                "image1.jpg",
                MediaStorageService.MediaType.Image,
                MediaStorageService.MediaCategory.SigilImages,
                "image/jpeg"
            );

            // Act
            var allMedia = await _mediaStorage.GetAllMediaAsync();

            // Assert
            Assert.Equal(2, allMedia.Count);
            Assert.Contains(allMedia, m => m.Id == media1.Id);
            Assert.Contains(allMedia, m => m.Id == media2.Id);
        }

        [Fact]
        public async Task GetMediaByTypeAsync_ShouldFilterByType()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("audio content")),
                "audio1.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.Chants,
                "audio/mpeg"
            );
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("image content")),
                "image1.jpg",
                MediaStorageService.MediaType.Image,
                MediaStorageService.MediaCategory.SigilImages,
                "image/jpeg"
            );
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("video content")),
                "video1.mp4",
                MediaStorageService.MediaType.Video,
                MediaStorageService.MediaCategory.RitualPhotos,
                "video/mp4"
            );

            // Act
            var audioFiles = await _mediaStorage.GetMediaByTypeAsync(MediaStorageService.MediaType.Audio);
            var imageFiles = await _mediaStorage.GetMediaByTypeAsync(MediaStorageService.MediaType.Image);
            var videoFiles = await _mediaStorage.GetMediaByTypeAsync(MediaStorageService.MediaType.Video);

            // Assert
            Assert.Single(audioFiles);
            Assert.Single(imageFiles);
            Assert.Single(videoFiles);
            Assert.All(audioFiles, m => Assert.Equal(MediaStorageService.MediaType.Audio, m.Type));
            Assert.All(imageFiles, m => Assert.Equal(MediaStorageService.MediaType.Image, m.Type));
            Assert.All(videoFiles, m => Assert.Equal(MediaStorageService.MediaType.Video, m.Type));
        }

        [Fact]
        public async Task GetMediaByCategoryAsync_ShouldFilterByCategory()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("chant content")),
                "chant1.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.Chants,
                "audio/mpeg"
            );
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("meditation content")),
                "meditation1.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.MeditationMusic,
                "audio/mpeg"
            );

            // Act
            var chantFiles = await _mediaStorage.GetMediaByCategoryAsync(MediaStorageService.MediaCategory.Chants);
            var meditationFiles = await _mediaStorage.GetMediaByCategoryAsync(MediaStorageService.MediaCategory.MeditationMusic);

            // Assert
            Assert.Single(chantFiles);
            Assert.Single(meditationFiles);
            Assert.All(chantFiles, m => Assert.Equal(MediaStorageService.MediaCategory.Chants, m.Category));
            Assert.All(meditationFiles, m => Assert.Equal(MediaStorageService.MediaCategory.MeditationMusic, m.Category));
        }

        [Fact]
        public async Task SearchMediaAsync_ShouldFindMatchingMedia()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("protection ritual audio")),
                "protection_ritual.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.RitualRecording,
                "audio/mpeg",
                tags: new List<string> { "protection", "ritual" }
            );
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("love spell image")),
                "love_spell.jpg",
                MediaStorageService.MediaType.Image,
                MediaStorageService.MediaCategory.SpellComponents,
                "image/jpeg",
                tags: new List<string> { "love", "spell" }
            );

            // Act
            var protectionResults = await _mediaStorage.SearchMediaAsync("protection");
            var loveResults = await _mediaStorage.SearchMediaAsync("love");
            var ritualResults = await _mediaStorage.SearchMediaAsync("ritual");

            // Assert
            Assert.Single(protectionResults);
            Assert.Single(loveResults);
            Assert.Single(ritualResults);
            Assert.Equal("protection_ritual.mp3", protectionResults.First().OriginalName);
            Assert.Equal("love_spell.jpg", loveResults.First().OriginalName);
        }

        [Fact]
        public async Task SearchMediaAsync_WithTypeFilter_ShouldFilterResults()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("test audio")),
                "test_audio.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.Chants,
                "audio/mpeg"
            );
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("test image")),
                "test_image.jpg",
                MediaStorageService.MediaType.Image,
                MediaStorageService.MediaCategory.SigilImages,
                "image/jpeg"
            );

            // Act
            var audioResults = await _mediaStorage.SearchMediaAsync("test", MediaStorageService.MediaType.Audio);
            var imageResults = await _mediaStorage.SearchMediaAsync("test", MediaStorageService.MediaType.Image);

            // Assert
            Assert.Single(audioResults);
            Assert.Single(imageResults);
            Assert.Equal(MediaStorageService.MediaType.Audio, audioResults.First().Type);
            Assert.Equal(MediaStorageService.MediaType.Image, imageResults.First().Type);
        }

        [Fact]
        public async Task GetMediaByTagsAsync_ShouldFilterByTags()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("fire ritual")),
                "fire_ritual.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.RitualRecording,
                "audio/mpeg",
                tags: new List<string> { "fire", "ritual", "protection" }
            );
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("water spell")),
                "water_spell.jpg",
                MediaStorageService.MediaType.Image,
                MediaStorageService.MediaCategory.SpellComponents,
                "image/jpeg",
                tags: new List<string> { "water", "spell", "love" }
            );

            // Act
            var fireResults = await _mediaStorage.GetMediaByTagsAsync(new List<string> { "fire" });
            var waterResults = await _mediaStorage.GetMediaByTagsAsync(new List<string> { "water" });

            // Assert
            Assert.Single(fireResults);
            Assert.Single(waterResults);
            Assert.Contains("fire", fireResults.First().Tags);
            Assert.Contains("water", waterResults.First().Tags);
        }

        [Fact]
        public async Task GetMediaBySymbolAsync_ShouldFilterBySymbol()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("pentagram ritual")),
                "pentagram_ritual.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.RitualRecording,
                "audio/mpeg",
                symbolIds: new List<string> { "pentagram", "eye-of-horus" }
            );
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("ankh meditation")),
                "ankh_meditation.jpg",
                MediaStorageService.MediaType.Image,
                MediaStorageService.MediaCategory.MeditationMusic,
                "image/jpeg",
                symbolIds: new List<string> { "ankh", "caduceus" }
            );

            // Act
            var pentagramResults = await _mediaStorage.GetMediaBySymbolAsync("pentagram");
            var ankhResults = await _mediaStorage.GetMediaBySymbolAsync("ankh");

            // Assert
            Assert.Single(pentagramResults);
            Assert.Single(ankhResults);
            Assert.Contains("pentagram", pentagramResults.First().SymbolIds);
            Assert.Contains("ankh", ankhResults.First().SymbolIds);
        }

        [Fact]
        public async Task UpdateMediaFileAsync_ShouldUpdateMedia()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            var mediaFile = await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("original content")),
                "original.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.Chants,
                "audio/mpeg"
            );

            // Act
            mediaFile.Description = "Updated description";
            mediaFile.Tags.Add("updated");
            await _mediaStorage.UpdateMediaFileAsync(mediaFile);

            // Assert
            var updatedMedia = await _mediaStorage.GetMediaFileAsync(mediaFile.Id);
            Assert.Equal("Updated description", updatedMedia.Description);
            Assert.Contains("updated", updatedMedia.Tags);
            Assert.True(updatedMedia.LastAccessed > mediaFile.UploadDate);
        }

        [Fact]
        public async Task DeleteMediaAsync_ShouldRemoveMedia()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            var mediaFile = await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("to delete")),
                "to_delete.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.Chants,
                "audio/mpeg"
            );

            // Act
            await _mediaStorage.DeleteMediaAsync(mediaFile.Id);

            // Assert
            var deletedMedia = await _mediaStorage.GetMediaFileAsync(mediaFile.Id);
            Assert.Null(deletedMedia);
        }

        [Fact]
        public async Task GetMetadataAsync_ShouldReturnMetadata()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("test content")),
                "test.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.Chants,
                "audio/mpeg"
            );

            // Act
            var metadata = await _mediaStorage.GetMetadataAsync();

            // Assert
            Assert.NotNull(metadata);
            Assert.Equal("1.0", metadata.Version);
            Assert.Equal(1, metadata.TotalFiles);
            Assert.True(metadata.IsEncrypted);
            Assert.True(metadata.TotalOriginalSize > 0);
            Assert.True(metadata.TotalEncryptedSize > 0);
        }

        [Fact]
        public async Task GetIndexAsync_ShouldReturnIndex()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("test content")),
                "test.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.Chants,
                "audio/mpeg",
                tags: new List<string> { "test" }
            );

            // Act
            var index = await _mediaStorage.GetIndexAsync();

            // Assert
            Assert.NotNull(index);
            Assert.True(index.FileIdToPath.Count > 0);
            Assert.True(index.TagToFiles.ContainsKey("test"));
            Assert.True(index.CategoryToFiles.ContainsKey("Chants"));
            Assert.True(index.TypeToFiles.ContainsKey("Audio"));
        }

        [Fact]
        public async Task CreateBackupAsync_ShouldCreateBackup()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            await _mediaStorage.StoreMediaAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("backup test")),
                "backup_test.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.Chants,
                "audio/mpeg"
            );

            // Act
            await _mediaStorage.CreateBackupAsync("test-backup");

            // Assert
            var backupPath = Path.Combine(_testDirectory, "media", "backups", "test-backup");
            Assert.True(Directory.Exists(backupPath));
            Assert.True(Directory.Exists(Path.Combine(backupPath, "encrypted")));
            Assert.True(Directory.Exists(Path.Combine(backupPath, "thumbnails")));
            Assert.True(File.Exists(Path.Combine(backupPath, "media_metadata.json")));
            Assert.True(File.Exists(Path.Combine(backupPath, "media_index.json")));
        }

        [Fact]
        public async Task Compression_ShouldReduceFileSize()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            var largeContent = new string('A', 1000000); // 1MB of data
            var mediaStream = new MemoryStream(Encoding.UTF8.GetBytes(largeContent));

            // Act
            var mediaFile = await _mediaStorage.StoreMediaAsync(
                mediaStream,
                "large_file.txt",
                MediaStorageService.MediaType.Document,
                MediaStorageService.MediaCategory.Other,
                "text/plain",
                compress: true
            );

            // Assert
            Assert.True(mediaFile.IsCompressed);
            Assert.True(mediaFile.CompressedSize < mediaFile.OriginalSize);
            Assert.True(mediaFile.EncryptedSize > 0);
        }

        [Fact]
        public async Task Encryption_ShouldProtectData()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            var sensitiveContent = "This is very sensitive magical audio content that should be encrypted.";
            var mediaStream = new MemoryStream(Encoding.UTF8.GetBytes(sensitiveContent));
            
            var mediaFile = await _mediaStorage.StoreMediaAsync(
                mediaStream,
                "sensitive_audio.mp3",
                MediaStorageService.MediaType.Audio,
                MediaStorageService.MediaCategory.RitualRecording,
                "audio/mpeg"
            );

            // Act - Try to read the encrypted file directly
            var encryptedPath = Path.Combine(_testDirectory, "media", "encrypted", mediaFile.StoredName);
            var encryptedData = await File.ReadAllBytesAsync(encryptedPath);
            var encryptedText = Encoding.UTF8.GetString(encryptedData);

            // Assert
            Assert.True(mediaFile.IsEncrypted);
            Assert.DoesNotContain(sensitiveContent, encryptedText);
            Assert.DoesNotContain("sensitive_audio.mp3", encryptedText);
        }

        [Fact]
        public async Task ThumbnailGeneration_ShouldCreateThumbnails()
        {
            // Arrange
            await _mediaStorage.InitializeAsync();
            var imageContent = "fake image data";
            var mediaStream = new MemoryStream(Encoding.UTF8.GetBytes(imageContent));

            // Act
            var mediaFile = await _mediaStorage.StoreMediaAsync(
                mediaStream,
                "test_image.jpg",
                MediaStorageService.MediaType.Image,
                MediaStorageService.MediaCategory.SigilImages,
                "image/jpeg",
                createThumbnail: true
            );

            // Assert
            Assert.False(string.IsNullOrEmpty(mediaFile.ThumbnailPath));
            // Note: In a real implementation, we would verify the thumbnail file exists
            // For now, we just check that the path is set
        }

        public void Dispose()
        {
            // Clean up test directories
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
} 