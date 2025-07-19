# SymbolWiki - RitualOS Symbol Database

## Overview

SymbolWiki is a comprehensive occult symbol database system for RitualOS that provides a centralized repository for symbols, sigils, and magical glyphs from various traditions and practices. It includes advanced search, filtering, and categorization capabilities.

## Features

### Core Functionality
- **Comprehensive Symbol Database**: Store symbols from multiple magical traditions
- **Advanced Search**: Search by name, description, tags, and associated entities
- **Category Filtering**: Filter by magical tradition (Egyptian, Celtic, Goetic, etc.)
- **Power Level Classification**: Categorize symbols by their magical potency
- **Rich Metadata**: Store correspondences, historical context, and usage instructions
- **Goetic Spirit Database**: Specialized database for Goetic spirits with additional metadata

### Search & Filtering
- **Text Search**: Search across names, descriptions, and tags
- **Category Filter**: Filter by magical tradition
- **Power Level Filter**: Filter by minimum power level
- **Tag Filter**: Filter by specific tags
- **Planetary Correspondence**: Filter by planetary associations
- **Elemental Correspondence**: Filter by elemental associations

### Data Management
- **Import/Export**: JSON import/export functionality
- **Symbol Editor**: Add, edit, and delete symbols
- **Verification System**: Mark symbols as verified or unverified
- **Usage Tracking**: Track symbol usage and ratings
- **Personal Symbols**: Distinguish between system and personal symbols

## Symbol Categories

The SymbolWiki supports the following categories:

1. **Alchemical** - Alchemical symbols and processes
2. **Astrological** - Astrological symbols and planetary glyphs
3. **Celtic** - Celtic and Irish magical symbols
4. **Chinese** - Chinese magical and philosophical symbols
5. **Egyptian** - Ancient Egyptian symbols and hieroglyphs
6. **Goetic** - Goetic spirits and their seals
7. **Hermetic** - Hermetic and classical magical symbols
8. **Kabbalistic** - Kabbalistic symbols and the Tree of Life
9. **Nordic** - Norse and Viking magical symbols
10. **Planetary** - Planetary symbols and correspondences
11. **Protection** - Protective symbols and amulets
12. **Runic** - Rune symbols and their meanings
13. **Sigil** - Modern sigil magic symbols
14. **Tibetan** - Tibetan Buddhist symbols
15. **Unified** - Universal or cross-traditional symbols
16. **Wiccan** - Wiccan and modern pagan symbols
17. **Other** - Miscellaneous symbols

## Power Levels

Symbols are classified by power level:

1. **Minor** - Basic symbols with limited magical potency
2. **Moderate** - Standard symbols with moderate magical effects
3. **Major** - Powerful symbols with significant magical influence
4. **Greater** - Very powerful symbols requiring careful handling
5. **Supreme** - The most powerful symbols, often requiring special preparation

## Symbol Metadata

Each symbol stores comprehensive metadata:

### Basic Information
- **Name**: Primary name of the symbol
- **Alternative Names**: Alternative names and variations
- **Category**: Magical tradition category
- **Power Level**: Magical potency classification

### Visual Representation
- **SVG Data**: Vector graphics data
- **Image Path**: Path to image file
- **Original/Rewritten**: Original and modified versions

### Description and Context
- **Description**: Detailed description of the symbol
- **Historical Context**: Historical background and origins
- **Usage Instructions**: How to use the symbol
- **Warnings**: Important warnings and precautions
- **Ritual Text**: Associated ritual text or incantations

### Source and Attribution
- **Source**: Source material or tradition
- **Author**: Original author or creator
- **License**: Usage license information

### Categorization and Tags
- **Tags**: Searchable tags for categorization
- **Associated Entities**: Related deities, spirits, or entities

### Correspondences
- **Planetary**: Planetary correspondences
- **Elemental**: Elemental correspondences
- **Color**: Color correspondences
- **Material**: Material correspondences
- **Time**: Temporal correspondences
- **Chakra**: Chakra associations
- **Element**: Element associations

### Relationships
- **Related Symbols**: Links to related symbols

### Metadata
- **Date Added**: When the symbol was added
- **Last Modified**: When the symbol was last modified
- **Is Personal**: Whether it's a personal or system symbol
- **Is Verified**: Whether the symbol is verified
- **Usage Count**: How many times the symbol has been used
- **Rating**: User rating of the symbol
- **Rating Count**: Number of ratings

### Goetic Spirit Specific
- **Goetic Rank**: Rank of the Goetic spirit
- **Goetic Legion**: Legion affiliation
- **Goetic Appearance**: Physical appearance description
- **Goetic Powers**: Powers and abilities
- **Goetic Seal**: The spirit's seal or sigil

## Usage

### Basic Search
1. Open the SymbolWiki interface
2. Enter search terms in the search box
3. Results will update automatically as you type

### Advanced Filtering
1. Use the category dropdown to filter by tradition
2. Set minimum power level filter
3. Toggle advanced filters for more options
4. Use tag, planetary, and elemental filters

### Adding Symbols
1. Click the "Add Symbol" button
2. Fill in the required information
3. Add tags and correspondences
4. Save the symbol

### Editing Symbols
1. Select a symbol from the list
2. Click the "Edit" button
3. Modify the information as needed
4. Save changes

### Import/Export
1. Use the Import button to load symbols from JSON
2. Use the Export button to save symbols to JSON
3. Select specific categories for export

## File Structure

```
symbols/
├── symbols_database.json          # Main symbol database
├── goetic_spirits.json            # Goetic spirits database
├── initial_symbols.json           # Initial symbol set
└── symbol_images/                 # Symbol image files
```

## API Reference

### SymbolWikiService

#### Methods
- `InitializeAsync()` - Initialize the service and load data
- `GetAllSymbols()` - Get all symbols
- `GetSymbolsByCategory(category)` - Get symbols by category
- `GetGoeticSpirits()` - Get Goetic spirits
- `SearchSymbols(term, category, minPower)` - Search symbols
- `GetSymbolsByTags(tags)` - Get symbols by tags
- `GetSymbolsByPlanet(planet)` - Get symbols by planetary correspondence
- `GetSymbolsByElement(element)` - Get symbols by elemental correspondence
- `AddSymbolAsync(symbol)` - Add a new symbol
- `UpdateSymbolAsync(symbol)` - Update an existing symbol
- `DeleteSymbolAsync(name, category)` - Delete a symbol
- `GetAllTags()` - Get all available tags
- `GetAllPlanets()` - Get all available planets
- `GetAllElements()` - Get all available elements
- `ImportSymbolsFromJsonAsync(json)` - Import symbols from JSON
- `ExportSymbolsToJsonAsync(path, category)` - Export symbols to JSON

#### Events
- `SymbolsLoaded` - Fired when symbols are loaded
- `SymbolAdded` - Fired when a symbol is added
- `SymbolUpdated` - Fired when a symbol is updated

## Contributing

### Adding New Symbols
1. Research the symbol thoroughly
2. Gather accurate historical information
3. Include proper attributions and sources
4. Add appropriate tags and correspondences
5. Mark as verified if from reliable sources

### Symbol Guidelines
- Use accurate historical information
- Include proper warnings and precautions
- Respect cultural and religious traditions
- Provide clear usage instructions
- Include source material references

## Security and Ethics

### Cultural Sensitivity
- Respect the origins and traditions of symbols
- Include appropriate warnings and context
- Don't appropriate closed cultural practices
- Provide accurate historical information

### Usage Warnings
- Some symbols have specific cultural or religious significance
- Use symbols with understanding and respect
- Consider the power and implications of symbols
- Follow proper ritual protocols when applicable

## Future Enhancements

### Planned Features
- **Symbol Visualization**: Interactive symbol viewer
- **SVG Support**: Vector graphics for symbols
- **Community Features**: User contributions and ratings
- **Advanced Search**: Semantic search capabilities
- **Symbol Relationships**: Network of related symbols
- **Ritual Integration**: Direct integration with ritual system
- **Mobile Support**: Mobile-friendly interface
- **Offline Mode**: Offline symbol database access

### Technical Improvements
- **Performance Optimization**: Faster search and filtering
- **Data Validation**: Enhanced data validation
- **Backup System**: Automated backup and restore
- **Version Control**: Symbol version history
- **API Integration**: External symbol database APIs

## Support

For questions, issues, or contributions:
1. Check the existing documentation
2. Review the test suite for examples
3. Submit issues through the project repository
4. Contribute improvements through pull requests

## License

This SymbolWiki system is part of RitualOS and follows the same licensing terms as the main project. 