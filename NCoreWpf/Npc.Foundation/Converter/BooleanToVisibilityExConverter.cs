using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Npc.Foundation.Converter
{
    public class BooleanToVisibilityExConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (true.Equals(value))
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

    public class EmptyValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility rtnValue = Visibility.Collapsed;

            if (value == null || value.ToString() == "")
            {
                rtnValue = Visibility.Visible;
            }

            if ((parameter ?? "").Equals(""))
            {
                return rtnValue;
            }
            else
            {
                return rtnValue == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }

            //if (value == null || value.ToString() == "")
            //{
            //    return "0";
            //}
            //else
            //{
            //    return "1";
            //}
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EmptyListToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility rtnValue = Visibility.Collapsed;
            ObservableCollection<string> selectedValue = value as ObservableCollection<string>;
            if (value == null || value.ToString() == "")
            {
                rtnValue = Visibility.Visible;
            }
            else if (selectedValue == null || selectedValue.Count == 0)
            {
                rtnValue = Visibility.Visible;
            }

            if ((parameter ?? "").Equals(""))
            {
                return rtnValue;
            }
            else
            {
                return rtnValue == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }

            //if (value == null || value.ToString() == "")
            //{
            //    return "0";
            //}
            //else
            //{
            //    return "1";
            //}
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
