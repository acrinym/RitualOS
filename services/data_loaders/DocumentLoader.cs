using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Markdig;
using Docnet.Core;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using VersOne.Epub;
using RitualOS.Models;

namespace RitualOS.Services
{
    public static class DocumentLoader
    {
        public static DocumentFile Load(string filePath)
        {
            var doc = new DocumentFile
            {
                FilePath = filePath,
                Title = Path.GetFileName(filePath)
            };

            if (!File.Exists(filePath))
            {
                doc.Content = "File not found.";
                return doc;
            }

            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            switch (ext)
            {
                case ".pdf":
                    doc.Content = LoadPdf(filePath);
                    break;
                case ".md":
                    doc.Content = LoadMarkdown(filePath);
                    break;
                case ".json":
                    doc.Content = LoadJson(filePath);
                    break;
                case ".epub":
                    doc.Content = LoadEpub(filePath);
                    break;
                case ".mobi":
                    doc.Content = "MOBI support not implemented.";
                    break;
                case ".html":
                case ".htm":
                    doc.Content = LoadHtml(filePath);
                    break;
                default:
                    doc.Content = File.ReadAllText(filePath);
                    break;
            }

            return doc;
        }

        /// <summary>
        /// Asynchronously loads a document for viewing without blocking the UI. 😄
        /// </summary>
        public static async Task<DocumentFile> LoadAsync(string filePath)
        {
            var doc = new DocumentFile
            {
                FilePath = filePath,
                Title = Path.GetFileName(filePath)
            };

            if (!File.Exists(filePath))
            {
                doc.Content = "File not found.";
                return doc;
            }

            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            switch (ext)
            {
                case ".pdf":
                    doc.Content = await Task.Run(() => LoadPdf(filePath));
                    break;
                case ".md":
                    var md = await File.ReadAllTextAsync(filePath);
                    doc.Content = Markdown.ToPlainText(md);
                    break;
                case ".json":
                    var json = await File.ReadAllTextAsync(filePath);
                    var obj = JsonSerializer.Deserialize<object>(json);
                    doc.Content = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
                    break;
                case ".epub":
                    doc.Content = await Task.Run(() => LoadEpub(filePath));
                    break;
                case ".mobi":
                    doc.Content = "MOBI support not implemented.";
                    break;
                case ".html":
                case ".htm":
                    var html = await File.ReadAllTextAsync(filePath);
                    var hdoc = new HtmlDocument();
                    hdoc.LoadHtml(html);
                    doc.Content = hdoc.DocumentNode.InnerText;
                    break;
                default:
                    doc.Content = await File.ReadAllTextAsync(filePath);
                    break;
            }

            return doc;
        }

        private static string LoadPdf(string filePath)
        {
            try
            {
                var sb = new StringBuilder();

                // Docnet requires a singleton library instance
                var library = DocLib.Instance;

                using var docReader = library.GetDocReader(filePath, new PageDimensions(1080, 1920));
                var pageCount = docReader.GetPageCount();
                for (var i = 0; i < pageCount; i++)
                {
                    using var page = docReader.GetPageReader(i);
                    sb.AppendLine(page.GetText());
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"PDF load error: {ex.Message}";
            }
        }

        private static string LoadMarkdown(string filePath)
        {
            var markdown = File.ReadAllText(filePath);
            return Markdown.ToPlainText(markdown);
        }

        private static string LoadJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var doc = JsonSerializer.Deserialize<object>(json);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }

        private static string LoadHtml(string filePath)
        {
            var html = File.ReadAllText(filePath);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode.InnerText;
        }

        private static string LoadEpub(string filePath)
        {
            try
            {
                var book = EpubReader.ReadBook(filePath);
                var sb = new StringBuilder();
                foreach (var contentFile in book.ReadingOrder)
                {
                    if (contentFile.ContentType == EpubContentType.XHTML_1_1)
                    {
                        sb.AppendLine(contentFile.Content);
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"EPUB load error: {ex.Message}";
            }
        }
    }
}