using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NVisionInspectGUI.Converter
{
    public class EnumBooleanToBooleanMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (((emImageSource)value[0] == emImageSource.FromToCamera) && ((bool)value[1]) == true)
                {
                    return true;
                }
                else if (((emImageSource)value[0] == emImageSource.FromToImage) || ((bool)value[1]) == false)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class EnumBooleanToReverseBooleanMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (((emImageSource)value[0] == emImageSource.FromToCamera) && ((bool)value[1]) == true)
                {
                    return false;
                }
                else if (((emImageSource)value[0] == emImageSource.FromToImage))
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumBooleanToDoubleMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (((emImageSource)value[0] == emImageSource.FromToCamera) && ((bool)value[1]) == true)
                {
                    return 1.0;
                }
                else if (((emImageSource)value[0] == emImageSource.FromToImage) || ((bool)value[1]) == false)
                {
                    return 0.3;
                }
                else
                {
                    return 0.3;
                }
            }
            else
            {
                return 0.3;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class EnumBooleanToReverseDoubleMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (((emImageSource)value[0] == emImageSource.FromToCamera) && ((bool)value[1]) == true)
                {
                    return 0.3;
                }
                else if (((emImageSource)value[0] == emImageSource.FromToImage) || ((bool)value[1]) == false)
                {
                    return 1.0;
                }
                else
                {
                    return 1.0;
                }
            }
            else
            {
                return 0.3;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
