using System;
using System.IO;
using System.Text.Json;

namespace RitualOS.Services
{
    /// <summary>
    /// Persists simple user settings such as the last used template path.
    /// </summary>
    public class UserSettingsService : IUserSettingsService
    {
        private readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "user_settings.json");

        public class UserSettings
        {
            public string LastTemplatePath { get; set; } = string.Empty;
        }

        public UserSettings Current { get; private set; }

        public UserSettingsService()
        {
            Current = Load();
        }

        private UserSettings Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);
                    var settings = JsonSerializer.Deserialize<UserSettings>(json);
                    if (settings != null)
                        return settings;
                }
            }
            catch
            {
                // ignore settings load failures
            }

            return new UserSettings();
        }

        public void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(Current, options);
                File.WriteAllText(FilePath, json);
            }
            catch
            {
                // ignore settings save failures
            }
        }
    }
}

