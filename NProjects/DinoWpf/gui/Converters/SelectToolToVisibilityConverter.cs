using DinoWpf.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DinoWpf.Converters
{
    public class SelectToolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch ((ToolSelected)value)
                {
                    case ToolSelected.LocatorTool:
                    case ToolSelected.SelectROITool:
                        if(string.Compare((string)parameter, "empty") == 0)
                            return Visibility.Collapsed;
                        else return Visibility.Visible;
                    case ToolSelected.None:
                        if (string.Compare((string)parameter, "empty") == 0)
                            return Visibility.Visible;
                        else return Visibility.Collapsed;
                    default:
                        return Visibility.Collapsed;
                }
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
   
    }
}
