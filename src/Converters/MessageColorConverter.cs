using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RitualOS.Converters
{
    public class MessageColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string message)
            {
                return message.StartsWith("Error", StringComparison.OrdinalIgnoreCase) ? "Red" : "Green";
            }
            return "Black";
        }

        public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}