using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class EnumDescriptionConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return string.Empty;

            Enum myEnum = (Enum)value;
            string description = GetEnumDescription(myEnum);
            return description;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }

        /// <summary>
        /// Enum에 명시된 [Description("")] Attribute를 표시한다.
        /// </summary>
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            // [NCS-2695]
            //  - CID 171132 Unchecked dynamic_cast
            //  - CID 171151 Dereference null return value
            //object[] attribArray = fieldInfo.GetCustomAttributes(false);
            //if (attribArray.Length == 0)
            //{
            //    return enumObj.ToString();
            //}
            //else
            //{
            //    DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
            //    return attrib.Description;
            //}
            if (fieldInfo != null)
            {
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                if (attribArray != null && attribArray.Length > 0 && attribArray[0] is DescriptionAttribute attrib)
                {
                    return attrib.Description;
                }
            }
            return enumObj.ToString();
        }
    }
}
