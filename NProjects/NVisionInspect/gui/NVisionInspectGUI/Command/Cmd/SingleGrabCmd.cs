using NVisionInspectGUI.Commons;
using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
{
    public class SingleGrabCmd : CommandBase
    {
        public SingleGrabCmd() { }
        public override void Execute(object parameter)
        {
            emCameraBrand camBrand = MainViewModel.Instance.SettingVM.CameraBrandSelected;

            MainViewModel.Instance.SettingVM.SettingView.Dispatcher.BeginInvoke(new Action(() =>
            {
               MainViewModel.Instance.SettingVM.CameraStreamingController.SingleGrab(camBrand);
            }));
        }
    }
}
