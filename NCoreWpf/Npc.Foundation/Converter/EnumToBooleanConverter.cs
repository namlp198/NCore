using Npc.Foundation.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Npc.Foundation.Converter
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return ((Enum)value).HasFlag((Enum)parameter) && parameter.Equals(value);
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value.ToString().Trim().Length == 0) return null;
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }

    public class EnumToBooleanDefalutTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return ((Enum)value).HasFlag((Enum)parameter) && parameter.Equals(value);
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }

    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return ((Enum)value).HasFlag((Enum)parameter) && parameter.Equals(value) ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(Visibility.Visible) ? parameter : Binding.DoNothing;
        }
    }

    public class EnumToVisibilityCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return ((Enum)value).HasFlag((Enum)parameter) && parameter.Equals(value) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(Visibility.Visible) ? parameter : Binding.DoNothing;
        }
    }
    public class EnumToVisibilityCollapsedReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return ((Enum)value).HasFlag((Enum)parameter) && parameter.Equals(value) ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(Visibility.Visible) ? parameter : Binding.DoNothing;
        }
    }

    public class ValueToArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return System.Windows.Application.Current.Resources["IMG_Array_" + value.ToString()];
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValueToArrayConverterShape : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return System.Windows.Application.Current.Resources["IMG_IC_" + value.ToString()];
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValueToImageViewOptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return System.Windows.Application.Current.Resources["IMG_IC_" + value.ToString()];
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    // [NCS-2261] CreateRecipe] Board Setting 화면 - 6.Direction_Dropdown 동작하지 않음 및 디자인 미반영됨
    // 20210420 kst0909 수정
    public class ValueToArrayConverterForThumbnail : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return System.Windows.Application.Current.Resources["IMG_Array_Thumbnail_" + value.ToString()];
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InspectionModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = Visibility.Collapsed;
            if (value != null)
            {
                visibility = ((Enum)value).HasFlag((Enum)parameter) && parameter.Equals(value) ? Visibility.Visible : Visibility.Collapsed;
            }

            LogHub.Write($"InspectionModeToVisibilityConverter - InspectionMode : {value}, Visibility : {visibility}");

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(Visibility.Visible) ? parameter : Binding.DoNothing;
        }
    }
}
