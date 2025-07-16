using System;
using System.Collections.Generic;
using RitualOS.Models;
using System.IO;
using System.Text.Json;

namespace RitualOS.Services
{
    /// <summary>
    /// Provides simple role-based feature access evaluation.
    /// </summary>
    public static class SigilLock
    {
        private static readonly Dictionary<string, Func<Role, bool>> _rules = new()
        {
            ["CodexRewrite"] = role => role == Role.Technomage || role == Role.Guide,
            ["RitualBuilder"] = role => role != Role.Dreamworker,
            ["AppAccess"] = role => role != Role.Apprentice
        };

        static SigilLock()
        {
            LoadConfiguredRules();
        }

        private static void LoadConfiguredRules()
        {
            try
            {
                var path = Path.Combine("settings", "permissions.json");
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    var data = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
                    if (data != null)
                    {
                        foreach (var pair in data)
                        {
                            _rules[pair.Key] = role => pair.Value.Contains(role.ToString());
                        }
                    }
                }
            }
            catch
            {
                // ignore errors loading permissions
            }
        }

        /// <summary>
        /// Determine if the specified role has access to a feature key.
        /// </summary>
        public static bool HasAccess(Role role, string featureKey)
        {
            bool allowed = true;
            if (_rules.TryGetValue(featureKey, out var rule))
            {
                allowed = rule(role);
            }
            LogAccess(role, featureKey, allowed);
            return allowed;
        }

        private static void LogAccess(Role role, string featureKey, bool allowed)
        {
            try
            {
                Directory.CreateDirectory("logs");
                var line = $"{DateTime.Now:u} {role} {(allowed ? "granted" : "denied")} {featureKey}";
                File.AppendAllLines(Path.Combine("logs", "access.log"), new[] { line });
            }
            catch
            {
                // ignore logging failures
            }
        }
    }
}
