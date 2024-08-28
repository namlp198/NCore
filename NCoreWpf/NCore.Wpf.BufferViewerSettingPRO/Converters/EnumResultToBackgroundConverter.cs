using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NCore.Wpf.BufferViewerSettingPRO.Converters
{
    public class EnumResultToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                EnInspectResult inspectResult = (EnInspectResult)value;
                switch (inspectResult)
                {
                    case EnInspectResult.InspectResult_UNKNOWN:
                        return (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#808080");
                    case EnInspectResult.InspectResult_OK:
                        return (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#009900");
                    case EnInspectResult.InspectResult_NG:
                        return (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#CC0000");
                    default:
                        return (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#808080");
                }
            }
            else return null;

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumResultToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                EnInspectResult inspectResult = (EnInspectResult)value;
                switch (inspectResult)
                {
                    case EnInspectResult.InspectResult_UNKNOWN:
                        return "NONE";
                    case EnInspectResult.InspectResult_OK:
                        return "OK";
                    case EnInspectResult.InspectResult_NG:
                        return "NG";
                    default:
                        return "NONE";
                }
            }
            else return "NONE";

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class IntegerToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int nInspectResult = (int)value;
                switch (nInspectResult)
                {
                    case -1:
                        return (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#808080");
                    case 0:
                        return (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#CC0000");
                    case 1:
                        return (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#009900");
                    default:
                        return (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#808080");
                }
            }
            else return (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#808080");

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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
}
