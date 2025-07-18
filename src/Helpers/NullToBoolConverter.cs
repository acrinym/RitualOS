using System;
using Avalonia.Data.Converters;

namespace RitualOS.Helpers
{
    public class NullToBoolConverter : IValueConverter
    {
        public static readonly NullToBoolConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            return value != null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
