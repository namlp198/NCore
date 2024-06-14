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
        public int m_nThresholdBinaryMinEnclosing;                    // 0.
        public int m_nThresholdBinaryCannyHoughCircle;                // 1.
        public int m_nDistanceRadiusDiffMin;                          // 2.
        public double m_dDistanceMeasurementTolerance_Refer;          // 3.
        public double m_dDistanceMeasurementTolerance_Min;            // 4.
        public double m_dDistanceMeasurementTolerance_Max;            // 5.
        public int m_nRadiusInner_Min;                                // 6.
        public int m_nRadiusInner_Max;                                // 7.
        public int m_nRadiusOuter_Min;                                // 8.
        public int m_nRadiusOuter_Max;                                // 9.
        public int m_nDeltaRadiusOuterInner;                          // 10.
        public int m_nROIWidth_Hor;                                   // 11
        public int m_nROIHeight_Hor;                                  // 12
        public int m_nROIWidth_Ver;                                   // 13
        public int m_nROIHeight_Ver;                                  // 14
        public int m_nROI12H_OffsetX;                                 // 15.
        public int m_nROI12H_OffsetY;                                 // 16.
        public int m_nROI3H_OffsetX;                                  // 17.
        public int m_nROI3H_OffsetY;                                  // 18.
        public int m_nROI6H_OffsetX;                                  // 19.
        public int m_nROI6H_OffsetY;                                  // 20.
        public int m_nROI9H_OffsetX;                                  // 21.
        public int m_nROI9H_OffsetY;                                  // 22.
        public int m_bUseAdvancedAlgorithms;                          // 23.
        public int m_nContourSizeMinEnclosingCircle_Min;              // 24.
        public int m_nContourSizeMinEnclosingCircle_Max;              // 25.
        public double m_dIncrementAngle;                              // 26.
        public double m_dThresholdCanny1_MakeROI;                     // 27.
        public double m_dThresholdCanny2_MakeROI;                     // 28.
        public int m_nDelayTimeGrab;                                  // 29.
        public int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;    // 30.
        public int m_nHoughCircleParam1;                              // 31.
        public int m_nHoughCircleParam2;                              // 32.
        public int m_nHMin;
        public int m_nHMax;
        public int m_nSMin;
        public int m_nSMax;
        public int m_nVMin;
        public int m_nVMax;
        public double m_dRatioPxlUm;
        public double m_dRatioUmPxl;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_TopCam_Frame2
    {
        public int m_nROIWidth;                                       // 0. 
        public int m_nROIHeight;                                      // 1.
        public int m_nROI1H_OffsetX;                                  // 2.
        public int m_nROI1H_OffsetY;                                  // 3.
        public int m_nROI5H_OffsetX;                                  // 4.
        public int m_nROI5H_OffsetY;                                  // 5.
        public int m_nROI7H_OffsetX;                                  // 6.
        public int m_nROI7H_OffsetY;                                  // 7.
        public int m_nROI11H_OffsetX;                                 // 8.
        public int m_nROI11H_OffsetY;                                 // 9.
        public double m_dDistanceMeasurementTolerance_Refer;          // 10.
        public double m_dDistanceMeasurementTolerance_Min;            // 11.
        public double m_dDistanceMeasurementTolerance_Max;            // 12.
        public int m_nDelayTimeGrab;                                  // 13.
        public int m_nThresholdBinary;                                // 14.
        public int m_nContourSizeFindBlob_Min;                        // 15.
        public int m_nContourSizeFindBlob_Max;                        // 16.
        public int m_nThreshBinary_FindBlobWhite;                     // 17.
        public int m_nThreshBinary_FindBlobWhite_Max;                 // 18.
        public int m_nThreshBinary_FindBlobBlack;                     // 19.
        public int m_nThreshBinary_FindBlobBlack_Max;                 // 20.
        public int m_nBlobCount_Max;                                  // 21.
        public double m_dBlobArea_Min;                                // 22.
        public double m_dBlobArea_Max;                                // 23.
        public int m_nUseCheckSurface;                                // 24. 
        public int m_nSelectMethodFindCircle;                         // 25.
        public int m_nThresholdBinary_FindCircle;                     // 26.
        public int m_nContourSizeMin;                                 // 27.
        public int m_nContourSizeMax;                                 // 28.
        public int m_nRadiusMin;                                      // 29.
        public int m_nRadiusMax;                                      // 30.
        public int m_nMinDist_HoughCircle;                            // 31.
        public int m_nParam1_HoughCircle;                             // 32.
        public int m_nParam2_HoughCircle;                             // 33.
        public int m_nThreshold1_Canny;                               // 34.
        public int m_nThreshold2_Canny;                               // 35.
        public int Offset_ROIFindMeasurePoint1H_X;                    // 36.
        public int Offset_ROIFindMeasurePoint1H_Y;                    // 37.
        public int Offset_ROIFindMeasurePoint5H_X;                    // 38.
        public int Offset_ROIFindMeasurePoint5H_Y;                    // 39.
        public int Offset_ROIFindMeasurePoint7H_X;                    // 40.
        public int Offset_ROIFindMeasurePoint7H_Y;                    // 41.
        public int Offset_ROIFindMeasurePoint11H_X;                   // 42.
        public int Offset_ROIFindMeasurePoint11H_Y;                   // 43.
        public int m_nWidth_ROIFindSealingOverflow;                   // 44.
        public int m_nHeight_ROIFindSealingOverflow;                  // 45.
        public int m_nOffset_ROIFindSealingOverflow_X_1H_Hoz;         // 46.
        public int m_nOffset_ROIFindSealingOverflow_Y_1H_Hoz;         // 47.
        public int m_nOffset_ROIFindSealingOverflow_X_1H_Ver;         // 48.
        public int m_nOffset_ROIFindSealingOverflow_Y_1H_Ver;         // 49.
        public int m_nOffset_ROIFindSealingOverflow_X_5H_Hoz;         // 50.
        public int m_nOffset_ROIFindSealingOverflow_Y_5H_Hoz;         // 51.
        public int m_nOffset_ROIFindSealingOverflow_X_5H_Ver;         // 52.
        public int m_nOffset_ROIFindSealingOverflow_Y_5H_Ver;         // 53.
        public int m_nOffset_ROIFindSealingOverflow_X_7H_Hoz;         // 54.
        public int m_nOffset_ROIFindSealingOverflow_Y_7H_Hoz;         // 55.
        public int m_nOffset_ROIFindSealingOverflow_X_7H_Ver;         // 56.
        public int m_nOffset_ROIFindSealingOverflow_Y_7H_Ver;         // 57.
        public int m_nOffset_ROIFindSealingOverflow_X_11H_Hoz;        // 58.
        public int m_nOffset_ROIFindSealingOverflow_Y_11H_Hoz;        // 59.
        public int m_nOffset_ROIFindSealingOverflow_X_11H_Ver;        // 60.
        public int m_nOffset_ROIFindSealingOverflow_Y_11H_Ver;        // 61.
        public int m_nThresholdBinary_FindSealingOverflow;            // 62.
        public int m_nContourSize_FindSealingOverflow_Max;            // 63.
        public double m_dAreaContour_FindSealingOverflow_Max;         // 64.
        public int m_nThresholdBinary_MeasureWidth;                   // 65.
        public int m_nHMin;                                           // 66.
        public int m_nHMax;                                           // 67.
        public int m_nSMin;                                           // 68.
        public int m_nSMax;                                           // 69.
        public int m_nVMin;                                           // 70.
        public int m_nVMax;                                           // 71.
        public double m_dRatioPxlUm;                                   // 72.
        public double m_dRatioUmPxl;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame1
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Top;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Bottom;
        public double m_dDistanceMeasurementTolerance_Refer;
        public double m_dDistanceMeasurementTolerance_Min;                // 1.
        public double m_dDistanceMeasurementTolerance_Max;                // 2.
        public int m_nDelayTimeGrab;
        public int m_nFindStartEndX;
        public int m_nFindStartEndSearchRangeX;
        public int m_nFindStartEndXThresholdGray;
        public double m_dThresholdCanny1_MakeROI;
        public double m_dThresholdCanny2_MakeROI;
        public int m_bUseAdvancedAlgorithms;
        public int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
        public int b_bUseFindROIAdvancedAlgorithms;
        public int m_nOffetY_Top;
        public int m_nOffetY_Bottom;
        public int m_nThresholdBinaryFindROI;
        public int m_nHMin;
        public int m_nHMax;
        public int m_nSMin;
        public int m_nSMax;
        public int m_nVMin;
        public int m_nVMax;
        public int m_bJustJudgeByMinBoundingRect;
        public double m_dRatioPxlUm;
        public double m_dRatioUmPxl;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame2
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Top;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Bottom;
        public double m_dDistanceMeasurementTolerance_Refer;
        public double m_dDistanceMeasurementTolerance_Min;                // 1.
        public double m_dDistanceMeasurementTolerance_Max;                // 2.
        public int m_nDelayTimeGrab;
        public int m_nFindStartEndX;
        public int m_nFindStartEndSearchRangeX;
        public int m_nFindStartEndXThresholdGray;
        public double m_dThresholdCanny1_MakeROI;
        public double m_dThresholdCanny2_MakeROI;
        public int m_bUseAdvancedAlgorithms;
        public int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
        public int b_bUseFindROIAdvancedAlgorithms;
        public int m_nOffetY_Top;
        public int m_nOffetY_Bottom;
        public int m_nThresholdBinaryFindROI;
        public int m_nHMin;
        public int m_nHMax;
        public int m_nSMin;
        public int m_nSMax;
        public int m_nVMin;
        public int m_nVMax;
        public int m_bJustJudgeByMinBoundingRect;
        public double m_dRatioPxlUm;
        public double m_dRatioUmPxl;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame3
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Top;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Bottom;
        public double m_dDistanceMeasurementTolerance_Refer;
        public double m_dDistanceMeasurementTolerance_Min;                // 1.
        public double m_dDistanceMeasurementTolerance_Max;                // 2.
        public int m_nDelayTimeGrab;
        public int m_nFindStartEndX;
        public int m_nFindStartEndSearchRangeX;
        public int m_nFindStartEndXThresholdGray;
        public double m_dThresholdCanny1_MakeROI;
        public double m_dThresholdCanny2_MakeROI;
        public int m_bUseAdvancedAlgorithms;
        public int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
        public int b_bUseFindROIAdvancedAlgorithms;
        public int m_nOffetY_Top;
        public int m_nOffetY_Bottom;
        public int m_nThresholdBinaryFindROI;
        public int m_nHMin;
        public int m_nHMax;
        public int m_nSMin;
        public int m_nSMax;
        public int m_nVMin;
        public int m_nVMax;
        public int m_bJustJudgeByMinBoundingRect;
        public double m_dRatioPxlUm;
        public double m_dRatioUmPxl;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame4
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Top;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Defines.ROI_PARAMETER_COUNT)]
        public int[] m_nROI_Bottom;
        public double m_dDistanceMeasurementTolerance_Refer;
        public double m_dDistanceMeasurementTolerance_Min;                // 1.
        public double m_dDistanceMeasurementTolerance_Max;                // 2.
        public int m_nDelayTimeGrab;
        public int m_nFindStartEndX;
        public int m_nFindStartEndSearchRangeX;
        public int m_nFindStartEndXThresholdGray;
        public double m_dThresholdCanny1_MakeROI;
        public double m_dThresholdCanny2_MakeROI;
        public int m_bUseAdvancedAlgorithms;
        public int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
        public int b_bUseFindROIAdvancedAlgorithms;
        public int m_nOffetY_Top;
        public int m_nOffetY_Bottom;
        public int m_nThresholdBinaryFindROI;
        public int m_nHMin;
        public int m_nHMax;
        public int m_nSMin;
        public int m_nSMax;
        public int m_nVMin;
        public int m_nVMax;
        public int m_bJustJudgeByMinBoundingRect;
        public double m_dRatioPxlUm;
        public double m_dRatioUmPxl;
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
