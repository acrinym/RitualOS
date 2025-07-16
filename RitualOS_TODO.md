Fixed and Merged RitualOS_TODO.md (Markdown Inline)markdown

# RitualOS – Expanded TODO File
**Project Phase:** RitualTemplateBuilder + Codex Expansion  
**Status:** 🚧 In Development  
**File Source:** allfiles (2).txt (parsed and analyzed)

## ✅ Completed (Partial Snapshot)
These components are known to exist or are partially implemented:
- Symbol Viewer UI w/ filter + rewritten fields
- RitualLibraryViewModel with dynamic filter
- ThemeViewModel + ThemeLoader + Flame/Amanda/Light themes
- Theme resources: Theme.Dark.xaml, others present
- Shared ViewModelBase (for INotifyPropertyChanged)
- Basic Codex entries from extracted dream dictionary (592 .md files)
- Ritual library visual UI with side-by-side layout

## 🚧 TODO: RitualTemplateBuilder System
### 🛠️ Files & Components Needed
1. **RitualTemplateBuilder.xaml**
   - Two-panel layout: Ritual form (left), Preview/Result (right)
   - Include toolbar with Save, Load, and Clear buttons
   - Add validation hints when required fields are empty
   - Use a Grid layout with responsive columns for consistent scaling
   - Provide drag-and-drop ordering for RitualSteps
   - Use TextBox, ComboBox, TagControl, RichTextBox
   - Optional: Live preview box w/ Markdown or rendered Codex-style ritual output
     - Preview updates as the user types
     - Allow toggling between raw Markdown and styled preview
   - **Professional Addition:** Add input validation (e.g., required fields with red borders) and tooltips for guidance.

2. **RitualTemplateBuilderViewModel.cs**
   - Bind fields:
     - Implements INotifyDataErrorInfo for validation
     - Commands: AddStepCommand, RemoveStepCommand, SaveTemplateCommand, LoadTemplateCommand
     - ObservableCollection<string> for Tools and RitualSteps to auto-update UI
     - Expose SelectedStepIndex to support reordering
     - RitualName
     - Intention
     - Tools (ObservableCollection)
     - SpiritsInvoked (Multi-select)
     - ChakrasAffected (Enum w/ icons)
       - Root
       - Sacral
       - Solar Plexus
       - Heart
       - Throat
       - Third Eye
       - Crown
     - ElementsAligned (Enum w/ icons)
       - Earth
       - Air
       - Fire
       - Water
       - Spirit
     - MoonPhase (string or enum)
     - RitualSteps (ObservableCollection<string>)
       - Steps should support drag-and-drop reordering
     - OutcomeField
     - Notes
   - Implement “Save Template” & “Load Template”
     - Use standard file dialogs targeting the ritual_templates folder
     - Remember last save location in user settings
     - Validate fields before allowing save
   - **Professional Addition:** Add error handling for save/load failures and a confirmation dialog before overwriting templates.

3. **RitualTemplate.cs (Model)**
   ```csharp
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

Professional Addition: Add data annotations (e.g., [Required]) for validation and consider versioning (e.g., Version property) for future updates.

ritual_templates/*.jsonSave rituals in a serializable JSON format
Files named as template_{TemplateId}.json
Persist last save/load path in user settings
Create RitualTemplateSerializer.cs if neededProvide async methods SaveAsync and LoadAsync
Validate template schema version before loading

Professional Addition: Implement JSON schema validation and backup functionality (e.g., timestamped copies) to prevent data loss.

 TODO: Codex Rewrite EngineCreate CodexRewriteEngine.csStage 1: Parse Markdown to a structured object
Stage 2: Apply rewriting rules (synonyms, grammar fixes)
Stage 3: Add metadata tags (Chakra, Element)
Plugin interface IRewriteRule for custom transformations
Export rewritten content back to Markdown files
Add toggle between “Original / Rewritten” modes in UIAllow exporting the rewritten entry as a new .md file

Professional Addition: Add logging for rewrite operations, support for batch processing, and an export option for rewritten entries.

 TODO: SigilLock (Role-Based Feature Lockout) Already written into internal TODO by you, now include:SigilLock.cs to evaluate user access levelSigilLockAttribute for gating commands and views
Check role during app startup before loading main window
Future: tie roles to license key validation

ClientProfile.cs with Role enum:Apprentice
Adept
Ritualist
Dreamworker
Technomage
Guide
Admin

Optional: Tie into UI visibility bindings (via BooleanToVisibilityConverter)
Professional Addition: Add role-based permissions configuration (e.g., JSON file or UI editor) and audit logging for access changes.

 TODO: Directory & File Structurebash

/rituals/         → JSON ritual templates
/codex/           → Markdown + rewritten entries
/themes/          → ThemeResource.xaml, colors
/viewmodels/      → Builder, SymbolViewer, Theme
/components/      → UI Elements (modular)
/services/        → ThemeLoader, TemplateSaver, SigilLock
/assets/          → Images, icons, elemental symbols
/settings/        → user preferences
/plugins/         → rewrite engine modules
/logs/            → Application logs for debugging and auditing (Professional Addition)
/docs/user_guide/ → User manual and quick-start guide (Professional Addition)

Professional Addition: Standardize file naming (e.g., ritual_<timestamp>.json) and add a .gitignore entry for /logs/.

 BONUS: Future Modules (Add to TODO, Not Yet Active) Dream Parser: Use dream logs to auto-suggest rituals
 FieldMap Generator: Shows chakra + element heatmaps across rituals
 AutoChanneled Rituals: Use AI to draft ritual templates based on dream/Codex entries
 Module Plugin System: Allow drop-in codex transformations, ritual steps, etc.
 Mobile Companion: sync rituals on the go
 Cloud Sync Service: encrypted backup across devices
 Exporters: Ritual → Markdown → PDF / EPUB / Website (SAAS vision)
 WikiSync Tool: Push Codex entries to the RitualOS GitHub wiki directly
Professional Addition: Analytics Dashboard: Track usage stats (e.g., most-used rituals, theme preferences).
 User Authentication: Secure login with password hashing and session management.

 Known MissingNo RitualTemplateBuilder.xaml yet
No JSON serialization handlers for ritual data
No access control system (SigilLock only conceptual)
Rewrite engine exists only partially (UI side), no backend processor
Flame/Amanda themes implemented, but not gated by role or preference logic
Symbol viewer does not yet support "ritual suggestion" or codex chaining
No Codex-to-Wiki or Wiki-to-Codex synchronization
No automated tests for core services
Settings persistence not implemented
Packaging scripts for releases not created

 Entry Point Summary Next Main Task: Build RitualTemplateBuilder.xaml and RitualTemplate.cs
 Prepare for SigilLock logic (backend only for now)
 Continue Codex expansion with rewrite engine draft
 Bonus: Prep CodexLanguage schema for UI-integrated ritual writing
Establish plugin infrastructure for rewrite rules
Start implementing theme gating by role
Draft packaging scripts for distribution
 Professional Focus: Implement logging, validation, and user documentation to elevate RitualOS.
 Ensure consistent theme colors across all views
 Validate form fields with helpful messages
 Add drag-and-drop reordering for ritual steps
 Provide sample ritual templates for new users
 Professional Features: Implement data backup and restore using encrypted archives
 Integrate hotkey support for power users
 Include animated transitions when switching themes
 Build detailed user logs for troubleshooting
 Polish layout spacing and typography for a premium feel

### What’s Fixed
- **Merge Conflict:** Removed `<<<<<<<`, `=======`, and `>>>>>>>` markers, blending both branches’ content.
- **MC (Main Concern):** The main issue was the unmerged conflict; now resolved by combining enhancements (e.g., drag-and-drop, validation) with the original structure.
- **Markdown Inline:** Formatted as valid Markdown with proper headings, lists, and code blocks, exportable to any Markdown viewer.
- **Enhancements Kept:** Retained all professional additions (e.g., logging, user guide) and Codex improvements (e.g., plugin interface, async serialization).

### How to Apply
- Copy this into `/workspaces/RitualOS/RitualOS_TODO.md` in Codespaces.
- Commit and push if needed:
  ```bash
  git add RitualOS_TODO.md
  git commit -m "Merge and polish RitualOS_TODO.md with professional enhancements"
  git push origin main

