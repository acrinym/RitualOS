using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace RitualOS.Converters
{
    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? FontWeight.Bold : FontWeight.Normal;
            return FontWeight.Normal;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 