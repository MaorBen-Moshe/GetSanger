﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace GetSanger.Converters
{
    public class SwitchToggledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventArgs = value as ToggledEventArgs;
            return eventArgs.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}