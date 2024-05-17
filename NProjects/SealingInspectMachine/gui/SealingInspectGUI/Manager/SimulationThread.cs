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

        public delegate void UpdateUIHandler(int nBuff);
        public static event UpdateUIHandler UpdateUI;

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

            if (ofd.CheckFileExists == false)
                return;

            string filePath = ofd.FileName;

            if (MainViewModel.Instance.SettingVM.UseColor)
                InterfaceManager.Instance.m_sealingInspProcessor.LoadImageBuffer_Color(nBuff, filePath);
            else
                InterfaceManager.Instance.m_sealingInspProcessor.LoadImageBuffer_Mono(nBuff, filePath);

            UpdateUI?.Invoke(nBuff);
        }
    }
}
