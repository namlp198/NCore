using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CSealingInspect_Simulation_IO
    {
        public int m_bRing;
        public int m_b4Bar;
        public int m_bFrame1;
        public int m_bFrame2;
        public int m_bFrame3;
        public int m_bFrame4;
        public int m_bNG;
    }
}
