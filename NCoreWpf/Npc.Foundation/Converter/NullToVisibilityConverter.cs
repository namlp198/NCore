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
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility returnValue = Visibility.Visible;

            bool isEmpty = false;
            if ((value ?? "").Equals(""))
            {
                isEmpty = true;
            }

            if (isEmpty == true)
            {
                string param = parameter == null ? "" : parameter.ToString();
                switch (param.ToUpper())
                {
                    case "":
                    case "COLLAPSED":
                        returnValue = Visibility.Collapsed;
                        break;
                    case "HIDDEN":
                        returnValue = Visibility.Hidden;
                        break;
                }
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
