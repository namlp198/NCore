using NVisionInspectGUI.Commons;
using NVisionInspectGUI.Manager;
using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
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
            int nHikCamCount = MainViewModel.Instance.SettingVM.NumberOfCamBrandList.ElementAt(0); // Index 0 is number of Hik Cam
            emCameraBrand camBrand = MainViewModel.Instance.SettingVM.CameraBrandSelected;

            switch (camBrand)
            {
                case emCameraBrand.CameraBrand_Hik:
                    break;
                case emCameraBrand.CameraBrand_Basler:
                    nCamIdx = nCamIdx - nHikCamCount;
                    break;
                case emCameraBrand.CameraBrand_Jai:
                    break;
                case emCameraBrand.CameraBrand_IRayple:
                    break;
            }

            if (nCamIdx < 0)
                return;

            if (!MainViewModel.Instance.SettingVM.IsStartedAcq)
            {
                if (!InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.ContinuousGrab((int)camBrand, nCamIdx)) return;

                MainViewModel.Instance.SettingVM.IsStartedAcq = true;
            }
            else
            {
                if (!InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.StopGrab((int)camBrand, nCamIdx)) return;

                MainViewModel.Instance.SettingVM.IsStartedAcq = false;
            }
        }
    }
}
