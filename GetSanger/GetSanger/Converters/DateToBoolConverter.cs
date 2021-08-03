using System;
using System.Globalization;
using Xamarin.Forms;

namespace GetSanger.Converters
{
    public class DateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            int years = DateTime.Now.Year - date.Year;
            return (years > 18) || (years == 18 && DateTime.Now.Month >= date.Month);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
