using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DinoWpf
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


        #region Train
#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool TrainLocator_TemplateMatching(IntPtr tempInspProcessor, int nCamIdx, IntPtr dataTrain);
        public bool TrainLocator_TemplateMatching(int nCamIdx, ref RectForTrainLocTool rectForTrainLocTool)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(rectForTrainLocTool));
            Marshal.StructureToPtr(rectForTrainLocTool, pPointer, false);
            bool bRetValue = TrainLocator_TemplateMatching(m_pTempInspectProcessor, nCamIdx, pPointer);
            return bRetValue;
        }

#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool CountPixelAlgorithm_Train(IntPtr tempInspProcessor, IntPtr pParamCntPxlTrain);
        public bool CountPixelAlgorithm_Train(ref CParamCntPxlAlgorithm pParamCntPxlTrain)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(pParamCntPxlTrain));
            Marshal.StructureToPtr(pParamCntPxlTrain, pPointer, false);
            bool bRetValue = CountPixelAlgorithm_Train(m_pTempInspectProcessor, pPointer);
            return bRetValue;
        }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool CalculateAreaAlgorithm_Train(IntPtr tempInspProcessor, IntPtr pParamTrainCalArea);
        public bool CalculateAreaAlgorithm_Train(ref CParamCalAreaAlgorithm pParamTrainCalArea)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(pParamTrainCalArea));
            Marshal.StructureToPtr(pParamTrainCalArea, pPointer, false);
            bool bRetValue = CalculateAreaAlgorithm_Train(m_pTempInspectProcessor, pPointer);
            return bRetValue;
        }
        #endregion

        #region Get Image
#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetTemplateImage(IntPtr tempInspProcessor);
        public IntPtr GetTemplateImage()
        {
            return GetTemplateImage(m_pTempInspectProcessor);
        }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetResultROIBuffer_Train(IntPtr tempInspProcessor);
        public IntPtr GetResultROIBuffer_Train()
        {
            return GetResultROIBuffer_Train(m_pTempInspectProcessor);
        }
        #endregion

        #region Get Data

#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetDataTrained_TemplateMatching(IntPtr tempInspProcessor, IntPtr dataTrained);
        public bool GetDataTrained_TemplateMatching(ref CLocatorToolResult locatorToolResult)
        {
            CLocatorToolResult dataTrained = new CLocatorToolResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(dataTrained));
            Marshal.StructureToPtr(dataTrained, pPointer, false);
            bool bRetValue = GetDataTrained_TemplateMatching(m_pTempInspectProcessor, pPointer);
            locatorToolResult = (CLocatorToolResult)Marshal.PtrToStructure(pPointer, typeof(CLocatorToolResult));
            return bRetValue;
        }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetResultCntPxl_Train(IntPtr tempInspProcessor, IntPtr dataTrained);
        public bool GetResultCntPxl_Train(ref CAlgorithmsCountPixelResult cntPxlRes)
        {
            CAlgorithmsCountPixelResult dataTrained = new CAlgorithmsCountPixelResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(dataTrained));
            Marshal.StructureToPtr(dataTrained, pPointer, false);
            bool bRetValue = GetResultCntPxl_Train(m_pTempInspectProcessor, pPointer);
            cntPxlRes = (CAlgorithmsCountPixelResult)Marshal.PtrToStructure(pPointer, typeof(CAlgorithmsCountPixelResult));
            return bRetValue;
        }

#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetResultCalArea_Train(IntPtr tempInspProcessor, IntPtr dataTrained);
        public bool GetResultCalArea_Train(ref CAlgorithmsCalculateAreaResult cntCalArea)
        {
            CAlgorithmsCalculateAreaResult dataTrained = new CAlgorithmsCalculateAreaResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(dataTrained));
            Marshal.StructureToPtr(dataTrained, pPointer, false);
            bool bRetValue = GetResultCalArea_Train(m_pTempInspectProcessor, pPointer);
            cntCalArea = (CAlgorithmsCalculateAreaResult)Marshal.PtrToStructure(pPointer, typeof(CAlgorithmsCalculateAreaResult));
            return bRetValue;
        }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetSumResult(IntPtr tempInspProcessor, int nCamIdx, IntPtr sumResult);
        public bool GetSumResult(int nCamIdx, ref CSumResult sumRes)
        {
            CSumResult sumResult = new CSumResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(sumResult));
            Marshal.StructureToPtr(sumResult, pPointer, false);
            bool bRetValue = GetSumResult(m_pTempInspectProcessor, nCamIdx, pPointer);
            sumRes = (CSumResult)Marshal.PtrToStructure(pPointer, typeof(CSumResult));
            return bRetValue;
        }
        #endregion

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
