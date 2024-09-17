using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NVisionInspectGUI.Commons;

namespace NVisionInspectGUI.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult_Cam1
    {
        public int m_bResultStatus;
        public int m_bInspectCompleted;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE_RESULT)]
        public string m_sResultString;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult_Cam2
    {
        public int m_bResultStatus;
        public int m_bInspectCompleted;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE_RESULT)]
        public string m_sResultString;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult_Cam3
    {
        public int m_bResultStatus;
        public int m_bInspectCompleted;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult_Cam4
    {
        public int m_bResultStatus;
        public int m_bInspectCompleted;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult_Cam5
    {
        public int m_bResultStatus;
        public int m_bInspectCompleted;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult_Cam6
    {
        public int m_bResultStatus;
        public int m_bInspectCompleted;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult_Cam7
    {
        public int m_bResultStatus;
        public int m_bInspectCompleted;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult_Cam8
    {
        public int m_bResultStatus;
        public int m_bInspectCompleted;
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectResult
    {
        public CNVisionInspectResult_Cam1 m_NVisionInspRes_Cam1;
        public CNVisionInspectResult_Cam2 m_NVisionInspRes_Cam2;
        public CNVisionInspectResult_Cam3 m_NVisionInspRes_Cam3;
        public CNVisionInspectResult_Cam4 m_NVisionInspRes_Cam4;
        public CNVisionInspectResult_Cam5 m_NVisionInspRes_Cam5;
        public CNVisionInspectResult_Cam6 m_NVisionInspRes_Cam6;
        public CNVisionInspectResult_Cam7 m_NVisionInspRes_Cam7;
        public CNVisionInspectResult_Cam8 m_NVisionInspRes_Cam8;
    }
}
