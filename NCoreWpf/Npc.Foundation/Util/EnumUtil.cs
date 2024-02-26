using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Util
{
    public static class EnumUtil
    {
        /// <summary>
        /// Enum 의 Description
        /// </summary>
        /// <param name="type"></param>
        /// <param name="enumString"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Type type, string enumString)
        {
            FieldInfo field = type.GetField(enumString);
            if (null == field)
            {
                System.Diagnostics.Debug.WriteLine($"[Error] Enum '{type}' does not contain '{enumString}'.");
                return string.Empty;
            }

            System.ComponentModel.DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
            if (null == attr)
            {
                System.Diagnostics.Debug.WriteLine($"[Error] Enum '{type}' Field '{field}' Attribute does not exist.");
                return string.Empty;
            }

            return attr.Description;
        }
    }
}
