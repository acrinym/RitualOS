# Plugin System Guide

RitualOS supports simple drop-in plugins so you can extend functionality without modifying the core application.

## Directory Layout

Plugins live in the `plugins/` folder next to `RitualOS.exe`. Each plugin has its own subfolder containing:

- `plugin.dll` – the compiled assembly implementing `IRitualOSPlugin`
- `manifest.json` – metadata with at least an `entryPoint` field specifying the full type name

Example manifest:

```json
{
    "name": "MyPlugin",
    "version": "1.0",
    "entryPoint": "MyPlugin.MainPlugin"
}
```

## Loading and Shutdown

The `PluginLoader` scans each subfolder at startup. If the manifest and DLL are present, the plugin is loaded and `Initialize` is called with a basic context exposing the plugin's data path. When RitualOS closes, `Shutdown` is called on all loaded plugins.

Use this system to build custom ritual steps, codex transformations, or integrations with external services.

## Plugin Development Quickstart

1. Create a folder under `plugins/` for your plugin.
2. Add a `manifest.json` describing the plugin and its `entryPoint`.
3. Build a `plugin.dll` targeting .NET 6.0 that implements `IRitualOSPlugin`.
4. Place the DLL and manifest in your plugin folder and restart RitualOS.

See [plugins/README.md](../plugins/README.md) for a full guide and examples.

### Community Contributions

We love seeing new tarot spreads, astrology tools, and other mystical add-ons.
Share your creations by opening a pull request in the `plugins/` directory!
