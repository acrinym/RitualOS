using RitualOS.Models;

namespace RitualOS.Helpers
{
    /// <summary>
    /// Provides emoji helpers for ritual elements. 😄
    /// </summary>
    public static class ElementHelper
    {
        public static string GetEmoji(Element element) => element switch
        {
            Element.Air => "🌬️",
            Element.Water => "💧",
            Element.Fire => "🔥",
            Element.Earth => "🌱",
            Element.Ether => "✨",
            _ => string.Empty
        };

        public static string GetDisplayName(Element element) => element.ToString();
    }
}
