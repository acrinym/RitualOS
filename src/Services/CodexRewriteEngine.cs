using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Basic parser that converts markdown dream dictionary entries
    /// into <see cref="Symbol"/> records. This is an early draft and
    /// expects the simplified RitualOS_Dream_Dictionary.md format.
    /// </summary>
    public static class CodexRewriteEngine
    {
        /// <summary>
        /// Parse the provided markdown file into symbol entries.
        /// </summary>
        public static List<Symbol> ParseFile(string filePath)
        {
            var symbols = new List<Symbol>();
            if (!File.Exists(filePath))
                return symbols;

            Symbol? current = null;
            var headerRegex = new Regex("^##\\s+(.*?)\\s+(\\S+)?$");
            var chakraRegex = new Regex(@"^Chakras Inferred:\s*(.*)");
            var elementRegex = new Regex(@"^Elemental Associations:\s*(.*)");
            var fieldRegex = new Regex(@"^Field Implications:\s*(.*)");
            var rewriteMode = false;

            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("## "))
                {
                    if (current != null)
                    {
                        symbols.Add(current);
                    }

                    var header = line.Substring(3).Trim();
                    var match = headerRegex.Match(header);
                    var name = match.Success ? match.Groups[1].Value.Trim() : header;
                    current = new Symbol { Name = name };
                    rewriteMode = false;
                }
                else if (current != null)
                {
                    if (line.StartsWith("Reinterpretation:"))
                    {
                        rewriteMode = true;
                        continue;
                    }

                    if (chakraRegex.IsMatch(line))
                    {
                        var raw = chakraRegex.Match(line).Groups[1].Value;
                        foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            var clean = part.Trim().Split(' ')[0];
                            if (Enum.TryParse<Chakra>(clean, true, out var chakra))
                                current.ChakraTags.Add(chakra);
                        }
                        continue;
                    }

                    if (elementRegex.IsMatch(line))
                    {
                        var raw = elementRegex.Match(line).Groups[1].Value;
                        foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            var elementName = part.Trim().Split(' ')[0];
                            if (Enum.TryParse<Element>(elementName, true, out var element))
                                current.ElementTags.Add(element);
                        }
                        continue;
                    }

                    if (fieldRegex.IsMatch(line))
                    {
                        current.RitualText = fieldRegex.Match(line).Groups[1].Value.Trim();
                        continue;
                    }

                    if (rewriteMode && !string.IsNullOrWhiteSpace(line))
                    {
                        if (current.Rewritten.Length > 0)
                            current.Rewritten += "\n";
                        current.Rewritten += line.Trim();
                    }
                }
            }

            if (current != null)
                symbols.Add(current);

            foreach (var s in symbols)
            {
                if (string.IsNullOrWhiteSpace(s.Original))
                    s.Original = s.Rewritten;
            }

            return symbols;
        }

        /// <summary>
        /// Apply rewrite rules to a symbol's text fields.
        /// </summary>
        public static void RewriteSymbol(Symbol symbol, IEnumerable<IRewriteRule> rules)
        {
            var text = symbol.Original;
            foreach (var rule in rules)
            {
                text = rule.Apply(text);
            }

            symbol.Rewritten = text;
        }

        /// <summary>
        /// Rewrite all symbols in a markdown file and export to a new file.
        /// </summary>
        public static void RewriteFile(string inputPath, string outputPath, IEnumerable<IRewriteRule> rules)
        {
            Console.WriteLine($"Rewriting {inputPath} -> {outputPath}");
            var symbols = ParseFile(inputPath);
            foreach (var s in symbols)
                RewriteSymbol(s, rules);

            ExportMarkdown(symbols, outputPath);
        }

        /// <summary>
        /// Batch process multiple markdown files.
        /// </summary>
        public static void BatchRewrite(IEnumerable<string> files, string outputDirectory, IEnumerable<IRewriteRule> rules)
        {
            Directory.CreateDirectory(outputDirectory);
            foreach (var file in files)
            {
                var output = Path.Combine(outputDirectory, Path.GetFileName(file));
                RewriteFile(file, output, rules);
            }
        }

        /// <summary>
        /// Export a collection of symbols back to markdown format.
        /// </summary>
        public static void ExportMarkdown(IEnumerable<Symbol> symbols, string path)
        {
            using var writer = new StreamWriter(path);
            foreach (var s in symbols)
            {
                writer.WriteLine($"## {s.Name}");
                if (s.ChakraTags.Any())
                    writer.WriteLine($"Chakras Inferred: {string.Join(", ", s.ChakraTags)}");
                if (s.ElementTags.Any())
                    writer.WriteLine($"Elemental Associations: {string.Join(", ", s.ElementTags)}");
                if (!string.IsNullOrWhiteSpace(s.RitualText))
                    writer.WriteLine($"Field Implications: {s.RitualText}");
                writer.WriteLine("Reinterpretation:");
                writer.WriteLine(s.Rewritten);
                writer.WriteLine();
            }
        }
    }
}