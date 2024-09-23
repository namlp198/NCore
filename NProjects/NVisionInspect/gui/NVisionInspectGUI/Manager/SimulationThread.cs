using Microsoft.Win32;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Manager
{
    public class SimulationThread
    {
        private List<string> m_nImageListPath = new List<string>();
        private int m_nCurImageIndex;
        public List<string> ImageListPath { get => m_nImageListPath; }
        public int CurrentImageIndex { get => m_nCurImageIndex; set => m_nCurImageIndex = value; }
        public SimulationThread()
        {

        }

        public delegate void UpdateUIHandler(int nBuff, int nFrame);
        public static event UpdateUIHandler UpdateUI;
        public delegate void UpdateUIHandler_FakeCam(int nFrame);
        public static event UpdateUIHandler_FakeCam UpdateUI_FakeCam;
        public delegate void UpdateUIHandler_SumCameraView();
        public static event UpdateUIHandler_SumCameraView UpdateUI_SumCameraView;

        public void LoadImage(int nCamIdx, int nBuff, int nFrame)
        {
            Thread imgLoadThread;
            imgLoadThread = new Thread(() => LoadImageThread(nCamIdx, nBuff, nFrame));
            imgLoadThread.SetApartmentState(ApartmentState.STA);
            imgLoadThread.IsBackground = true;
            imgLoadThread.Start();
        }

        public void LoadImage_FakeCam(int nFrame)
        {
            Thread imgLoadThread;
            imgLoadThread = new Thread(() => LoadImageThread_FakeCam(nFrame));
            imgLoadThread.SetApartmentState(ApartmentState.STA);
            imgLoadThread.IsBackground = true;
            imgLoadThread.Start();
        }

        public void LoadAllImage(int nCamIdx, int nBuff, int nFrame)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image file(*.bmp, *.jpg, *.png, *.tif) | *.BMP;*.JPG;*.PNG;*.TIF;*.bmp;*.jpg;*.png;*.tif; |All Files(*.*)|*.*||";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() != true)
                return;

            if (ofd.CheckFileExists == false)
                return;

            m_nImageListPath.Clear();

            string[] arrFilePaths = ofd.FileNames;
            for (int i = 0; i < arrFilePaths.Length; i++)
            {
                m_nImageListPath.Add(arrFilePaths[i]);
            }

            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer(nBuff, nFrame, m_nImageListPath.ElementAt(0));
            m_nCurImageIndex = 0;
            UpdateUI?.Invoke(nBuff, nFrame);
        }
        public void LoadAllImage_FakeCam(int nFrame)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image file(*.bmp, *.jpg, *.png, *.tif) | *.BMP;*.JPG;*.PNG;*.TIF;*.bmp;*.jpg;*.png;*.tif; |All Files(*.*)|*.*||";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() != true)
                return;

            if (ofd.CheckFileExists == false)
                return;

            m_nImageListPath.Clear();

            string[] arrFilePaths = ofd.FileNames;
            for (int i = 0; i < arrFilePaths.Length; i++)
            {
                m_nImageListPath.Add(arrFilePaths[i]);
            }

            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer_FakeCam(nFrame, m_nImageListPath.ElementAt(0));
            m_nCurImageIndex = 0;
            UpdateUI_FakeCam?.Invoke(nFrame);
        }
        private void LoadImageThread(int nCamIdx, int nBuff, int nFrame)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image file(*.bmp, *.jpg, *.png, *.tif) | *.BMP;*.JPG;*.PNG;*.TIF;*.bmp;*.jpg;*.png;*.tif; |All Files(*.*)|*.*||";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != true)
                return;

            if (ofd.CheckFileExists == false)
                return;
            
            string filePath = ofd.FileName;

            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer(nBuff, nFrame, filePath);
            UpdateUI?.Invoke(nBuff, nFrame);
        }
        private void LoadImageThread_FakeCam(int nFrame)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image file(*.bmp, *.jpg, *.png, *.tif) | *.BMP;*.JPG;*.PNG;*.TIF;*.bmp;*.jpg;*.png;*.tif; |All Files(*.*)|*.*||";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != true)
                return;

            if (ofd.CheckFileExists == false)
                return;

            string filePath = ofd.FileName;

            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSimulatorBuffer_FakeCam(nFrame, filePath);
            UpdateUI_FakeCam?.Invoke(nFrame);
        }
    }
}
