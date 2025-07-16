using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using RitualOS.Models;

namespace RitualOS.Converters
{
    /// <summary>
    /// Converter for binding element checkboxes to a collection. 🧰
    /// </summary>
    public class ElementCheckConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is List<Element> tags && parameter is Element element)
            {
                return tags.Contains(element);
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isChecked && parameter is Element element && targetType == typeof(List<Element>))
            {
                var list = new List<Element>();
                if (isChecked)
                    list.Add(element);
                return list;
            }
            return new List<Element>();
        }
    }
}
