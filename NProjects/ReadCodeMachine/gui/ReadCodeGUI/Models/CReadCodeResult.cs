using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ReadCodeGUI.Commons;

namespace ReadCodeGUI.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CReadCodeResult
    {
        public int m_bResultStatus;
        public int m_bInspectCompleted;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Defines.MAX_STRING_SIZE_RESULT)]
        public string m_sResultString;
    }
}
