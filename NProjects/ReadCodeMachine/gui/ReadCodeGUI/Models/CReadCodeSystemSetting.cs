using ReadCodeGUI.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReadCodeGUI.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CReadCodeSystemSetting
    {
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
        public string m_sTemplateImagePath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE)]
        public string m_sModelName;
        public int m_bTestMode;
    }
}
