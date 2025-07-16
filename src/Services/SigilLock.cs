using System;
using System.Collections.Generic;
using RitualOS.Models;

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
            ["RitualBuilder"] = role => role != Role.Dreamworker
        };

        /// <summary>
        /// Determine if the specified role has access to a feature key.
        /// </summary>
        public static bool HasAccess(Role role, string featureKey)
        {
            if (_rules.TryGetValue(featureKey, out var rule))
            {
                return rule(role);
            }
            return true;
        }
    }
}
