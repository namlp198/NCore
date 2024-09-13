using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NVisionInspectGUI.Converter
{
    public class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }

            return value;
        }
    }
    public class BooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                bool bRes = (bool)value;
                if (bRes)
                {
                    return 1.0;
                }
                else
                {
                    return 0.3;
                }
            }

            return 0.0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class InvertBooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                bool bRes = (bool)value;
                if (!bRes)
                {
                    return 1.0;
                }
                else
                {
                    return 0.3;
                }
            }

            return 0.0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class RecipeIndexToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int nRecipeIdx = int.Parse(parameter.ToString());

            if (value != null)
            {
                int nCamCount = (int)value;

                if (nRecipeIdx <= nCamCount)
                    return Visibility.Visible;
                else if (nRecipeIdx > nCamCount)
                    return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CameraIndexToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int nCamIdx = int.Parse(value.ToString());

                if (nCamIdx == -1 || nCamIdx == 99)
                    return false;
                else
                    return true;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
