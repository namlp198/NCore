using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Helper
{
    public static class UnitHelper
    {
        public static double RoundedValueByUnit(double value, string unit)
        {
            if (unit.ToLower() == "mm")
                return System.Math.Round(value, 2);
            else if ((unit.ToLower() == "um") || ((unit == "μm")))
                return System.Math.Round(value, 1);
            else
                return System.Math.Round(value, 0);
        }
        public static string ConvertToStringByUnit(double value, string unit)
        {
            if (unit.ToLower().Equals("mm"))
                return String.Format("{0:0.00}", value);
            else if (unit.Equals("μm") || unit.Equals("um") || (unit.ToLower().Equals("μm")))
                return String.Format("{0:0.0}", value);
            else
                return String.Format("{0:0.0}", value);
        }

        public static string ConvertToStringRoundedValueToUnit(double value)
        {
            return String.Format("{0:0}", value);
        }
    }
}
