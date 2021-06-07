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
        public static eCategory FromString(string i_Value)
        {
            string fixedStr = i_Value.Replace(" ", "_");
            Enum.TryParse(fixedStr, out eCategory toRet);
            return toRet;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string originString = ((eCategory)value).ToString();
            string retVal = originString.Replace("_", " ");
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            eCategory toRet = eCategory.All;
            if (value is string categoryString)
            {
                toRet = FromString(categoryString);
            }

            return toRet;
        }
    }
}
