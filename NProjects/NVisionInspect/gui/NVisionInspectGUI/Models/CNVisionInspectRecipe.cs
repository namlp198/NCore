using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectRecipe
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
        public int m_nROI1_X;
        public int m_nROI1_Y;
        public int m_nROI1_Width;
        public int m_nROI1_Height;
        public int m_nROI1_Offset_X;
        public int m_nROI1_Offset_Y;
        public double m_nROI1_AngleRotate;
        public int m_nROI1_GrayThreshold_Min;
        public int m_nROI1_GrayThreshold_Max;
        public int m_nROI1_PixelCount_Min;
        public int m_nROI1_PixelCount_Max;
        public int m_bROI1UseOffset;
        public int m_bROI1UseLocator;
        public int m_bROI1ShowGraphics;
        // ROI2
        public int m_nROI2_X;
        public int m_nROI2_Y;
        public int m_nROI2_Width;
        public int m_nROI2_Height;
        public int m_nROI2_Offset_X;
        public int m_nROI2_Offset_Y;
        public double m_nROI2_AngleRotate;
        public int m_nROI2_GrayThreshold_Min;
        public int m_nROI2_GrayThreshold_Max;
        public int m_nROI2_PixelCount_Min;
        public int m_nROI2_PixelCount_Max;
        public int m_bROI2UseOffset;
        public int m_bROI2UseLocator;
        public int m_bROI2ShowGraphics;
        // ROI3
        public int m_nROI3_X;
        public int m_nROI3_Y;
        public int m_nROI3_Width;
        public int m_nROI3_Height;
        public int m_nROI3_Offset_X;
        public int m_nROI3_Offset_Y;
        public double m_nROI3_AngleRotate;
        public int m_nROI3_GrayThreshold_Min;
        public int m_nROI3_GrayThreshold_Max;
        public int m_nROI3_PixelCount_Min;
        public int m_nROI3_PixelCount_Max;
        public int m_bROI3UseOffset;
        public int m_bROI3UseLocator;
        public int m_bROI3ShowGraphics;
    }
}
