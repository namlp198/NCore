using Microsoft.Win32;
using Prism.Services.Dialogs;
using SealingInspectGUI.Commons;
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
        public delegate void UpdateUIHandler_SumCameraView(int nBuffIdx, string posCam);
        public static event UpdateUIHandler_SumCameraView UpdateUI_SumCameraView;

        public void LoadImage(int nBuffIdx)
        {
            Thread imgLoadThread;
            imgLoadThread = new Thread(new ParameterizedThreadStart(LoadImageThread));
            imgLoadThread.SetApartmentState(ApartmentState.STA);
            imgLoadThread.IsBackground = true;
            imgLoadThread.Start(nBuffIdx);
        }
        public void LoadAllImage()
        {
            Thread imgLoadThread;
            imgLoadThread = new Thread(LoadAllImageThread);
            imgLoadThread.SetApartmentState(ApartmentState.STA);
            imgLoadThread.IsBackground = true;
            imgLoadThread.Start();
        }
        public void LoadAllImageThread()
        {
            OpenFileDialog fileOpenDlg = new OpenFileDialog();

            fileOpenDlg.Filter = "Image File(*.bmp, *.jpg, *.png) | *.BMP;*.JPG;*.PNG;*.bmp;*.jpg;*.png; |All Files(*.*)|*.*||";
            fileOpenDlg.Multiselect = false;

            if (fileOpenDlg.ShowDialog() != true)
                return;

            if (fileOpenDlg.CheckFileExists == false)
                return;

            string strFilePath = fileOpenDlg.FileName;

            string strDirPath = strFilePath.Substring(0, strFilePath.LastIndexOf('\\'));
            string strFileName = fileOpenDlg.SafeFileName;
            string strExt = strFileName.Substring(strFileName.IndexOf(".") + 1);

            InterfaceManager.Instance.m_sealingInspProcessor.LoadAllImageBuffer(strDirPath, strExt);

            for(int topIdx = 0; topIdx < Defines.MAX_IMAGE_BUFFER_TOP; topIdx++)
            {
                UpdateUI_SumCameraView?.Invoke(topIdx, "Top");
            }
            for (int sideIdx = 0; sideIdx < Defines.MAX_IMAGE_BUFFER_SIDE; sideIdx++)
            {
                UpdateUI_SumCameraView?.Invoke(sideIdx, "Side");
            }
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
