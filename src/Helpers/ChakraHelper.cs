using System.Collections.Generic;
using RitualOS.Models;

namespace RitualOS.Helpers
{
    public static class ChakraHelper
    {
        public static string GetEmoji(Chakra chakra) => chakra switch
        {
            Chakra.Root => "🔴",
            Chakra.Sacral => "🟠",
            Chakra.SolarPlexus => "🟡",
            Chakra.Heart => "💚",
            Chakra.Throat => "🔵",
            Chakra.ThirdEye => "🟣",
            Chakra.Crown => "⚪",
            _ => string.Empty
        };

        public static string GetDisplayName(Chakra chakra) => chakra switch
        {
            Chakra.SolarPlexus => "Solar Plexus",
            _ => chakra.ToString()
        };
    }
}
