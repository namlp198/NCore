using NVisionInspectGUI.Commons;
using NVisionInspectGUI.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NVisionInspectGUI.ViewModels;

namespace NVisionInspectGUI.Command.Cmd
{
    public class InspectSimulatorCmd : CommandBase
    {
        public InspectSimulatorCmd() { }
        public override void Execute(object parameter)
        {
            int nCamIdx = MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.CameraIndex;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.Inspect_Simulator((int)emCameraBrand.CameraBrand_Hik, nCamIdx);
        }
    }
}
