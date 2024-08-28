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
                if (btnName.CompareTo("btnSaveSystemSetting") == 0 || btnName.CompareTo("btnSaveSystemSetting_PropertyGrid") == 0)
                {
                    {
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSaveFullImage = MainViewModel.Instance.SettingVM.ReadCodeSystemSettingsPropertyGrid.m_bSaveFullImage == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSaveDefectImage = MainViewModel.Instance.SettingVM.ReadCodeSystemSettingsPropertyGrid.m_bSaveDefectImage == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bShowDetailImage = MainViewModel.Instance.SettingVM.ReadCodeSystemSettingsPropertyGrid.m_bShowDetailImage == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSimulation = MainViewModel.Instance.SettingVM.ReadCodeSystemSettingsPropertyGrid.m_bSimulation == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bByPass = MainViewModel.Instance.SettingVM.ReadCodeSystemSettingsPropertyGrid.m_bByPass == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sFullImagePath = MainViewModel.Instance.SettingVM.ReadCodeSystemSettingsPropertyGrid.m_sFullImagePath;
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sDefectImagePath = MainViewModel.Instance.SettingVM.ReadCodeSystemSettingsPropertyGrid.m_sDefectImagePath;
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sTemplateImagePath = MainViewModel.Instance.SettingVM.ReadCodeSystemSettingsPropertyGrid.m_sTemplateImagePath;
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sModelName = MainViewModel.Instance.SettingVM.ReadCodeSystemSettingsPropertyGrid.m_sModelName;
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bTestMode = MainViewModel.Instance.SettingVM.ReadCodeSystemSettingsPropertyGrid.m_bTestMode == true ? 1 : 0;

                    }

                    //MainViewModel.Instance.SettingVM.SetValue_SystemSetting();

                    InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.SaveSystemSetting(ref InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings);
                }
                else if (btnName.CompareTo("btnSavePLCSetting") == 0 || btnName.CompareTo("btnSavePLCSetting_PropertyGrid") == 0)
                {
                    MainViewModel.Instance.SettingVM.SavePlcSettings();
                }
            }
        }
    }
}
