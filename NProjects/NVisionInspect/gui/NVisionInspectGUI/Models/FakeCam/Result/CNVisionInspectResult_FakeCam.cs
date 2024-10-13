using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NVisionInspectGUI.Commons;

namespace NVisionInspectGUI.Models.FakeCam.Result
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult_CountPixel
    {
        public int m_bResultStatus;
        public int m_bInspectCompleted;
        public float m_fNumberOfPixel;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult_Decode
    {
        public int m_bResultStatus;
        public int m_bInspectCompleted;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE_RESULT)]
        public string m_sResultString;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult_FakeCam
    {
        public CNVisionInspectResult_CountPixel m_NVisionInspRes_CntPxl;
        public CNVisionInspectResult_Decode m_NVisionInspRes_Decode;
    }
}
