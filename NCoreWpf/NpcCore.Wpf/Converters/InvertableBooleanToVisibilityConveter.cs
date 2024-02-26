using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace NpcCore.Wpf.Converters
{
    public static class CommonConvert
    {
        public static bool? BoolToBool(bool bVal)
        {
            if (bVal)
                return true;

            return false;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InvertableBooleanToVisibilityConverter : IValueConverter
    {
        enum Parameters
        {
            Normal, Inverted
        }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {

            var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);

            var boolValue = false;
            if (direction == Parameters.Inverted)
                boolValue = true;
            if (value != null)
                boolValue = (bool)value;


            if (direction == Parameters.Inverted)
                return !boolValue ? Visibility.Visible : Visibility.Collapsed;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class ReverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!true.Equals(value))
            {
                return Visibility.Visible;
            }

            if ("Hidden".Equals((parameter ?? "").ToString()) == true)
            {
                return Visibility.Hidden;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
