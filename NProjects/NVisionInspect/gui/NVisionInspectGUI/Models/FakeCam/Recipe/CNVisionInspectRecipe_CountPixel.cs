using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Models.FakeCam.Recipe
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CNVisionInspectRecipe_CountPixel
    {
        public int m_nCountPixel_ROI_X;
        public int m_nCountPixel_ROI_Y;
        public int m_nCountPixel_ROI_Width;
        public int m_nCountPixel_ROI_Height;
        public int m_nCountPixel_ROI_Offset_X;
        public int m_nCountPixel_ROI_Offset_Y;
        public double m_nCountPixel_ROI_AngleRotate;
        public int m_nCountPixel_GrayThreshold_Min;
        public int m_nCountPixel_GrayThreshold_Max;
        public int m_nCountPixel_PixelCount_Min;
        public int m_nCountPixel_PixelCount_Max;
        public int m_bCountPixel_ShowGraphics;
        public int m_bCountPixel_UseOffset;
        public int m_bCountPixel_UseLocator;
    }
}
