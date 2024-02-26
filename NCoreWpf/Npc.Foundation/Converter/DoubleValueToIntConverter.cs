using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class DoubleValueToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double == false)
            {
                return value;
            }

            double v = (double)value;
            return System.Math.Round(v, 0);// System.Convert.ToInt32(v);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value;
        }

        public static string Convert(double value)
        {
            double v = (double)value;
            return System.Math.Round(v, 0, MidpointRounding.AwayFromZero).ToString();
        }
    }
}
