# RitualOS

RitualOS is a sacred operating system and ritual management application.
It provides practitioners with a structured way to track rituals, maintain
inventory of magical ingredients, log dreams, and manage clients.

## Core Features
- Structured ritual logs with moon phases, steps, and outcomes
- Inventory tracking with expiration and reorder alerts
- Dream journal with symbolic tagging
- Basic client CRM for session notes and ritual history
- Calendar timeline to review past rituals
- Offline-first JSON storage with simple import/export

## Project Structure
See the `/src` folder for code and `/samples` for example JSON files.

## Building
This project targets **.NET 8** and uses **Avalonia UI** for its interface.
A minimal project file is provided.

## Documentation
Schema references live in the `docs` folder:
- [Ritual Schema](docs/ritual_schema.md)
- [Inventory Schema](docs/inventory_schema.md)
- [Client CRM Schema](docs/crm_schema.md)
- [Dream Schema](docs/dream_schema.md)


## Pitch Deck
A markdown pitch deck summarizing the vision can be found in `docs/pitch_deck.md`.


## Mockups
UI mockups are stored as base64 strings in [docs/mockups_base64.md](docs/mockups_base64.md). Decode them to view the images locally.
