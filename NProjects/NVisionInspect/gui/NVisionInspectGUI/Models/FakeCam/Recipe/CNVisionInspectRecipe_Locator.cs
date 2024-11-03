using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Models.FakeCam.Recipe
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectRecipe_Locator
    {
        // params ROI of Template Matching
        public int m_nTemplateROI_OuterX;
        public int m_nTemplateROI_OuterY;
        public int m_nTemplateROI_Outer_Width;
        public int m_nTemplateROI_Outer_Height;
        public int m_nTemplateROI_InnerX;
        public int m_nTemplateROI_InnerY;
        public int m_nTemplateROI_Inner_Width;
        public int m_nTemplateROI_Inner_Height;
        public int m_nTemplateCoordinatesX;
        public int m_nTemplateCoordinatesY;
        public double m_dTemplateMatchingRate;
        public int m_bTemplateShowGraphics;
    }
}
