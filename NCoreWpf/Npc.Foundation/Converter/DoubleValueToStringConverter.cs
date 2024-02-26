using Npc.Foundation.Math;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class DoubleValueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double == false)
            {
                return value;
            }

            double v = (double)value;
            if (parameter != null && parameter.ToString() == "Truncate")
            {
                return IEEE754Util.ConvertDoubleToRoundFormat(v).ToString();
            }
            else
            {
                return v.ToString((parameter ?? "").ToString());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue;
            string strvalue = value as string;
            if (double.TryParse(strvalue, out doubleValue))
            {
                return doubleValue;
            }
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }

    public class DoubleNaNToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return "-";
            }
            else if (value is double)
            {
                string fmt = "F3";
                if (parameter != null)
                {
                    fmt = parameter.ToString();
                }

                if (value.ToString() == double.NaN.ToString())
                {
                    return "-";
                }
                return double.Parse(value.ToString()).ToString(fmt);
            }
            else if (value.ToString() == double.NaN.ToString())
            {
                return "-";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (value ?? "").ToString().Trim();
            if (str == "" || str == parameter.ToString())
            {
                return double.NaN;
            }

            double d = 0;
            double.TryParse(str, out d);
            return d;
        }
    }
}
