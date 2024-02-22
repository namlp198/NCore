using NViewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wfTestTaskProcessor
{
    public partial class wfTestNViewer : Form
    {
        private int nBuff = 0;
        private int m_nCamIdx = 0;
        NBufferViewer nBufferViewer = new NBufferViewer(0);
        InspectResult inspectData = new InspectResult();
        public wfTestNViewer()
        {
            InitializeComponent();

            panel1.Controls.Add(nBufferViewer);
            nBufferViewer.Dock = DockStyle.Fill;


            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
        }

        private void SimulationThread_UpdateUI(int nBuff)
        {
            this.BeginInvoke(new Action(() =>
            {
                if (nBufferViewer.IsDrawLineTest)
                {
                    nBufferViewer.m_p1 = new Point(InterfaceManager.Instance.m_simulationThread.inspectData.m_nX1, InterfaceManager.Instance.m_simulationThread.inspectData.m_nY1);
                    nBufferViewer.m_p2 = new Point(InterfaceManager.Instance.m_simulationThread.inspectData.m_nX2, InterfaceManager.Instance.m_simulationThread.inspectData.m_nY2);
                }
                nBufferViewer.NpBuffer = InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetBufferImage(nBuff, 0);
                nBufferViewer.Refresh();
            }));
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            InterfaceManager.Instance.m_simulationThread.LoadImage(nBuff);
        }

        private void wfTestNViewer_Load(object sender, EventArgs e)
        {
            //InterfaceManager.Instance.m_imageProcessorManager.Initialize();
        }

        private void wfTestNViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Release();
        }

        private void btnFindLine_Click(object sender, EventArgs e)
        {
            nBufferViewer.IsDrawLineTest = true;
            InterfaceManager.Instance.m_simulationThread.FindLineTest(nBuff);
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Initialize();
            InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.LiveBaslerCam(m_nCamIdx);

            btnCameraLive.Enabled = true;
            btnFindLine.Enabled = true;
            btnLoadImage.Enabled = true;
            btnTrigger.Enabled = true;

            btnInit.Enabled = false;

            timer_Cam_Live.Enabled = true;
        }

        private void timer_Cam_Live_Tick(object sender, EventArgs e)
        {
            //nBufferViewer.NpBuffer = InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetHikCamBufferImage(m_nCamIdx);
            //nBufferViewer.SetViewIdx(m_nCamIdx);

            nBufferViewer.NpBuffer = InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetBaslerCamBufferImage(m_nCamIdx);
            nBufferViewer.SetViewIdx(m_nCamIdx);
        }

        private void btnCameraLive_Click(object sender, EventArgs e)
        {

        }

        private void btnLiveBaslerCam_Click(object sender, EventArgs e)
        {

        }
    }
}
