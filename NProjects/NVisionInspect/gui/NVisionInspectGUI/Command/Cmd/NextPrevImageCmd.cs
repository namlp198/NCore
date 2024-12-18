﻿using Microsoft.Win32;
using NVisionInspectGUI.Manager;
using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
{
    public class NextPrevImageCmd : CommandBase
    {
        public NextPrevImageCmd()
        {
        }
        public override void Execute(object parameter)
        {
            if (parameter == null)
                return;

            string btn = parameter as string;

            int nCamIdx = MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.CameraIndex;
            int nBuff = nCamIdx;
            int nFrame = 0;
            string filePath = string.Empty;
            int nCurImgIndex = InterfaceManager.Instance.m_simulationThread.CurrentImageIndex;
            int nImageCount = InterfaceManager.Instance.m_simulationThread.ImageListPath.Count;

            if (nCamIdx < 0)
                return;

            if (btn.CompareTo("btnPrevImage") == 0)
            {
                if (nCurImgIndex == 0)
                {
                    nCurImgIndex = nImageCount - 1;
                    filePath = InterfaceManager.Instance.m_simulationThread.ImageListPath[nCurImgIndex];

                    if (nCamIdx >= MainViewModel.Instance.SettingVM.CameraCount)
                    {
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer_FakeCam(nFrame, filePath);
                        MainViewModel.Instance.SettingVM.SimulationThread_UpdateUI_FakeCam(nFrame);

                        InterfaceManager.Instance.m_simulationThread.CurrentImageIndex = nCurImgIndex;

                        MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.ResetImageExtBasic();
                        return;
                    }

                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer(nBuff, nFrame, filePath);
                    MainViewModel.Instance.SettingVM.SimulationThread_UpdateUI(nBuff, nFrame);

                    InterfaceManager.Instance.m_simulationThread.CurrentImageIndex = nCurImgIndex;
                    MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.ResetImageExtBasic();
                    return;
                }

                nCurImgIndex--;
                filePath = InterfaceManager.Instance.m_simulationThread.ImageListPath[nCurImgIndex];
                InterfaceManager.Instance.m_simulationThread.CurrentImageIndex = nCurImgIndex;

                if (nCamIdx >= MainViewModel.Instance.SettingVM.CameraCount)
                {
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer_FakeCam(nFrame, filePath);
                    MainViewModel.Instance.SettingVM.SimulationThread_UpdateUI_FakeCam(nFrame);

                    MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.ResetImageExtBasic();
                    return;
                }

                InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer(nBuff, nFrame, filePath);
                MainViewModel.Instance.SettingVM.SimulationThread_UpdateUI(nBuff, nFrame);

                MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.ResetImageExtBasic();
            }
            else if (btn.CompareTo("btnNextImage") == 0)
            {
                if (nCurImgIndex == nImageCount - 1)
                {
                    nCurImgIndex = 0;
                    filePath = InterfaceManager.Instance.m_simulationThread.ImageListPath[nCurImgIndex];

                    if (nCamIdx >= MainViewModel.Instance.SettingVM.CameraCount)
                    {
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer_FakeCam(nFrame, filePath);
                        MainViewModel.Instance.SettingVM.SimulationThread_UpdateUI_FakeCam(nFrame);

                        InterfaceManager.Instance.m_simulationThread.CurrentImageIndex = nCurImgIndex;
                        MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.ResetImageExtBasic();
                        return;
                    }

                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer(nBuff, nFrame, filePath);
                    MainViewModel.Instance.SettingVM.SimulationThread_UpdateUI(nBuff, nFrame);

                    InterfaceManager.Instance.m_simulationThread.CurrentImageIndex = nCurImgIndex;
                    MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.ResetImageExtBasic();
                    return;
                }

                nCurImgIndex++;
                filePath = InterfaceManager.Instance.m_simulationThread.ImageListPath[nCurImgIndex];
                InterfaceManager.Instance.m_simulationThread.CurrentImageIndex = nCurImgIndex;

                if (nCamIdx >= MainViewModel.Instance.SettingVM.CameraCount)
                {
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer_FakeCam(nFrame, filePath);
                    MainViewModel.Instance.SettingVM.SimulationThread_UpdateUI_FakeCam(nFrame);

                    MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.ResetImageExtBasic();
                    return;
                }

                InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer(nBuff, nFrame, filePath);
                MainViewModel.Instance.SettingVM.SimulationThread_UpdateUI(nBuff, nFrame);
                MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.ResetImageExtBasic();
            }
        }
    }
}
