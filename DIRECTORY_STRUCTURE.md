# RitualOS Directory Structure

## Overview

RitualOS follows a modular, organized directory structure designed for scalability, maintainability, and professional development practices. This document outlines the purpose and contents of each directory.

## Root Directory

```
RitualOS/
├── src/                    # Main application source code
├── rituals/               # JSON ritual templates
├── codex/                 # Markdown + rewritten entries
├── themes/                # ThemeResource.xaml, colors
├── viewmodels/            # Builder, SymbolViewer, Theme
├── components/            # UI Elements (modular)
├── services/              # ThemeLoader, TemplateSaver, SigilLock
├── assets/                # Images, icons, elemental symbols
├── settings/              # User preferences
├── plugins/               # Rewrite engine modules
├── logs/                  # Application logs for debugging and auditing
├── docs/                  # Documentation and user guides
├── inventory/             # Inventory data files
├── samples/               # Sample data and examples
└── bin/                   # Build output (gitignored)
```

## Directory Details

### `/src/` - Main Application Source
Contains the core Avalonia application code:
- **App.axaml** - Main application definition
- **Program.cs** - Application entry point
- **Models/** - Data models and entities
- **ViewModels/** - MVVM view models
- **Views/** - Avalonia UI views
- **Services/** - Business logic and data services
- **Converters/** - XAML value converters
- **Helpers/** - Utility classes
- **Styles/** - XAML styles and themes

### `/rituals/` - Ritual Templates
JSON files containing ritual template definitions:
- **File Naming**: `ritual_<timestamp>.json` (e.g., `ritual_20241216_143022.json`)
- **Format**: Structured JSON with validation
- **Content**: Ritual steps, ingredients, timing, elemental associations
- **Examples**: Sample templates for different ritual types

### `/codex/` - Enhanced Knowledge Base
Markdown files containing processed and enhanced dream dictionary entries:
- **Source**: Original dream dictionary content
- **Processing**: Enhanced by Codex Rewrite Engine
- **Format**: Markdown with metadata
- **Features**: Cross-references, elemental associations, modern interpretations

### `/themes/` - Visual Themes
Centralized theme resources and color definitions:
- **ThemeResource.xaml** - Main theme resource dictionary
- **Color Palettes** - Elemental and chakra color schemes
- **Styles** - Reusable UI component styles
- **Icons** - Theme-specific iconography

### `/viewmodels/` - View Model Extensions
Additional view models for specialized features:
- **Builder/** - Ritual template builder view models
- **SymbolViewer/** - Symbol and codex viewer models
- **Theme/** - Theme management view models

### `/components/` - Modular UI Components
Reusable UI elements and controls:
- **ui_elements/** - Basic UI components
- **modular/** - Complex composite components
- **Custom Controls** - Specialized ritual-related controls

### `/services/` - Service Layer
Business logic and data access services:
- **theme_loader/** - Theme loading and management
- **template_saver/** - Ritual template persistence
- **sigil_lock/** - Security and encryption services
- **data_loaders/** - Data loading and caching

### `/assets/` - Static Resources
Images, icons, and visual assets:
- **images/** - General application images
- **icons/** - Application icons and UI icons
- **elemental_symbols/** - Element and chakra symbols
- **SVG Files** - Scalable vector graphics

### `/settings/` - User Configuration
User preferences and application settings:
- **permissions.json** - User permissions and roles
- **user_preferences.json** - User-specific settings
- **application_config.json** - Application configuration

### `/plugins/` - Plugin System
Extensible plugin architecture:
- **README.md** - Plugin system documentation
- **sample_astrology_plugin/** - Example plugin
- **manifest.json** - Plugin metadata and configuration
- **Plugin DLLs** - Compiled plugin assemblies

### `/logs/` - Application Logs
Debugging and auditing information:
- **access.log** - User access and activity logs
- **error.log** - Error and exception logs
- **performance.log** - Performance metrics
- **plugins/** - Plugin-specific logs

### `/docs/` - Documentation
Comprehensive documentation:
- **user_guide/** - User manuals and guides
- **quick_start.md** - Getting started guide
- **api_reference/** - Developer documentation
- **tutorials/** - Step-by-step tutorials

### `/inventory/` - Inventory Data
Inventory management data files:
- **items.json** - Inventory item definitions
- **categories.json** - Item categorization
- **alerts.json** - Low stock alerts
- **usage_history.json** - Item usage tracking

### `/samples/` - Sample Data
Example data and templates:
- **client_profiles/** - Sample client data
- **ritual_templates/** - Example ritual templates
- **dream_entries/** - Sample dream interpretations
- **export_examples/** - Export format examples

## File Naming Conventions

### Ritual Templates
```
ritual_<YYYYMMDD>_<HHMMSS>.json
Example: ritual_20241216_143022.json
```

### Codex Entries
```
<category>_<symbol_name>_rewritten.md
Example: elements_moon_phases_rewritten.md
```

### Log Files
```
<type>_<YYYYMMDD>.log
Example: access_20241216.log
```

### Plugin Files
```
<plugin_name>/
├── manifest.json
├── plugin.dll
├── resources/
└── documentation.md
```

## Professional Additions

### Standardized Structure
- **Consistent Naming**: All files follow established naming conventions
- **Version Control**: Proper .gitignore entries for logs and build artifacts
- **Documentation**: Comprehensive documentation for each directory
- **Modularity**: Clear separation of concerns and responsibilities

### Security Considerations
- **Logging**: Secure logging practices with sensitive data filtering
- **Permissions**: Role-based access control for different directories
- **Encryption**: Sensitive data encryption in appropriate locations
- **Audit Trail**: Comprehensive logging for debugging and auditing

### Scalability Features
- **Plugin System**: Extensible architecture for new features
- **Modular Components**: Reusable UI components
- **Service Layer**: Clean separation of business logic
- **Asset Management**: Organized resource management

## Development Workflow

### Adding New Features
1. **Identify Directory**: Choose appropriate directory for new feature
2. **Follow Conventions**: Use established naming and structure patterns
3. **Update Documentation**: Document new additions
4. **Test Integration**: Ensure proper integration with existing systems

### Maintenance Tasks
- **Regular Cleanup**: Remove obsolete files and directories
- **Log Rotation**: Manage log file sizes and retention
- **Backup Strategy**: Regular backups of important data
- **Version Control**: Proper git workflow for changes

### Best Practices
- **Consistency**: Maintain consistent structure across all directories
- **Documentation**: Keep documentation up to date
- **Testing**: Test changes across different environments
- **Performance**: Monitor impact on application performance

---

*This directory structure is designed to support RitualOS's growth from a simple ritual management tool to a comprehensive magical practice platform.* 