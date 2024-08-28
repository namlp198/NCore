using ReadCodeGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReadCodeGUI.Manager.Class
{
    public class ReadCodeProcessorDll
    {
        IntPtr m_readCodeProcessor;

        public static CallbackInsCompleteFunc m_RegInsCompleteCallBack;
        public static CallbackLocatorTrainedFunc m_RegLocatorTrainedCallBack;
        public ReadCodeProcessorDll()
        {
            m_readCodeProcessor = CreateReadCodeProcessor();
        }

        // Inspection Compete CallBack
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackInsCompleteFunc(int bSetting);

        // Inspection Compete CallBack
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackLocatorTrainedFunc(int bSetting);

        #region Init and delete
        /// <summary>
        /// Create a pointer the read code processor
        /// </summary>
        /// <returns></returns>
#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr CreateReadCodeProcessor();


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Initialize(IntPtr readCodeProcessor);
        public bool Initialize() { return Initialize(m_readCodeProcessor); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static void DeleteReadCodeProcessor(IntPtr readCodeProcessor);
        public void DeleteReadCodeProcessor()
        {
            DeleteReadCodeProcessor(m_readCodeProcessor);
        }

        #endregion

        #region Operation
#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStart(IntPtr readCodeProcessor, int nThreadCount, int isSimulator);
        public bool InspectStart(int nThreadCount, int isSimulator) { return InspectStart(m_readCodeProcessor, nThreadCount, isSimulator); }
        /**********************************
         - Inspection Ready / Start
         - Parameter : Inspection Cavity
        **********************************/

#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStop(IntPtr readCodeProcessor);
        public bool InspectStop() { return InspectStop(m_readCodeProcessor); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetResultBuffer(IntPtr readCodeProcessor, int nBuff, int nFrame);
        public IntPtr GetResultBuffer(int nBuff, int nFrame)
        {
            return GetResultBuffer(m_readCodeProcessor, nBuff, nFrame);
        }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetSimulatorBuffer(IntPtr readCodeProcessor, int nBuff, int nFrame);
        public IntPtr GetSimulatorBuffer(int nBuff, int nFrame)
        {
            return GetSimulatorBuffer(m_readCodeProcessor, nBuff, nFrame);
        }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadSimulatorBuffer(IntPtr readCodeProcessor, int nBuff, int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath);
        public bool LoadSimulatorBuffer(int nBuff, int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath) { return LoadSimulatorBuffer(m_readCodeProcessor, nBuff, nFrame, filePath); }

        #endregion

        #region Load Setting and Recipe
#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadSystemSettings(IntPtr readCodeProcessor, IntPtr pSysSetting);
        public bool LoadSystemSettings(ref CReadCodeSystemSetting sysSetting)
        {
            CReadCodeSystemSetting settings = new CReadCodeSystemSetting();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(settings));
            Marshal.StructureToPtr(settings, pPointer, false);
            bool bRet = LoadSystemSettings(m_readCodeProcessor, pPointer);
            sysSetting = (CReadCodeSystemSetting)Marshal.PtrToStructure(pPointer, typeof(CReadCodeSystemSetting));

            return bRet;
        }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadRecipe(IntPtr readCodeProcessor, IntPtr pRecipe);
        public bool LoadRecipe(ref CReadCodeRecipe pRecipe)
        {
            CReadCodeRecipe recipe = new CReadCodeRecipe();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(recipe));
            Marshal.StructureToPtr(recipe, pPointer, false);
            bool bRet = LoadRecipe(m_readCodeProcessor, pPointer);
            pRecipe = (CReadCodeRecipe)Marshal.PtrToStructure(pPointer, typeof(CReadCodeRecipe));

            return bRet;
        }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ReloadSystenSettings(IntPtr sealingInspProcessor);
        public bool ReloadSystenSettings()
        {
            bool bRet = ReloadSystenSettings(m_readCodeProcessor);
            return bRet;
        }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ReloadRecipe(IntPtr sealingInspProcessor);
        public bool ReloadRecipe()
        {
            bool bRet = ReloadRecipe(m_readCodeProcessor);
            return bRet;
        }


        #endregion

        #region Save Setting and Recipe

#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveSystemSetting(IntPtr readCodeProcessor, IntPtr pSysSetting);
        public bool SaveSystemSetting(ref CReadCodeSystemSetting sysSetting)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(sysSetting));
            Marshal.StructureToPtr(sysSetting, pPointer, false);
            bool bRet = SaveSystemSetting(m_readCodeProcessor, pPointer);

            return bRet;
        }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveRecipe(IntPtr readCodeProcessor, IntPtr pRecipe);
        public bool SaveRecipe(ref CReadCodeRecipe pRecipe)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(pRecipe));
            Marshal.StructureToPtr(pRecipe, pPointer, false);
            bool bRet = SaveRecipe(m_readCodeProcessor, pPointer);

            return bRet;
        }

        #endregion

        #region Get Results
#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetInspectionResult(IntPtr sealingInspProcessor, int nCoreIdx, IntPtr InspResults);
        public bool GetInspectionResult(int nCoreIdx, ref CReadCodeResult InspResults)
        {
            CReadCodeResult inspRes = new CReadCodeResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(inspRes));
            Marshal.StructureToPtr(inspRes, pPointer, false);
            bool bRet = GetInspectionResult(m_readCodeProcessor, nCoreIdx, pPointer);
            InspResults = (CReadCodeResult)Marshal.PtrToStructure(pPointer, typeof(CReadCodeResult));

            return bRet;
        }


        #endregion

        #region Callback
#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackInspectCompleteFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCompleteFunc callbackPointer);
        public void RegCallBackInspectCompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCompleteFunc callbackPointer)
        {
            m_RegInsCompleteCallBack = callbackPointer;

            RegCallBackInspectCompleteFunc(m_readCodeProcessor, m_RegInsCompleteCallBack);
        }
        /**********************************
         - Register Inspection Complete CallBack
         - Parameter : CallBack Func Pointer
        **********************************/

#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallbackLocatorTrainedFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackLocatorTrainedFunc callbackPointer);
        public void RegCallbackLocatorTrainedFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackLocatorTrainedFunc callbackPointer)
        {
            m_RegLocatorTrainedCallBack = callbackPointer;

            RegCallbackLocatorTrainedFunc(m_readCodeProcessor, m_RegLocatorTrainedCallBack);
        }
        /**********************************
         - Register Locator Trained CallBack
         - Parameter : CallBack Func Pointer
        **********************************/
        #endregion

        #region Basler Cam

#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ContinuousGrabBaslerCam(IntPtr readCodeProcessor, int nCamIdx);
        public bool ContinuousGrabBaslerCam(int nCamIdx) { return ContinuousGrabBaslerCam(m_readCodeProcessor, nCamIdx); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SingleGrabBaslerCam(IntPtr readCodeProcessor, int nCamIdx);
        public bool SingleGrabBaslerCam(int nCamIdx) { return SingleGrabBaslerCam(m_readCodeProcessor, nCamIdx); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StopGrabBaslerCam(IntPtr readCodeProcessor, int nCamIdx);
        public bool StopGrabBaslerCam(int nCamIdx) { return StopGrabBaslerCam(m_readCodeProcessor, nCamIdx); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetImageBufferBaslerCam(IntPtr readCodeProcessor, int nCamIdx);
        public IntPtr GetImageBufferBaslerCam(int nCamIdx) { return GetImageBufferBaslerCam(m_readCodeProcessor, nCamIdx); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetTriggerMode(IntPtr readCodeProcessor, int nCamIdx, int nMode);
        public bool SetTriggerMode(int nCamIdx, int nMode) { return SetTriggerMode(m_readCodeProcessor, nCamIdx, nMode); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetTriggerSource(IntPtr readCodeProcessor, int nCamIdx, int nSource);
        public bool SetTriggerSource(int nCamIdx, int nSource) { return SetTriggerSource(m_readCodeProcessor, nCamIdx, nSource); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetExposureTime(IntPtr readCodeProcessor, int nCamIdx, double dExpTime);
        public bool SetExposureTime(int nCamIdx, double dExpTime) { return SetExposureTime(m_readCodeProcessor, nCamIdx, dExpTime); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetAnalogGain(IntPtr readCodeProcessor, int nCamIdx, double dGain);
        public bool SetAnalogGain(int nCamIdx, double dGain) { return SetAnalogGain(m_readCodeProcessor, nCamIdx, dGain); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveImage(IntPtr readCodeProcessor, int nCamIdx);
        public bool SaveImage(int nCamIdx) { return SaveImage(m_readCodeProcessor, nCamIdx); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LocatorTool_Train(IntPtr readCodeProcessor, int nCamIdx);
        public bool LocatorTool_Train(int nCamIdx) { return LocatorTool_Train(m_readCodeProcessor, nCamIdx); }


#if DEBUG
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("ReadCodeProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LocatorToolSimulator_Train(IntPtr readCodeProcessor, int nSimuBuff, int nFrame);
        public bool LocatorToolSimulator_Train(int nSimuBuff, int nFrame) { return LocatorToolSimulator_Train(m_readCodeProcessor, nSimuBuff, nFrame); }

        #endregion
    }
}
