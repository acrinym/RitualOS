using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RitualOS.Converters
{
    public class LowQuantityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int quantity && quantity < 5)
            {
                return "Bold";
            }
            return "Normal";
        }

        public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}