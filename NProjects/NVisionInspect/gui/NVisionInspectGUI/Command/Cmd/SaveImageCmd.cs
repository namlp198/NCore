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
    public class SaveImageCmd : CommandBase
    {
        public SaveImageCmd() { }
        public override void Execute(object parameter)
        {
            int nCamIdx = MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.CameraIndex;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SaveImage(nCamIdx);
        }
    }
}
