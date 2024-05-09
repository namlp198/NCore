using NpcCore.Wpf.Struct_Vision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DinoVisionGUI
{
    public class ConstDefine
    {
        public const int MAX_STRING_SIZE = 256;

        public const int MAX_CAMERA_INSP_COUNT = 1;

        public const int MAX_ROI_AUTO = 2;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CJigInspectResults
    {
        public int m_bInspectCompleted;
        public int m_bResultOKNG;
        public CLocatorTool_TemplateMatching_Result m_TemplateMatchingResult;
    }

    // System Setting
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CJigInspectSystemConfig
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sRecipePath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sModel;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sCOMPort;
        public int m_bUsePCControl;
        public int m_bShowDetail;
        public int m_bSaveImage;
    }

    // Cam setting
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CJigInspectCameraConfig
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sInterfaceType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sSensorType;
        public int m_nChannels;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sManufacturer;
        public int m_nFrameWidth;
        public int m_nFrameHeight;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sSerialNumber;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sImageSavePath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sImageTemplatePath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sRecipeName;
    }

    // Recipe setting
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CJigInspectRecipe
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sRecipeName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sAlgorithm;
        public int m_nRectX;
        public int m_nRectY;
        public int m_nRectWidth;
        public int m_nRectHeight;
        public double m_dMatchingRate;
        public int m_nCenterX;
        public int m_nCenterY;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstDefine.MAX_STRING_SIZE)]
        public string m_sImageTemplate;
        public int m_nOffsetROI0_X;
        public int m_nOffsetROI0_Y;
        public int m_nOffsetROI1_X;
        public int m_nOffsetROI1_Y;
        public int m_nROIWidth;
        public int m_nROIHeight;
        public int m_nNumberOfArray;
        public int m_nThresholdHeightMin;
        public int m_nThresholdHeightMax;
        public int m_nThresholdWidthMin;
        public int m_nThresholdWidthMax;
        public int m_nKSizeX_Open;
        public int m_nKSizeY_Open;
        public int m_nKSizeX_Close;
        public int m_nKSizeY_Close;
        public int m_nContourSizeMin;
        public int m_nContourSizeMax;
        public int m_nThresholdBinary;
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
        extern private static bool GrabImageForLocatorTool(IntPtr tempInspProcessor, int nCamIdx);
        public bool GrabImageForLocatorTool(int nCamIdx) { return GrabImageForLocatorTool(m_JigInspectProcessor, nCamIdx); }



#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LocatorTrain(IntPtr tempInspProcessor, int nCamIdx, IntPtr pRecipe);
        public bool LocatorTrain(int nCamIdx, ref CJigInspectRecipe recipe) 
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(recipe));
            Marshal.StructureToPtr(recipe, pPointer, false);
            bool bRet = LocatorTrain(m_JigInspectProcessor, nCamIdx, pPointer);

            return bRet;
        }



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
        extern private static IntPtr GetResultBufferImageDinoCam_BGR(IntPtr tempInspProcessor, int nCamIdx);
        public IntPtr GetResultBufferImageDinoCam_BGR(int nCamIdx) { return GetResultBufferImageDinoCam_BGR(m_JigInspectProcessor, nCamIdx); }



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
        extern private static bool LoadSysConfigurations(IntPtr tempInspProcessor, IntPtr pSysConfig);
        public bool LoadSysConfigurations(ref CJigInspectSystemConfig sysConfig)
        {
            CJigInspectSystemConfig config = new CJigInspectSystemConfig();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(config));
            Marshal.StructureToPtr(config, pPointer, false);
            bool bRet = LoadSysConfigurations(m_JigInspectProcessor, pPointer);
            sysConfig = (CJigInspectSystemConfig)Marshal.PtrToStructure(pPointer, typeof(CJigInspectSystemConfig));

            return bRet;
        }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadCamConfigurations(IntPtr tempInspProcessor, int nCamIdx, IntPtr pCamConfig);
        public bool LoadCamConfigurations(int nCamIdx, ref CJigInspectCameraConfig camConfig)
        {
            CJigInspectCameraConfig config = new CJigInspectCameraConfig();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(config));
            Marshal.StructureToPtr(config, pPointer, false);
            bool bRet = LoadCamConfigurations(m_JigInspectProcessor, nCamIdx, pPointer);
            camConfig = (CJigInspectCameraConfig)Marshal.PtrToStructure(pPointer, typeof(CJigInspectCameraConfig));

            return bRet;
        }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool LoadRecipe(IntPtr tempInspProcessor, int nCamIdx, IntPtr pRecipe);
        public bool LoadRecipe(int nCamIdx, ref CJigInspectRecipe recipe)
        {
            CJigInspectRecipe config = new CJigInspectRecipe();
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(config));
            Marshal.StructureToPtr(config, pPointer, false);
            bool bRet = LoadRecipe(m_JigInspectProcessor, nCamIdx, pPointer);
            recipe = (CJigInspectRecipe)Marshal.PtrToStructure(pPointer, typeof(CJigInspectRecipe));

            return bRet;
        }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveSysConfigurations(IntPtr tempInspProcessor, IntPtr pSysConfig);
        public bool SaveSysConfigurations(ref CJigInspectSystemConfig sysConfig)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(sysConfig));
            Marshal.StructureToPtr(sysConfig, pPointer, false);
            bool bRet = SaveSysConfigurations(m_JigInspectProcessor, pPointer);

            return bRet;
        }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveCamConfigurations(IntPtr tempInspProcessor, int nCamIdx, IntPtr pCamConfig);
        public bool SaveCamConfigurations(int nCamIdx, ref CJigInspectCameraConfig camConfig)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(camConfig));
            Marshal.StructureToPtr(camConfig, pPointer, false);
            bool bRet = SaveCamConfigurations(m_JigInspectProcessor, nCamIdx, pPointer);

            return bRet;
        }


#if DEBUG
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("JigInspectProcessor_Release64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
        extern private static bool SaveRecipe(IntPtr tempInspProcessor, int nCamIdx, IntPtr pRecipe);
        public bool SaveRecipe(int nCamIdx, ref CJigInspectRecipe recipe)
        {
            IntPtr pPointer = Marshal.AllocHGlobal(Marshal.SizeOf(recipe));
            Marshal.StructureToPtr(recipe, pPointer, false);
            bool bRet = SaveRecipe(m_JigInspectProcessor, nCamIdx, pPointer);

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
