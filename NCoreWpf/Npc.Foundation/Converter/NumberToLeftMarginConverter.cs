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
    public class NumberToLeftMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double v = 0d;
            double multi = 1d;
            double.TryParse((parameter ?? "").ToString(), out multi);

            if (double.TryParse((value ?? "").ToString(), out v) == true)
            {
                if ((int)value == 1)
                {
                    return new Thickness(0, 0, 0, 0);
                }
                else
                {
                    v = v - 1d;
                    if ((int)value == 3)
                    {
                        v = v - 0.5d;
                    }
                    return new Thickness(v * multi, 0, 0, 0);
                }

            }

            return new Thickness();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
