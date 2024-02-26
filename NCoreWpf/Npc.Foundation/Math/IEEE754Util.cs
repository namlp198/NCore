using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Math
{
    public class IEEE754Util
    {
        public enum RoundType
        {
            Ceiling,
            Round,
            Truncate
        }

        public static double PlusOfTwoDouble(double number1, double number2, int digit = 3, RoundType rd = RoundType.Truncate)
        {
            var toDouble = Convert.ToDouble(Convert.ToDecimal(number1) + Convert.ToDecimal(number2));
            return ConvertDoubleToRoundFormat(toDouble, digit, rd);
        }

        public static double MinusOfTwoDouble(double number1, double number2, int digit = 3, RoundType rd = RoundType.Truncate)
        {
            var toDouble = Convert.ToDouble(Convert.ToDecimal(number1) - Convert.ToDecimal(number2));
            return ConvertDoubleToRoundFormat(toDouble, digit, rd);
        }

        public static double MultiplyofTwoDouble(double number1, double number2, int digit = 3, RoundType rd = RoundType.Truncate)
        {
            var toDouble = Convert.ToDouble(Convert.ToDecimal(number1) * Convert.ToDecimal(number2));
            return ConvertDoubleToRoundFormat(toDouble, digit, rd);
        }

        public static double DivisionofTwoDouble(double number1, double number2, int digit = 3, RoundType rd = RoundType.Truncate)
        {
            var toDouble = Convert.ToDouble(Convert.ToDecimal(number1) / Convert.ToDecimal(number2));
            return ConvertDoubleToRoundFormat(toDouble, digit, rd);
        }

        public static double ConvertDoubleToRoundFormat(double number1, int digit = 3, RoundType rd = RoundType.Truncate)
        {
            double digitCal = System.Math.Pow(10, digit) / 10;
            var ret = System.Math.Truncate(number1 * digitCal) / digitCal;
            switch (rd)
            {
                case RoundType.Ceiling:
                    ret = System.Math.Ceiling(number1 * digitCal) / digitCal;
                    break;
                case RoundType.Round:
                    ret = System.Math.Round(number1 * digitCal) / digitCal;
                    break;
                case RoundType.Truncate:
                    ret = System.Math.Truncate(number1 * digitCal) / digitCal;
                    break;
            }

            return ret;
        }

        public static int PlusOfTwoDoubleToInt32(double number1, double number2)
        {
            return Convert.ToInt32(Convert.ToDecimal(number1) + Convert.ToDecimal(number2));
        }

        public static int MinusOfTwoDoubleToInt32(double number1, double number2)
        {
            return Convert.ToInt32(Convert.ToDecimal(number1) - Convert.ToDecimal(number2));
        }

        public static int MultiplyofTwoDoubleToInt32(double number1, double number2)
        {
            return Convert.ToInt32(Convert.ToDecimal(number1) * Convert.ToDecimal(number2));
        }

        public static int DivisionofTwoDoubleToInt32(double number1, double number2)
        {
            return Convert.ToInt32(Convert.ToDecimal(number1) / Convert.ToDecimal(number2));
        }
    }
}
