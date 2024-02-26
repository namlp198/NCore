using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int returnValue = 0;
                if (parameter is Type)
                {
                    returnValue = (int)Enum.Parse((Type)parameter, value.ToString());
                }
                return returnValue;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                Enum enumValue = default(Enum);
                if (parameter is Type)
                {
                    enumValue = (Enum)Enum.Parse((Type)parameter, value.ToString());
                }
                return enumValue;
            }
            else
            {
                return null;
            }
        }
    }

    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string EnumString;
            try
            {
                // [NCS-1252] [Coverity : 128602]
                //EnumString = Enum.GetName((value.GetType()), value);
                var str = Enum.GetName((value.GetType()), value);
                EnumString = str != null ? str : string.Empty;
                return EnumString;
            }
            catch
            {
                return string.Empty;
            }
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
