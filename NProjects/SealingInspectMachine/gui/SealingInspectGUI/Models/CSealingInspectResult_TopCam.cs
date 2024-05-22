﻿using System;
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
    }
}
