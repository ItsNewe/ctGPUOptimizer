using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace OptimizationEngine.UI.Converters
{
    /// <summary>
    /// Converter that returns true when value is null, false otherwise.
    /// Used to invert null check for UI visibility.
    /// </summary>
    public class NotNullToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for NotNullToFalseConverter.");
        }
    }
}
