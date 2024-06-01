using SealingInspectGUI.Manager;
using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SealingInspectGUI.Command.Cmd
{
    public class InspectSimulationCmd : CommandBase
    {
        public InspectSimulationCmd() { }
        public override void Execute(object parameter)
        {
            ECameraList cameraSelected  = (ECameraList)parameter;
            int nCoreIdx = MainViewModel.Instance.SettingVM.CoreIdx;
            int nCamIdx = MainViewModel.Instance.SettingVM.BuffIdx;
            int nFrame = MainViewModel.Instance.SettingVM.Frame;

            switch (cameraSelected)
            {
                case ECameraList.TopCam1:
                case ECameraList.TopCam2:
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.
                        m_sealingInspProcessorDll.Inspect_TopCam_Simulation(nCoreIdx, nCamIdx, nFrame);
                    break;
                case ECameraList.SideCam1:
                case ECameraList.SideCam2:
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.
                        m_sealingInspProcessorDll.Inspect_SideCam_Simulation(nCoreIdx, nCamIdx, nFrame);
                    break;
            }
        }
    }
}
