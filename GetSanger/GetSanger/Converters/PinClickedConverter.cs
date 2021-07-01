using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GetSanger.Converters
{
    public class PinClickedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventArgs = value as PinClickedEventArgs;
            return eventArgs.Pin.Position;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
