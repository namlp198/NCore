using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml.Linq
{
    public static class XElementExtension
    {
        public static string SafeGetAttributeByString(this XElement target, string name)
        {
            if (target == null || string.IsNullOrWhiteSpace(name) == true)
            {
                return null;
            }

            var att = target.Attribute(name);
            if (att == null)
            {
                return null;
            }

            return att.Value;
        }

        public static int SafeGetAttributeByInt(this XElement target, string name, int defaultValue = 0)
        {
            var value = target.SafeGetAttributeByString(name);

            int result = defaultValue;
            int.TryParse(value, out result);

            return result;
        }

        public static int? SafeGetAttributeByNullableInt(this XElement target, string name, int? defaultValue = null)
        {
            var value = target.SafeGetAttributeByString(name);

            int result = 0;
            if (int.TryParse(value, out result) == true)
            {
                return result;
            }
            return defaultValue;
        }

        public static uint? SafeGetAttributeByNullableUint(this XElement target, string name, uint? defaultValue = null)
        {
            var value = target.SafeGetAttributeByString(name);

            uint result = 0;
            if (uint.TryParse(value, out result) == true)
            {
                return result;
            }
            return defaultValue;
        }

        public static float SafeGetAttributeByFloat(this XElement target, string name, float defaultValue = 0f)
        {
            var value = target.SafeGetAttributeByString(name);

            float result = defaultValue;

            if (float.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }

            //return result;
        }

        public static double SafeGetAttributeByDouble(this XElement target, string name, double defaultValue = 0.0d)
        {
            var value = target.SafeGetAttributeByString(name);

            double result = defaultValue;

            if (double.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }

            //return result;
        }

        public static double? SafeGetAttributeByNullableDouble(this XElement target, string name, double? defaultValue = null)
        {
            var value = target.SafeGetAttributeByString(name);

            double result = 0d;
            if (double.TryParse(value, out result) == true)
            {
                return result;
            }
            return defaultValue;
        }

        public static bool SafeGetAttributeByBoolean(this XElement target, string name)
        {
            var value = target.SafeGetAttributeByString(name) ?? "";
            value = value.ToLower();

            if ("true".Equals(value) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool? SafeGetAttributeByNullableBoolean(this XElement target, string name)
        {
            var value = target.SafeGetAttributeByString(name) ?? "";
            value = value.ToLower();

            if ("true".Equals(value) == true)
            {
                return true;
            }
            else if ("false".Equals(value) == true)
            {
                return false;
            }
            return null;
        }


        public static Nullable<T> SafeGetAttributeByEnum<T>(this XElement target, string name)
            where T : struct
        {
            if (target == null || string.IsNullOrWhiteSpace(name) == true)
            {
                return null;
            }

            var att = target.Attribute(name);
            if (att == null)
            {
                return null;
            }

            T result;
            if (Enum.TryParse<T>(att.Value, out result) == true)
            {
                return new Nullable<T>(result);
            }

            return null;
        }


    }
}
