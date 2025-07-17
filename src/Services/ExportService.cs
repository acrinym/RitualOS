using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RitualOS.Models;
using System.Text.Json;

namespace RitualOS.Services
{
    public interface IExportService
    {
        Task<string> ExportToMarkdownAsync(RitualTemplate ritual);
        Task<string> ExportToHtmlAsync(RitualTemplate ritual);
        Task<string> ExportToJsonAsync(RitualTemplate ritual);
        Task<string> ExportToPdfAsync(RitualTemplate ritual);
        Task<string> ExportToEpubAsync(RitualTemplate ritual);
        Task<string> ExportToWebsiteAsync(RitualTemplate ritual);
        Task ExportRitualLibraryAsync(IEnumerable<RitualTemplate> rituals, string format, string outputPath);
    }

    public class ExportService : IExportService
    {
        private readonly IUserSettingsService _settingsService;
        private readonly string _exportTemplatesPath;

        public ExportService(IUserSettingsService settingsService)
        {
            _settingsService = settingsService;
            _exportTemplatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "export_templates");
        }

        public async Task<string> ExportToMarkdownAsync(RitualTemplate ritual)
        {
            var markdown = new StringBuilder();
            
            // Header
            markdown.AppendLine($"# {ritual.Name}");
            markdown.AppendLine();
            markdown.AppendLine($"**Created:** {ritual.Created:yyyy-MM-dd HH:mm}  ");
            markdown.AppendLine($"**Modified:** {ritual.Modified:yyyy-MM-dd HH:mm}  ");
            markdown.AppendLine($"**Author:** {ritual.Author}  ");
            markdown.AppendLine($"**Version:** {ritual.Version}  ");
            markdown.AppendLine();
            
            // Description
            markdown.AppendLine("## Description");
            markdown.AppendLine(ritual.Description);
            markdown.AppendLine();
            
            // Metadata
            markdown.AppendLine("## Ritual Information");
            markdown.AppendLine($"- **Difficulty:** {ritual.Difficulty}");
            markdown.AppendLine($"- **Duration:** {ritual.Duration} minutes");
            markdown.AppendLine($"- **Moon Phase:** {ritual.MoonPhase}");
            markdown.AppendLine($"- **Elements:** {string.Join(", ", ritual.Elements)}");
            markdown.AppendLine($"- **Chakras:** {string.Join(", ", ritual.Chakras)}");
            markdown.AppendLine($"- **Tags:** {string.Join(", ", ritual.Tags)}");
            markdown.AppendLine();
            
            // Ingredients
            markdown.AppendLine("## Ingredients");
            foreach (var ingredient in ritual.Ingredients)
            {
                var required = ingredient.Required ? "**Required**" : "Optional";
                markdown.AppendLine($"- **{ingredient.Name}** ({ingredient.Quantity} {ingredient.Unit}) - {ingredient.Category} - {required}");
            }
            markdown.AppendLine();
            
            // Steps
            markdown.AppendLine("## Ritual Steps");
            foreach (var step in ritual.Steps.OrderBy(s => s.Order))
            {
                markdown.AppendLine($"### {step.Order}. {step.Title}");
                markdown.AppendLine($"**Duration:** {step.Duration} minutes");
                markdown.AppendLine($"**Elements:** {string.Join(", ", step.Elements)}");
                markdown.AppendLine($"**Chakras:** {string.Join(", ", step.Chakras)}");
                markdown.AppendLine();
                markdown.AppendLine(step.Description);
                markdown.AppendLine();
            }
            
            // Notes
            if (!string.IsNullOrEmpty(ritual.Notes))
            {
                markdown.AppendLine("## Notes");
                markdown.AppendLine(ritual.Notes);
                markdown.AppendLine();
            }
            
            // Footer
            markdown.AppendLine("---");
            markdown.AppendLine($"*Exported from RitualOS on {DateTime.Now:yyyy-MM-dd HH:mm}*");
            
            return markdown.ToString();
        }

        public async Task<string> ExportToHtmlAsync(RitualTemplate ritual)
        {
            var html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine($"    <title>{ritual.Name} - RitualOS</title>");
            html.AppendLine("    <style>");
            html.AppendLine(GetDefaultCss());
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            // Header
            html.AppendLine($"    <header class=\"ritual-header\">");
            html.AppendLine($"        <h1>{ritual.Name}</h1>");
            html.AppendLine($"        <div class=\"ritual-meta\">");
            html.AppendLine($"            <span>Created: {ritual.Created:yyyy-MM-dd HH:mm}</span>");
            html.AppendLine($"            <span>Author: {ritual.Author}</span>");
            html.AppendLine($"            <span>Version: {ritual.Version}</span>");
            html.AppendLine($"        </div>");
            html.AppendLine($"    </header>");
            
            // Description
            html.AppendLine($"    <section class=\"ritual-description\">");
            html.AppendLine($"        <h2>Description</h2>");
            html.AppendLine($"        <p>{ritual.Description}</p>");
            html.AppendLine($"    </section>");
            
            // Information
            html.AppendLine($"    <section class=\"ritual-info\">");
            html.AppendLine($"        <h2>Ritual Information</h2>");
            html.AppendLine($"        <div class=\"info-grid\">");
            html.AppendLine($"            <div><strong>Difficulty:</strong> {ritual.Difficulty}</div>");
            html.AppendLine($"            <div><strong>Duration:</strong> {ritual.Duration} minutes</div>");
            html.AppendLine($"            <div><strong>Moon Phase:</strong> {ritual.MoonPhase}</div>");
            html.AppendLine($"            <div><strong>Elements:</strong> {string.Join(", ", ritual.Elements)}</div>");
            html.AppendLine($"            <div><strong>Chakras:</strong> {string.Join(", ", ritual.Chakras)}</div>");
            html.AppendLine($"            <div><strong>Tags:</strong> {string.Join(", ", ritual.Tags)}</div>");
            html.AppendLine($"        </div>");
            html.AppendLine($"    </section>");
            
            // Ingredients
            html.AppendLine($"    <section class=\"ritual-ingredients\">");
            html.AppendLine($"        <h2>Ingredients</h2>");
            html.AppendLine($"        <ul class=\"ingredients-list\">");
            foreach (var ingredient in ritual.Ingredients)
            {
                var requiredClass = ingredient.Required ? "required" : "optional";
                html.AppendLine($"            <li class=\"{requiredClass}\">");
                html.AppendLine($"                <strong>{ingredient.Name}</strong> ({ingredient.Quantity} {ingredient.Unit}) - {ingredient.Category}");
                html.AppendLine($"            </li>");
            }
            html.AppendLine($"        </ul>");
            html.AppendLine($"    </section>");
            
            // Steps
            html.AppendLine($"    <section class=\"ritual-steps\">");
            html.AppendLine($"        <h2>Ritual Steps</h2>");
            foreach (var step in ritual.Steps.OrderBy(s => s.Order))
            {
                html.AppendLine($"        <div class=\"ritual-step\">");
                html.AppendLine($"            <h3>{step.Order}. {step.Title}</h3>");
                html.AppendLine($"            <div class=\"step-meta\">");
                html.AppendLine($"                <span>Duration: {step.Duration} minutes</span>");
                html.AppendLine($"                <span>Elements: {string.Join(", ", step.Elements)}</span>");
                html.AppendLine($"                <span>Chakras: {string.Join(", ", step.Chakras)}</span>");
                html.AppendLine($"            </div>");
                html.AppendLine($"            <p>{step.Description}</p>");
                html.AppendLine($"        </div>");
            }
            html.AppendLine($"    </section>");
            
            // Notes
            if (!string.IsNullOrEmpty(ritual.Notes))
            {
                html.AppendLine($"    <section class=\"ritual-notes\">");
                html.AppendLine($"        <h2>Notes</h2>");
                html.AppendLine($"        <p>{ritual.Notes}</p>");
                html.AppendLine($"    </section>");
            }
            
            // Footer
            html.AppendLine($"    <footer class=\"ritual-footer\">");
            html.AppendLine($"        <p>Exported from RitualOS on {DateTime.Now:yyyy-MM-dd HH:mm}</p>");
            html.AppendLine($"    </footer>");
            
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return html.ToString();
        }

        public async Task<string> ExportToJsonAsync(RitualTemplate ritual)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            return JsonSerializer.Serialize(ritual, options);
        }

        public async Task<string> ExportToPdfAsync(RitualTemplate ritual)
        {
            // For now, we'll generate HTML and suggest using a browser's print-to-PDF
            // In a full implementation, we'd use a library like iText7 or PdfSharp
            var html = await ExportToHtmlAsync(ritual);
            
            // Create a temporary HTML file that can be opened in browser for PDF export
            var tempPath = Path.Combine(Path.GetTempPath(), $"ritual_{ritual.Id}_{DateTime.Now:yyyyMMddHHmmss}.html");
            await File.WriteAllTextAsync(tempPath, html);
            
            return tempPath;
        }

        public async Task<string> ExportToEpubAsync(RitualTemplate ritual)
        {
            // EPUB generation would require a library like EpubSharp
            // For now, we'll create a basic EPUB structure
            var epubContent = new StringBuilder();
            
            // This is a simplified EPUB structure
            // In production, we'd use a proper EPUB library
            epubContent.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            epubContent.AppendLine("<package xmlns=\"http://www.idpf.org/2007/opf\" version=\"3.0\">");
            epubContent.AppendLine("  <metadata xmlns:dc=\"http://purl.org/dc/elements/1.1/\">");
            epubContent.AppendLine($"    <dc:title>{ritual.Name}</dc:title>");
            epubContent.AppendLine($"    <dc:creator>{ritual.Author}</dc:creator>");
            epubContent.AppendLine($"    <dc:date>{ritual.Created:yyyy-MM-dd}</dc:date>");
            epubContent.AppendLine("    <dc:language>en</dc:language>");
            epubContent.AppendLine("  </metadata>");
            epubContent.AppendLine("  <manifest>");
            epubContent.AppendLine("    <item id=\"nav\" href=\"nav.xhtml\" media-type=\"application/xhtml+xml\" properties=\"nav\"/>");
            epubContent.AppendLine("    <item id=\"chapter1\" href=\"chapter1.xhtml\" media-type=\"application/xhtml+xml\"/>");
            epubContent.AppendLine("  </manifest>");
            epubContent.AppendLine("  <spine>");
            epubContent.AppendLine("    <itemref idref=\"nav\"/>");
            epubContent.AppendLine("    <itemref idref=\"chapter1\"/>");
            epubContent.AppendLine("  </spine>");
            epubContent.AppendLine("</package>");
            
            return epubContent.ToString();
        }

        public async Task<string> ExportToWebsiteAsync(RitualTemplate ritual)
        {
            // Generate a complete website structure
            var websiteDir = Path.Combine(Path.GetTempPath(), $"ritual_website_{ritual.Id}_{DateTime.Now:yyyyMMddHHmmss}");
            Directory.CreateDirectory(websiteDir);
            
            var html = await ExportToHtmlAsync(ritual);
            var css = GetDefaultCss();
            
            // Create index.html
            await File.WriteAllTextAsync(Path.Combine(websiteDir, "index.html"), html);
            
            // Create styles.css
            await File.WriteAllTextAsync(Path.Combine(websiteDir, "styles.css"), css);
            
            // Create a simple JavaScript file for interactivity
            var js = GetDefaultJavaScript();
            await File.WriteAllTextAsync(Path.Combine(websiteDir, "script.js"), js);
            
            return websiteDir;
        }

        public async Task ExportRitualLibraryAsync(IEnumerable<RitualTemplate> rituals, string format, string outputPath)
        {
            switch (format.ToLower())
            {
                case "markdown":
                    await ExportLibraryToMarkdownAsync(rituals, outputPath);
                    break;
                case "html":
                    await ExportLibraryToHtmlAsync(rituals, outputPath);
                    break;
                case "json":
                    await ExportLibraryToJsonAsync(rituals, outputPath);
                    break;
                default:
                    throw new ArgumentException($"Unsupported format: {format}");
            }
        }

        private async Task ExportLibraryToMarkdownAsync(IEnumerable<RitualTemplate> rituals, string outputPath)
        {
            var markdown = new StringBuilder();
            markdown.AppendLine("# RitualOS Library");
            markdown.AppendLine();
            markdown.AppendLine($"**Exported:** {DateTime.Now:yyyy-MM-dd HH:mm}  ");
            markdown.AppendLine($"**Total Rituals:** {rituals.Count()}  ");
            markdown.AppendLine();
            
            foreach (var ritual in rituals.OrderBy(r => r.Name))
            {
                markdown.AppendLine($"## {ritual.Name}");
                markdown.AppendLine($"**Difficulty:** {ritual.Difficulty} | **Duration:** {ritual.Duration} min | **Elements:** {string.Join(", ", ritual.Elements)}");
                markdown.AppendLine();
                markdown.AppendLine(ritual.Description);
                markdown.AppendLine();
            }
            
            await File.WriteAllTextAsync(outputPath, markdown.ToString());
        }

        private async Task ExportLibraryToHtmlAsync(IEnumerable<RitualTemplate> rituals, string outputPath)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("    <title>RitualOS Library</title>");
            html.AppendLine("    <style>");
            html.AppendLine(GetDefaultCss());
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <header>");
            html.AppendLine("        <h1>RitualOS Library</h1>");
            html.AppendLine($"        <p>Exported on {DateTime.Now:yyyy-MM-dd HH:mm} | {rituals.Count()} rituals</p>");
            html.AppendLine("    </header>");
            html.AppendLine("    <main>");
            
            foreach (var ritual in rituals.OrderBy(r => r.Name))
            {
                html.AppendLine("        <article class=\"ritual-card\">");
                html.AppendLine($"            <h2>{ritual.Name}</h2>");
                html.AppendLine($"            <div class=\"ritual-meta\">");
                html.AppendLine($"                <span>Difficulty: {ritual.Difficulty}</span>");
                html.AppendLine($"                <span>Duration: {ritual.Duration} min</span>");
                html.AppendLine($"                <span>Elements: {string.Join(", ", ritual.Elements)}</span>");
                html.AppendLine($"            </div>");
                html.AppendLine($"            <p>{ritual.Description}</p>");
                html.AppendLine("        </article>");
            }
            
            html.AppendLine("    </main>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            await File.WriteAllTextAsync(outputPath, html.ToString());
        }

        private async Task ExportLibraryToJsonAsync(IEnumerable<RitualTemplate> rituals, string outputPath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var library = new
            {
                Exported = DateTime.Now,
                TotalRituals = rituals.Count(),
                Rituals = rituals.ToList()
            };
            
            var json = JsonSerializer.Serialize(library, options);
            await File.WriteAllTextAsync(outputPath, json);
        }

        private string GetDefaultCss()
        {
            return @"
                body {
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    line-height: 1.6;
                    margin: 0;
                    padding: 20px;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    min-height: 100vh;
                }
                
                .ritual-header {
                    background: rgba(255, 255, 255, 0.95);
                    padding: 30px;
                    border-radius: 15px;
                    margin-bottom: 30px;
                    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
                }
                
                .ritual-header h1 {
                    color: #2d3748;
                    margin: 0 0 15px 0;
                    font-size: 2.5em;
                }
                
                .ritual-meta {
                    display: flex;
                    gap: 20px;
                    flex-wrap: wrap;
                    color: #718096;
                }
                
                section {
                    background: rgba(255, 255, 255, 0.95);
                    padding: 25px;
                    border-radius: 15px;
                    margin-bottom: 25px;
                    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
                }
                
                h2 {
                    color: #2d3748;
                    border-bottom: 3px solid #667eea;
                    padding-bottom: 10px;
                    margin-bottom: 20px;
                }
                
                .info-grid {
                    display: grid;
                    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
                    gap: 15px;
                }
                
                .ingredients-list li {
                    padding: 10px;
                    margin: 5px 0;
                    border-radius: 8px;
                    background: #f7fafc;
                }
                
                .ingredients-list li.required {
                    border-left: 4px solid #e53e3e;
                }
                
                .ingredients-list li.optional {
                    border-left: 4px solid #38a169;
                }
                
                .ritual-step {
                    border: 1px solid #e2e8f0;
                    border-radius: 10px;
                    padding: 20px;
                    margin: 15px 0;
                    background: #f7fafc;
                }
                
                .step-meta {
                    display: flex;
                    gap: 15px;
                    margin: 10px 0;
                    font-size: 0.9em;
                    color: #718096;
                }
                
                .ritual-footer {
                    text-align: center;
                    color: #718096;
                    font-style: italic;
                    margin-top: 40px;
                }
                
                .ritual-card {
                    background: rgba(255, 255, 255, 0.95);
                    padding: 20px;
                    border-radius: 10px;
                    margin-bottom: 20px;
                    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
                }
            ";
        }

        private string GetDefaultJavaScript()
        {
            return @"
                // Add interactivity to the ritual website
                document.addEventListener('DOMContentLoaded', function() {
                    // Add print functionality
                    const printButton = document.createElement('button');
                    printButton.textContent = 'Print Ritual';
                    printButton.style.cssText = 'position: fixed; top: 20px; right: 20px; padding: 10px 20px; background: #667eea; color: white; border: none; border-radius: 5px; cursor: pointer;';
                    printButton.onclick = () => window.print();
                    document.body.appendChild(printButton);
                    
                    // Add step navigation
                    const steps = document.querySelectorAll('.ritual-step');
                    steps.forEach((step, index) => {
                        const stepNumber = document.createElement('div');
                        stepNumber.textContent = `Step ${index + 1}`;
                        stepNumber.style.cssText = 'position: absolute; top: -10px; left: -10px; background: #667eea; color: white; padding: 5px 10px; border-radius: 5px; font-size: 0.8em;';
                        step.style.position = 'relative';
                        step.appendChild(stepNumber);
                    });
                });
            ";
        }
    }
} 