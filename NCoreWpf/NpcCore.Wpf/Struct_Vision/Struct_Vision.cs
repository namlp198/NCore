using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NpcCore.Wpf.Struct_Vision
{
    #region Tool Results

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CSumResult
    {
        bool m_bSumResult;
        IntPtr m_resultImageBuffer;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CLocatorToolResult
    {
        public int m_nX { get; set; }
        public int m_nY { get; set; }
        public double m_dMatchingRate { get; set; }
        public int m_nDelta_x { get; set; }
        public int m_nDelta_y { get; set; }
        public double m_dDif_Angle { get; set; }
        public bool m_bResult { get; set; }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CAlgorithmsCountPixelResult
    {
        public int m_nNumberOfPixel;
        public bool m_bResult;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CAlgorithmsCalculateAreaResult
    {
        public int m_dArea;
        public bool m_bResult;
    }
    #endregion

    #region Data for Train Tool
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct RectForTrainLocTool
    {
        public int m_nRectIn_X { get; set; }
        public int m_nRectIn_Y { get; set; }
        public int m_nRectIn_Width { get; set; }
        public int m_nRectIn_Height { get; set; }
        public int m_nRectOut_X { get; set; }
        public int m_nRectOut_Y { get; set; }
        public int m_nRectOut_Width { get; set; }
        public int m_nRectOut_Height { get; set; }
        public double m_dMatchingRateLimit {  get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CParamCntPxlAlgorithm
    {
        public int m_nROIX {  get; set; }
        public int m_nROIY { get; set; }
        public int m_nROIWidth { get; set; }
        public int m_nROIHeight { get; set; }
        public double m_dROIAngleRotate { get; set; }
        public int m_nThresholdGrayMin { get; set; }
        public int m_nThresholdGrayMax { get; set; }
        public int m_nNumberOfPxlMin { get; set; }
        public int m_nNumberOfPxlMax { get; set; }
    }
    public struct CParamCalAreaAlgorithm
    {
        public int m_nROIX { get; set; }
        public int m_nROIY { get; set; }
        public int m_nROIWidth { get; set; }
        public int m_nROIHeight { get; set; }
        public double m_dROIAngleRotate { get; set; }
        public int m_nThreshold { get; set; }
        public int m_nAreaMin { get; set; }
        public int m_nAreaMax { get; set; }
    }
    #endregion
}
