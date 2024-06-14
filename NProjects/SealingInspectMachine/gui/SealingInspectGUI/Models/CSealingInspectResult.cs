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
    public struct CSealingInspectResult
    {
        public CSealingInspectResult_TopCam m_sealingInspResult_TopCam;
        public CSealingInspectResult_SideCam m_sealingInspResult_SideCam;

        public int m_bStatus;
        public int m_bInspectComplete;
    }
}
