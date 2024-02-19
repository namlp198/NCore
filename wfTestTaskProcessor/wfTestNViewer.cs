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
        NBufferViewer nBufferViewer = new NBufferViewer(0);
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
            InterfaceManager.Instance.m_imageProcessorManager.Initialize();
        }

        private void wfTestNViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Release();
        }
    }
}
