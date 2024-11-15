﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class DoubleToStringTwowayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return ((double)value * 100d).ToString("F0");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (value ?? "").ToString();

            double d = 0;
            double.TryParse(str, out d);
            return d / 100d;
        }
    }
}
