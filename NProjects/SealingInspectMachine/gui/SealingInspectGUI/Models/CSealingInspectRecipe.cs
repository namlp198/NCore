using SealingInspectGUI.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_TopCam_Frame1
    {
        public int m_nThresholdBinaryMinEnclosing;              // 0.
        public int m_nThresholdBinaryCannyHoughCircle;          // 1.
        public int m_nDistanceRadiusDiffMin;                    // 2.
        public int m_nDistanceMeasurementTolerance_Min;         // 3.
        public int m_nDistanceMeasurementTolerance_Max;         // 4.
        public int m_nRadiusInner_Min;                          // 5.
        public int m_nRadiusInner_Max;                          // 6.
        public int m_nRadiusOuter_Min;                          // 7.
        public int m_nRadiusOuter_Max;                          // 8.
        public int m_nDeltaRadiusOuterInner;                    // 9.
        public int m_nROIWidth;                                 // 10.
        public int m_nROIHeight;                                // 11.
        public int m_nROI12H_OffsetX;                           // 12.
        public int m_nROI12H_OffsetY;                           // 13.
        public int m_nROI3H_OffsetX;                            // 14.
        public int m_nROI3H_OffsetY;                            // 15.
        public int m_nROI6H_OffsetX;                            // 16.
        public int m_nROI6H_OffsetY;                            // 17.
        public int m_nROI9H_OffsetX;                            // 18.
        public int m_nROI9H_OffsetY;                            // 19.
        public int m_bUseAdvancedAlgorithms;                    // 20.
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_TopCam_Frame2
    {
        public int m_nDistanceMeasurementTolerance_Min;                // 1.            
        public int m_nDistanceMeasurementTolerance_Max;                // 2.
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame1
    {
        public int m_nDistanceMeasurementTolerance_Min;                // 1.
        public int m_nDistanceMeasurementTolerance_Max;                // 2.
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame2
    {
        public int m_nDistanceMeasurementTolerance_Min;                // 1.
        public int m_nDistanceMeasurementTolerance_Max;                // 2.
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame3
    {
        public int m_nDistanceMeasurementTolerance_Min;                // 1.
        public int m_nDistanceMeasurementTolerance_Max;                // 2.
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame4
    {
        public int m_nDistanceMeasurementTolerance_Min;                // 1.
        public int m_nDistanceMeasurementTolerance_Max;                // 2.
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CSealingInspectRecipe_TopCam
    {
        public CRecipe_TopCam_Frame1 m_recipeFrame1;
        public CRecipe_TopCam_Frame2 m_recipeFrame2;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CSealingInspectRecipe_SideCam
    {
        public CRecipe_SideCam_Frame1 m_recipeFrame1;
        public CRecipe_SideCam_Frame2 m_recipeFrame2;
        public CRecipe_SideCam_Frame3 m_recipeFrame3;
        public CRecipe_SideCam_Frame4 m_recipeFrame4;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CSealingInspectRecipe
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.MAX_TOPCAM_COUNT)]
        public CSealingInspectRecipe_TopCam[] m_sealingInspRecipe_TopCam;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.MAX_SIDECAM_COUNT)]
        public CSealingInspectRecipe_SideCam[] m_sealingInspRecipe_SideCam;
    }
}
