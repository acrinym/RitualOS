using System;
using System.Globalization;
using Avalonia.Data.Converters;
using RitualOS.Helpers;
using RitualOS.Models;

namespace RitualOS.Converters
{
    public class ChakraToEmojiConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Chakra chakra)
            {
                return $"{ChakraHelper.GetEmoji(chakra)} {ChakraHelper.GetDisplayName(chakra)}";
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}