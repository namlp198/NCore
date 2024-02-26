using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class IEnumerableFindSeqConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object data = values.ElementAtOrDefault(0);
            IEnumerable<object> source = values.ElementAtOrDefault(1) as IEnumerable<object>;

            if (source != null)
            {
                switch ((parameter ?? "").ToString())
                {
                    case "IsLast":
                        return source.LastOrDefault() == data;
                }
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
