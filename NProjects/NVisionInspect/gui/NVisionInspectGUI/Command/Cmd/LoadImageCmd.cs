using Microsoft.Win32;
using NVisionInspectGUI.Manager;
using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
{
    public class LoadImageCmd : CommandBase
    {
        public LoadImageCmd()
        {
        }
        public override void Execute(object parameter)
        {
            if (parameter == null)
                return;

            string btn = parameter as string;

            int nCamIdx = MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.CameraIndex;
            int nBuff = nCamIdx;
            int nFrame = 0;

            if (btn.CompareTo("btnLoadImage") == 0)
            {
                InterfaceManager.Instance.m_simulationThread.LoadImage(nCamIdx, nBuff, nFrame);
            }
            else if(btn.CompareTo("btnLoadAllImage") == 0)
            {
                InterfaceManager.Instance.m_simulationThread.LoadAllImage(nCamIdx, nBuff, nFrame);
            }
        }
    }
}
