using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
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
                    await MainViewModel.Instance.SettingVM.CameraStreamingController.ContinuousGrab(Manager.Class.CameraType.Hik);
                }));
            }
            else
            {
                MainViewModel.Instance.SettingVM.SettingView.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await MainViewModel.Instance.SettingVM.CameraStreamingController.StopGrab(Manager.Class.CameraType.Hik);
                }));
            }
        }
    }
}
