using System;
using System.Globalization;
using Avalonia.Data.Converters;
using RitualOS.Helpers;
using RitualOS.Models;

namespace RitualOS.Converters
{
    /// <summary>
    /// Converts <see cref="Element"/> values to emoji labels. 😄
    /// </summary>
    public class ElementToEmojiConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Element element)
            {
                return $"{ElementHelper.GetEmoji(element)} {ElementHelper.GetDisplayName(element)}";
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
