using NVisionInspectGUI.Manager;
using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
{
    public class SavePlcSettingCmd : CommandBase
    {
        public SavePlcSettingCmd() { }
        public override void Execute(object parameter)
        {
            MainViewModel.Instance.SettingVM.SavePlcSettings();
        }
    }
}
