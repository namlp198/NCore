
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
    public class CalibrationCmd : CommandBase
    {
        public CalibrationCmd() { }
        public override void Execute(object parameter)
        {
            if (MainViewModel.Instance.SettingVM.OpenPopupCalibration == false)
            {
                MainViewModel.Instance.SettingVM.OpenPopupCalibration = true;
            }
            else
            {
                MainViewModel.Instance.SettingVM.OpenPopupCalibration = false;
            }
        }
    }
}
