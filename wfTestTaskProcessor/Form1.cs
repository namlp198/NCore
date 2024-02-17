using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace wfTestTaskProcessor
{
    public partial class Form1 : Form
    {
        private int nBuff = 0;
        private int nX = 0;
        private int nY = 0;

        private int m_nFrameWidth = 1280;
        private int m_nFrameHeight = 1024;
        private int m_nFrameCount = 1;

        private double m_dZoomRatio = 1.0;
        private double m_dZoomMax = 16.0;
        private double m_dZoomMin = 0.125;

        public Form1()
        {
            InitializeComponent();

            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
        }

        private void SimulationThread_UpdateUI(int nBuff)
        {
            this.nBuff = nBuff;
            this.Invoke((MethodInvoker)delegate
            {
                picBuffer1.Refresh();
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Initialize();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Release();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                nBuff = 0;
                InterfaceManager.Instance.m_simulationThread.LoadImage(nBuff);
            }
            if (checkBox2.Checked == true)
            {
                nBuff = 1;
                InterfaceManager.Instance.m_simulationThread.LoadImage(nBuff);
            }
        }

        private void picBuffer1_Paint(object sender, PaintEventArgs e)
        {
            BufferedGraphics bufferedGraphics = BufferedGraphicsManager.Current.Allocate(e.Graphics, picBuffer1.ClientRectangle);

            bufferedGraphics.Graphics.Clear(Color.Black);
            bufferedGraphics.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            DrawImage(sender, bufferedGraphics);
            bufferedGraphics.Render(e.Graphics);
        }

        private void DrawImage(object sender, BufferedGraphics e)
        {
            int nViewWidth = picBuffer1.Width;
            int nViewHeight = picBuffer1.Height;

            int nViewRealWidth = (int)(nViewWidth / m_dZoomRatio);
            int nViewRealHeight = (int)(nViewHeight / m_dZoomRatio);

            nViewRealWidth = ((int)(nViewRealWidth + 3) / 4) * 4;

            IntPtr pBuffer = InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetBufferImage(nBuff, nY);

            if (pBuffer == IntPtr.Zero)
                return;

            Bitmap canvas = new Bitmap(nViewRealWidth, nViewRealHeight, PixelFormat.Format8bppIndexed);
            BitmapData canvasData = canvas.LockBits(new Rectangle(0, 0, nViewRealWidth, nViewRealHeight), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            //Bitmap canvas = new Bitmap(m_nFrameWidth, m_nFrameHeight, PixelFormat.Format8bppIndexed);
            //BitmapData canvasData = canvas.LockBits(new Rectangle(0, 0, m_nFrameWidth, m_nFrameHeight), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            unsafe
            {
                int nBuffer_TotalY = m_nFrameHeight * m_nFrameCount;
                int nCopyWidth = (m_nFrameWidth < picBuffer1.Width) ? m_nFrameWidth : picBuffer1.Width;
                //int nCopyWidth = m_nFrameWidth;

                IntPtr Ptr = (IntPtr)canvasData.Scan0.ToPointer();

                for (int i = 0; i < nViewRealHeight; i++)
                {
                    if (nBuffer_TotalY <= (nY + i))
                        continue;

                    if (nX < 0)
                        nX = 0;

                    try
                    {
                        ImageProcessorDll.CopyMemory((byte*)(Ptr + (i * nViewRealWidth)), (byte*)(pBuffer + (i * m_nFrameWidth + nX)), nCopyWidth);
                    }
                    catch
                    {

                    }
                }

                canvas.UnlockBits(canvasData);
                ImageProcessorDll.SetGrayscalePalette(canvas);
            }

            Bitmap pImageBMP = canvas;
            //this.picBuffer1.Image = pImageBMP;

            Rectangle rectView = new Rectangle(0, 0, nViewWidth, nViewHeight);
            e.Graphics.DrawImage(pImageBMP, rectView, 0, 0, nViewRealWidth, nViewRealHeight, GraphicsUnit.Pixel);
        }
    }
}
