using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class HasCameraToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch ((int)value)
                {
                    case -1:
                        if (string.Compare((string)parameter, "textViewer") == 0)
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    default:
                        if (string.Compare((string)parameter, "textViewer") == 0)
                            return Visibility.Collapsed;
                        else
                            return Visibility.Visible;
                }
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
