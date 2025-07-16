using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Provides a simple way to store the current user's role.
    /// In the future this could load from settings or authentication.
    /// </summary>
    public static class UserContext
    {
        public static Role CurrentRole { get; set; } = Role.Ritualist;
    }
}
