using System;
using System.Globalization;
using Xamarin.Forms.GoogleMaps;

namespace GetSanger.Converters
{
    class MapClickedConverter
    {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var eventArgs = value as MapClickedEventArgs;
                return eventArgs.Point;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
    }
}
