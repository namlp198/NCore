using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class CombiningConverter : IValueConverter
    {
        public IValueConverter ConverterFirst { get; set; }
        public IValueConverter ConverterSecond { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object convertedValue = ConverterFirst.Convert(value, targetType, parameter, culture);
            if (convertedValue != null)
            {
                return ConverterSecond.Convert(convertedValue, targetType, parameter, culture);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object convertedValue = ConverterSecond.ConvertBack(value, targetType, parameter, culture);
            if (convertedValue != null)
            {
                return ConverterFirst.ConvertBack(convertedValue, targetType, parameter, culture);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
