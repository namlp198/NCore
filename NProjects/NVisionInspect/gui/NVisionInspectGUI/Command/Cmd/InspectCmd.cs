
using NVisionInspectGUI.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NVisionInspectGUI.ViewModels;

namespace NVisionInspectGUI.Command.Cmd
{
    public class InspectCmd : CommandBase
    {
        public InspectCmd() { }
        public override void Execute(object parameter)
        {
            int nThreadCount = 1;
            int nCamCount = MainViewModel.Instance.SettingVM.CameraCount;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.InspectStart(nThreadCount, nCamCount);
        }
    }
}
