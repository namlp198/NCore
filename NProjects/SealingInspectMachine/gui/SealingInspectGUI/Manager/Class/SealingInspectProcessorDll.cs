using SealingInspectGUI.Commons;
using SealingInspectGUI.Models;
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
        public static CallbackInsCavity1CompleteFunc m_RegInsCavity1CompleteCallBack;
        public static CallbackInsCavity2CompleteFunc m_RegInsCavity2CompleteCallBack;
        public static CallbackInsTopCam1CompleteFunc m_RegInsTopCam1CompleteCallBack;
        public static CallbackInsTopCam2CompleteFunc m_RegInsTopCam2CompleteCallBack;

        public static CallbackGrabFrameSideCam1CompleteFunc m_RegGrabFrameSideCam1CompleteCallBack;
        public static CallbackGrabFrameSideCam2CompleteFunc m_RegGrabFrameSideCam2CompleteCallBack;
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

        // Inspection Cavity 1 Compete CallBack
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackInsCavity1CompleteFunc(int bSetting);

        // Inspection TopCam 1 Compete CallBack
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackInsTopCam1CompleteFunc(int bSetting);

        // Inspection Cavity 2 Compete CallBack
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackInsCavity2CompleteFunc(int bSetting);

        // Inspection TopCam 2 Compete CallBack
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackInsTopCam2CompleteFunc(int bSetting);

        // Frab Frame Side Cam 1 Compete CallBack
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackGrabFrameSideCam1CompleteFunc(int bSetting);

        // Frab Frame Side Cam 2 Compete CallBack
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallbackGrabFrameSideCam2CompleteFunc(int bSetting);

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

        #region Load Setting and Recipe
#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadSystemSettings(IntPtr sealingInspProcessor, IntPtr pSysSetting);
        public bool LoadSystemSettings(ref CSealingInspectSystemSetting sysSetting)
        {
            CSealingInspectSystemSetting settings = new CSealingInspectSystemSetting();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(settings));
            Marshal.StructureToPtr(settings, pPointer, false);
            bool bRet = LoadSystemSettings(m_sealingInspectProcessor, pPointer);
            sysSetting = (CSealingInspectSystemSetting)Marshal.PtrToStructure(pPointer, typeof(CSealingInspectSystemSetting));

            return bRet;
        }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadRecipe(IntPtr sealingInspProcessor, IntPtr pRecipe);
        public bool LoadRecipe(ref CSealingInspectRecipe pRecipe)
        {
            CSealingInspectRecipe recipe = new CSealingInspectRecipe();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(recipe));
            Marshal.StructureToPtr(recipe, pPointer, false);
            bool bRet = LoadRecipe(m_sealingInspectProcessor, pPointer);
            pRecipe = (CSealingInspectRecipe)Marshal.PtrToStructure(pPointer, typeof(CSealingInspectRecipe));

            return bRet;
        }
        #endregion

        #region Save Setting and Recipe
#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveSystemSetting(IntPtr sealingInspProcessor, IntPtr pSysSetting);
        public bool SaveSystemSetting(ref CSealingInspectSystemSetting sysSetting)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(sysSetting));
            Marshal.StructureToPtr(sysSetting, pPointer, false);
            bool bRet = SaveSystemSetting(m_sealingInspectProcessor, pPointer);

            return bRet;
        }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveLightSetting(IntPtr sealingInspProcessor, IntPtr pSysSetting, int nLightIdx);
        public bool SaveLightSetting(ref CSealingInspectSystemSetting sysSetting, int nLightIdx)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(sysSetting));
            Marshal.StructureToPtr(sysSetting, pPointer, false);
            bool bRet = SaveLightSetting(m_sealingInspectProcessor, pPointer, nLightIdx);

            return bRet;
        }



#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveRecipe(IntPtr sealingInspProcessor, IntPtr pRecipe, [MarshalAs(UnmanagedType.LPStr)] string sPosCam, int nFrameIdx);
        public bool SaveRecipe(ref CSealingInspectRecipe pRecipe, [MarshalAs(UnmanagedType.LPStr)] string sPosCam, int nFrameIdx)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(pRecipe));
            Marshal.StructureToPtr(pRecipe, pPointer, false);
            bool bRet = SaveRecipe(m_sealingInspectProcessor, pPointer, sPosCam, nFrameIdx);

            return bRet;
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
        extern private static IntPtr GetResultBuffer_SIDE(IntPtr sealingInspProcessor, int nBuff, int nFrame);
        public IntPtr GetResultBuffer_SIDE(int nBuff, int nFrame) { return GetResultBuffer_SIDE(m_sealingInspectProcessor, nBuff, nFrame); }


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
        extern private static IntPtr GetResultBuffer_TOP(IntPtr sealingInspProcessor, int nBuff, int nFrame);
        public IntPtr GetResultBuffer_TOP(int nBuff, int nFrame) { return GetResultBuffer_TOP(m_sealingInspectProcessor, nBuff, nFrame); }


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

        #region Get Result
#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStart(IntPtr pInstance, int nThreadCount, emInspectCavity eInspCav, int isSimulator);
        public bool InspectStart(int nThreadCount, emInspectCavity eInspCav, int isSimulator) { return InspectStart(m_sealingInspectProcessor, nThreadCount, eInspCav, isSimulator); }
        /**********************************
         - Inspection Ready / Start
         - Parameter : Inspection Cavity
        **********************************/


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStop(IntPtr pInstance, emInspectCavity eInspCav);
        public bool InspectStop(emInspectCavity eInspCav) { return InspectStop(m_sealingInspectProcessor, eInspCav); }
        /**********************************
         - Inspection Ready / Start
         - Parameter : Inspection Cavity
        **********************************/


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetProcessStatus(IntPtr pInstance, int nCoreIdx, int nProcessStatus);
        public bool SetProcessStatus(int nCoreIdx, int nProcessStatus) { return SetProcessStatus(m_sealingInspectProcessor, nCoreIdx, nProcessStatus); }
        /**********************************
         - Start Inspection
         - Parameter : Index Core, Process Status
        **********************************/


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetGrabFrameSideCam(IntPtr pInstance, int nCoreIdx, int nTriggerSideCam);
        public bool SetGrabFrameSideCam(int nCoreIdx, int nTriggerSideCam) { return SetGrabFrameSideCam(m_sealingInspectProcessor, nCoreIdx, nTriggerSideCam); }
        /**********************************
         - SetTriggerSideCam
         - Parameter : Index Core, Process Status
        **********************************/



#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetInspectionResult(IntPtr sealingInspProcessor, int nCoreIdx, IntPtr InspResults);
        public bool GetInspectionResult(int nCoreIdx, ref CSealingInspectResult InspResults)
        {
            CSealingInspectResult inspRes = new CSealingInspectResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(inspRes));
            Marshal.StructureToPtr(inspRes, pPointer, false);
            bool bRet = GetInspectionResult(m_sealingInspectProcessor, nCoreIdx, pPointer);
            InspResults = (CSealingInspectResult)Marshal.PtrToStructure(pPointer, typeof(CSealingInspectResult));

            return bRet;
        }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetSealingInspectSimulationIO(IntPtr sealingInspProcessor, int nCoreIdx, IntPtr simulationIO);
        public bool SetSealingInspectSimulationIO(int nCoreIdx, ref CSealingInspect_Simulation_IO sealingInspSimulationIO)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(sealingInspSimulationIO));
            Marshal.StructureToPtr(sealingInspSimulationIO, pPointer, false);
            bool bRet = SetSealingInspectSimulationIO(m_sealingInspectProcessor, nCoreIdx, pPointer);

            return bRet;
        }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetCavityInfo(IntPtr sealingInspProcessor, [MarshalAs(UnmanagedType.LPStr)] string strLoadingTime);
        public bool SetCavityInfo([MarshalAs(UnmanagedType.LPStr)] string strLoadingTime)
        {
            return SetCavityInfo(strLoadingTime);
        }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool TestInspectCavity1(IntPtr sealingInspProcessor);
        public bool TestInspectCavity1() { return TestInspectCavity1(m_sealingInspectProcessor); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool TestInspectCavity2(IntPtr sealingInspProcessor);
        public bool TestInspectCavity2() { return TestInspectCavity2(m_sealingInspectProcessor); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Inspect_TopCam_Simulation(IntPtr sealingInspProcessor, int nCoreIdx, int nCamIdx, int nFrame);
        public bool Inspect_TopCam_Simulation(int nCoreIdx, int nCamIdx, int nFrame) { return Inspect_TopCam_Simulation(m_sealingInspectProcessor, nCoreIdx, nCamIdx, nFrame); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Inspect_SideCam_Simulation(IntPtr sealingInspProcessor, int nCoreIdx, int nCamIdx, int nFrame);
        public bool Inspect_SideCam_Simulation(int nCoreIdx, int nCamIdx, int nFrame) { return Inspect_SideCam_Simulation(m_sealingInspectProcessor, nCoreIdx, nCamIdx, nFrame); }


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool TestTCPSocket(IntPtr sealingInspProcessor);
        public bool TestTCPSocket() { return TestTCPSocket(m_sealingInspectProcessor); }


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
        private static extern void RegCallBackInspectCavity1CompleteFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCavity1CompleteFunc callbackPointer);
        public void RegCallBackInspectCavity1CompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCavity1CompleteFunc callbackPointer)
        {
            m_RegInsCavity1CompleteCallBack = callbackPointer;

            RegCallBackInspectCavity1CompleteFunc(m_sealingInspectProcessor, m_RegInsCavity1CompleteCallBack);
        }
        /**********************************
         - Register Inspection Complete CallBack
         - Parameter : CallBack Func Pointer
        **********************************/


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackInspectCavity2CompleteFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCavity2CompleteFunc callbackPointer);
        public void RegCallBackInspectCavity2CompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsCavity2CompleteFunc callbackPointer)
        {
            m_RegInsCavity2CompleteCallBack = callbackPointer;

            RegCallBackInspectCavity2CompleteFunc(m_sealingInspectProcessor, m_RegInsCavity2CompleteCallBack);
        }
        /**********************************
         - Register Inspection Complete CallBack
         - Parameter : CallBack Func Pointer
        **********************************/


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackInspectTopCam1CompleteFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsTopCam1CompleteFunc callbackPointer);
        public void RegCallBackInspectTopCam1CompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsTopCam1CompleteFunc callbackPointer)
        {
            m_RegInsTopCam1CompleteCallBack = callbackPointer;

            RegCallBackInspectTopCam1CompleteFunc(m_sealingInspectProcessor, m_RegInsTopCam1CompleteCallBack);
        }
        /**********************************
         - Register Inspection TopCam Complete CallBack
         - Parameter : CallBack Func Pointer
        **********************************/


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackInspectTopCam2CompleteFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsTopCam2CompleteFunc callbackPointer);
        public void RegCallBackInspectTopCam2CompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsTopCam2CompleteFunc callbackPointer)
        {
            m_RegInsTopCam2CompleteCallBack = callbackPointer;

            RegCallBackInspectTopCam2CompleteFunc(m_sealingInspectProcessor, m_RegInsTopCam2CompleteCallBack);
        }
        /**********************************
         - Register Inspection TopCam Complete CallBack
         - Parameter : CallBack Func Pointer
        **********************************/


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackGrabFrameSideCam1CompleteFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackGrabFrameSideCam1CompleteFunc callbackPointer);
        public void RegCallBackGrabFrameSideCam1CompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackGrabFrameSideCam1CompleteFunc callbackPointer)
        {
            m_RegGrabFrameSideCam1CompleteCallBack = callbackPointer;

            RegCallBackGrabFrameSideCam1CompleteFunc(m_sealingInspectProcessor, m_RegGrabFrameSideCam1CompleteCallBack);
        }
        /**********************************
         - Register Inspection TopCam Complete CallBack
         - Parameter : CallBack Func Pointer
        **********************************/


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallBackGrabFrameSideCam2CompleteFunc(IntPtr pInstance, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackGrabFrameSideCam2CompleteFunc callbackPointer);
        public void RegCallBackGrabFrameSideCam2CompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackGrabFrameSideCam2CompleteFunc callbackPointer)
        {
            m_RegGrabFrameSideCam2CompleteCallBack = callbackPointer;

            RegCallBackGrabFrameSideCam2CompleteFunc(m_sealingInspectProcessor, m_RegGrabFrameSideCam2CompleteCallBack);
        }
        /**********************************
         - Register Inspection TopCam Complete CallBack
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


#if DEBUG
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("SealingInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveImageHikCam(IntPtr sealingInspProcessor, int nCamIdx, [MarshalAs(UnmanagedType.LPStr)] string strImageSavePath);
        public bool SaveImageHikCam(int nCamIdx, [MarshalAs(UnmanagedType.LPStr)] string strImageSavePath)
        {
            bool bRet = SaveImageHikCam(m_sealingInspectProcessor, nCamIdx, strImageSavePath);
            return bRet;
        }
    }
    #endregion

}
