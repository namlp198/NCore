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
                if (btnName.CompareTo("btnSaveSystemSetting_PropertyGrid") == 0)
                {
                    {
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_nInspectCameraCount = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.InspectCameraCount;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSimulation = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.Simulation == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bByPass = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.ByPass == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bTestMode = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.TestMode == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sModelName = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.ModelName;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sModelList = MainViewModel.Instance.SettingVM.NVisionInspectSystemSettingsPropertyGrid.ModelList;
                    }

                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SaveSystemSetting(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings);
                }
                else if(btnName.CompareTo("btnSaveCameraSetting_PropertyGrid") == 0)
                {
                    int nCamIdx = 0;
                    {
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_bSaveFullImage = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.IsSaveFullImage == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_bSaveDefectImage = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.IsSaveDefectImage == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_bShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.IsShowGraphics == true ? 1 : 0;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nChannels = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.Channels;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nFrameWidth = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.FrameWidth;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nFrameHeight = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.FrameHeight;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nFrameDepth = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.FrameDepth;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nMaxFrameCount = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.MaxFrameCount;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nNumberOfROI = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.NumberOfROI;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sCameraName = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.CameraName;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sInterfaceType = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.InterfaceType;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sSensorType = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.SensorType;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sManufacturer = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.Manufacturer;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sSerialNumber = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.SerialNumber;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sModel = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.Model;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sFullImagePath = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.FullImagePath;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sDefectImagePath = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.DefectImagePath;
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sTemplateImagePath = MainViewModel.Instance.SettingVM.NVisionInspectCamera1SettingsPropertyGrid.TemplateImagePath;
                    }

                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SaveCameraSetting(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx], nCamIdx);
                }
                else if (btnName.CompareTo("btnSavePLCSetting_PropertyGrid") == 0)
                {
                    MainViewModel.Instance.SettingVM.SavePlcSettings();
                }
            }
        }
    }
}
