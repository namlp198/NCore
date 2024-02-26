using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Data
{
    public struct DoubleRangeValue
    {
        public DoubleRangeValue(double? value1, double? value2)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }
        public double? Value1 { get; set; }
        public double? Value2 { get; set; }
    }
}
