using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NpcCore.Wpf.Struct_Vision;

namespace DinoWpf
{

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
        extern private static bool CountPixelAlgorithm_Train(IntPtr tempInspProcessor, int nCamIdx, IntPtr pParamCntPxlTrain);
        public bool CountPixelAlgorithm_Train(int nCamIdx, ref CParamCntPxlAlgorithm pParamCntPxlTrain)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(pParamCntPxlTrain));
            Marshal.StructureToPtr(pParamCntPxlTrain, pPointer, false);
            bool bRetValue = CountPixelAlgorithm_Train(m_pTempInspectProcessor, nCamIdx, pPointer);
            return bRetValue;
        }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool CalculateAreaAlgorithm_Train(IntPtr tempInspProcessor, int nCamIdx, IntPtr pParamTrainCalArea);
        public bool CalculateAreaAlgorithm_Train(int nCamIdx, ref CParamCalAreaAlgorithm pParamTrainCalArea)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(pParamTrainCalArea));
            Marshal.StructureToPtr(pParamTrainCalArea, pPointer, false);
            bool bRetValue = CalculateAreaAlgorithm_Train(m_pTempInspectProcessor, nCamIdx, pPointer);
            return bRetValue;
        }
        #endregion

        #region Get Image
#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetTemplateImage(IntPtr tempInspProcessor, int nCamIdx);
        public IntPtr GetTemplateImage(int nCamIdx)
        {
            return GetTemplateImage(m_pTempInspectProcessor, nCamIdx);
        }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetResultImageBuffer(IntPtr tempInspProcessor, int nCamIdx);
        public IntPtr GetResultImageBuffer(int nCamIdx)
        {
            return GetResultImageBuffer(m_pTempInspectProcessor, nCamIdx);
        }



#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetResultROIBuffer_Train(IntPtr tempInspProcessor, int nCamIdx);
        public IntPtr GetResultROIBuffer_Train(int nCamIdx)
        {
            return GetResultROIBuffer_Train(m_pTempInspectProcessor, nCamIdx);
        }
        #endregion

        #region Get Data

#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetDataTrained_TemplateMatching(IntPtr tempInspProcessor, int nCamIdx, IntPtr dataTrained);
        public bool GetDataTrained_TemplateMatching(int nCamIdx, ref CLocatorToolResult locatorToolResult)
        {
            CLocatorToolResult dataTrained = new CLocatorToolResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(dataTrained));
            Marshal.StructureToPtr(dataTrained, pPointer, false);
            bool bRetValue = GetDataTrained_TemplateMatching(m_pTempInspectProcessor, nCamIdx, pPointer);
            locatorToolResult = (CLocatorToolResult)Marshal.PtrToStructure(pPointer, typeof(CLocatorToolResult));
            return bRetValue;
        }


#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetResultCntPxl_Train(IntPtr tempInspProcessor, int nCamIdx, IntPtr dataTrained);
        public bool GetResultCntPxl_Train(int nCamIdx, ref CAlgorithmsCountPixelResult cntPxlRes)
        {
            CAlgorithmsCountPixelResult dataTrained = new CAlgorithmsCountPixelResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(dataTrained));
            Marshal.StructureToPtr(dataTrained, pPointer, false);
            bool bRetValue = GetResultCntPxl_Train(m_pTempInspectProcessor, nCamIdx, pPointer);
            cntPxlRes = (CAlgorithmsCountPixelResult)Marshal.PtrToStructure(pPointer, typeof(CAlgorithmsCountPixelResult));
            return bRetValue;
        }

#if DEBUG
        [DllImport("NTempInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NTempInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetResultCalArea_Train(IntPtr tempInspProcessor, int nCamIdx, IntPtr dataTrained);
        public bool GetResultCalArea_Train(int nCamIdx, ref CAlgorithmsCalculateAreaResult cntCalArea)
        {
            CAlgorithmsCalculateAreaResult dataTrained = new CAlgorithmsCalculateAreaResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(dataTrained));
            Marshal.StructureToPtr(dataTrained, pPointer, false);
            bool bRetValue = GetResultCalArea_Train(m_pTempInspectProcessor, nCamIdx, pPointer);
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
