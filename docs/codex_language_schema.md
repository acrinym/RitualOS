# CodexLanguage Schema

This draft defines a structured schema for authoring Codex entries directly within RitualOS. It is designed for future UI integration so practitioners can write rituals and symbols using a consistent format.

```json
{
  "name": "string",
  "original": "markdown",
  "rewritten": "markdown",
  "ritual_text": "markdown",
  "elements": ["fire", "water", "earth", "air", "spirit"],
  "chakras": ["root", "sacral", "solar_plexus", "heart", "throat", "third_eye", "crown"],
  "tags": ["string"],
  "references": ["symbol_name"]
}
```

Each entry can be stored as JSON or embedded in Markdown frontmatter. The schema will enable the editor to validate fields and cross-reference other symbols.
