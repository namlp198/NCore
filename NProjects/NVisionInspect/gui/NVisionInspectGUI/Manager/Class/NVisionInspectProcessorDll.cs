using NVisionInspectGUI.Models;
using NVisionInspectGUI.Models.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Manager.Class
{
    // Log Message CallBack
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallbackLogFunc([MarshalAs(UnmanagedType.LPStr)] string strLogMsg);

    // Alarm CallBack
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallbackAlarmFunc([MarshalAs(UnmanagedType.LPStr)] string strAlarm);

    // Inspection Compete CallBack
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallbackInsCompleteFunc(int nCamIdx, int bSetting);

    // Inspection Compete CallBack
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallbackLocatorTrainCompleteFunc(int nCamIdx);

    public class NVisionInspectProcessorDll
    {
        IntPtr m_NVisionInspectProcessor;

        public static CallbackLogFunc m_RegLogCallBack;
        public static CallbackAlarmFunc m_RegAlarmCallBack;
        public static CallbackInsCompleteFunc m_RegInsCompleteCallBack;
        public static CallbackLocatorTrainCompleteFunc m_RegLocatorTrainCompleteCallBack;
        public NVisionInspectProcessorDll()
        {
            m_NVisionInspectProcessor = CreateNVisionInspectProcessor();
        }

        #region Init and delete
        /// <summary>
        /// Create a pointer the read code processor
        /// </summary>
        /// <returns></returns>
#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr CreateNVisionInspectProcessor();


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Initialize(IntPtr NVisionInspectProcessor);
        public bool Initialize() { return Initialize(m_NVisionInspectProcessor); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static void DeleteNVisionInspectProcessor(IntPtr NVisionInspectProcessor);
        public void DeleteNVisionInspectProcessor()
        {
            DeleteNVisionInspectProcessor(m_NVisionInspectProcessor);
        }

        #endregion

        #region Operation
#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStart(IntPtr NVisionInspectProcessor, int nThreadCount, int nCamCount);
        public bool InspectStart(int nThreadCount, int nCamCount) { return InspectStart(m_NVisionInspectProcessor, nThreadCount, nCamCount); }
        /**********************************
         - Inspection Ready / Start
         - Parameter : Inspection Cavity
        **********************************/

#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStop(IntPtr NVisionInspectProcessor);
        public bool InspectStop() { return InspectStop(m_NVisionInspectProcessor); }



#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Inspect_Simulator(IntPtr NVisionInspectProcessor, int nCamIdx);
        public bool Inspect_Simulator(int nCamIdx) { return Inspect_Simulator(m_NVisionInspectProcessor, nCamIdx); }
        /**********************************
         - Inspection Simulator
         - Parameter : Inspection Cam Index
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetResultBuffer(IntPtr NVisionInspectProcessor, int nBuff, int nFrame);
        public IntPtr GetResultBuffer(int nBuff, int nFrame)
        {
            return GetResultBuffer(m_NVisionInspectProcessor, nBuff, nFrame);
        }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetSimulatorBuffer(IntPtr NVisionInspectProcessor, int nBuff, int nFrame);
        public IntPtr GetSimulatorBuffer(int nBuff, int nFrame)
        {
            return GetSimulatorBuffer(m_NVisionInspectProcessor, nBuff, nFrame);
        }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadSimulatorBuffer(IntPtr NVisionInspectProcessor, int nBuff, int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath);
        public bool LoadSimulatorBuffer(int nBuff, int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath) { return LoadSimulatorBuffer(m_NVisionInspectProcessor, nBuff, nFrame, filePath); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LocatorTool_Train(IntPtr NVisionInspectProcessor, int nCamIdx);
        public bool LocatorTool_Train(int nCamIdx) { return LocatorTool_Train(m_NVisionInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LocatorToolSimulator_Train(IntPtr NVisionInspectProcessor, int nSimuBuff, int nFrame);
        public bool LocatorToolSimulator_Train(int nSimuBuff, int nFrame) { return LocatorToolSimulator_Train(m_NVisionInspectProcessor, nSimuBuff, nFrame); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SelectROI(IntPtr NVisionInspectProcessor, int nCamIdx, int nROIIdx, int nFrom);
        public bool SelectROI(int nCamIdx, int nROIIdx, int nFrom) { return SelectROI(m_NVisionInspectProcessor, nCamIdx, nROIIdx, nFrom); }

        #endregion

        #region Load Setting and Recipe
#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadSystemSettings(IntPtr NVisionInspectProcessor, IntPtr pSysSetting);
        public bool LoadSystemSettings(ref CNVisionInspectSystemSetting sysSetting)
        {
            CNVisionInspectSystemSetting settings = new CNVisionInspectSystemSetting();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(settings));
            Marshal.StructureToPtr(settings, pPointer, false);
            bool bRet = LoadSystemSettings(m_NVisionInspectProcessor, pPointer);
            sysSetting = (CNVisionInspectSystemSetting)Marshal.PtrToStructure(pPointer, typeof(CNVisionInspectSystemSetting));

            return bRet;
        }

#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadCameraSettings(IntPtr NVisionInspectProcessor, IntPtr pCamSetting, int nCamIdx);
        public bool LoadCameraSettings(ref CNVisionInspectCameraSetting camSetting, int nCamIdx)
        {
            CNVisionInspectCameraSetting settings = new CNVisionInspectCameraSetting();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(settings));
            Marshal.StructureToPtr(settings, pPointer, false);
            bool bRet = LoadCameraSettings(m_NVisionInspectProcessor, pPointer, nCamIdx);
            camSetting = (CNVisionInspectCameraSetting)Marshal.PtrToStructure(pPointer, typeof(CNVisionInspectCameraSetting));

            return bRet;
        }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadRecipe(IntPtr NVisionInspectProcessor, int nCamIdx, IntPtr pRecipe);
        public bool LoadRecipe(int nCamIdx, ref CNVisionInspectRecipe pRecipe)
        {
            CNVisionInspectRecipe recipe = new CNVisionInspectRecipe();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(recipe));
            Marshal.StructureToPtr(recipe, pPointer, false);
            bool bRet = LoadRecipe(m_NVisionInspectProcessor, nCamIdx, pPointer);
            pRecipe = (CNVisionInspectRecipe)Marshal.PtrToStructure(pPointer, typeof(CNVisionInspectRecipe));

            return bRet;
        }

        #endregion

        #region Save Setting and Recipe

#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveSystemSetting(IntPtr NVisionInspectProcessor, IntPtr pSysSetting);
        public bool SaveSystemSetting(ref CNVisionInspectSystemSetting sysSetting)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(sysSetting));
            Marshal.StructureToPtr(sysSetting, pPointer, false);
            bool bRet = SaveSystemSetting(m_NVisionInspectProcessor, pPointer);

            return bRet;
        }

#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveCameraSetting(IntPtr NVisionInspectProcessor, int nCamIdx, IntPtr pSysSetting);
        public bool SaveCameraSetting(int nCamIdx, ref CNVisionInspectCameraSetting camSetting)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(camSetting));
            Marshal.StructureToPtr(camSetting, pPointer, false);
            bool bRet = SaveCameraSetting(m_NVisionInspectProcessor, nCamIdx, pPointer);

            return bRet;
        }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveRecipe(IntPtr NVisionInspectProcessor, int nCamIdx, IntPtr pRecipe);
        public bool SaveRecipe(int nCamIdx, ref CNVisionInspectRecipe pRecipe)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(pRecipe));
            Marshal.StructureToPtr(pRecipe, pPointer, false);
            bool bRet = SaveRecipe(m_NVisionInspectProcessor, nCamIdx, pPointer);

            return bRet;
        }

        #endregion

        #region Get Results
#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetInspectionResult(IntPtr NVisionInspectProcessor, int nCoreIdx, IntPtr InspResults);
        public bool GetInspectionResult(int nCoreIdx, ref CNVisionInspectResult InspResults)
        {
            CNVisionInspectResult inspRes = new CNVisionInspectResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(inspRes));
            Marshal.StructureToPtr(inspRes, pPointer, false);
            bool bRet = GetInspectionResult(m_NVisionInspectProcessor, nCoreIdx, pPointer);
            InspResults = (CNVisionInspectResult)Marshal.PtrToStructure(pPointer, typeof(CNVisionInspectResult));

            return bRet;
        }


        #endregion

        #region Callback
#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackAlarmFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackAlarmFunc callbackPointer);
        public void RegCallBackAlarmFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackAlarmFunc callbackPointer)
        {
            m_RegAlarmCallBack = callbackPointer;

            RegCallBackAlarmFunc(m_NVisionInspectProcessor, m_RegAlarmCallBack);
        }
        /**********************************
       - Register Alarm Message CallBack
       - Parameter : CallBack Func Pointer
      **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackLogFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackLogFunc callbackPointer);
        public void RegCallBackLogFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackLogFunc callbackPointer)
        {
            m_RegLogCallBack = callbackPointer;

            RegCallBackLogFunc(m_NVisionInspectProcessor, m_RegLogCallBack);
        }
        /**********************************
         - Register System Message CallBack
         - Parameter : CallBack Func Pointer
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackInspectCompleteFunc(IntPtr NVisionInspectProcessor, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCompleteFunc callbackPointer);
        public void RegCallBackInspectCompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCompleteFunc callbackPointer)
        {
            m_RegInsCompleteCallBack = callbackPointer;

            RegCallBackInspectCompleteFunc(m_NVisionInspectProcessor, m_RegInsCompleteCallBack);
        }
        /**********************************
         - Register Inspection Complete CallBack
         - Parameter : CallBack Func Pointer
        **********************************/

#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackLocatorTrainCompleteFunc(IntPtr NVisionInspectProcessor, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackLocatorTrainCompleteFunc callbackPointer);
        public void RegCallBackLocatorTrainCompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackLocatorTrainCompleteFunc callbackPointer)
        {
            m_RegLocatorTrainCompleteCallBack = callbackPointer;

            RegCallBackLocatorTrainCompleteFunc(m_NVisionInspectProcessor, m_RegLocatorTrainCompleteCallBack);
        }
        /**********************************
         - Register Locator Trained CallBack
         - Parameter : CallBack Func Pointer
        **********************************/
        #endregion

        #region Hik Cam

#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ContinuousGrabHikCam(IntPtr NVisionInspectProcessor, int nCamIdx);
        public bool ContinuousGrabHikCam(int nCamIdx) { return ContinuousGrabHikCam(m_NVisionInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SingleGrabHikCam(IntPtr NVisionInspectProcessor, int nCamIdx);
        public bool SingleGrabHikCam(int nCamIdx) { return SingleGrabHikCam(m_NVisionInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StopGrabHikCam(IntPtr NVisionInspectProcessor, int nCamIdx);
        public bool StopGrabHikCam(int nCamIdx) { return StopGrabHikCam(m_NVisionInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetImageBufferHikCam(IntPtr NVisionInspectProcessor, int nCamIdx);
        public IntPtr GetImageBufferHikCam(int nCamIdx) { return GetImageBufferHikCam(m_NVisionInspectProcessor, nCamIdx); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetTriggerMode(IntPtr NVisionInspectProcessor, int nCamIdx, int nMode);
        public bool SetTriggerMode(int nCamIdx, int nMode) { return SetTriggerMode(m_NVisionInspectProcessor, nCamIdx, nMode); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetTriggerSource(IntPtr NVisionInspectProcessor, int nCamIdx, int nSource);
        public bool SetTriggerSource(int nCamIdx, int nSource) { return SetTriggerSource(m_NVisionInspectProcessor, nCamIdx, nSource); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetExposureTime(IntPtr NVisionInspectProcessor, int nCamIdx, double dExpTime);
        public bool SetExposureTime(int nCamIdx, double dExpTime) { return SetExposureTime(m_NVisionInspectProcessor, nCamIdx, dExpTime); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetAnalogGain(IntPtr NVisionInspectProcessor, int nCamIdx, double dGain);
        public bool SetAnalogGain(int nCamIdx, double dGain) { return SetAnalogGain(m_NVisionInspectProcessor, nCamIdx, dGain); }


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveImage(IntPtr NVisionInspectProcessor, int nCamIdx);
        public bool SaveImage(int nCamIdx) { return SaveImage(m_NVisionInspectProcessor, nCamIdx); }

        #endregion
    }
}
