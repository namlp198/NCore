using SealingInspectGUI.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Manager.Class
{
    public class SealingInspectProcessorDll
    {
        IntPtr m_sealingInspectProcessor;

        public static CallbackLogFunc m_RegLogCallBack;
        public static CallbackAlarmFunc m_RegAlarmCallBack;
        public static CallbackInsCompleteFunc m_RegInsCompleteCallBack;
        public SealingInspectProcessorDll()
        {
            m_sealingInspectProcessor = CreateSealingInspectProcessor();
        }

        // Log Message CallBack
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackLogFunc([MarshalAs(UnmanagedType.LPStr)] string strLogMsg);

        // Log Message CallBack
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackAlarmFunc(emInspectCavity nInspCavity, [MarshalAs(UnmanagedType.LPStr)] string strLogMsg);

        // Inspection Compete CallBack
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackInsCompleteFunc(emInspectCavity nInspCavity);

        #region Init and delete
        /// <summary>
        /// Create a pointer the sealing inspect processor
        /// </summary>
        /// <returns></returns>
#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr CreateSealingInspectProcessor();


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Initialize(IntPtr sealingInspProcessor);
        public bool Initialize() { return Initialize(m_sealingInspectProcessor); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static void DeleteSealingInspectProcessor(IntPtr sealingInspProcessor);
        public void DeleteSealingInspectProcessor()
        {
            DeleteSealingInspectProcessor(m_sealingInspectProcessor);
        }
        #endregion

        #region Simulation
#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetBufferImage_SIDE(IntPtr sealingInspProcessor, int nBuff, int nFrame);
        public IntPtr GetBufferImage_SIDE(int nBuff, int nFrame) { return GetBufferImage_SIDE(m_sealingInspectProcessor, nBuff, nFrame); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetBufferImage_TOP(IntPtr sealingInspProcessor, int nBuff, int nFrame);
        public IntPtr GetBufferImage_TOP(int nBuff, int nFrame) { return GetBufferImage_TOP(m_sealingInspectProcessor, nBuff, nFrame); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadImageBuffer_SIDE(IntPtr sealingInspProcessor, int nBuff, int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath);
        public bool LoadImageBuffer_SIDE(int nBuff, int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath) { return LoadImageBuffer_SIDE(m_sealingInspectProcessor, nBuff, nFrame, filePath); }

#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadImageBuffer_TOP(IntPtr sealingInspProcessor, int nBuff, int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath);
        public bool LoadImageBuffer_TOP(int nBuff, int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath) { return LoadImageBuffer_TOP(m_sealingInspectProcessor, nBuff, nFrame, filePath); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadAllImageBuffer(IntPtr sealingInspProcessor, [MarshalAs(UnmanagedType.LPStr)] string dirPath, [MarshalAs(UnmanagedType.LPStr)] string imageType);
        public bool LoadAllImageBuffer([MarshalAs(UnmanagedType.LPStr)] string dirPath, [MarshalAs(UnmanagedType.LPStr)] string imageType) { return LoadAllImageBuffer(m_sealingInspectProcessor, dirPath, imageType); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ClearBufferImage_SIDE(IntPtr sealingInspProcessor, int nBuff);
        public bool ClearBufferImage_SIDE(int nBuff) { return ClearBufferImage_SIDE(m_sealingInspectProcessor, nBuff); }

#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ClearBufferImage_TOP(IntPtr sealingInspProcessor, int nBuff);
        public bool ClearBufferImage_TOP(int nBuff) { return ClearBufferImage_TOP(m_sealingInspectProcessor, nBuff); }


        #endregion

        #region Callback
#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackAlarmFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackAlarmFunc callbackPointer);
        public void RegCallBackAlarmFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackAlarmFunc callbackPointer)
        {
            m_RegAlarmCallBack = callbackPointer;

            RegCallBackAlarmFunc(m_sealingInspectProcessor, m_RegAlarmCallBack);
        }
        /**********************************
       - Register Alarm Message CallBack
       - Parameter : CallBack Func Pointer
      **********************************/


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackLogFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackLogFunc callbackPointer);
        public void RegCallBackLogFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackLogFunc callbackPointer)
        {
            m_RegLogCallBack = callbackPointer;

            RegCallBackLogFunc(m_sealingInspectProcessor, m_RegLogCallBack);
        }
        /**********************************
         - Register System Message CallBack
         - Parameter : CallBack Func Pointer
        **********************************/


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackInspectCompleteFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCompleteFunc callbackPointer);
        public void RegCallBackInspectCompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCompleteFunc callbackPointer)
        {
            m_RegInsCompleteCallBack = callbackPointer;

            RegCallBackInspectCompleteFunc(m_sealingInspectProcessor, m_RegInsCompleteCallBack);
        }
        /**********************************
         - Register Inspection Complete CallBack
         - Parameter : CallBack Func Pointer
        **********************************/
        #endregion

        #region Hik Cam
#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ContinuousGrabHikCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool ContinuousGrabHikCam(int nCamIdx) { return ContinuousGrabHikCam(m_sealingInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SoftwareTriggerHikCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool SoftwareTriggerHikCam(int nCamIdx) { return SoftwareTriggerHikCam(m_sealingInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StopGrabHikCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool StopGrabHikCam(int nCamIdx) { return StopGrabHikCam(m_sealingInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetTriggerModeHikCam(IntPtr tempInspProcessor, int nCamIdx, int nMode);
        public bool SetTriggerModeHikCam(int nCamIdx, int nMode) { return SetTriggerModeHikCam(m_sealingInspectProcessor, nCamIdx, nMode); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetTriggerSourceHikCam(IntPtr tempInspProcessor, int nCamIdx, int nSource);
        public bool SetTriggerSourceHikCam(int nCamIdx, int nSource) { return SetTriggerSourceHikCam(m_sealingInspectProcessor, nCamIdx, nSource); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetBufferImageHikCam(IntPtr tempInspProcessor, int nCamIdx);
        public IntPtr GetBufferImageHikCam(int nCamIdx) { return GetBufferImageHikCam(m_sealingInspectProcessor, nCamIdx); }
        #endregion
    }
}
