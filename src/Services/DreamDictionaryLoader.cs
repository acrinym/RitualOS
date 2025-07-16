using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Loads dream dictionary entries from the markdown file.
    /// </summary>
    public static class DreamDictionaryLoader
    {
        public static List<DreamDictionaryEntry> LoadFromMarkdown(string filePath)
        {
            var entries = new List<DreamDictionaryEntry>();
            if (!File.Exists(filePath))
                return entries;

            DreamDictionaryEntry? current = null;

            var headerRegex = new Regex("^##\\s+(.*?)\\s+(\\S+)$");
            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("## "))
                {
                    if (current != null)
                    {
                        entries.Add(current);
                    }

                    var header = line.Substring(3).Trim();
                    var match = headerRegex.Match(header);
                    string term = header;
                    string emoji = string.Empty;
                    if (match.Success)
                    {
                        term = match.Groups[1].Value.Trim();
                        emoji = match.Groups[2].Value.Trim();
                    }

                    current = new DreamDictionaryEntry
                    {
                        Term = term,
                        Emoji = emoji,
                        Content = string.Empty
                    };
                }
                else if (current != null)
                {
                    current.Content += line + "\n";
                }
            }

            if (current != null)
            {
                entries.Add(current);
            }

            return entries;
        }
    }
}
