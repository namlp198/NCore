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
        public int m_nThresholdBinaryMinEnclosing;                 // 0.
        public int m_nThresholdBinaryCannyHoughCircle;             // 1.
        public int m_nDistanceRadiusDiffMin;                       // 2.
        public double m_dDistanceMeasurementTolerance_Min;         // 3.
        public double m_dDistanceMeasurementTolerance_Max;         // 4.
        public int m_nRadiusInner_Min;                             // 5.
        public int m_nRadiusInner_Max;                             // 6.
        public int m_nRadiusOuter_Min;                             // 7.
        public int m_nRadiusOuter_Max;                             // 8.
        public int m_nDeltaRadiusOuterInner;                       // 9.
        public int m_nROIWidth_Hor;                                // 10
        public int m_nROIHeight_Hor;                               // 11
        public int m_nROIWidth_Ver;                                // 12
        public int m_nROIHeight_Ver;                               // 13
        public int m_nROI12H_OffsetX;                              // 14.
        public int m_nROI12H_OffsetY;                              // 15.
        public int m_nROI3H_OffsetX;                               // 16.
        public int m_nROI3H_OffsetY;                               // 17.
        public int m_nROI6H_OffsetX;                               // 18.
        public int m_nROI6H_OffsetY;                               // 19.
        public int m_nROI9H_OffsetX;                               // 20.
        public int m_nROI9H_OffsetY;                               // 21.
        public int m_bUseAdvancedAlgorithms;                       // 22.
        public int m_nContourSizeMinEnclosingCircle_Min;           // 23.
        public int m_nContourSizeMinEnclosingCircle_Max;           // 24.
        public double m_dIncrementAngle;                           // 25.
        public double m_dThresholdCanny1_MakeROI;                     // 26.
        public double m_dThresholdCanny2_MakeROI;                     // 27.
        public int m_nDelayTimeGrab;                               // 28.
        public int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms; // 29.
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_TopCam_Frame2
    {
        public int m_nDistanceMeasurementTolerance_Min;                // 1.            
        public int m_nDistanceMeasurementTolerance_Max;                // 2.
        public int m_nDelayTimeGrab;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame1
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Top;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Bottom;
        public int m_nDistanceMeasurementTolerance_Min;                // 1.
        public int m_nDistanceMeasurementTolerance_Max;                // 2.
        public int m_nDelayTimeGrab;
        public int m_nFindStartEndX;
        public int m_nFindStartEndSearchRangeX;
        public int m_nFindStartEndXThresholdGray;
        public double m_dThresholdCanny1_MakeROI;
        public double m_dThresholdCanny2_MakeROI;
        public int m_bUseAdvancedAlgorithms;
        public int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame2
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Top;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Bottom;
        public int m_nDistanceMeasurementTolerance_Min;                // 1.
        public int m_nDistanceMeasurementTolerance_Max;                // 2.
        public int m_nDelayTimeGrab;
        public int m_nFindStartEndX;
        public int m_nFindStartEndSearchRangeX;
        public int m_nFindStartEndXThresholdGray;
        public double m_dThresholdCanny1_MakeROI;
        public double m_dThresholdCanny2_MakeROI;
        public int m_bUseAdvancedAlgorithms;
        public int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame3
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Top;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Bottom;
        public int m_nDistanceMeasurementTolerance_Min;                // 1.
        public int m_nDistanceMeasurementTolerance_Max;                // 2.
        public int m_nDelayTimeGrab;
        public int m_nFindStartEndX;
        public int m_nFindStartEndSearchRangeX;
        public int m_nFindStartEndXThresholdGray;
        public double m_dThresholdCanny1_MakeROI;
        public double m_dThresholdCanny2_MakeROI;
        public int m_bUseAdvancedAlgorithms;
        public int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame4
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Top;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Bottom;
        public int m_nDistanceMeasurementTolerance_Min;                // 1.
        public int m_nDistanceMeasurementTolerance_Max;                // 2.
        public int m_nDelayTimeGrab;
        public int m_nFindStartEndX;
        public int m_nFindStartEndSearchRangeX;
        public int m_nFindStartEndXThresholdGray;
        public double m_dThresholdCanny1_MakeROI;
        public double m_dThresholdCanny2_MakeROI;
        public int m_bUseAdvancedAlgorithms;
        public int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
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
