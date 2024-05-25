using SealingInspectGUI.Manager;
using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SealingInspectGUI.Command.Cmd
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

                    InterfaceManager.Instance.m_sealingInspectProcessorManager.
                        m_sealingInspProcessorDll.SaveSystemSetting(ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting);
                }
                else if (btnName.CompareTo("btnSaveLightSetting_1") == 0)
                {
                    MainViewModel.Instance.SettingVM.SetValue_LightSetting(0);
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.
                        m_sealingInspProcessorDll.SaveLightSetting(ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting, 0);
                }
                else if (btnName.CompareTo("btnSaveLightSetting_2") == 0)
                {
                    MainViewModel.Instance.SettingVM.SetValue_LightSetting(1);
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.
                        m_sealingInspProcessorDll.SaveLightSetting(ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting, 1);
                }
            }
        }
    }
}
