using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
                            var element = part.Trim().Split(' ')[0];
                            if (!string.IsNullOrWhiteSpace(element))
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
    }
}
