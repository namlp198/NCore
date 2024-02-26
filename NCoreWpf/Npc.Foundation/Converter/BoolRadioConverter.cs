using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class BoolRadioConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //bool boolValue = (bool)value;
            //return this.Inverse ? !boolValue : boolValue;
            // [NCS-1252] [Coverity : 128538]
            bool boolValue = (value != null && value is bool) ? (bool)value : false;
            return this.Inverse == true ? !boolValue : boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //bool boolValue = (bool)value;

            //if (!boolValue)
            //{
            //    // We only care when the user clicks a radio button to select it.
            //    return null;
            //}

            //return !this.Inverse;

            // [NCS-1252] [Coverity : 128538]
            bool boolValue = (value != null && value is bool) ? (bool)value : false;
            if (boolValue == false)
            {
                // We only care when the user clicks a radio button to select it.
                return null;
            }

            return !this.Inverse;
        }
    }
}
