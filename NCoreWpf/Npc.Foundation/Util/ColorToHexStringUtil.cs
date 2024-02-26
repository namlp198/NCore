using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Util
{
    public static class ColorToHexStringUtil
    {
        /// <summary>
        /// iter 순번에 맞는 hex color string을 반환
        /// </summary>
        /// <param name="iter"></param>
        /// <returns></returns>
        public static string GetHexStringByInt(string orignalHex, int iter)
        {
            string retColor = string.Empty;
            if (!string.IsNullOrEmpty(orignalHex))
            {
                var lstColorSplit = orignalHex.Split('|').ToList();
                foreach (var clr in lstColorSplit)
                {
                    var arrIterator = clr.Split('-').ToArray();
                    if (arrIterator[0] == iter.ToString())
                    {
                        retColor = arrIterator[1];
                        break;
                    }
                }
            }
            return retColor;
        }
    }
}
