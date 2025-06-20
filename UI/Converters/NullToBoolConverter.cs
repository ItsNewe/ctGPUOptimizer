using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace OptimizationEngine.UI.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for NullToBoolConverter.");
        }
    }
}
