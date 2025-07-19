using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using RitualOS.Services;
using Xunit;

namespace RitualOS.Tests
{
    public class GrimoireFSTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly GrimoireFS _grimoireFS;
        private readonly string _masterKey = "test-master-key-12345";
        private readonly string _salt = "test-salt-67890";

        public GrimoireFSTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "RitualOS_GrimoireFS_Test");
            _grimoireFS = new GrimoireFS(_masterKey, _salt);
        }

        [Fact]
        public async Task InitializeAsync_ShouldCreateDirectories()
        {
            // Act
            await _grimoireFS.InitializeAsync();

            // Assert
            Assert.True(Directory.Exists(Path.Combine(_testDirectory, "grimoire")));
            Assert.True(Directory.Exists(Path.Combine(_testDirectory, "grimoire", "encrypted")));
            Assert.True(Directory.Exists(Path.Combine(_testDirectory, "grimoire", "backups")));
            Assert.True(Directory.Exists(Path.Combine(_testDirectory, "grimoire", "temp")));
        }

        [Fact]
        public async Task CreateEntryAsync_ShouldCreateEncryptedEntry()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            var title = "Test Ritual";
            var content = "This is a test ritual entry with sensitive information.";
            var tags = new List<string> { "test", "ritual", "protection" };
            var symbolIds = new List<string> { "pentagram", "eye-of-horus" };

            // Act
            var entry = await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                title,
                content,
                tags: tags,
                symbolIds: symbolIds
            );

            // Assert
            Assert.NotNull(entry);
            Assert.Equal(title, entry.Title);
            Assert.Equal(content, entry.Content);
            Assert.Equal(GrimoireFS.GrimoireEntryType.RitualLog, entry.Type);
            Assert.True(entry.IsEncrypted);
            Assert.Equal("1.0", entry.EncryptionVersion);
            Assert.Equal(tags, entry.Tags);
            Assert.Equal(symbolIds, entry.SymbolIds);
            Assert.True(entry.CreatedDate > DateTime.Now.AddMinutes(-1));
            Assert.True(entry.ModifiedDate > DateTime.Now.AddMinutes(-1));
        }

        [Fact]
        public async Task GetEntryAsync_ShouldRetrieveEncryptedEntry()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            var entry = await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.DreamEntry,
                "Test Dream",
                "I dreamed of flying over ancient temples..."
            );

            // Act
            var retrievedEntry = await _grimoireFS.GetEntryAsync(entry.Id);

            // Assert
            Assert.NotNull(retrievedEntry);
            Assert.Equal(entry.Id, retrievedEntry.Id);
            Assert.Equal(entry.Title, retrievedEntry.Title);
            Assert.Equal(entry.Content, retrievedEntry.Content);
            Assert.Equal(entry.Type, retrievedEntry.Type);
        }

        [Fact]
        public async Task GetAllEntriesAsync_ShouldReturnAllEntries()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            var entry1 = await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Ritual 1",
                "First ritual"
            );
            var entry2 = await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.DreamEntry,
                "Dream 1",
                "First dream"
            );

            // Act
            var allEntries = await _grimoireFS.GetAllEntriesAsync();

            // Assert
            Assert.Equal(2, allEntries.Count);
            Assert.Contains(allEntries, e => e.Id == entry1.Id);
            Assert.Contains(allEntries, e => e.Id == entry2.Id);
        }

        [Fact]
        public async Task GetEntriesByTypeAsync_ShouldFilterByType()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Ritual 1",
                "First ritual"
            );
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.DreamEntry,
                "Dream 1",
                "First dream"
            );
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Ritual 2",
                "Second ritual"
            );

            // Act
            var ritualEntries = await _grimoireFS.GetEntriesByTypeAsync(GrimoireFS.GrimoireEntryType.RitualLog);
            var dreamEntries = await _grimoireFS.GetEntriesByTypeAsync(GrimoireFS.GrimoireEntryType.DreamEntry);

            // Assert
            Assert.Equal(2, ritualEntries.Count);
            Assert.Equal(1, dreamEntries.Count);
            Assert.All(ritualEntries, e => Assert.Equal(GrimoireFS.GrimoireEntryType.RitualLog, e.Type));
            Assert.All(dreamEntries, e => Assert.Equal(GrimoireFS.GrimoireEntryType.DreamEntry, e.Type));
        }

        [Fact]
        public async Task SearchEntriesAsync_ShouldFindMatchingEntries()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Protection Ritual",
                "A ritual for protection using pentagram"
            );
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.DreamEntry,
                "Flying Dream",
                "I dreamed of flying over ancient temples"
            );

            // Act
            var protectionResults = await _grimoireFS.SearchEntriesAsync("protection");
            var flyingResults = await _grimoireFS.SearchEntriesAsync("flying");
            var pentagramResults = await _grimoireFS.SearchEntriesAsync("pentagram");

            // Assert
            Assert.Single(protectionResults);
            Assert.Single(flyingResults);
            Assert.Single(pentagramResults);
            Assert.Equal("Protection Ritual", protectionResults.First().Title);
            Assert.Equal("Flying Dream", flyingResults.First().Title);
        }

        [Fact]
        public async Task SearchEntriesAsync_WithTypeFilter_ShouldFilterResults()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Test Ritual",
                "A test ritual"
            );
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.DreamEntry,
                "Test Dream",
                "A test dream"
            );

            // Act
            var ritualResults = await _grimoireFS.SearchEntriesAsync("test", GrimoireFS.GrimoireEntryType.RitualLog);
            var dreamResults = await _grimoireFS.SearchEntriesAsync("test", GrimoireFS.GrimoireEntryType.DreamEntry);

            // Assert
            Assert.Single(ritualResults);
            Assert.Single(dreamResults);
            Assert.Equal(GrimoireFS.GrimoireEntryType.RitualLog, ritualResults.First().Type);
            Assert.Equal(GrimoireFS.GrimoireEntryType.DreamEntry, dreamResults.First().Type);
        }

        [Fact]
        public async Task GetEntriesByTagsAsync_ShouldFilterByTags()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Ritual 1",
                "First ritual",
                tags: new List<string> { "protection", "fire" }
            );
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Ritual 2",
                "Second ritual",
                tags: new List<string> { "love", "water" }
            );

            // Act
            var protectionResults = await _grimoireFS.GetEntriesByTagsAsync(new List<string> { "protection" });
            var loveResults = await _grimoireFS.GetEntriesByTagsAsync(new List<string> { "love" });

            // Assert
            Assert.Single(protectionResults);
            Assert.Single(loveResults);
            Assert.Contains("protection", protectionResults.First().Tags);
            Assert.Contains("love", loveResults.First().Tags);
        }

        [Fact]
        public async Task GetEntriesBySymbolAsync_ShouldFilterBySymbol()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Ritual 1",
                "First ritual",
                symbolIds: new List<string> { "pentagram", "eye-of-horus" }
            );
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Ritual 2",
                "Second ritual",
                symbolIds: new List<string> { "ankh", "caduceus" }
            );

            // Act
            var pentagramResults = await _grimoireFS.GetEntriesBySymbolAsync("pentagram");
            var ankhResults = await _grimoireFS.GetEntriesBySymbolAsync("ankh");

            // Assert
            Assert.Single(pentagramResults);
            Assert.Single(ankhResults);
            Assert.Contains("pentagram", pentagramResults.First().SymbolIds);
            Assert.Contains("ankh", ankhResults.First().SymbolIds);
        }

        [Fact]
        public async Task GetEntriesByDateRangeAsync_ShouldFilterByDate()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            var yesterday = DateTime.Now.AddDays(-1);
            var tomorrow = DateTime.Now.AddDays(1);

            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Past Ritual",
                "A ritual from the past"
            );

            // Act
            var pastResults = await _grimoireFS.GetEntriesByDateRangeAsync(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1));
            var futureResults = await _grimoireFS.GetEntriesByDateRangeAsync(DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));

            // Assert
            Assert.Empty(pastResults);
            Assert.Empty(futureResults);
        }

        [Fact]
        public async Task UpdateEntryAsync_ShouldUpdateEntry()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            var entry = await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Original Title",
                "Original content"
            );

            // Act
            entry.Title = "Updated Title";
            entry.Content = "Updated content";
            entry.Tags.Add("updated");
            await _grimoireFS.UpdateEntryAsync(entry);

            // Assert
            var updatedEntry = await _grimoireFS.GetEntryAsync(entry.Id);
            Assert.Equal("Updated Title", updatedEntry.Title);
            Assert.Equal("Updated content", updatedEntry.Content);
            Assert.Contains("updated", updatedEntry.Tags);
            Assert.True(updatedEntry.ModifiedDate > entry.CreatedDate);
        }

        [Fact]
        public async Task DeleteEntryAsync_ShouldRemoveEntry()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            var entry = await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "To Delete",
                "This will be deleted"
            );

            // Act
            await _grimoireFS.DeleteEntryAsync(entry.Id);

            // Assert
            var deletedEntry = await _grimoireFS.GetEntryAsync(entry.Id);
            Assert.Null(deletedEntry);
        }

        [Fact]
        public async Task GetAllTagsAsync_ShouldReturnUniqueTags()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Ritual 1",
                "First ritual",
                tags: new List<string> { "protection", "fire" }
            );
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Ritual 2",
                "Second ritual",
                tags: new List<string> { "protection", "water" }
            );

            // Act
            var allTags = await _grimoireFS.GetAllTagsAsync();

            // Assert
            Assert.Equal(3, allTags.Count);
            Assert.Contains("protection", allTags);
            Assert.Contains("fire", allTags);
            Assert.Contains("water", allTags);
        }

        [Fact]
        public async Task GetAllSymbolIdsAsync_ShouldReturnUniqueSymbolIds()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Ritual 1",
                "First ritual",
                symbolIds: new List<string> { "pentagram", "eye-of-horus" }
            );
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Ritual 2",
                "Second ritual",
                symbolIds: new List<string> { "pentagram", "ankh" }
            );

            // Act
            var allSymbolIds = await _grimoireFS.GetAllSymbolIdsAsync();

            // Assert
            Assert.Equal(3, allSymbolIds.Count);
            Assert.Contains("pentagram", allSymbolIds);
            Assert.Contains("eye-of-horus", allSymbolIds);
            Assert.Contains("ankh", allSymbolIds);
        }

        [Fact]
        public async Task GetMetadataAsync_ShouldReturnMetadata()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Test Entry",
                "Test content"
            );

            // Act
            var metadata = await _grimoireFS.GetMetadataAsync();

            // Assert
            Assert.NotNull(metadata);
            Assert.Equal("1.0", metadata.Version);
            Assert.Equal(1, metadata.TotalEntries);
            Assert.True(metadata.IsEncrypted);
            Assert.True(metadata.TotalSize > 0);
        }

        [Fact]
        public async Task CreateBackupAsync_ShouldCreateBackup()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Test Entry",
                "Test content"
            );

            // Act
            await _grimoireFS.CreateBackupAsync("test-backup");

            // Assert
            var backups = await _grimoireFS.GetBackupListAsync();
            Assert.Contains("test-backup", backups);
        }

        [Fact]
        public async Task RestoreFromBackupAsync_ShouldRestoreData()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            var entry = await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Test Entry",
                "Test content"
            );
            await _grimoireFS.CreateBackupAsync("test-backup");
            await _grimoireFS.DeleteEntryAsync(entry.Id);

            // Act
            await _grimoireFS.RestoreFromBackupAsync("test-backup");

            // Assert
            var restoredEntry = await _grimoireFS.GetEntryAsync(entry.Id);
            Assert.NotNull(restoredEntry);
            Assert.Equal("Test Entry", restoredEntry.Title);
        }

        [Fact]
        public async Task Encryption_ShouldProtectData()
        {
            // Arrange
            await _grimoireFS.InitializeAsync();
            var sensitiveContent = "This is very sensitive magical information that should be encrypted.";
            var entry = await _grimoireFS.CreateEntryAsync(
                GrimoireFS.GrimoireEntryType.RitualLog,
                "Sensitive Ritual",
                sensitiveContent
            );

            // Act - Try to read the encrypted file directly
            var encryptedPath = Path.Combine("grimoire", "encrypted", $"{entry.Id}.enc");
            var encryptedData = await File.ReadAllBytesAsync(encryptedPath);
            var encryptedText = System.Text.Encoding.UTF8.GetString(encryptedData);

            // Assert
            Assert.True(entry.IsEncrypted);
            Assert.DoesNotContain(sensitiveContent, encryptedText);
            Assert.DoesNotContain("Sensitive Ritual", encryptedText);
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