using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Helper
{
    public static class EnumHelper
    {
        public static double TOLERANCE = 0.001;
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            try
            {
                if (!Enum.TryParse(value, true, out T enumValue))
                {
                    return defaultValue;
                }

                return enumValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static T ToEnum<T>(this int value, T defaultValue) where T : struct
        {
            return ToEnum(value.ToString(), defaultValue);
        }
    }
}
