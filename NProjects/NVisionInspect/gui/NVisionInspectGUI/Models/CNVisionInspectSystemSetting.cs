using NVisionInspectGUI.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectSystemSetting
    {
        public int m_nInspectCameraCount;
        public int m_bSimulation;
        public int m_bByPass;
        public int m_bTestMode;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sModelName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sModelList;
    }
}
