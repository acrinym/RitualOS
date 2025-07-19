# 🔮 RitualOS - The Magical Operating System

RitualOS is a comprehensive, offline-first magical practice management system designed for serious practitioners. It provides secure, encrypted storage for all aspects of magical work, from ritual logs and dream journals to symbol libraries and media management.

## 🌟 Core Features

### **🔐 GrimoireFS - Encrypted Magical Data Storage**
- **AES-256 Encryption** - Military-grade encryption for all magical data
- **Rich Entry Types** - Ritual logs, dream entries, spell templates, personal sigils, journal entries
- **Advanced Metadata** - Moon phases, planetary influences, weather, location, participants
- **Tag System** - Flexible categorization and organization
- **Symbol Integration** - Link entries to SymbolWiki symbols
- **Cross-References** - Link related entries together
- **Search & Filter** - Full-text search with type and tag filtering
- **Backup & Restore** - Encrypted backup system
- **Offline-First** - Works without internet

### **🎵 Media Storage - Comprehensive Media Management**
- **All Media Types** - Images, audio, video, documents, SVG, 3D models
- **Smart Compression** - Automatic compression for large files (20-80% savings)
- **Thumbnail Generation** - Preview images for all media types
- **Rich Metadata** - Duration, bitrate, dimensions, format info
- **Massive Scale** - Supports 1,000,000+ files, individual files up to 10GB
- **Media Categories** - Ritual recordings, meditation music, chants, dream journal audio
- **Symbol Integration** - Link media to SymbolWiki symbols
- **Batch Operations** - Upload, download, and manage multiple files
- **Storage Efficiency** - Compression ratios and space optimization

### **🔮 SymbolWiki - Comprehensive Symbol Library**
- **Multiple Traditions** - Goetic, Egyptian, Celtic, Norse, and more
- **Rich Metadata** - Power levels, categories, descriptions, correspondences
- **SVG Support** - Vector graphics for crisp display at any size
- **Image Management** - PNG and SVG support with metadata
- **Interactive Viewer** - Zoom, rotate, flip, and export capabilities
- **Search & Filter** - Find symbols by tradition, power level, or keywords
- **Batch Import** - Import symbols from books and external sources
- **Goetic Spirits** - Specialized category with spirit-specific metadata

### **📚 Dream Dictionary Integration**
- **Comprehensive Database** - 10,000+ dream interpretations
- **Symbol Recognition** - Link dreams to SymbolWiki symbols
- **Rich Metadata** - Archetypes, emotions, contexts, interpretations
- **Search & Filter** - Find interpretations by symbols, emotions, or keywords
- **Personal Notes** - Add your own interpretations and insights

### **🎨 Modern, Mystical UI**
- **Dark Theme** - Perfect for magical work and night-time use
- **Responsive Design** - Works on desktop and tablet
- **Intuitive Navigation** - Easy to find and organize your magical work
- **Beautiful Icons** - Type-specific icons and mystical symbols
- **Accessibility** - Designed for all users

## 🚀 Quick Start

### **Installation**
```bash
# Clone the repository
git clone https://github.com/yourusername/RitualOS.git
cd RitualOS

# Install dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

### **Basic Usage**

#### **GrimoireFS - Store Your Magical Work**
```csharp
// Initialize with secure master key
var masterKey = "your-secure-master-key-here";
var grimoireFS = new GrimoireFS(masterKey);
await grimoireFS.InitializeAsync();

// Create a ritual log
var ritualEntry = await grimoireFS.CreateEntryAsync(
    GrimoireFS.GrimoireEntryType.RitualLog,
    "Full Moon Protection Ritual",
    "Performed a protection ritual during the full moon...",
    tags: new List<string> { "protection", "full-moon", "pentagram" },
    symbolIds: new List<string> { "pentagram", "eye-of-horus" }
);
```

#### **Media Storage - Manage Your Magical Media**
```csharp
// Initialize media storage
var mediaStorage = new MediaStorageService(masterKey);
await mediaStorage.InitializeAsync();

// Store ritual recording
using var audioStream = File.OpenRead("ritual_recording.wav");
var mediaFile = await mediaStorage.StoreMediaAsync(
    audioStream,
    "full_moon_ritual.wav",
    MediaStorageService.MediaType.Audio,
    MediaStorageService.MediaCategory.RitualRecording,
    "audio/wav",
    tags: new List<string> { "full-moon", "protection", "goddess" }
);
```

#### **SymbolWiki - Access Magical Symbols**
```csharp
// Initialize symbol wiki
var symbolWiki = new SymbolWikiService();
await symbolWiki.InitializeAsync();

// Search for symbols
var pentagramSymbols = await symbolWiki.SearchSymbolsAsync("pentagram");
var goeticSpirits = await symbolWiki.GetSymbolsByCategoryAsync("Goetic");
```

## 📁 Project Structure

```
RitualOS/
├── services/                    # Core services
│   ├── GrimoireFS.cs           # Encrypted data storage
│   ├── MediaStorageService.cs  # Media management
│   ├── SymbolWikiService.cs    # Symbol library
│   └── SymbolImageService.cs   # Image management
├── src/                        # Application source
│   ├── ViewModels/             # UI logic
│   ├── Views/                  # User interfaces
│   └── Models/                 # Data models
├── tests/                      # Unit tests
├── docs/                       # Documentation
├── symbols/                    # Symbol data
├── media/                      # Media storage
└── grimoire/                   # Encrypted data
```

## 🔒 Security Features

- **AES-256 Encryption** for all sensitive data
- **PBKDF2 Key Derivation** with 10,000 iterations
- **Unique IV per File** to prevent pattern analysis
- **Checksum Verification** for data integrity
- **Salt-based Security** against rainbow table attacks
- **Offline-First** - No cloud dependencies
- **Local Storage** - All data stays on your device

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test suites
dotnet test --filter "FullyQualifiedName~GrimoireFSTests"
dotnet test --filter "FullyQualifiedName~MediaStorageTests"
dotnet test --filter "FullyQualifiedName~SymbolWikiTests"
```

## 📚 Documentation

- **[GrimoireFS Documentation](docs/GrimoireFS_README.md)** - Encrypted data storage
- **[Media Storage Documentation](docs/MediaStorage_README.md)** - Media management
- **[SymbolWiki Documentation](docs/SymbolWiki_README.md)** - Symbol library
- **[User Guide](docs/user_guide/)** - Getting started and usage

## 🔮 Roadmap

### **Phase 1 - Foundation** ✅
- [x] GrimoireFS - Encrypted magical data storage
- [x] Media Storage - Comprehensive media management
- [x] SymbolWiki - Symbol library with SVG support
- [x] Dream Dictionary - Integration with dream interpretations
- [x] Modern UI - Beautiful, mystical interface

### **Phase 2 - Advanced Features** 🚧
- [ ] Ritual Scheduler - Moon phases and planetary alignments
- [ ] Audio Ritual Deck - Soundboard for chants and meditation
- [ ] Dream Journal AI - Symbol recognition and archetype identification
- [ ] Sigil Generator - AI/manual sigil creation
- [ ] Focus Mode UI - Distraction-free ritual interface

### **Phase 3 - Advanced Magic** 🔮
- [ ] ONNX Sigil Synthesizer - AI-powered sigil generation
- [ ] Depth Map Generator - Magic Eye-style ritual visuals
- [ ] Stereogram Generator - 3D magical imagery
- [ ] Astrological Integration - Planetary timing and influences
- [ ] Energy Work Tools - Aura visualization and tracking

## 🤝 Contributing

We welcome contributions from the magical community! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### **Development Setup**
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new features
5. Submit a pull request

### **Code Standards**
- Follow C# coding conventions
- Add unit tests for new features
- Update documentation for API changes
- Use async/await patterns consistently
- Implement proper error handling

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- **Issues**: [GitHub Issues](https://github.com/yourusername/RitualOS/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/RitualOS/discussions)
- **Documentation**: [Project Wiki](https://github.com/yourusername/RitualOS/wiki)

## 🙏 Acknowledgments

- The magical community for inspiration and feedback
- Open source contributors who made this possible
- All practitioners who shared their wisdom and experiences

---

**🔮 RitualOS - Your Magical Practice, Securely Stored** ✨

*"In the digital age, even magic needs a proper filing system."*