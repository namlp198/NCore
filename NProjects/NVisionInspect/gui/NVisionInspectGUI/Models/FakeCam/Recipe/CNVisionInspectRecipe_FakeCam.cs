using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Models.FakeCam.Recipe
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectRecipe_FakeCam
    {
        public CNVisionInspectRecipe_Locator m_NVisionInspRecipe_Locator;
        public CNVisionInspectRecipe_CountPixel m_NVisionInspRecipe_CountPixel;
        public CNVisionInspectRecipe_Decode m_NVisionInspRecipe_Decode;
        public CNVisionInspectRecipe_HSV m_NVisionInspRecipe_HSV;
    }
}
