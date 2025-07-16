RitualOS – Expanded TODO File
Project Phase: RitualTemplateBuilder + Codex Expansion
Status: 🚧 In Development

✅ Completed (Partial Snapshot)
These components are known to exist or are partially implemented:

Symbol Viewer UI w/ filter + rewritten fields

RitualLibraryViewModel with dynamic filter

ThemeViewModel + ThemeLoader + Flame/Amanda/Light themes

Theme resources: Theme.Dark.xaml, others present

Shared ViewModelBase (for INotifyPropertyChanged)

Basic Codex entries from extracted dream dictionary (592 .md files)

Ritual library visual UI with side-by-side layout

🚧 TODO: RitualTemplateBuilder System
🛠️ Files & Components Needed
RitualTemplateBuilder.xaml

Two-panel layout: Ritual form (left), Preview/Result (right)

Include toolbar with Save, Load, and Clear buttons

Add validation hints when required fields are empty (e.g., red borders).

Use a Grid layout with responsive columns for consistent scaling.

Provide drag-and-drop ordering for RitualSteps.

Use TextBox, ComboBox, TagControl, RichTextBox.

Add tooltips for guidance on complex fields.

Optional: Live preview box w/ Markdown or rendered Codex-style ritual output.

Preview updates as the user types.

Allow toggling between raw Markdown and styled preview.

RitualTemplateBuilderViewModel.cs

Implements INotifyDataErrorInfo for validation.

Commands: AddStepCommand, RemoveStepCommand, SaveTemplateCommand, LoadTemplateCommand.

ObservableCollection<string> for Tools and RitualSteps to auto-update UI.

Expose SelectedStepIndex to support reordering.

Bindable Fields:

RitualName

Intention

Tools (ObservableCollection)

SpiritsInvoked (Multi-select)

ChakrasAffected (Enum w/ icons: Root, Sacral, Solar Plexus, Heart, Throat, Third Eye, Crown)

ElementsAligned (Enum w/ icons: Earth, Air, Fire, Water, Spirit)

MoonPhase (string or enum)

RitualSteps (ObservableCollection<string>)

OutcomeField

Notes

Implement “Save Template” & “Load Template”.

Use standard file dialogs targeting the ritual_templates folder.

Remember last save location in user settings.

Validate fields before allowing save.

Add error handling for save/load failures and a confirmation dialog before overwriting templates.

RitualTemplate.cs (Model)

Add data annotations (e.g., [Required]) for validation.

Consider a Version property for future schema updates.

// The sacred blueprint for rituals! 😄
public class RitualTemplate {
  public Guid TemplateId { get; set; }
  public DateTime CreatedDate { get; set; }
  public string Name { get; set; }
  public string Intention { get; set; }
  public List<string> Tools { get; set; }
  public List<string> SpiritsInvoked { get; set; }
  public List<string> ChakrasAffected { get; set; }
  public List<string> Elements { get; set; }
  public string MoonPhase { get; set; }
  public List<string> Steps { get; set; }
  public string OutcomeField { get; set; }
  public string Notes { get; set; }
}

RitualTemplateSerializer.cs & JSON Storage

Save rituals in a serializable JSON format to ritual_templates/.

Files named as template_{TemplateId}.json.

Persist last save/load path in user settings.

Provide async methods SaveAsync and LoadAsync.

Implement JSON schema validation and backup functionality (e.g., timestamped copies) to prevent data loss.

📦 TODO: Codex Rewrite Engine
Create CodexRewriteEngine.cs.

Stage 1: Parse Markdown to a structured object.

Stage 2: Apply rewriting rules (synonyms, grammar fixes).

Stage 3: Add metadata tags (Chakra, Element).

Create a plugin interface IRewriteRule for custom transformations.

Export rewritten content back to Markdown files.

Add a toggle between “Original / Rewritten” modes in the UI.

Allow exporting the rewritten entry as a new .md file.

Add logging for rewrite operations and support for batch processing.

🔐 TODO: SigilLock (Role-Based Feature Lockout)
Create SigilLock.cs to evaluate user access level.

Create SigilLockAttribute for gating commands and views.

Check role during app startup before loading the main window.

Create ClientProfile.cs with a Role enum: Apprentice, Adept, Ritualist, Dreamworker, Technomage, Guide, Admin.

Tie into UI visibility bindings (via BooleanToVisibilityConverter).

Add role-based permissions configuration (e.g., JSON file or UI editor) and audit logging for access changes.

Future: Tie roles to license key validation.

📁 TODO: Directory & File Structure
Standardize file naming (e.g., ritual_<timestamp>.json).

Add a .gitignore entry for /logs/.

/rituals/         → JSON ritual templates
/codex/           → Markdown + rewritten entries
/themes/          → ThemeResource.xaml, colors
/viewmodels/      → Builder, SymbolViewer, Theme
/components/      → UI Elements (modular)
/services/        → ThemeLoader, TemplateSaver, SigilLock
/assets/          → Images, icons, elemental symbols
/settings/        → User preferences
/plugins/         → Rewrite engine modules
/logs/            → Application logs for debugging and auditing
/docs/user_guide/ → User manual and quick-start guide

🔮 BONUS: Future Modules (Not Yet Active)
Dream Parser: Use dream logs to auto-suggest rituals.

FieldMap Generator: Shows chakra + element heatmaps across rituals.

AutoChanneled Rituals: Use AI to draft ritual templates based on dream/Codex entries.

Module Plugin System: Allow drop-in codex transformations, ritual steps, etc.

Mobile Companion: Sync rituals on the go.

Cloud Sync Service: Encrypted backup across devices.

Exporters: Ritual → Markdown → PDF / EPUB / Website.

WikiSync Tool: Push Codex entries to the RitualOS GitHub wiki directly.

Analytics Dashboard: Track usage stats (e.g., most-used rituals, theme preferences).

User Authentication: Secure login with password hashing and session management.

🛑 Known Missing
No RitualTemplateBuilder.xaml yet.

No JSON serialization handlers for ritual data.

No access control system (SigilLock only conceptual).

Rewrite engine exists only partially (UI side), no backend processor.

Flame/Amanda themes implemented, but not gated by role or preference logic.

Symbol viewer does not yet support "ritual suggestion" or codex chaining.

No Codex-to-Wiki or Wiki-to-Codex synchronization.

No automated tests for core services.

Settings persistence not implemented.

Packaging scripts for releases not created.

🎯 Next Steps & Focus
Main Task: Build RitualTemplateBuilder.xaml and RitualTemplate.cs.

Secondary Task: Prepare for SigilLock logic (backend only for now).

Tertiary Task: Continue Codex expansion with a draft of the rewrite engine.

Bonus: Prep CodexLanguage schema for UI-integrated ritual writing.

Professional Focus: Implement logging, validation, and user documentation to elevate RitualOS.

Polish Checklist:

🎨 Ensure consistent theme colors across all views.

🔍 Validate form fields with helpful messages.

🖱️ Add drag-and-drop reordering for ritual steps.

📂 Provide sample ritual templates for new users.

⌨️ Integrate hotkey support for power users.

🎞️ Include animated transitions when switching themes.