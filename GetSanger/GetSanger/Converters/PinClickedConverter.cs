using System;
using System.Globalization;
using Xamarin.Forms.GoogleMaps;

namespace GetSanger.Converters
{
    class PinClickedConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventArgs = value as PinClickedEventArgs;
            return eventArgs.Pin.Position;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
