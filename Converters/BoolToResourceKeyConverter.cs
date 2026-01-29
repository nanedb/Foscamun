using System;
using System.Globalization;
using System.Windows.Data;

namespace Foscamun2026.Converters
{
    public class BoolToResourceKeyConverter : IValueConverter
    {
        public string TrueKey { get; set; }
        public string FalseKey { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = value is bool b && b;
            return flag ? TrueKey : FalseKey;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}