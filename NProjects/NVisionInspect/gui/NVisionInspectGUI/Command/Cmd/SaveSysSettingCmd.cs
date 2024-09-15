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
    public class SaveSysSettingCmd : CommandBase
    {
        public SaveSysSettingCmd() { }
        public override void Execute(object parameter)
        {
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_nNumberOfInspectionCamera = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.NumberOfInspectionCamera;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSimulation = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.Simulation == true ? 1 : 0;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bByPass = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.ByPass == true ? 1 : 0;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bTestMode = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.TestMode == true ? 1 : 0;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sModelName = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.ModelName;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sModelList = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.ModelList;

            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SaveSystemSetting(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings);
        }
    }
}
