using System;
using System.Globalization;
using System.Windows.Data;

namespace Foscamun2026.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        // ConverterParameter must be: "TextIfTrue|TextIfFalse"
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string param)
                return "";

            var parts = param.Split('|');
            if (parts.Length != 2)
                return "";

            bool flag = value is bool b && b;

            return flag ? parts[0] : parts[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}