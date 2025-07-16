using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RitualOS.Converters
{
    public class StringListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is List<string> list)
            {
                return string.Join(", ", list);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return new List<string>();
        }
    }
}