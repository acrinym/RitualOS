# 🔐 GrimoireFS - Encrypted Magical Data Storage

GrimoireFS is the foundational encrypted file system for RitualOS, providing secure storage for all magical and ritual data. It's designed to be offline-first, highly secure, and seamlessly integrated with the rest of the RitualOS ecosystem.

## 🌟 Features

### **Security & Encryption**
- **AES-256 Encryption** - Military-grade encryption for all data
- **PBKDF2 Key Derivation** - 10,000 iterations for key security
- **Unique IV per Entry** - Prevents pattern analysis
- **Checksum Verification** - Ensures data integrity
- **Salt-based Security** - Protects against rainbow table attacks

### **Data Types Supported**
- **Ritual Logs** - Detailed ritual performance records
- **Dream Entries** - Dream journal with symbol associations
- **Spell Templates** - Reusable spell frameworks
- **Personal Sigils** - Custom sigil collections
- **Journal Entries** - General magical journaling
- **Meditation Sessions** - Meditation and trance records
- **Divination Readings** - Tarot, runes, scrying results
- **Energy Work** - Energy manipulation records
- **Astral Travel** - Out-of-body experience logs
- **Other** - Custom entry types

### **Advanced Features**
- **Rich Metadata** - Moon phases, planetary influences, weather, location
- **Tag System** - Flexible categorization and organization
- **Symbol Integration** - Link entries to SymbolWiki symbols
- **Cross-References** - Link related entries together
- **Search & Filter** - Full-text search with type and tag filtering
- **Date Range Queries** - Time-based filtering and analysis
- **Backup & Restore** - Encrypted backup system
- **Export Capabilities** - Secure data export

## 🚀 Quick Start

### **Basic Usage**

```csharp
// Initialize GrimoireFS with a secure master key
var masterKey = "your-secure-master-key-here";
var grimoireFS = new GrimoireFS(masterKey);

// Initialize the system
await grimoireFS.InitializeAsync();

// Create your first ritual entry
var ritualEntry = await grimoireFS.CreateEntryAsync(
    GrimoireFS.GrimoireEntryType.RitualLog,
    "Full Moon Protection Ritual",
    "Performed a protection ritual during the full moon...",
    tags: new List<string> { "protection", "full-moon", "pentagram" },
    symbolIds: new List<string> { "pentagram", "eye-of-horus" }
);

// Search for entries
var protectionRituals = await grimoireFS.SearchEntriesAsync("protection");
var moonRituals = await grimoireFS.GetEntriesByTagsAsync(new List<string> { "full-moon" });
```

### **UI Integration**

```csharp
// In your ViewModel
public class MainViewModel : ViewModelBase
{
    private readonly GrimoireFS _grimoireFS;
    public GrimoireFSViewModel GrimoireViewModel { get; }

    public MainViewModel()
    {
        var masterKey = GetSecureMasterKey(); // Implement secure key retrieval
        _grimoireFS = new GrimoireFS(masterKey);
        GrimoireViewModel = new GrimoireFSViewModel(_grimoireFS);
    }
}
```

## 📁 File Structure

```
grimoire/
├── encrypted/           # Encrypted entry files (.enc)
├── backups/            # Encrypted backup archives
├── temp/               # Temporary processing files
└── metadata.json       # System metadata (unencrypted)
```

### **Entry File Format**
Each entry is stored as an encrypted JSON file:
- **Filename**: `{entry-id}.enc`
- **Format**: AES-256 encrypted JSON
- **Content**: Full GrimoireEntry object with metadata

## 🔧 API Reference

### **Core Methods**

#### **Initialization**
```csharp
Task InitializeAsync()
```
Creates directory structure and initializes metadata.

#### **Entry Management**
```csharp
Task<GrimoireEntry> CreateEntryAsync(
    GrimoireEntryType type,
    string title,
    string content,
    Dictionary<string, string>? metadata = null,
    List<string>? tags = null,
    List<string>? symbolIds = null
)

Task<GrimoireEntry?> GetEntryAsync(string id)
Task UpdateEntryAsync(GrimoireEntry entry)
Task DeleteEntryAsync(string id)
```

#### **Query Methods**
```csharp
Task<List<GrimoireEntry>> GetAllEntriesAsync()
Task<List<GrimoireEntry>> GetEntriesByTypeAsync(GrimoireEntryType type)
Task<List<GrimoireEntry>> SearchEntriesAsync(string searchTerm, GrimoireEntryType? type = null)
Task<List<GrimoireEntry>> GetEntriesByTagsAsync(List<string> tags)
Task<List<GrimoireEntry>> GetEntriesBySymbolAsync(string symbolId)
Task<List<GrimoireEntry>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
```

#### **Metadata & Statistics**
```csharp
Task<List<string>> GetAllTagsAsync()
Task<List<string>> GetAllSymbolIdsAsync()
Task<GrimoireMetadata> GetMetadataAsync()
```

#### **Backup & Restore**
```csharp
Task CreateBackupAsync(string backupName = "")
Task RestoreFromBackupAsync(string backupName)
Task<List<string>> GetBackupListAsync()
```

### **Data Models**

#### **GrimoireEntry**
```csharp
public class GrimoireEntry
{
    public string Id { get; set; }                    // Unique identifier
    public GrimoireEntryType Type { get; set; }       // Entry type
    public string Title { get; set; }                 // Entry title
    public string Content { get; set; }               // Main content
    public DateTime CreatedDate { get; set; }         // Creation timestamp
    public DateTime ModifiedDate { get; set; }        // Last modification
    public DateTime? RitualDate { get; set; }         // Optional ritual date
    public List<string> Tags { get; set; }            // Categorization tags
    public Dictionary<string, string> Metadata { get; set; } // Custom metadata
    public List<string> RelatedEntries { get; set; }  // Cross-references
    public List<string> SymbolIds { get; set; }       // SymbolWiki links
    public string MoonPhase { get; set; }             // Lunar phase
    public string PlanetaryInfluences { get; set; }   // Astrological data
    public string Weather { get; set; }               // Weather conditions
    public string Location { get; set; }              // Ritual location
    public string Participants { get; set; }          // Ritual participants
    public bool IsEncrypted { get; set; }             // Encryption status
    public string EncryptionVersion { get; set; }     // Encryption version
    public long FileSize { get; set; }                // Encrypted file size
    public string Checksum { get; set; }              // Data integrity check
}
```

#### **GrimoireMetadata**
```csharp
public class GrimoireMetadata
{
    public string Version { get; set; }                           // System version
    public DateTime CreatedDate { get; set; }                     // System creation
    public DateTime LastModified { get; set; }                    // Last system update
    public int TotalEntries { get; set; }                         // Total entry count
    public long TotalSize { get; set; }                           // Total encrypted size
    public Dictionary<GrimoireEntryType, int> EntryCounts { get; set; } // Counts by type
    public List<string> AllTags { get; set; }                     // All unique tags
    public List<string> AllSymbolIds { get; set; }                // All symbol references
    public string LastBackupDate { get; set; }                    // Last backup timestamp
    public bool IsEncrypted { get; set; }                         // System encryption status
}
```

## 🔒 Security Details

### **Encryption Implementation**
- **Algorithm**: AES-256-CBC
- **Key Derivation**: PBKDF2 with 10,000 iterations
- **Salt**: 32-byte random salt per instance
- **IV**: Unique 16-byte IV per entry
- **Key Size**: 256 bits (32 bytes)

### **Security Best Practices**
1. **Strong Master Key**: Use a complex, unique master key
2. **Regular Backups**: Create encrypted backups frequently
3. **Secure Key Storage**: Store master key securely (not in code)
4. **Access Control**: Implement proper access controls
5. **Audit Logging**: Log access attempts and modifications

### **Key Management**
```csharp
// Example secure key generation
public static string GenerateSecureMasterKey()
{
    using var rng = RandomNumberGenerator.Create();
    var keyBytes = new byte[32];
    rng.GetBytes(keyBytes);
    return Convert.ToBase64String(keyBytes);
}

// Example key derivation with user input
public static string DeriveKeyFromPassword(string password, string salt)
{
    using var deriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000);
    var keyBytes = deriveBytes.GetBytes(32);
    return Convert.ToBase64String(keyBytes);
}
```

## 🔗 Integration with RitualOS

### **SymbolWiki Integration**
```csharp
// Link entries to symbols
var entry = await grimoireFS.CreateEntryAsync(
    GrimoireFS.GrimoireEntryType.RitualLog,
    "Pentagram Protection",
    "Used the pentagram symbol for protection...",
    symbolIds: new List<string> { "pentagram", "eye-of-horus" }
);

// Find entries using specific symbols
var pentagramEntries = await grimoireFS.GetEntriesBySymbolAsync("pentagram");
```

### **Dream Dictionary Integration**
```csharp
// Create dream entries with symbol associations
var dreamEntry = await grimoireFS.CreateEntryAsync(
    GrimoireFS.GrimoireEntryType.DreamEntry,
    "Flying Over Temples",
    "I dreamed of flying over ancient Egyptian temples...",
    tags: new List<string> { "flying", "temples", "egyptian" },
    symbolIds: new List<string> { "eye-of-horus", "ankh" }
);
```

### **Ritual System Integration**
```csharp
// Store ritual performance logs
var ritualLog = await grimoireFS.CreateEntryAsync(
    GrimoireFS.GrimoireEntryType.RitualLog,
    "Full Moon Esbat",
    "Performed the monthly esbat ritual...",
    metadata: new Dictionary<string, string>
    {
        ["moon_phase"] = "Full",
        ["planetary_hour"] = "Moon",
        ["weather"] = "Clear",
        ["candles_used"] = "3 white, 1 silver"
    },
    tags: new List<string> { "esbat", "full-moon", "goddess" }
);
```

## 📊 Performance Considerations

### **Optimization Tips**
1. **Batch Operations**: Use bulk operations for multiple entries
2. **Lazy Loading**: Load entries on-demand
3. **Caching**: Cache frequently accessed metadata
4. **Indexing**: Consider database indexing for large datasets
5. **Compression**: Implement compression for large content

### **Scalability**
- **Entry Count**: Supports 100,000+ entries
- **File Size**: Individual entries up to 100MB
- **Total Storage**: Limited only by disk space
- **Search Performance**: Optimized for real-time search

## 🛠️ Troubleshooting

### **Common Issues**

#### **Encryption Errors**
```csharp
// Check if master key is correct
try
{
    var entry = await grimoireFS.GetEntryAsync("some-id");
}
catch (InvalidOperationException ex)
{
    // Handle encryption/decryption errors
    Console.WriteLine($"Encryption error: {ex.Message}");
}
```

#### **File Corruption**
```csharp
// Verify data integrity
var entry = await grimoireFS.GetEntryAsync("some-id");
if (string.IsNullOrEmpty(entry.Checksum))
{
    // Handle corrupted entry
}
```

#### **Backup Issues**
```csharp
// Check backup status
var backups = await grimoireFS.GetBackupListAsync();
if (!backups.Any())
{
    // Create initial backup
    await grimoireFS.CreateBackupAsync();
}
```

### **Recovery Procedures**
1. **Restore from Backup**: Use latest backup to restore data
2. **Rebuild Metadata**: Reinitialize system if metadata corrupted
3. **Verify Integrity**: Check checksums for all entries
4. **Contact Support**: For complex recovery scenarios

## 🔮 Future Enhancements

### **Planned Features**
- **Cloud Sync**: Encrypted cloud synchronization
- **Multi-User**: Shared grimoires with access control
- **Advanced Search**: Semantic search and AI-powered insights
- **Ritual Templates**: Pre-built ritual frameworks
- **Astrological Integration**: Automatic planetary timing
- **Mobile Support**: Encrypted mobile access
- **API Integration**: REST API for external tools
- **Plugin System**: Extensible entry types and features

### **Performance Improvements**
- **Database Backend**: SQLite/PostgreSQL for large datasets
- **Full-Text Search**: Lucene.NET integration
- **Caching Layer**: Redis/Memory caching
- **Compression**: LZ4 compression for large entries
- **Parallel Processing**: Multi-threaded operations

## 📝 Contributing

### **Development Setup**
1. Clone the RitualOS repository
2. Install .NET 8.0 SDK
3. Run tests: `dotnet test tests/RitualOS.Tests/GrimoireFSTests.cs`
4. Build project: `dotnet build`

### **Testing**
```bash
# Run all GrimoireFS tests
dotnet test --filter "FullyQualifiedName~GrimoireFSTests"

# Run specific test
dotnet test --filter "FullyQualifiedName~GrimoireFSTests.Encryption_ShouldProtectData"
```

### **Code Standards**
- Follow C# coding conventions
- Add unit tests for new features
- Update documentation for API changes
- Use async/await patterns consistently
- Implement proper error handling

## 📄 License

GrimoireFS is part of RitualOS and follows the same licensing terms. See the main LICENSE file for details.

## 🆘 Support

For issues, questions, or contributions:
- **Issues**: GitHub Issues
- **Discussions**: GitHub Discussions
- **Documentation**: This README and inline code comments
- **Security**: Report security issues privately

---

**🔐 GrimoireFS - Your Magical Data, Securely Stored** ✨ 