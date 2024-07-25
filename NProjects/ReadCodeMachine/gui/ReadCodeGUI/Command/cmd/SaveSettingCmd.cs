using ReadCodeGUI.Manager;
using ReadCodeGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReadCodeGUI.Command.Cmd
{
    public class SaveSettingCmd : CommandBase
    {
        public SaveSettingCmd() { }
        public override void Execute(object parameter)
        {
            string btnName = parameter as string;
            if (btnName != null)
            {
                if (btnName.CompareTo("btnSaveSystemSetting") == 0)
                {
                    MainViewModel.Instance.SettingVM.SetValue_SystemSetting();

                    InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.SaveSystemSetting(ref InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings);
                }
                else if (btnName.CompareTo("btnSavePLCSetting") == 0)
                {
                    MainViewModel.Instance.SettingVM.SavePlcSettings();
                }
            }
        }
    }
}
