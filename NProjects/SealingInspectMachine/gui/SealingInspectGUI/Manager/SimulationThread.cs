using Microsoft.Win32;
using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SealingInspectGUI.Manager
{
    public class SimulationThread
    {
        public SimulationThread()
        {

        }

        public delegate void UpdateUIHandler(int nBuffIdx);
        public static event UpdateUIHandler UpdateUI;

        public void LoadImage(int nBuffIdx)
        {
            Thread imgLoadThread;
            imgLoadThread = new Thread(new ParameterizedThreadStart(LoadImageThread));
            imgLoadThread.SetApartmentState(ApartmentState.STA);
            imgLoadThread.IsBackground = true;
            imgLoadThread.Start(nBuffIdx);
        }

        private void LoadImageThread(object nBuffIdx)
        {
            int nBuff = (int)nBuffIdx;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image file(*.bmp, *.jpg, *.png, *.tif) | *.BMP;*.JPG;*.PNG;*.TIF;*.bmp;*.jpg;*.png;*.tif; |All Files(*.*)|*.*||";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != true)
                return;

            if (ofd.CheckFileExists == false)
                return;

            string filePath = ofd.FileName;

            if (MainViewModel.Instance.SettingVM.CameraSelected == ECameraList.TopCam1 ||
                MainViewModel.Instance.SettingVM.CameraSelected == ECameraList.TopCam2)
                InterfaceManager.Instance.m_sealingInspProcessor.LoadImageBuffer_TOP(nBuff, filePath);
            else if (MainViewModel.Instance.SettingVM.CameraSelected == ECameraList.SideCam1 ||
                MainViewModel.Instance.SettingVM.CameraSelected == ECameraList.SideCam2)
                InterfaceManager.Instance.m_sealingInspProcessor.LoadImageBuffer_SIDE(nBuff, filePath);

            UpdateUI?.Invoke(nBuff);
        }
    }
}
