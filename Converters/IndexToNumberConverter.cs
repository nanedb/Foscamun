using System;
using System.Globalization;
using System.Windows.Data;

namespace Foscamun.Converters
{
    /// <summary>
    /// Turns a zero-based index (e.g. ItemsControl.AlternationIndex) into a 1-based position.
    /// </summary>
    public class IndexToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is int i ? (i + 1).ToString(culture) : string.Empty;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
