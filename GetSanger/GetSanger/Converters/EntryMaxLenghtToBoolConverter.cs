using System;
using System.Globalization;
using Xamarin.Forms;

namespace GetSanger.Converters
{
    class EntryMaxLenghtToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is int length) && length < 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}