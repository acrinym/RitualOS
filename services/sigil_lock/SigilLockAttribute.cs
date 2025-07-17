using System;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Attribute to gate commands or views behind role-based SigilLock rules.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SigilLockAttribute : Attribute
    {
        /// <summary>
        /// Feature key to evaluate via <see cref="SigilLock"/>.
        /// </summary>
        public string FeatureKey { get; }

        public SigilLockAttribute(string featureKey)
        {
            FeatureKey = featureKey;
        }

        /// <summary>
        /// Determines if the provided role has access to this feature.
        /// </summary>
        public bool IsAllowed(Role role) => SigilLock.HasAccess(role, FeatureKey);
    }
}
