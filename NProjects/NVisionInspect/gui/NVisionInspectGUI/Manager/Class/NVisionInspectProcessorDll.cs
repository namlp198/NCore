using NVisionInspectGUI.Commons;
using NVisionInspectGUI.Models;
using NVisionInspectGUI.Models.FakeCam.Recipe;
using NVisionInspectGUI.Models.FakeCam.Result;
using NVisionInspectGUI.Models.FakeCam.Setting;
using NVisionInspectGUI.Models.Recipe;
using NVisionInspectGUI.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
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

    // Locator Train CallBack
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallbackLocatorTrainCompleteFunc(int nCamIdx);

    // Insp Complete Fake Cam CallBack
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallbackInsComplete_FakeCamFunc(int nInspTool);

    // HSV complete train Callback
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallbackHSVTrainCompleteFunc(int nCamIdx);

    public class NVisionInspectProcessorDll
    {
        IntPtr m_NVisionInspectProcessor;

        public static CallbackLogFunc m_RegLogCallback;
        public static CallbackAlarmFunc m_RegAlarmCallback;
        public static CallbackInsCompleteFunc m_RegInsCompleteCallback;
        public static CallbackLocatorTrainCompleteFunc m_RegLocatorTrainCompleteCallback;
        public static CallbackInsComplete_FakeCamFunc m_RegInsComplete_FakeCamCallback;
        public static CallbackHSVTrainCompleteFunc m_RegHsvCompleteCallback;
        public NVisionInspectProcessorDll()
        {
            m_NVisionInspectProcessor = CreateNVisionInspectProcessor();
        }

        #region Init and delete
        /// <summary>
        /// Create a pointer the NVisionInspect processor
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
         - Parameter : Instance Poiter, Thread Count, Camera Count
        **********************************/

#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool InspectStop(IntPtr NVisionInspectProcessor, int nCamCount);
        public bool InspectStop(int nCamCount) { return InspectStop(m_NVisionInspectProcessor, nCamCount); }
        /**********************************
        - Inspection Ready / Start
        - Parameter : Instance Poiter, Camera Count
       **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool Inspect_Simulator(IntPtr NVisionInspectProcessor, int nCamBrand, int nCamIdx);
        public bool Inspect_Simulator(int nCamBrand, int nCamIdx) { return Inspect_Simulator(m_NVisionInspectProcessor, nCamBrand, nCamIdx); }
        /**********************************
         - Inspection Simulator
         - Parameter : Instance Pointer, Camera Brand, Camera Index
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
        /**********************************
         - Get Result Buffer
         - Parameter : Instance Pointer, Buffer Index, Frame Index
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetResultBuffer_FakeCam(IntPtr NVisionInspectProcessor, int nFrame);
        public IntPtr GetResultBuffer_FakeCam(int nFrame)
        {
            return GetResultBuffer_FakeCam(m_NVisionInspectProcessor, nFrame);
        }
        /**********************************
         - Get Result Buffer for Fake Camera
         - Parameter : Instance Pointer, Frame Index
        **********************************/


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
        /**********************************
         - Get Simulator Buffer
         - Parameter : Instance Pointer, Buffer Index, Frame Index
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetSimulatorBuffer_FakeCam(IntPtr NVisionInspectProcessor, int nFrame);
        public IntPtr GetSimulatorBuffer_FakeCam(int nFrame)
        {
            return GetSimulatorBuffer_FakeCam(m_NVisionInspectProcessor, nFrame);
        }
        /**********************************
        - Get Simulator Buffer for Fake Camera
        - Parameter : Instance Pointer, Frame Index
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadSimulatorBuffer(IntPtr NVisionInspectProcessor, int nBuff, int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath);
        public bool LoadSimulatorBuffer(int nBuff, int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath) { return LoadSimulatorBuffer(m_NVisionInspectProcessor, nBuff, nFrame, filePath); }
        /**********************************
       - Load Simulator Buffer
       - Parameter : Instance Pointer, Buffer Index, Frame Index
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadSimulatorBuffer_FakeCam(IntPtr NVisionInspectProcessor, int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath);
        public bool LoadSimulatorBuffer_FakeCam(int nFrame, [MarshalAs(UnmanagedType.LPStr)] string filePath) { return LoadSimulatorBuffer_FakeCam(m_NVisionInspectProcessor, nFrame, filePath); }
        /**********************************
       - Load Simulator Buffer for Fake Camera
       - Parameter : Instance Pointer, Frame Index, File Path
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LocatorTool_Train(IntPtr NVisionInspectProcessor, int nCamIdx);
        public bool LocatorTool_Train(int nCamIdx) { return LocatorTool_Train(m_NVisionInspectProcessor, nCamIdx); }
        /**********************************
       - Train Locator Tool
       - Parameter : Instance Pointer, Camera Index
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LocatorToolSimulator_Train(IntPtr NVisionInspectProcessor, int nSimuBuff, int nFrame);
        public bool LocatorToolSimulator_Train(int nSimuBuff, int nFrame) { return LocatorToolSimulator_Train(m_NVisionInspectProcessor, nSimuBuff, nFrame); }
        /**********************************
       - Train Locator Tool Simulator
       - Parameter : Instance Pointer, Simulator Buffer Index, Frame Index
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LocatorToolFakeCam_Train(IntPtr NVisionInspectProcessor, int nFrame);
        public bool LocatorToolFakeCam_Train(int nFrame) { return LocatorToolFakeCam_Train(m_NVisionInspectProcessor, nFrame); }
        /**********************************
       - Train Locator Tool Fake Camera
       - Parameter : Instance Pointer, Frame Index
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SelectROI(IntPtr NVisionInspectProcessor, int nCamIdx, int nROIIdx, int nFrom, int nROIX, int nROIY, int nROIWidth, int nROIHeight);
        public bool SelectROI(int nCamIdx, int nROIIdx, int nFrom, int nROIX, int nROIY, int nROIWidth, int nROIHeight) { return SelectROI(m_NVisionInspectProcessor, nCamIdx, nROIIdx, nFrom, nROIX, nROIY, nROIWidth, nROIHeight); }
        /**********************************
       - Select ROI
       - Parameter : Instance Pointer, Camera Index, ROI Index, From Index
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool CallInspectTool(IntPtr NVisionInspectProcessor, int nInspTool);
        public void CallInspectTool(int nInspTool) { CallInspectTool(m_NVisionInspectProcessor, nInspTool); }
        /**********************************
       - Call Inspect Tool
       - Parameter : Instance Pointer, Inspect Tool Index
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool HSVTrain(IntPtr NVisionInspectProcessor, int nCamIdx, int nFrame, IntPtr pRecipeHSV);
        public bool HSVTrain(int nCamIdx, int nFrame, CNVisionInspectRecipe_HSV recipeHSV)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(recipeHSV));
            Marshal.StructureToPtr(recipeHSV, pPointer, false);

            bool bRet = HSVTrain(m_NVisionInspectProcessor, nCamIdx, nFrame, pPointer);

            return bRet;
        }
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
        /**********************************
       - Load System Settings
       - Parameter : Instance Pointer, System Setting Pointer
        **********************************/


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
        /**********************************
       - Load Camera Settings
       - Parameter : Instance Pointer, Camera Setting Pointer, Camera Index
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadFakeCameraSettings(IntPtr NVisionInspectProcessor, IntPtr pFakeCamSetting);
        public bool LoadFakeCameraSettings(ref CNVisionInspect_FakeCameraSetting fakeCamSetting)
        {
            CNVisionInspect_FakeCameraSetting settings = new CNVisionInspect_FakeCameraSetting();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(settings));
            Marshal.StructureToPtr(settings, pPointer, false);
            bool bRet = LoadFakeCameraSettings(m_NVisionInspectProcessor, pPointer);
            fakeCamSetting = (CNVisionInspect_FakeCameraSetting)Marshal.PtrToStructure(pPointer, typeof(CNVisionInspect_FakeCameraSetting));

            return bRet;
        }
        /**********************************
       - Load Fake Camera Settings
       - Parameter : Instance Pointer, Fake Camera Setting Pointer
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadRecipe(IntPtr NVisionInspectProcessor, int nCamCount, IntPtr pRecipe);
        public bool LoadRecipe(int nCamCount, ref CNVisionInspectRecipe pRecipe)
        {
            CNVisionInspectRecipe recipe = new CNVisionInspectRecipe();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(recipe));
            Marshal.StructureToPtr(recipe, pPointer, false);
            bool bRet = LoadRecipe(m_NVisionInspectProcessor, nCamCount, pPointer);
            pRecipe = (CNVisionInspectRecipe)Marshal.PtrToStructure(pPointer, typeof(CNVisionInspectRecipe));

            return bRet;
        }
        /**********************************
       - Load Recipe
       - Parameter : Instance Pointer, Camera Count, Recipe Pointer
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadRecipe_FakeCam(IntPtr NVisionInspectProcessor, IntPtr pRecipeFakeCam);
        public bool LoadRecipe_FakeCam(ref CNVisionInspectRecipe_FakeCam pRecipeFakeCam)
        {
            CNVisionInspectRecipe_FakeCam recipe = new CNVisionInspectRecipe_FakeCam();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(recipe));
            Marshal.StructureToPtr(recipe, pPointer, false);
            bool bRet = LoadRecipe_FakeCam(m_NVisionInspectProcessor, pPointer);
            pRecipeFakeCam = (CNVisionInspectRecipe_FakeCam)Marshal.PtrToStructure(pPointer, typeof(CNVisionInspectRecipe_FakeCam));

            return bRet;
        }
        /**********************************
      - Load Recipe Fake Camera
      - Parameter : Instance Pointer, Recipe Fake Camera Pointer
       **********************************/

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
        /**********************************
      - Save System Setting
      - Parameter : Instance Pointer, System Setting Pointer
       **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveCameraSetting(IntPtr NVisionInspectProcessor, int nCamIdx, IntPtr pCamSetting);
        public bool SaveCameraSetting(int nCamIdx, ref CNVisionInspectCameraSetting camSetting)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(camSetting));
            Marshal.StructureToPtr(camSetting, pPointer, false);
            bool bRet = SaveCameraSetting(m_NVisionInspectProcessor, nCamIdx, pPointer);

            return bRet;
        }
        /**********************************
      - Save Camera Setting
      - Parameter : Instance Pointer, Camera Index, Camera Setting Pointer
       **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveFakeCameraSetting(IntPtr NVisionInspectProcessor, IntPtr pFakeCamSetting);
        public bool SaveFakeCameraSetting(ref CNVisionInspect_FakeCameraSetting fakeCamSetting)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(fakeCamSetting));
            Marshal.StructureToPtr(fakeCamSetting, pPointer, false);
            bool bRet = SaveFakeCameraSetting(m_NVisionInspectProcessor, pPointer);

            return bRet;
        }
        /**********************************
      - Save Fake Camera Setting
      - Parameter : Instance Pointer, Fake Camera Setting Pointer
       **********************************/


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
        /**********************************
      - Save Recipe
      - Parameter : Instance Pointer, Camera Index, Recipe Pointer
       **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveRecipe_FakeCam(IntPtr NVisionInspectProcessor, IntPtr pRecipeFakeCam);
        public bool SaveRecipe_FakeCam(ref CNVisionInspectRecipe_FakeCam pRecipeFakeCam)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(pRecipeFakeCam));
            Marshal.StructureToPtr(pRecipeFakeCam, pPointer, false);
            bool bRet = SaveRecipe_FakeCam(m_NVisionInspectProcessor, pPointer);

            return bRet;
        }
        /**********************************
      - Save Recipe Fake Camera
      - Parameter : Instance Pointer, Recipe Fake Camera Pointer
       **********************************/

        #endregion

        #region Get Results
#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetInspectionResult(IntPtr NVisionInspectProcessor, IntPtr InspResults);
        public bool GetInspectionResult(ref CNVisionInspectResult InspResults)
        {
            CNVisionInspectResult inspRes = new CNVisionInspectResult();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(inspRes));
            Marshal.StructureToPtr(inspRes, pPointer, false);
            bool bRet = GetInspectionResult(m_NVisionInspectProcessor, pPointer);
            InspResults = (CNVisionInspectResult)Marshal.PtrToStructure(pPointer, typeof(CNVisionInspectResult));

            return bRet;
        }
        /**********************************
      - Get Inspection Result
      - Parameter : Instance Pointer, Inspection Result Pointer
       **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool GetInspectToolResult_FakeCam(IntPtr NVisionInspectProcessor, IntPtr pInspRes_FakeCam);
        public bool GetInspectToolResult_FakeCam(ref CNVisionInspectResult_FakeCam inspResult_FakeCam)
        {
            CNVisionInspectResult_FakeCam inspRes = new CNVisionInspectResult_FakeCam();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(inspRes));
            Marshal.StructureToPtr(inspRes, pPointer, false);
            bool bRet = GetInspectToolResult_FakeCam(m_NVisionInspectProcessor, pPointer);
            inspResult_FakeCam = (CNVisionInspectResult_FakeCam)Marshal.PtrToStructure(pPointer, typeof(CNVisionInspectResult_FakeCam));

            return bRet;
        }
        /**********************************
      - Get Inspect Tool Result Fake Cam
      - Parameter : Instance Pointer, Inspect Result Fake Camera
       **********************************/

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
            m_RegAlarmCallback = callbackPointer;

            RegCallBackAlarmFunc(m_NVisionInspectProcessor, m_RegAlarmCallback);
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
            m_RegLogCallback = callbackPointer;

            RegCallBackLogFunc(m_NVisionInspectProcessor, m_RegLogCallback);
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
            m_RegInsCompleteCallback = callbackPointer;

            RegCallBackInspectCompleteFunc(m_NVisionInspectProcessor, m_RegInsCompleteCallback);
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
            m_RegLocatorTrainCompleteCallback = callbackPointer;

            RegCallBackLocatorTrainCompleteFunc(m_NVisionInspectProcessor, m_RegLocatorTrainCompleteCallback);
        }
        /**********************************
         - Register Locator Trained CallBack
         - Parameter : CallBack Func Pointer
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallbackInspComplete_FakeCamFunc(IntPtr NVisionInspectProcessor, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsComplete_FakeCamFunc callbackPointer);
        public void RegCallbackInspComplete_FakeCamFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackInsComplete_FakeCamFunc callbackPointer)
        {
            m_RegInsComplete_FakeCamCallback = callbackPointer;

            RegCallbackInspComplete_FakeCamFunc(m_NVisionInspectProcessor, m_RegInsComplete_FakeCamCallback);
        }
        /**********************************
         - Register Inspect Tool Fake Cam CallBack
         - Parameter : CallBack Func Pointer
        **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        private static extern void RegCallbackHSVTrainCompleteFunc(IntPtr NVisionInspectProcessor, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackHSVTrainCompleteFunc callbackPointer);
        public void RegCallbackHSVTrainCompleteFunc([MarshalAs(UnmanagedType.FunctionPtr)] CallbackHSVTrainCompleteFunc callbackPointer)
        {
            m_RegHsvCompleteCallback = callbackPointer;

            RegCallbackHSVTrainCompleteFunc(m_NVisionInspectProcessor, m_RegHsvCompleteCallback);
        }
        /**********************************
         - Register HSV Train Complete CallBack
         - Parameter : CallBack Func Pointer
        **********************************/
        #endregion

        #region Camera

#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool ContinuousGrab(IntPtr NVisionInspectProcessor, int nCamBrand, int nCamIdx);
        public bool ContinuousGrab(int nCamBrand, int nCamIdx) { return ContinuousGrab(m_NVisionInspectProcessor, nCamBrand, nCamIdx); }
        /**********************************
      - Continuous Grab
      - Parameter : Instance Pointer, Camera Brand, Camera Index
       **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool StopGrab(IntPtr NVisionInspectProcessor, int nCamBrand, int nCamIdx);
        public bool StopGrab(int nCamBrand, int nCamIdx) { return StopGrab(m_NVisionInspectProcessor, nCamBrand, nCamIdx); }
        /**********************************
     - Stop Grab
     - Parameter : Instance Pointer, Camera Brand, Camera Index
      **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static IntPtr GetImageBuffer(IntPtr NVisionInspectProcessor, int nCamBrand, int nCamIdx);
        public IntPtr GetImageBuffer(int nCamBrand, int nCamIdx) { return GetImageBuffer(m_NVisionInspectProcessor, nCamBrand, nCamIdx); }
        /**********************************
     - Get Image Buffer
     - Parameter : Instance Pointer, Camera Brand, Camera Index
      **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetTriggerMode(IntPtr NVisionInspectProcessor, int nCamIdx, int nMode);
        public bool SetTriggerMode(int nCamIdx, int nMode) { return SetTriggerMode(m_NVisionInspectProcessor, nCamIdx, nMode); }
        /**********************************
     - Set Trigger Mode
     - Parameter : Instance Pointer, Camera Index, Mode Index
      **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetTriggerSource(IntPtr NVisionInspectProcessor, int nCamIdx, int nSource);
        public bool SetTriggerSource(int nCamIdx, int nSource) { return SetTriggerSource(m_NVisionInspectProcessor, nCamIdx, nSource); }
        /**********************************
    - Set Trigger Source
    - Parameter : Instance Pointer, Camera Index, Source Index
     **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetExposureTime(IntPtr NVisionInspectProcessor, int nCamIdx, double dExpTime);
        public bool SetExposureTime(int nCamIdx, double dExpTime) { return SetExposureTime(m_NVisionInspectProcessor, nCamIdx, dExpTime); }
        /**********************************
    - Set Exposure
    - Parameter : Instance Pointer, Camera Index, Exposure Time
     **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SetAnalogGain(IntPtr NVisionInspectProcessor, int nCamIdx, double dGain);
        public bool SetAnalogGain(int nCamIdx, double dGain) { return SetAnalogGain(m_NVisionInspectProcessor, nCamIdx, dGain); }
        /**********************************
   - Set Analog Gain
   - Parameter : Instance Pointer, Camera Index, Gain Analog
    **********************************/


#if DEBUG
        [DllImport("NVisionInspectProcessor_Debug64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("NVisionInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveImage(IntPtr NVisionInspectProcessor, int nCamIdx);
        public bool SaveImage(int nCamIdx) { return SaveImage(m_NVisionInspectProcessor, nCamIdx); }
        /**********************************
   - Save Image
   - Parameter : Instance Pointer, Camera Index
    **********************************/

        #endregion
    }
}
