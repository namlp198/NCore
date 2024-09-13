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
    public struct CNVisionInspectCameraSetting
    {
        public int m_bSaveFullImage;
        public int m_bSaveDefectImage;
        public int m_bShowGraphics;
        public int m_nChannels;
        public int m_nFrameWidth;
        public int m_nFrameHeight;
        public int m_nFrameDepth;
        public int m_nMaxFrameCount;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sCameraName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sInterfaceType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sSensorType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sManufacturer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sSerialNumber;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sModel;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sFullImagePath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sDefectImagePath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sTemplateImagePath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sROIsPath;
    }
}
