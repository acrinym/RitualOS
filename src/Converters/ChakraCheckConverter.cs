using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using RitualOS.Models;

namespace RitualOS.Converters
{
    public class ChakraCheckConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is List<Chakra> chakraTags && parameter is Chakra chakra)
            {
                return chakraTags.Contains(chakra);
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isChecked && parameter is Chakra chakra && targetType == typeof(List<Chakra>))
            {
                var list = (List<Chakra>)Activator.CreateInstance(targetType);
                if (isChecked)
                {
                    list.Add(chakra);
                }
                return list;
            }
            return new List<Chakra>();
        }
    }
}