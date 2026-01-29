using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Foscamun2026.Converters
{
    public class EditAddTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isEdit = value is bool b && b;

            string key = isEdit ? "EditCommittee" : "AddCommittee";

            return Application.Current.TryFindResource(key) ?? key;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}