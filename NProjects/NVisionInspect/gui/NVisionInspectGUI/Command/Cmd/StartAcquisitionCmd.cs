using NVisionInspectGUI.Manager;
using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
{
    public class StartAcquisitionCmd : CommandBase
    {
        public StartAcquisitionCmd() { }
        public override void Execute(object parameter)
        {
            int nCamIdx = MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.CameraIndex;
            if (!MainViewModel.Instance.SettingVM.IsStartedAcq)
            {
                if (!InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.ContinuousGrabHikCam(nCamIdx)) return;

                MainViewModel.Instance.SettingVM.IsStartedAcq = true;
            }
            else
            {
                if (!InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.StopGrabHikCam(nCamIdx)) return;

                MainViewModel.Instance.SettingVM.IsStartedAcq = false;
            }
        }
    }
}
