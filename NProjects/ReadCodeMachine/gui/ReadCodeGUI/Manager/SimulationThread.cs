﻿using Microsoft.Win32;
using Prism.Services.Dialogs;
using ReadCodeGUI.Commons;
using ReadCodeGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadCodeGUI.Manager
{
    public class SimulationThread
    {
        public SimulationThread()
        {

        }

        public delegate void UpdateUIHandler();
        public static event UpdateUIHandler UpdateUI;
        public delegate void UpdateUIHandler_SumCameraView();
        public static event UpdateUIHandler_SumCameraView UpdateUI_SumCameraView;

        public void LoadImage()
        {
            Thread imgLoadThread;
            imgLoadThread = new Thread(LoadImageThread);
            imgLoadThread.SetApartmentState(ApartmentState.STA);
            imgLoadThread.IsBackground = true;
            imgLoadThread.Start();
        }

        private void LoadImageThread()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image file(*.bmp, *.jpg, *.png, *.tif) | *.BMP;*.JPG;*.PNG;*.TIF;*.bmp;*.jpg;*.png;*.tif; |All Files(*.*)|*.*||";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != true)
                return;

            if (ofd.CheckFileExists == false)
                return;

            string filePath = ofd.FileName;

            InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.LoadSimulatorBuffer(0, 0, filePath);
            UpdateUI?.Invoke();
        }
    }
}
