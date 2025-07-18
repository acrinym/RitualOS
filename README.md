# RitualOS

RitualOS is a sacred operating system and ritual management application.
It provides practitioners with a structured way to track rituals, maintain
inventory of magical ingredients, log dreams, and manage clients.

## Core Features
- Structured ritual logs with moon phases, steps, and outcomes
- Inventory tracking with expiration and reorder alerts
- Dream journal with symbolic tagging
- Basic client CRM for session notes and ritual history
- File-backed **ClientDatabase** service for storing and searching clients
- Timeline view to review past rituals chronologically
- Offline-first JSON storage with simple import/export
- Ritual creation wizard for step-by-step template building
- Symbol index editor with chakra tagging
- Codex rewrite preview tool
- Dashboard view for client profiles
- Embedded document viewer for common formats (PDF, Markdown, JSON, EPUB, HTML)
- Keyboard shortcuts for tab navigation
- Screen reader labels on major controls
- Plugin loader for community extensions
- Drag-and-drop step reordering in the ritual builder
- Dream dictionary integration in the Dream Parser
- Asynchronous document loading for large files

## Themes
RitualOS supports multiple visual themes to match different aesthetics:
- **Material** for a clean, modern look
- **Magical** with deep purples and vibrant accents
- **Parchment** for a premium, old-world feel
- **HighContrast** for maximum readability

## Project Structure
Key folders you will find in RitualOS:

```
/rituals/         -> JSON ritual templates
/codex/           -> Markdown + rewritten entries
/themes/          -> ThemeResource.xaml, colors
/viewmodels/      -> Builder, SymbolViewer, Theme
/components/      -> UI Elements (modular)
/services/        -> ThemeLoader, TemplateSaver, SigilLock
/assets/          -> Images, icons, elemental symbols
/settings/        -> user preferences
/plugins/         -> rewrite engine modules
/logs/            -> Application logs for debugging and auditing
/docs/user_guide/ -> User manual and quick-start guide
```
Example ritual files are stored using the pattern `ritual_<timestamp>.json`.
See the `/src` folder for code and `/samples` for example JSON files.

## Building and Running
Prerequisite: install the **.NET 8 SDK**.

Restore dependencies, build the project, and run it with:

`​`​`bash
dotnet restore
dotnet build
dotnet run
`​`​`

Launching the project will open the Avalonia UI and create a local `ritualos.json` data file.

## Documentation
Schema references live in the `docs` folder:
- [Ritual Schema](docs/ritual_schema.md)
- [Inventory Schema](docs/inventory_schema.md)
- [Client CRM Schema](docs/crm_schema.md)
- [Dream Schema](docs/dream_schema.md)
- [Dream Dictionary](docs/DreamDictionary/RitualOS_Dream_Dictionary.md)
- [Ritual Timeline](docs/ritual_timeline.md)
- [Plugin System](docs/plugin_system.md)
- [Sample Plugins](plugins/)
- [Custom Theme Guide](docs/custom_theme_guide.md)
- [Offline Roadmap](docs/offline_roadmap.md)


## Pitch Deck
A markdown pitch deck summarizing the vision can be found in `docs/pitch_deck.md`.


## Mockups
UI mockups are stored as base64 strings in [docs/mockups_base64.md](docs/mockups_base64.md). Decode them to view the images locally.