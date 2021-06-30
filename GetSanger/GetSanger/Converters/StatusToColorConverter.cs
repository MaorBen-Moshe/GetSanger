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
            ActivityStatus status = (ActivityStatus)value;
            switch (status)
            {
                case ActivityStatus.Rejected: brush = Brush.Red; break;
                case ActivityStatus.Pending: brush = Brush.Yellow; break;
                case ActivityStatus.Active: brush = Brush.Green; break;
                case ActivityStatus.Completed: brush = Brush.DarkGray; break;
                default:break;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as Brush;
            ActivityStatus status;

            if(brush == Brush.Red)
            {
                status = ActivityStatus.Rejected;
            }
            else if(brush == Brush.Yellow)
            {
                status = ActivityStatus.Pending;
            }
            else if (brush == Brush.Green)
            {
                status = ActivityStatus.Active;
            }
            else if(brush == Brush.DarkGray)
            {
                status = ActivityStatus.Completed;
            }
            else
            {
                throw new ArgumentException("Problem in 'statusToConverter'");
            }

            return status;
    }
}
}
