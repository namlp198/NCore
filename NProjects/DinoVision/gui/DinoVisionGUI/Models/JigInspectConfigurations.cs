using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DinoVisionGUI.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CJigInspectConfigurations
    {
        public string m_sCOMPort;
        public string m_sSaveImagePath;
        public bool m_bIsShowDetail;
        public bool m_bPCControlMode;
    }
}
