using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReadCodeGUI.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CReadCodeRecipe
    {
        public int m_bUseReadCode;
        public int m_bUseInkjetCharactersInspect;
        public int m_bUseRotateROI;
        public int m_nMaxCodeCount;
        // params ROI of Template Matching
        public int m_nTemplateROI_OuterX;
        public int m_nTemplateROI_OuterY;
        public int m_nTemplateROI_Outer_Width;
        public int m_nTemplateROI_Outer_Height;
        public int m_nTemplateROI_InnerX;
        public int m_nTemplateROI_InnerY;
        public int m_nTemplateROI_Inner_Width;
        public int m_nTemplateROI_Inner_Height;
        public int m_bTemplateShowGraphics;
        // params of Template Matching
        public int m_nTemplateCoordinatesX;
        public int m_nTemplateCoordinatesY;
        public double m_dTemplateAngleRotate;
        public double m_dTemplateMatchingRate;
        // ROI1
        public int m_nROI1_OffsetX;
        public int m_nROI1_OffsetY;
        public int m_nROI1_Width;
        public int m_nROI1_Height;
        public double m_nROI1_AngleRotate;
        public int m_nROI1_GrayThreshold_Min;
        public int m_nROI1_GrayThreshold_Max;
        public int m_nROI1_PixelCount_Min;
        public int m_nROI1_PixelCount_Max;
        public int m_bROI1ShowGraphics;
        // ROI2
        public int m_nROI2_OffsetX;
        public int m_nROI2_OffsetY;
        public int m_nROI2_Width;
        public int m_nROI2_Height;
        public double m_nROI2_AngleRotate;
        public int m_nROI2_GrayThreshold_Min;
        public int m_nROI2_GrayThreshold_Max;
        public int m_nROI2_PixelCount_Min;
        public int m_nROI2_PixelCount_Max;
        public int m_bROI2ShowGraphics;
        // ROI3
        public int m_nROI3_OffsetX;
        public int m_nROI3_OffsetY;
        public int m_nROI3_Width;
        public int m_nROI3_Height;
        public double m_nROI3_AngleRotate;
        public int m_nROI3_GrayThreshold_Min;
        public int m_nROI3_GrayThreshold_Max;
        public int m_nROI3_PixelCount_Min;
        public int m_nROI3_PixelCount_Max;
        public int m_bROI3ShowGraphics;
    }
}
