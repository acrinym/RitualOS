# RitualOS Plugin System

## Overview

The RitualOS Plugin System allows you to extend and customize your ritual practice with additional functionality. Plugins can add new ritual steps, codex transformations, export formats, and more.

## Plugin Types

### 1. Codex Transformers
Transform and enhance dream dictionary entries with custom processing rules.

### 2. Ritual Step Modules
Add new ritual step types and behaviors to the Ritual Builder.

### 3. Export Modules
Create custom export formats (PDF, EPUB, HTML, etc.) for rituals and codex entries.

### 4. Theme Extensions
Add new color schemes and visual themes to the application.

### 5. Integration Modules
Connect RitualOS with external services and APIs.

## Plugin Structure

```
plugins/
├── my_plugin/
│   ├── manifest.json          # Plugin metadata and configuration
│   ├── plugin.dll             # Compiled plugin assembly
│   ├── resources/             # Plugin resources (images, data, etc.)
│   ├── documentation.md       # Plugin documentation
│   └── examples/              # Example usage and templates
```

## Creating a Plugin

### 1. Plugin Manifest

```json
{
  "name": "My Custom Plugin",
  "version": "1.0.0",
  "description": "A custom plugin for RitualOS",
  "author": "Your Name",
  "type": "codex_transformer",
  "entryPoint": "MyPlugin.PluginMain",
  "dependencies": [],
  "permissions": ["read_codex", "write_codex"],
  "compatibility": ["1.0.0"]
}
```

### 2. Plugin Interface

```csharp
public interface IRitualOSPlugin
{
    string Name { get; }
    string Version { get; }
    void Initialize(IPluginContext context);
    void Shutdown();
}
```

### 3. Codex Transformer Example

```csharp
public class MyCodexTransformer : ICodexTransformer
{
    public string TransformEntry(string originalContent)
    {
        // Transform the content
        return transformedContent;
    }
}
```

## Installation

1. **Download Plugin**: Obtain the plugin files
2. **Extract**: Place in the `plugins/` directory
3. **Restart**: Restart RitualOS to load the plugin
4. **Configure**: Set up plugin settings if required

## Built-in Plugins

### Codex Rewrite Engine
- **Purpose**: Enhances dream dictionary entries
- **Features**: Modern language, cross-references, elemental associations
- **Status**: Active by default

### Ritual Template Library
- **Purpose**: Provides pre-built ritual templates
- **Features**: Moon phase rituals, elemental ceremonies, seasonal celebrations
- **Status**: Active by default

### Export Suite
- **Purpose**: Export rituals in various formats
- **Features**: PDF, Markdown, HTML, JSON
- **Status**: Available for installation

## Plugin Development

### Requirements
- .NET 6.0 or later
- RitualOS SDK (when available)
- Visual Studio or VS Code

### Development Workflow
1. **Setup**: Create plugin project structure
2. **Develop**: Implement plugin functionality
3. **Test**: Test with RitualOS development environment
4. **Package**: Create plugin package
5. **Distribute**: Share with the community

### Best Practices
- **Error Handling**: Implement robust error handling
- **Performance**: Optimize for minimal impact on application performance
- **Documentation**: Provide clear documentation and examples
- **Testing**: Thoroughly test with various data types
- **Security**: Follow security best practices for data handling

## Plugin API Reference

### IPluginContext
```csharp
public interface IPluginContext
{
    ICodexService Codex { get; }
    IRitualService Rituals { get; }
    IInventoryService Inventory { get; }
    ILogService Log { get; }
    ISettingsService Settings { get; }
}
```

### ICodexService
```csharp
public interface ICodexService
{
    Task<string> GetEntryAsync(string symbol);
    Task SaveEntryAsync(string symbol, string content);
    Task<IEnumerable<string>> SearchAsync(string query);
}
```

### IRitualService
```csharp
public interface IRitualService
{
    Task<RitualTemplate> LoadTemplateAsync(string path);
    Task SaveTemplateAsync(RitualTemplate template, string path);
    Task<IEnumerable<RitualTemplate>> GetTemplatesAsync();
}
```

## Community Plugins

### Available Plugins
- **Astrology Integration**: Planetary alignment calculations
- **Herb Database**: Comprehensive herb and crystal information
- **Ritual Calendar**: Sabbat and esbat tracking
- **Voice Commands**: Voice-activated ritual control
- **AR Sigil Projector**: Augmented reality sigil display

### Contributing Plugins
1. **Fork**: Fork the RitualOS repository
2. **Develop**: Create your plugin
3. **Test**: Ensure compatibility and functionality
4. **Submit**: Create a pull request with your plugin
5. **Review**: Community review and approval process

## Troubleshooting

### Common Issues

**Plugin not loading**
- Check plugin manifest format
- Verify assembly compatibility
- Review application logs

**Plugin errors**
- Check plugin logs in `/logs/plugins/`
- Verify plugin permissions
- Test with minimal configuration

**Performance issues**
- Monitor plugin resource usage
- Check for memory leaks
- Optimize plugin code

### Getting Help
- Check plugin documentation
- Review plugin examples
- Consult the community forum
- Report issues to plugin maintainers

## Future Enhancements

### Planned Features
- **Plugin Marketplace**: Centralized plugin distribution
- **Auto-updates**: Automatic plugin updates
- **Sandboxing**: Enhanced security isolation
- **Plugin Dependencies**: Dependency management system
- **Plugin Analytics**: Usage tracking and metrics

### Development Roadmap
- **Q1 2025**: Plugin marketplace beta
- **Q2 2025**: Enhanced plugin API
- **Q3 2025**: Plugin development tools
- **Q4 2025**: Advanced plugin features

---

*The plugin system is designed to grow with the RitualOS community. Share your creations and help others enhance their practice!*
