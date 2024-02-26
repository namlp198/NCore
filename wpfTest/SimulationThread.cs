using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wfTestTaskProcessor
{
    internal class SimulationThread
    {
        public SimulationThread()
        {

        }

        public delegate void UpdateUIHandler(int nBuff);
        public static event UpdateUIHandler UpdateUI;

        private bool bInspectRuning;
        public bool IsInspectRuning
        {
            get { return bInspectRuning; }
            set { bInspectRuning = value; }
        }

        public InspectResult inspectData = new InspectResult();

        public static IntPtr pBuffer = IntPtr.Zero;

        public void LoadImage(int nBuff)
        {
            Thread imgLoadThread;
            imgLoadThread = new Thread(new ParameterizedThreadStart(LoadImageThread));
            imgLoadThread.SetApartmentState(ApartmentState.STA);
            imgLoadThread.IsBackground = true;
            imgLoadThread.Start(nBuff);
        }

        private void LoadImageThread(object objnBuff)
        {
            int nBuff = (int)objnBuff;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image file(*.bmp, *.jpg, *.png, *.tif) | *.BMP;*.JPG;*.PNG;*.TIF;*.bmp;*.jpg;*.png;*.tif; |All Files(*.*)|*.*||";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != true)
                return;

            if(ofd.CheckFileExists == false)
                return;

            string filePath = ofd.FileName;

            InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.LoadImageBuffer(nBuff, filePath);

            UpdateUI?.Invoke(nBuff);
        }

        public void FindLineTest(int nBuff)
        {
            Thread findLineThread;
            findLineThread = new Thread(() => FindLineTestThread(nBuff));
            findLineThread.SetApartmentState(ApartmentState.STA);
            findLineThread.IsBackground = true;
            findLineThread.Start();
        }

        private void FindLineTestThread(object objBuff)
        {
            int nBuff = (int)objBuff;
            
            InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.FindLineWithHoughLine_Simul(192, 506, 260, 100, nBuff);
            InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetInspectData(ref inspectData);
            //string s = string.Format("p1: {0}-{1} , p2: {2}-{3}", inspectData.m_nX1, inspectData.m_nY1, inspectData.m_nX2, inspectData.m_nY2);
            //MessageBox.Show(s);

            UpdateUI?.Invoke(nBuff);
        }
    }
}
