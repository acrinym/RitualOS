using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Markdig;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Minimal export service used for tests and demos.
    /// </summary>
    public interface IExportService
    {
        Task<string> ExportToMarkdownAsync(RitualTemplate ritual);
        Task<string> ExportToHtmlAsync(RitualTemplate ritual);
        Task<string> ExportToJsonAsync(RitualTemplate ritual);
        Task ExportToPdfAsync(RitualTemplate ritual, string outputPath);
        Task ExportToEpubAsync(RitualTemplate ritual, string outputPath);
        Task ExportRitualLibraryAsync(IEnumerable<RitualTemplate> rituals, string format, string outputPath);
        Task ExportWebsiteAsync(IEnumerable<RitualTemplate> rituals, string outputDir);
    }

    /// <summary>
    /// Basic implementation that serializes templates to JSON or simple text.
    /// </summary>
    public class ExportService : IExportService
    {
        private readonly IUserSettingsService _settings;

        public ExportService(IUserSettingsService settings)
        {
            _settings = settings;
        }

        public Task<string> ExportToMarkdownAsync(RitualTemplate ritual)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# {ritual.Name}");
            sb.AppendLine();
            sb.AppendLine($"**Intention:** {ritual.Intention}");

            if (ritual.Tools.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("## Tools");
                foreach (var t in ritual.Tools)
                    sb.AppendLine($"- {t}");
            }

            if (ritual.Steps.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("## Steps");
                foreach (var step in ritual.Steps)
                    sb.AppendLine($"1. {step}");
            }

            if (!string.IsNullOrWhiteSpace(ritual.Notes))
            {
                sb.AppendLine();
                sb.AppendLine("## Notes");
                sb.AppendLine(ritual.Notes);
            }

            return Task.FromResult(sb.ToString());
        }

        public async Task<string> ExportToHtmlAsync(RitualTemplate ritual)
        {
            var md = await ExportToMarkdownAsync(ritual);
            return Markdown.ToHtml(md);
        }

        public Task<string> ExportToJsonAsync(RitualTemplate ritual)
        {
            var json = JsonSerializer.Serialize(ritual, new JsonSerializerOptions { WriteIndented = true });
            return Task.FromResult(json);
        }

        public async Task ExportToPdfAsync(RitualTemplate ritual, string outputPath)
        {
            var text = await ExportToMarkdownAsync(ritual);
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Content().Text(text);
                });
            });
            doc.GeneratePdf(outputPath);
        }

        public async Task ExportToEpubAsync(RitualTemplate ritual, string outputPath)
        {
            var html = await ExportToHtmlAsync(ritual);

            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            Directory.CreateDirectory(Path.Combine(tempDir, "OEBPS"));
            Directory.CreateDirectory(Path.Combine(tempDir, "META-INF"));

            await File.WriteAllTextAsync(Path.Combine(tempDir, "mimetype"), "application/epub+zip");

            var containerXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">" +
                "<rootfiles><rootfile full-path=\"OEBPS/content.opf\" media-type=\"application/oebps-package+xml\" /></rootfiles></container>";
            await File.WriteAllTextAsync(Path.Combine(tempDir, "META-INF", "container.xml"), containerXml);

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<package xmlns=\"http://www.idpf.org/2007/opf\" unique-identifier=\"BookId\" version=\"3.0\">");
            sb.AppendLine("  <metadata xmlns:dc=\"http://purl.org/dc/elements/1.1/\">");
            sb.AppendLine($"    <dc:title>{ritual.Name}</dc:title>");
            sb.AppendLine("    <dc:language>en</dc:language>");
            sb.AppendLine($"    <dc:identifier id=\"BookId\">{ritual.TemplateId}</dc:identifier>");
            sb.AppendLine("  </metadata>");
            sb.AppendLine("  <manifest>");
            sb.AppendLine("    <item id=\"content\" href=\"index.html\" media-type=\"application/xhtml+xml\" />");
            sb.AppendLine("  </manifest>");
            sb.AppendLine("  <spine>");
            sb.AppendLine("    <itemref idref=\"content\" />");
            sb.AppendLine("  </spine>");
            sb.AppendLine("</package>");

            await File.WriteAllTextAsync(Path.Combine(tempDir, "OEBPS", "content.opf"), sb.ToString());
            await File.WriteAllTextAsync(Path.Combine(tempDir, "OEBPS", "index.html"), html);

            if (File.Exists(outputPath))
                File.Delete(outputPath);
            ZipFile.CreateFromDirectory(tempDir, outputPath);
            Directory.Delete(tempDir, true);
        }

        public async Task ExportRitualLibraryAsync(IEnumerable<RitualTemplate> rituals, string format, string outputPath)
        {
            switch (format.ToLower())
            {
                case "json":
                    var json = JsonSerializer.Serialize(rituals, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(outputPath, json);
                    break;
                case "markdown":
                    var md = new StringBuilder();
                    foreach (var r in rituals)
                    {
                        md.AppendLine(await ExportToMarkdownAsync(r));
                        md.AppendLine();
                    }
                    await File.WriteAllTextAsync(outputPath, md.ToString());
                    break;
                case "html":
                    var html = new StringBuilder();
                    html.AppendLine("<html><body>");
                    foreach (var r in rituals)
                    {
                        html.AppendLine(await ExportToHtmlAsync(r));
                        html.AppendLine("<hr/>");
                    }
                    html.AppendLine("</body></html>");
                    await File.WriteAllTextAsync(outputPath, html.ToString());
                    break;
                case "pdf":
                    var tempPdfMd = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".md");
                    await ExportRitualLibraryAsync(rituals, "markdown", tempPdfMd);
                    var text = await File.ReadAllTextAsync(tempPdfMd);
                    var doc = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(20);
                            page.Content().Text(text);
                        });
                    });
                    doc.GeneratePdf(outputPath);
                    File.Delete(tempPdfMd);
                    break;
                case "epub":
                    var tempHtml = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".html");
                    await ExportRitualLibraryAsync(rituals, "html", tempHtml);
                    var htmlContent = await File.ReadAllTextAsync(tempHtml);
                    await CreateBasicEpubAsync(htmlContent, "Ritual Library", outputPath);
                    File.Delete(tempHtml);
                    break;
                case "website":
                    await ExportWebsiteAsync(rituals, outputPath);
                    break;
                default:
                    throw new ArgumentException($"Unsupported format: {format}");
            }

            _settings.Save();
        }

        public async Task ExportWebsiteAsync(IEnumerable<RitualTemplate> rituals, string outputDir)
        {
            if (Path.HasExtension(outputDir))
                outputDir = Path.Combine(Path.GetDirectoryName(outputDir) ?? string.Empty,
                    Path.GetFileNameWithoutExtension(outputDir));

            Directory.CreateDirectory(outputDir);

            var index = new StringBuilder();
            index.AppendLine("<html><body><h1>Ritual Library</h1><ul>");

            foreach (var ritual in rituals)
            {
                var fileName = SanitizeFileName(ritual.Name) + ".html";
                var html = await ExportToHtmlAsync(ritual);
                await File.WriteAllTextAsync(Path.Combine(outputDir, fileName), html);
                index.AppendLine($"<li><a href=\"{fileName}\">{ritual.Name}</a></li>");
            }

            index.AppendLine("</ul></body></html>");
            await File.WriteAllTextAsync(Path.Combine(outputDir, "index.html"), index.ToString());
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        private static async Task CreateBasicEpubAsync(string htmlContent, string title, string outputPath)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            Directory.CreateDirectory(Path.Combine(tempDir, "OEBPS"));
            Directory.CreateDirectory(Path.Combine(tempDir, "META-INF"));

            await File.WriteAllTextAsync(Path.Combine(tempDir, "mimetype"), "application/epub+zip");

            var containerXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">" +
                "<rootfiles><rootfile full-path=\"OEBPS/content.opf\" media-type=\"application/oebps-package+xml\" /></rootfiles></container>";
            await File.WriteAllTextAsync(Path.Combine(tempDir, "META-INF", "container.xml"), containerXml);

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<package xmlns=\"http://www.idpf.org/2007/opf\" unique-identifier=\"BookId\" version=\"3.0\">");
            sb.AppendLine("  <metadata xmlns:dc=\"http://purl.org/dc/elements/1.1/\">");
            sb.AppendLine($"    <dc:title>{title}</dc:title>");
            sb.AppendLine("    <dc:language>en</dc:language>");
            sb.AppendLine($"    <dc:identifier id=\"BookId\">{Guid.NewGuid()}</dc:identifier>");
            sb.AppendLine("  </metadata>");
            sb.AppendLine("  <manifest>");
            sb.AppendLine("    <item id=\"content\" href=\"index.html\" media-type=\"application/xhtml+xml\" />");
            sb.AppendLine("  </manifest>");
            sb.AppendLine("  <spine>");
            sb.AppendLine("    <itemref idref=\"content\" />");
            sb.AppendLine("  </spine>");
            sb.AppendLine("</package>");

            await File.WriteAllTextAsync(Path.Combine(tempDir, "OEBPS", "content.opf"), sb.ToString());
            await File.WriteAllTextAsync(Path.Combine(tempDir, "OEBPS", "index.html"), htmlContent);

            if (File.Exists(outputPath))
                File.Delete(outputPath);
            ZipFile.CreateFromDirectory(tempDir, outputPath);
            Directory.Delete(tempDir, true);
        }
    }
}
