using NVisionInspectGUI.Commons;
using NVisionInspectGUI.Manager.Class;
using NVisionInspectGUI.Models;
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

        public CNVisionInspectResult[] m_NVisionInspectResult = new CNVisionInspectResult[Defines.MAX_CAMERA_INSPECT_COUNT];
        public CNVisionInspectCameraSetting[] m_NVisionInspectCamSetting = new CNVisionInspectCameraSetting[Defines.MAX_CAMERA_INSPECT_COUNT];
        public CNVisionInspectSystemSetting m_NVisionInspectSysSettings = new CNVisionInspectSystemSetting();
        public CNVisionInspectRecipe m_NVisionInspectRecipe = new CNVisionInspectRecipe();
        public void Initialize()
        {
            m_NVisionInspectProcessorDll.RegCallBackLocatorTrainCompleteFunc(InterfaceManager.Instance.CallbackLocatorTrainedFunc);
            m_NVisionInspectProcessorDll.RegCallBackAlarmFunc(InterfaceManager.Instance.CallbackAlarmFunc);
            m_NVisionInspectProcessorDll.RegCallBackLogFunc(InterfaceManager.Instance.CallbackWriteLogFunc);
            m_NVisionInspectProcessorDll.RegCallBackInspectCompleteFunc(InterfaceManager.Instance.CallbackInsCompleteFunc);
            m_NVisionInspectProcessorDll.Initialize();
        }

        public void Destroy()
        {
            m_NVisionInspectProcessorDll.DeleteNVisionInspectProcessor();
        }
    }
}
