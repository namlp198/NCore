
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
    public class ColorSpaceCmd : CommandBase
    {
        public ColorSpaceCmd() { }
        public override void Execute(object parameter)
        {
            if (MainViewModel.Instance.SettingVM.OpenPopupColorSpace == false)
                MainViewModel.Instance.SettingVM.OpenPopupColorSpace = true;
            else
                MainViewModel.Instance.SettingVM.OpenPopupColorSpace = false;
        }
    }
}
