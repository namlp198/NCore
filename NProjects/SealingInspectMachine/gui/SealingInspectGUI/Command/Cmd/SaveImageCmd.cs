using SealingInspectGUI.Manager;
using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Command.Cmd
{
    public class SaveImageCmd : CommandBase
    {
        public SaveImageCmd() { }
        public override void Execute(object parameter)
        {
            string sImageSavePath = parameter as string;
            int nCamIdx = MainViewModel.Instance.SettingVM.SettingView.buffVSSettings.CameraIndex;
            string sTick = DateTime.Now.ToString("yyyyMMddHHmmss");

            sImageSavePath = string.Format("{0}{1}{2}", sImageSavePath, sTick, ".bmp");

            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SaveImageHikCam(nCamIdx, sImageSavePath);

        }
    }
}
