using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NCore.Wpf.BufferViewerSimple.Converters
{
    public class EnumResultToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                EInspectResult inspectResult = (EInspectResult)value;
                switch (inspectResult)
                {
                    case EInspectResult.InspectResult_UNKNOWN:
                        return (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#808080");
                    case EInspectResult.InspectResult_OK:
                        return (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#009900");
                    case EInspectResult.InspectResult_NG:
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
                EInspectResult inspectResult = (EInspectResult)value;
                switch (inspectResult)
                {
                    case EInspectResult.InspectResult_UNKNOWN:
                        return "NONE";
                    case EInspectResult.InspectResult_OK:
                        return "OK";
                    case EInspectResult.InspectResult_NG:
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
}
