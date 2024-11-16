using NVisionInspectGUI.Commons;
using NVisionInspectGUI.Models.FakeCam.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Models.Recipe
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectRecipe_Cam1
    {
        public CNVisionInspectRecipe_Locator m_NVisionInspRecipe_Locator;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.MAX_COUNT_PIXEL_TOOL_COUNT_CAM1)]
        public CNVisionInspectRecipe_CountPixel[] m_NVisionInspRecipe_CntPxl;
    }
}
