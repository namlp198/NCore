using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class NumberOverflowToIndicatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int number;
            int targetNumber;
            if (int.TryParse((value ?? "").ToString(), out number) == true && int.TryParse((parameter ?? "").ToString(), out targetNumber) == true)
            {
                if (number >= targetNumber)
                {
                    return targetNumber + "+";
                }
                else
                {
                    return number;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
