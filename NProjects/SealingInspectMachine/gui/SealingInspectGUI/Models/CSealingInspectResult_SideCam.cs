﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CSealingInspectResult_SideCam
    {
        public int m_bStatusFrame1;
        public int m_bStatusFrame2;
        public int m_bStatusFrame3;
        public int m_bStatusFrame4;
        public int m_bStatusFrame5;
        public int m_bStatusFrame6;
        public int m_bStatusFrame7;
        public int m_bStatusFrame8;
        public int m_bStatusFrame9;
        public int m_bStatusFrame10;
        public int m_bInspectComplete;
    }
}
