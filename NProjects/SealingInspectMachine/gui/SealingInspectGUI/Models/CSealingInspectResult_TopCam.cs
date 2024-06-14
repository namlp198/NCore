using SealingInspectGUI.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CSealingInspectResult_TopCam
    {
	    public int m_bStatusFrame1;
        public int m_bStatusFrame2;
        public int m_bInspectComplete;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.MAX_MEASURE_DIST_HOUGHCIRCLE_TOPCAM)]
        public double[] m_dArrDistResult_TopCam;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.MAX_MEASURE_DIST_HOUGHCIRCLE_TOPCAM)]
        public int[] m_nArrPosNG_TopCam;
    }
}
