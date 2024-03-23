using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DinoWpf
{
    #region Tool Results
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CLocatorToolResult
    {
        public int m_nX;
        public int m_nY;
        public int m_nDelta_x;
        public int m_nDelta_y;
        public double m_dDif_Angle;
        public bool m_bResult;
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
        public int m_nRectIn_X;
        public int m_nRectIn_Y;
        public int m_nRectIn_Width;
        public int m_nRectIn_Height;
        public int m_nRectOut_X;
        public int m_nRectOut_Y;
        public int m_nRectOut_Width;
        public int m_nRectOut_Height;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CParamCntPxlAlgorithm
    {
        public int m_nROIX;
        public int m_nROIY;
        public int m_nROIWidth;
        public int m_nROIHeight;
        public double m_dROIAngleRotate;
        public int m_nThresholdGrayMin;
        public int m_nThresholdGrayMax;
        public int m_nNumberOfPxlMin;
        public int m_nNumberOfPxlMax;
    }
    public struct CParamCalAreaAlgorithm
    {
        public int m_nROIX;
        public int m_nROIY;
        public int m_nROIWidth;
        public int m_nROIHeight;
        public double m_dROIAngleRotate;
        public int m_nThreshold;
        public int m_nAreaMin;
        public int m_nAreaMax;
    }
    #endregion

    public class TempInspectProcessorDll
    {
        IntPtr m_pTempInspectProcessor;
        public TempInspectProcessorDll()
        {
            m_pTempInspectProcessor = CreateTempInspectProcessor();
        }


        /// <summary>
        /// Create a pointer the image processor
        /// </summary>
        /// <returns></returns>
#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr CreateTempInspectProcessor();

#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Initialize(IntPtr tempInspProcessor);
        public bool Initialize() { return Initialize(m_pTempInspectProcessor); }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStart(IntPtr tempInspProcessor, int nThreadCount, int nCamIdx);
        public bool InspectStart(int nThreadCount, int nCamIdx) { return InspectStart(m_pTempInspectProcessor, nThreadCount, nCamIdx); }

#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStop(IntPtr tempInspProcessor, int nCamIdx);
        public bool InspectStop(int nCamIdx) { return InspectStop(m_pTempInspectProcessor, nCamIdx); }

#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ContinuousGrabHikCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool ContinuousGrabHikCam(int nCamIdx) { return ContinuousGrabHikCam(m_pTempInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SingleGrabHikCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool SingleGrabHikCam(int nCamIdx) { return SingleGrabHikCam(m_pTempInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StopGrabHikCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool StopGrabHikCam(int nCamIdx) { return StopGrabHikCam(m_pTempInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetTriggerModeHikCam(IntPtr tempInspProcessor, int nCamIdx, int nMode);
        public bool SetTriggerModeHikCam(int nCamIdx, int nMode) { return SetTriggerModeHikCam(m_pTempInspectProcessor, nCamIdx, nMode); }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetTriggerSourceHikCam(IntPtr tempInspProcessor, int nCamIdx, int nSource);
        public bool SetTriggerSourceHikCam(int nCamIdx, int nSource) { return SetTriggerSourceHikCam(m_pTempInspectProcessor, nCamIdx, nSource); }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetBufferImageHikCam(IntPtr tempInspProcessor, int nCamIdx);
        public IntPtr GetBufferImageHikCam(int nCamIdx) { return GetBufferImageHikCam(m_pTempInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static void DeleteTempInspectProcessor(IntPtr tempInspProcessor);
        public void DeleteTempInspectProcessor()
        {
            DeleteTempInspectProcessor(m_pTempInspectProcessor);
        }
    }
}
