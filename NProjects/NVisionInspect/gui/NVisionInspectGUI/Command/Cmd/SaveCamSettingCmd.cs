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
    public class SaveCamSettingCmd : CommandBase
    {
        public SaveCamSettingCmd() { }
        public override void Execute(object parameter)
        {
            if (parameter == null)
                return;

            int nCamIdx = int.Parse(parameter.ToString());

            if (nCamIdx < 0)
                return;

            // Fake Cam
            if(nCamIdx >= MainViewModel.Instance.SettingVM.CameraCount)
            {
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectFakeCamSetting.m_nChannels = MainViewModel.Instance.SettingVM.NVIFakeCamSetting_PropGrid.Channels;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectFakeCamSetting.m_nFrameWidth = MainViewModel.Instance.SettingVM.NVIFakeCamSetting_PropGrid.FrameWidth;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectFakeCamSetting.m_nFrameHeight = MainViewModel.Instance.SettingVM.NVIFakeCamSetting_PropGrid.FrameHeight;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectFakeCamSetting.m_nFrameDepth = MainViewModel.Instance.SettingVM.NVIFakeCamSetting_PropGrid.FrameDepth;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectFakeCamSetting.m_nMaxFrameCount = MainViewModel.Instance.SettingVM.NVIFakeCamSetting_PropGrid.MaxFrameCount;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectFakeCamSetting.m_sCameraName = MainViewModel.Instance.SettingVM.NVIFakeCamSetting_PropGrid.CameraName;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectFakeCamSetting.m_sFullImagePath = MainViewModel.Instance.SettingVM.NVIFakeCamSetting_PropGrid.FullImagePath;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectFakeCamSetting.m_sDefectImagePath = MainViewModel.Instance.SettingVM.NVIFakeCamSetting_PropGrid.DefectImagePath;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectFakeCamSetting.m_sTemplateImagePath = MainViewModel.Instance.SettingVM.NVIFakeCamSetting_PropGrid.TemplateImagePath;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectFakeCamSetting.m_sROIsPath = MainViewModel.Instance.SettingVM.NVIFakeCamSetting_PropGrid.ROIsPath;

                InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SaveFakeCameraSetting(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectFakeCamSetting);

                return;
            }

            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_bSaveFullImage = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.IsSaveFullImage == true ? 1 : 0;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_bSaveDefectImage = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.IsSaveDefectImage == true ? 1 : 0;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_bShowGraphics = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.IsShowGraphics == true ? 1 : 0;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nChannels = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.Channels;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nFrameWidth = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.FrameWidth;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nFrameHeight = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.FrameHeight;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nFrameDepth = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.FrameDepth;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nMaxFrameCount = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.MaxFrameCount;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sCameraName = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.CameraName;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sInterfaceType = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.InterfaceType;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sSensorType = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.SensorType;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sManufacturer = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.Manufacturer;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sSerialNumber = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.SerialNumber;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sModel = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.Model;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sFullImagePath = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.FullImagePath;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sDefectImagePath = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.DefectImagePath;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sTemplateImagePath = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.TemplateImagePath;
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sROIsPath = MainViewModel.Instance.SettingVM.NVICam1Setting_PropGrid.ROIsPath;


            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SaveCameraSetting(nCamIdx, ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx]);

        }
    }
}
