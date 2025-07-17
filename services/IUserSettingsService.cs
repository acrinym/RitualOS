namespace RitualOS.Services
{
    /// <summary>
    /// Provides access to persisted user settings.
    /// </summary>
    public interface IUserSettingsService
    {
        /// <summary>
        /// Current user settings loaded from disk.
        /// </summary>
        UserSettingsService.UserSettings Current { get; }

        /// <summary>
        /// Persist the current settings to disk.
        /// </summary>
        void Save();
    }
}
