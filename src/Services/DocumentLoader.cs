using System;
using System.IO;
using System.Text;
using HtmlAgilityPack;
using Markdig;
using UglyToad.PdfPig;
using VersOne.Epub;

namespace RitualOS.Services
{
    public static class DocumentLoader
    {
        public static DocumentFile Load(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => LoadPdf(path),
                ".md" => LoadMarkdown(path),
                ".html" => LoadHtml(path),
                ".epub" => LoadEpub(path),
                _ => throw new NotSupportedException($"Unsupported file type: {extension}")
            };
        }

        private static DocumentFile LoadPdf(string path)
        {
            try
            {
                using var document = PdfDocument.Open(path);
                var builder = new StringBuilder();
                foreach (var page in document.GetPages())
                {
                    builder.AppendLine(page.Text);
                }
                return new DocumentFile
                {
                    Path = path,
                    Content = builder.ToString(),
                    Type = "pdf"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load PDF: {ex.Message}", ex);
            }
        }

        private static DocumentFile LoadMarkdown(string path)
        {
            try
            {
                var markdown = File.ReadAllText(path);
                var html = Markdown.ToHtml(markdown);
                return new DocumentFile
                {
                    Path = path,
                    Content = html,
                    Type = "markdown"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load Markdown: {ex.Message}", ex);
            }
        }

        private static DocumentFile LoadHtml(string path)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.Load(path);
                return new DocumentFile
                {
                    Path = path,
                    Content = doc.DocumentNode.OuterHtml,
                    Type = "html"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load HTML: {ex.Message}", ex);
            }
        }

        private static DocumentFile LoadEpub(string path)
        {
            try
            {
                var epubBook = EpubReader.ReadBook(path);
                var builder = new StringBuilder();
                foreach (var contentFile in epubBook.ReadingOrder)
                {
                    if (contentFile.ContentType == EpubContentType.XHTML_1_1)
                    {
                        builder.AppendLine(contentFile.Content);
                    }
                }
                return new DocumentFile
                {
                    Path = path,
                    Content = builder.ToString(),
                    Type = "epub"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load EPUB: {ex.Message}", ex);
            }
        }
    }
}