using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            ofd.Filter = "Image file(*.bmp, *.jpg, *.png) | *.BMP;*.JPG;*.PNG;*.bmp;*.jpg;*.png; |All Files(*.*)|*.*||";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            if(ofd.CheckFileExists == false)
                return;

            string filePath = ofd.FileName;

            InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.LoadImageBuffer(nBuff, filePath);

            UpdateUI?.Invoke(nBuff);
        }
    }
}
