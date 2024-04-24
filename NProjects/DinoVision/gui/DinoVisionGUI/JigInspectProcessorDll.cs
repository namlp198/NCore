using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DinoVisionGUI
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CJigInspectConfigurations
    {
        public string m_sCOMPort;
        public string m_sSaveImagePath;
        public bool m_bIsShowDetail;
        public bool m_bPCControlMode;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CJigInspectResults
    {
        public int m_bInspectCompleted;
        public int m_bResultOKNG;
    }

    // Inspection Compete CallBack
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallbackInsCompleteFunc();

    public class JigInspectProcessorDll
    {
        IntPtr m_JigInspectProcessor;
        public static CallbackInsCompleteFunc m_RegInsCompleteCallBack;
        public JigInspectProcessorDll()
        {
            m_JigInspectProcessor = CreateJigInspectProcessor();
        }


        /// <summary>
        /// Create a pointer the image processor
        /// </summary>
        /// <returns></returns>
#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr CreateJigInspectProcessor();

#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Initialize(IntPtr tempInspProcessor);
        public bool Initialize() { return Initialize(m_JigInspectProcessor); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStart(IntPtr tempInspProcessor, int nThreadCount, int nCamIdx);
        public bool InspectStart(int nThreadCount, int nCamIdx) { return InspectStart(m_JigInspectProcessor, nThreadCount, nCamIdx); }

#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStop(IntPtr tempInspProcessor, int nCamIdx);
        public bool InspectStop(int nCamIdx) { return InspectStop(m_JigInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SingleGrabDinoCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool SingleGrabDinoCam(int nCamIdx) { return SingleGrabDinoCam(m_JigInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StartGrabDinoCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool StartGrabDinoCam(int nCamIdx) { return StartGrabDinoCam(m_JigInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StopGrabDinoCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool StopGrabDinoCam(int nCamIdx) { return StopGrabDinoCam(m_JigInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ConnectDinoCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool ConnectDinoCam(int nCamIdx) { return ConnectDinoCam(m_JigInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool DisconnectDinoCam(IntPtr tempInspProcessor, int nCamIdx);
        public bool DisconnectDinoCam(int nCamIdx) { return DisconnectDinoCam(m_JigInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetBufferDinoCam(IntPtr tempInspProcessor, int nCamIdx);
        public IntPtr GetBufferDinoCam(int nCamIdx) { return GetBufferDinoCam(m_JigInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetResultBufferImageDinoCam(IntPtr tempInspProcessor, int nCamIdx);
        public IntPtr GetResultBufferImageDinoCam(int nCamIdx) { return GetResultBufferImageDinoCam(m_JigInspectProcessor, nCamIdx); }



#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetInspectionResult(IntPtr tempInspProcessor, int nCamIdx, IntPtr InspResults);
        public bool GetInspectionResult(int nCamIdx, ref CJigInspectResults InspResults)
        {
            CJigInspectResults inspRes = new CJigInspectResults();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(inspRes));
            Marshal.StructureToPtr(inspRes, pPointer, false);
            bool bRet = GetInspectionResult(m_JigInspectProcessor, nCamIdx, pPointer);
            InspResults = (CJigInspectResults)Marshal.PtrToStructure(pPointer, typeof(CJigInspectResults));

            return bRet;
        }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackInspectCompleteFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCompleteFunc callbackPointer);
        public void RegCallBackInspectCompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCompleteFunc callbackPointer)
        {
            m_RegInsCompleteCallBack = callbackPointer;

            RegCallBackInspectCompleteFunc(m_JigInspectProcessor, m_RegInsCompleteCallBack);
        }
        /**********************************
         - Register Inspection Complete CallBack
         - Parameter : CallBack Func Pointer
        **********************************/

#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static void DeleteJigInspectProcessor(IntPtr tempInspProcessor);
        public void DeleteJigInspectProcessor()
        {
            DeleteJigInspectProcessor(m_JigInspectProcessor);
        }
    }
}
