using NVisionInspectGUI.Commons;
using NVisionInspectGUI.Manager.Class;
using NVisionInspectGUI.Models;
using NVisionInspectGUI.Models.Recipe;
using NVisionInspectGUI.Models.Result;
using NVisionInspectGUI.Models.FakeCam.Recipe;
using NVisionInspectGUI.Models.FakeCam.Result;
using NVisionInspectGUI.Models.FakeCam.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Manager.SumManager
{
    public class NVisionInspectProcessorManager
    {
        public NVisionInspectProcessorDll m_NVisionInspectProcessorDll = new NVisionInspectProcessorDll();

        // Camera setting
        public CNVisionInspectCameraSetting[] m_NVisionInspectCamSetting = new CNVisionInspectCameraSetting[Defines.MAX_CAMERA_INSPECT_COUNT];

        // Fake Camera Setting
        public CNVisionInspect_FakeCameraSetting m_NVisionInspectFakeCamSetting = new CNVisionInspect_FakeCameraSetting();

        // System setting
        public CNVisionInspectSystemSetting m_NVisionInspectSysSettings = new CNVisionInspectSystemSetting();

        // Recipe
        public CNVisionInspectRecipe m_NVisionInspectRecipe = new CNVisionInspectRecipe();

        // Fake Cam Recipe
        public CNVisionInspectRecipe_FakeCam m_NVisionInspectRecipe_FakeCam = new CNVisionInspectRecipe_FakeCam();

        // Result
        public CNVisionInspectResult m_NVisionInspectResult = new CNVisionInspectResult();

        // Fake Cam Result
        public CNVisionInspectResult_FakeCam m_NVisionInspectResult_FakeCam = new CNVisionInspectResult_FakeCam();

        public void Initialize()
        {
            m_NVisionInspectProcessorDll.RegCallBackLocatorTrainCompleteFunc(InterfaceManager.Instance.CallbackLocatorTrainedFunc);
            m_NVisionInspectProcessorDll.RegCallBackAlarmFunc(InterfaceManager.Instance.CallbackAlarmFunc);
            m_NVisionInspectProcessorDll.RegCallBackLogFunc(InterfaceManager.Instance.CallbackWriteLogFunc);
            m_NVisionInspectProcessorDll.RegCallBackInspectCompleteFunc(InterfaceManager.Instance.CallbackInsCompleteFunc);
            m_NVisionInspectProcessorDll.RegCallbackInspComplete_FakeCamFunc(InterfaceManager.Instance.CallbackInsComplete_FakeCamFunc);
            m_NVisionInspectProcessorDll.RegCallbackHSVTrainCompleteFunc(InterfaceManager.Instance.CallbackHSVTrainCompleteFunc);
            m_NVisionInspectProcessorDll.Initialize();
        }

        public void Destroy()
        {
            m_NVisionInspectProcessorDll.DeleteNVisionInspectProcessor();
        }
    }
}
