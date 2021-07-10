using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace GetSanger.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush = Brush.Transparent;
            eActivityStatus status = (eActivityStatus)value;
            switch (status)
            {
                case eActivityStatus.Rejected: brush = Brush.Red; break;
                case eActivityStatus.Pending: brush = Brush.Yellow; break;
                case eActivityStatus.Active: brush = Brush.Green; break;
                case eActivityStatus.Completed: brush = Brush.DarkGray; break;
                default:break;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as Brush;
            eActivityStatus status;

            if(brush == Brush.Red)
            {
                status = eActivityStatus.Rejected;
            }
            else if(brush == Brush.Yellow)
            {
                status = eActivityStatus.Pending;
            }
            else if (brush == Brush.Green)
            {
                status = eActivityStatus.Active;
            }
            else if(brush == Brush.DarkGray)
            {
                status = eActivityStatus.Completed;
            }
            else
            {
                throw new ArgumentException("Problem in 'statusToConverter'");
            }

            return status;
    }
}
}
