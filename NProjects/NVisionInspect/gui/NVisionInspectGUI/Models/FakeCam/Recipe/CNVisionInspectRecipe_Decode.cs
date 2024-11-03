using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Models.FakeCam.Recipe
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectRecipe_Decode
    {
        public int m_nDecode_ROI_X;
        public int m_nDecode_ROI_Y;
        public int m_nDecode_ROI_Width;
        public int m_nDecode_ROI_Height;
        public int m_nMaxCodeCount;
    }
}
