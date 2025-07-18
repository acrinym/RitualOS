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
