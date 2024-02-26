using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Util
{
    public static class DateUtil
    {
        public static int GetWeekOfYear(DateTime dateTime)
        {
            CultureInfo culture = CultureInfo.GetCultureInfo("en");
            CalendarWeekRule rule = CalendarWeekRule.FirstFullWeek;  
            DayOfWeek firstDay = DayOfWeek.Monday;                  

            return culture.Calendar.GetWeekOfYear(dateTime, rule, firstDay);
        }
    }
}
