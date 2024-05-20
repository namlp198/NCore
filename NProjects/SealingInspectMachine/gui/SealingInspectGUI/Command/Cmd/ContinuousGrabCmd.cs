using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SealingInspectGUI.Command.Cmd
{
    public class ContinuousGrabCmd : CommandBase
    {
        public ContinuousGrabCmd() { }
        public override void Execute(object parameter)
        {
            if (!MainViewModel.Instance.SettingVM.IsStreamming)
            {
                MainViewModel.Instance.SettingVM.SettingView.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await MainViewModel.Instance.SettingVM.m_cameraStreamingController.ContinuousGrab(Manager.Class.CameraType.Hik);
                }));
            }
            else
            {
                MainViewModel.Instance.SettingVM.SettingView.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await MainViewModel.Instance.SettingVM.m_cameraStreamingController.StopGrab(Manager.Class.CameraType.Hik);
                }));
            }
        }
    }
}
