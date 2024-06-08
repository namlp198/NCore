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
    public struct CSealingInspectSystemSetting
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sIPPLC1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sIPPLC2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sPortPLC1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sPortPLC2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sIPLightController1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sIPLightController2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sPortLightController1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sPortLightController2;
        public int m_bSaveFullImage;
        public int m_bSaveDefectImage;
        public int m_bShowDetailImage;
        public int m_bSimulation;
        public int m_bByPass;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sFullImagePath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sDefectImagePath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sModelName;
        public int m_nGoToPos1Time_Cavity1;
        public int m_nGoToPos2Time_Cavity1;
        public int m_nGoToPos3Time_Cavity1;
        public int m_nGoToPos4Time_Cavity1;
        public int m_nGoToPos5Time_Cavity1;
        public int m_nGoToPos6Time_Cavity1;
        public int m_nGoToPos7Time_Cavity1;
        public int m_nGoToPos8Time_Cavity1;
        public int m_nGoToPos9Time_Cavity1;
        public int m_nGoToPos10Time_Cavity1;
        public int m_nOffsetTime_Pos1_Cavity1;
        public int m_nOffsetTime_Pos2_Cavity1;
        public int m_nOffsetTime_Pos3_Cavity1;
        public int m_nOffsetTime_Pos4_Cavity1;
        public int m_nOffsetTime_Pos5_Cavity1;
        public int m_nOffsetTime_Pos6_Cavity1;
        public int m_nOffsetTime_Pos7_Cavity1;
        public int m_nOffsetTime_Pos8_Cavity1;
        public int m_nOffsetTime_Pos9_Cavity1;
        public int m_nOffsetTime_Pos10_Cavity1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.NUMBER_OF_LIGHT_CONTROLLER)]
        public CSealingInspectLightSetting[] m_LightSettings;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CSealingInspectLightSetting
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sCH1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sCH2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sCH3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sCH4;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sCH5;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sCH6;
    }
}
