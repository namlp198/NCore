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
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSaveFullImage = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.m_bSaveFullImage == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSaveDefectImage = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.m_bSaveDefectImage == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bShowDetailImage = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.m_bShowDetailImage == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSimulation = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.m_bSimulation == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bByPass = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.m_bByPass == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sFullImagePath = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.m_sFullImagePath;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sDefectImagePath = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.m_sDefectImagePath;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sTemplateImagePath = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.m_sTemplateImagePath;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sModelName = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.m_sModelName;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bTestMode = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.m_bTestMode == true ? 1 : 0;

                    }

                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SaveSystemSetting(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings);
                }
                else if (btnName.CompareTo("btnSavePLCSetting") == 0 || btnName.CompareTo("btnSavePLCSetting_PropertyGrid") == 0)
                {
                    MainViewModel.Instance.SettingVM.SavePlcSettings();
                }
            }
        }
    }
}
