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
        public int m_nDistanceMeasurementTolerance_Min;                // 0.
        public int m_nDistanceMeasurementTolerance_Max;                // 1.
        public int m_nRadius_Min;                                      // 2.
        public int m_nRadius_Max;                                      // 3.
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_TopCam_Frame2
    {
        public int m_nDistanceMeasurementTolerance_Min;                // 0.            
        public int m_nDistanceMeasurementTolerance_Max;                // 1.
        public int m_nRadius_Min;                                      // 2.
        public int m_nRadius_Max;                                      // 3.
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame1
    {
        public int m_nDistanceMeasurementTolerance_Min;                // 0.
        public int m_nDistanceMeasurementTolerance_Max;                // 1.
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame2
    {
        public int m_nDistanceMeasurementTolerance_Min;                // 0.
        public int m_nDistanceMeasurementTolerance_Max;                // 1.
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame3
    {
        public int m_nDistanceMeasurementTolerance_Min;                // 0.
        public int m_nDistanceMeasurementTolerance_Max;                // 1.
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CRecipe_SideCam_Frame4
    {
        public int m_nDistanceMeasurementTolerance_Min;                // 0.
        public int m_nDistanceMeasurementTolerance_Max;                // 1.
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
