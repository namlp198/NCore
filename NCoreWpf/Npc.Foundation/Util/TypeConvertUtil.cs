using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Util
{
    public static class TypeConvertUtil
    {
        /// <summary>
        /// value 를 Enum 으로 변환
        /// </summary>
        /// <param name="laneID"></param>
        /// <returns></returns>
        public static T EnumConverter<T>(object value)
        {
            T result = default(T);
            var type = typeof(T);

            if (Enum.IsDefined(type, value))
            {
                result = (T)Enum.Parse(type, value.ToString());
            }

            return result;
        }
    }
}
