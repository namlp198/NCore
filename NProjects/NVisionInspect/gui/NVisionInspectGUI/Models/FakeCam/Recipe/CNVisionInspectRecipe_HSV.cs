using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Models.FakeCam.Recipe
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectRecipe_HSV
    {
        public int m_nHueMin;
        public int m_nHueMax;
        public int m_nSaturationMin;
        public int m_nSaturationMax;
        public int m_nValueMin;
        public int m_nValueMax;
    }
}
