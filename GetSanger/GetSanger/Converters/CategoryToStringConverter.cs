using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace GetSanger.Converters
{
    public class CategoryToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string originString = ((Category)value).ToString();
            return originString.Replace("_", " ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object toRet = null;
            if (value is string category)
            {
                bool succeed = Enum.TryParse(category, out Category parsed);
                toRet = succeed ? parsed : toRet;
            }

            return toRet;
        }
    }
}
