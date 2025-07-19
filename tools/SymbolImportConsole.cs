using System;
using System.Threading.Tasks;
using RitualOS.Services;
using RitualOS.Tools;

namespace RitualOS.Tools
{
    public class SymbolImportConsole
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🔮 RitualOS SymbolWiki Import Tool");
            Console.WriteLine("==================================");
            Console.WriteLine();

            var symbolWikiService = new SymbolWikiService();
            var imageService = new SymbolImageService();
            var importTool = new SymbolImportTool(symbolWikiService, imageService);

            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            try
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "import":
                        if (args.Length < 4)
                        {
                            Console.WriteLine("Usage: import <book_directory> <book_name> <category>");
                            return;
                        }
                        await importTool.ImportFromBookDirectoryAsync(args[1], args[2], args[3]);
                        break;

                    case "convert":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("Usage: convert <png_directory> <output_directory>");
                            return;
                        }
                        await importTool.ConvertPngToSvgAsync(args[1], args[2]);
                        break;

                    case "add":
                        if (args.Length < 5)
                        {
                            Console.WriteLine("Usage: add <name> <description> <category> <tags>");
                            return;
                        }
                        await importTool.GenerateSymbolMetadataAsync(args[1], args[2], args[3], args[4]);
                        break;

                    case "export":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Usage: export <output_file> [category]");
                            return;
                        }
                        var category = args.Length > 2 ? args[2] : null;
                        await importTool.ExportSymbolDatabaseAsync(args[1], category);
                        break;

                    case "import-db":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Usage: import-db <input_file>");
                            return;
                        }
                        await importTool.ImportSymbolDatabaseAsync(args[1]);
                        break;

                    case "list":
                        await importTool.ListAllSymbolsAsync();
                        break;

                    case "search":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Usage: search <search_term>");
                            return;
                        }
                        await importTool.SearchSymbolsAsync(args[1]);
                        break;

                    case "stats":
                        await importTool.GetStorageStatsAsync();
                        break;

                    case "help":
                    default:
                        ShowHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine();
            Console.WriteLine("  import <book_directory> <book_name> <category>");
            Console.WriteLine("    Import symbols from a directory of images");
            Console.WriteLine("    Example: import ./books/lesser_key lesser_key goetic");
            Console.WriteLine();
            Console.WriteLine("  convert <png_directory> <output_directory>");
            Console.WriteLine("    Convert PNG files to SVG format");
            Console.WriteLine("    Example: convert ./png_symbols ./svg_symbols");
            Console.WriteLine();
            Console.WriteLine("  add <name> <description> <category> <tags>");
            Console.WriteLine("    Add a new symbol with metadata");
            Console.WriteLine("    Example: add \"Pentagram\" \"Five-pointed star\" unified \"protection,elements\"");
            Console.WriteLine();
            Console.WriteLine("  export <output_file> [category]");
            Console.WriteLine("    Export symbols to JSON file");
            Console.WriteLine("    Example: export symbols.json goetic");
            Console.WriteLine();
            Console.WriteLine("  import-db <input_file>");
            Console.WriteLine("    Import symbols from JSON file");
            Console.WriteLine("    Example: import-db symbols.json");
            Console.WriteLine();
            Console.WriteLine("  list");
            Console.WriteLine("    List all symbols in the database");
            Console.WriteLine();
            Console.WriteLine("  search <search_term>");
            Console.WriteLine("    Search for symbols");
            Console.WriteLine("    Example: search protection");
            Console.WriteLine();
            Console.WriteLine("  stats");
            Console.WriteLine("    Show database statistics");
            Console.WriteLine();
            Console.WriteLine("Available categories:");
            Console.WriteLine("  alchemical, astrological, celtic, chinese, egyptian,");
            Console.WriteLine("  goetic, hermetic, kabbalistic, nordic, planetary,");
            Console.WriteLine("  protection, runic, sigil, tibetan, unified, wiccan, other");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project tools import ./books/lesser_key \"Lesser Key of Solomon\" goetic");
            Console.WriteLine("  dotnet run --project tools convert ./png_symbols ./svg_symbols");
            Console.WriteLine("  dotnet run --project tools search pentagram");
        }
    }
} 