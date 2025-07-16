🔧 RitualOS – Expanded TODO File
Project Phase: RitualTemplateBuilder + Codex Expansion
Status: 🚧 In Development
File Source: allfiles (2).txt (parsed and analyzed)

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
1. RitualTemplateBuilder.xaml
Two-panel layout: Ritual form (left), Preview/Result (right)

Use TextBox, ComboBox, TagControl, RichTextBox

Optional: Live preview box w/ Markdown or rendered Codex-style ritual output

2. RitualTemplateBuilderViewModel.cs
Bind fields:

RitualName

Intention

Tools (ObservableCollection)

SpiritsInvoked (Multi-select)

ChakrasAffected (Enum w/ icons)

ElementsAligned (Enum w/ icons)

MoonPhase (string or enum)

RitualSteps (ObservableCollection<string>)

OutcomeField

Notes

Implement “Save Template” & “Load Template”

3. RitualTemplate.cs (Model)
csharp
Copy
Edit
public class RitualTemplate {
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
4. ritual_templates/*.json
Save rituals in a serializable JSON format

Create RitualTemplateSerializer.cs if needed

📦 TODO: Codex Rewrite Engine
Create CodexRewriteEngine.cs

Input: .md files from Codex folder

Output: Structured rewritten entries with:

Chakra tag

Element tag

Field directive

Add toggle between “Original / Rewritten” modes in UI

🔐 TODO: SigilLock (Role-Based Feature Lockout)
✅ Already written into internal TODO by you, now include:

SigilLock.cs to evaluate user access level

ClientProfile.cs with Role enum:

Ritualist

Dreamworker

Technomage

Guide

Optional: Tie into UI visibility bindings (via BooleanToVisibilityConverter)

📁 TODO: Directory & File Structure
bash
Copy
Edit
/rituals/         → JSON ritual templates  
/codex/           → Markdown + rewritten entries  
/themes/          → ThemeResource.xaml, colors  
/viewmodels/      → Builder, SymbolViewer, Theme  
/components/      → UI Elements (modular)  
/services/        → ThemeLoader, TemplateSaver, SigilLock  
/assets/          → Images, icons, elemental symbols  
🔮 BONUS: Future Modules (Add to TODO, Not Yet Active)
🧠 Dream Parser: Use dream logs to auto-suggest rituals

🧭 FieldMap Generator: Shows chakra + element heatmaps across rituals

✍️ AutoChanneled Rituals: Use AI to draft ritual templates based on dream/Codex entries

🧱 Module Plugin System: Allow drop-in codex transformations, ritual steps, etc.

🗃️ Exporters: Ritual → Markdown → PDF / EPUB / Website (SAAS vision)

💻 WikiSync Tool: Push Codex entries to the RitualOS GitHub wiki directly

🛑 Known Missing
No RitualTemplateBuilder.xaml yet

No JSON serialization handlers for ritual data

No access control system (SigilLock only conceptual)

Rewrite engine exists only partially (UI side), no backend processor

Flame/Amanda themes implemented, but not gated by role or preference logic

Symbol viewer does not yet support "ritual suggestion" or codex chaining

No Codex-to-Wiki or Wiki-to-Codex synchronization

🔖 Entry Point Summary
🎯 Next Main Task: Build RitualTemplateBuilder.xaml and RitualTemplate.cs
🔒 Prepare for SigilLock logic (backend only for now)
📚 Continue Codex expansion with rewrite engine draft
💎 Bonus: Prep CodexLanguage schema for UI-integrated ritual writing

🌟 Polish Checklist
🎨 Ensure consistent theme colors across all views
🔍 Validate form fields with helpful messages
🖱️ Add drag-and-drop reordering for ritual steps
📂 Provide sample ritual templates for new users

📈 Professional Features
🗄️ Implement data backup and restore using encrypted archives
⌨️ Integrate hotkey support for power users
🎞️ Include animated transitions when switching themes
📑 Build detailed user logs for troubleshooting
👁️ Polish layout spacing and typography for a premium feel
