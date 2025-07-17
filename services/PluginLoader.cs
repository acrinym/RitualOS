using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace RitualOS.Services
{
    public interface IRitualOSPlugin
    {
        string Name { get; }
        string Version { get; }
        void Initialize(IPluginContext context);
        void Shutdown();
    }

    public interface IPluginContext
    {
        string DataPath { get; }
    }

    public class DefaultPluginContext : IPluginContext
    {
        public string DataPath { get; }
        public DefaultPluginContext(string dataPath)
        {
            DataPath = dataPath;
        }
    }

    public class PluginLoader
    {
        public List<IRitualOSPlugin> LoadedPlugins { get; } = new();
        private readonly string _pluginsDir;

        public PluginLoader(string? pluginsDir = null)
        {
            _pluginsDir = pluginsDir ?? Path.Combine(AppContext.BaseDirectory, "plugins");
        }

        public void Load()
        {
            if (!Directory.Exists(_pluginsDir))
                return;

            foreach (var dir in Directory.GetDirectories(_pluginsDir))
            {
                var manifest = Path.Combine(dir, "manifest.json");
                var dll = Path.Combine(dir, "plugin.dll");
                if (!File.Exists(manifest) || !File.Exists(dll))
                    continue;
                try
                {
                    using var doc = JsonDocument.Parse(File.ReadAllText(manifest));
                    var entry = doc.RootElement.GetProperty("entryPoint").GetString();
                    if (string.IsNullOrWhiteSpace(entry))
                        continue;
                    var asm = Assembly.LoadFile(dll);
                    var type = asm.GetType(entry);
                    if (type == null || !typeof(IRitualOSPlugin).IsAssignableFrom(type))
                        continue;
                    var plugin = (IRitualOSPlugin)Activator.CreateInstance(type)!;
                    plugin.Initialize(new DefaultPluginContext(dir));
                    LoadedPlugins.Add(plugin);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Plugin load error in {dir}: {ex.Message}");
                }
            }
        }

        public void Shutdown()
        {
            foreach (var plugin in LoadedPlugins)
            {
                try
                {
                    plugin.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Plugin shutdown error: {ex.Message}");
                }
            }
            LoadedPlugins.Clear();
        }
    }
}
