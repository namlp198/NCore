using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class ValueToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null || parameter == null || string.IsNullOrWhiteSpace(value.ToString()) || string.IsNullOrWhiteSpace(parameter.ToString()))
                {
                    return false;
                }

                var paramList = parameter.ToString().Split('|');
                var type = value.ToString();
                return paramList.Any(x => x == type);
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
