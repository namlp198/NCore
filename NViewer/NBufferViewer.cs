using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace NViewer
{
    public partial class NBufferViewer : UserControl
    {
        int m_nCurrentViewPositionX;
        int m_nCurrentViewPositionY;

        string m_strViewTitle;
        int m_nViewIdx;

        Point m_ptMousePos;

        bool m_bMeasureStart_New;
        bool m_bMeasure_Move_StartPos;
        Point m_ptMeasureStartPos;
        bool m_bMeasure_Move_EndPos;
        Point m_ptMeasureEndPos;
        const int m_nPointSelectRange = 5;

        private double m_dZoomRatio = 0.5;
        const double m_dZoomMax = 16.0;
        const double m_dZoomMin = 0.125;//0.25;

        // Scale : Fit View Mode
        private double m_dFitViewRatio_X = 1.0;
        private double m_dFitViewRatio_Y = 1.0;

        //private int m_nFrameWidth = 640;
        //private int m_nFrameHeight = 486;
        private int m_nFrameWidth = 2464;
        private int m_nFrameHeight = 2056;
        private double m_dPixelSizeX_um = 10.0;
        private double m_dPixelSizeY_um = 10.0;

        // Cam Mode
        public bool m_bCamLive = true;
        public int m_nAlignPosIdx = -1;

        // Pan image mode
        //private const bool m_bPan = true;
        //private int startPanX = 0;
        //private int startPanY = 0;
        //private int imgX = 0;
        //private int imgY = 0;


        public double ZoomRatio
        {
            get { return m_dZoomRatio; }
            set { m_dZoomRatio = value; }
        }

        private IntPtr m_pBufferView = IntPtr.Zero;
        public IntPtr NpBuffer
        {
            get
            {
                return m_pBufferView;
            }
            set { m_pBufferView = value; }
        }

        public void SetViewIdx(int nViewIdx)
        {
            m_nViewIdx = nViewIdx;
            ReDraw();
        }

        public double RadianToDegree(double dRadian)
        {
            double dPI = 3.1415926535897932384626433832795;
            return ((dRadian * 180.0) / dPI);
        }
        public double DegreeToRadian(double dDegree)
        {
            double dPI = 3.1415926535897932384626433832795;
            return ((dDegree * dPI) / 180.0);
        }

        // bool m_bUserMeasure = false;

        public NBufferViewer(int nViewIdx)
        {
            InitializeComponent();

            picNViewer.ContextMenuStrip = contextMenuStrip_ViewMenu;

            fitViewToolStripMenuItem.Checked = false;
            zoomViewToolStripMenuItem.Checked = true;

            startendLineToolStripMenuItem.Checked = true;
            sideLineToolStripMenuItem.Checked = true;
            alignMarkToolStripMenuItem.Checked = true;
            defectToolStripMenuItem.Checked = true;
            cornerToolStripMenuItem.Checked = true;
            dimensionToolStripMenuItem.Checked = true;
            blackDefectToolStripMenuItem.Checked = false;
            whiteDefectToolStripMenuItem.Checked = false;

            m_nCurrentViewPositionX = -1;
            m_nCurrentViewPositionY = -1;

            m_ptMousePos.X = -1;
            m_ptMousePos.Y = -1;

            m_strViewTitle = string.Format("Cam {0}", nViewIdx + 1);
            m_nViewIdx = nViewIdx;

            m_bMeasureStart_New = false;
            m_bMeasure_Move_StartPos = false;
            m_bMeasure_Move_EndPos = false;
            m_ptMeasureStartPos.X = -1;
            m_ptMeasureStartPos.Y = -1;
            m_ptMeasureEndPos.X = -1;
            m_ptMeasureEndPos.Y = -1;

            //m_nFrameWidth = InterfaceManager.Instance.m_SettingManager.m_VisionSetting.m_nAlignCamera_1_FrameWidth;
            //m_nFrameHeight = InterfaceManager.Instance.m_SettingManager.m_VisionSetting.m_nAlignCamera_1_FrameHeight;
            //m_dPixelSizeX_um = InterfaceManager.Instance.m_SettingManager.m_VisionSetting.m_dAlignCamera_1_PixelSizeX;
            //m_dPixelSizeY_um = InterfaceManager.Instance.m_SettingManager.m_VisionSetting.m_dAlignCamera_1_PixelSizeY;

        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            //if (e.Delta > 0)
            //{
            //    ZoomOut();
            //}
            //else if (e.Delta < 0)
            //{
            //    ZoomIn();
            //}
        }

        public void ZoomIn()
        {
            if (m_dZoomMax < m_dZoomRatio * 2.0)
                return;

            m_dZoomRatio = m_dZoomRatio * 2.0;
            //m_dZoomRatio = Math.Max(m_dZoomRatio - 0.1d, 0.01d);

            SetScrollRange();

            // ReDraw();
        }

        public void ZoomOut()
        {
            if (m_dZoomRatio / 2.0 < m_dZoomMin)
                return;

            m_dZoomRatio = m_dZoomRatio / 2.0;
            //m_dZoomRatio += 0.1d;

            SetScrollRange();

            // ReDraw();
        }

        public void ReDraw()
        {
            m_nCurrentViewPositionX = -1;
            m_nCurrentViewPositionY = -1;

            Refresh();
        }

        private void SetScrollRange()
        {
            int nViewWidth = picNViewer.Width;
            int nViewHeight = picNViewer.Height;

            nViewWidth = (int)(nViewWidth / m_dZoomRatio);
            nViewHeight = (int)(nViewHeight / m_dZoomRatio);

            nViewWidth = ((int)(nViewWidth + 3) / 4) * 4;

            hScrollBar.Maximum = m_nFrameWidth - nViewWidth;

            if (hScrollBar.Maximum < 0)
            {
                hScrollBar.Hide();
                hScrollBar.Maximum = 1;
                SetHorizontalScrollPos(0);
            }
            else hScrollBar.Show();

            vScrollBar.Maximum = m_nFrameHeight - nViewHeight;

            if (vScrollBar.Maximum < 0)
            {
                vScrollBar.Hide();
                vScrollBar.Maximum = 1;
                SetVerticalScrollPos(0);
            }
            else vScrollBar.Show();

            ReDraw();
        }

        public int GetVerticalScrollRange()
        {
            return vScrollBar.Maximum;
        }

        public void ShowVerticalScroll(bool bShow)
        {
            if (bShow == true)
                vScrollBar.Show();
            else
                vScrollBar.Hide();
        }

        public void SetVerticalScrollPos(int nPos)
        {
            if (nPos < 0)
                return;

            if (vScrollBar.Minimum >= nPos)
                return;

            if (vScrollBar.Maximum <= nPos)
                return;

            vScrollBar.Value = nPos;

            //ReDraw();
        }

        public int GetVerticalScrollPos()
        {
            return vScrollBar.Value;
        }

        public void SetHorizontalScrollPos(int nPos)
        {
            if (nPos < 0)
                return;

            if (hScrollBar.Minimum >= nPos)
                return;

            if (hScrollBar.Maximum <= nPos)
                return;

            hScrollBar.Value = nPos;

            // ReDraw();
        }

        public int GetHorizontalScrollPos()
        {
            return hScrollBar.Value;
        }

        private void EdgeBufferViewer_Resize(object sender, EventArgs e)
        {
            int nWndWidth = this.Size.Width;
            int nWndHeight = this.Size.Height;

            this.picNViewer.Location = new System.Drawing.Point(0, 0);
            this.picNViewer.Size = new System.Drawing.Size(nWndWidth - 20, nWndHeight - 20);

            this.hScrollBar.Location = new System.Drawing.Point(0, nWndHeight - 20);
            this.hScrollBar.Size = new System.Drawing.Size(nWndWidth - 20, 20);

            this.vScrollBar.Location = new System.Drawing.Point(nWndWidth - 20, 0);
            this.vScrollBar.Size = new System.Drawing.Size(20, nWndHeight - 20);

            SetScrollRange();
        }

        public void pictureBox_BufferView_Paint(object sender, PaintEventArgs e)
        {
            BufferedGraphics bufferedgraphic = BufferedGraphicsManager.Current.Allocate(e.Graphics, picNViewer.ClientRectangle);

            bufferedgraphic.Graphics.Clear(Color.Black);
            bufferedgraphic.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //bufferedgraphic.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int nX = hScrollBar.Value;
            int nY = vScrollBar.Value;

            //if (nX == m_nCurrentViewPositionX && nY == m_nCurrentViewPositionY)
            //    return;

            if (zoomViewToolStripMenuItem.Checked == true)
            {
                m_nCurrentViewPositionX = nX;
                m_nCurrentViewPositionY = nY;
            }
            else
            {
                m_nCurrentViewPositionX = 0;
                m_nCurrentViewPositionY = 0;
                m_dFitViewRatio_X = ((double)m_nFrameWidth) / ((double)this.picNViewer.Size.Width);
                m_dFitViewRatio_Y = ((double)m_nFrameHeight) / ((double)this.picNViewer.Size.Height);
            }



            if (fitViewToolStripMenuItem.Checked == false)
            {
                ScrollBarHide(false);
                DrawImage_ZoomMode(sender, bufferedgraphic);
                DrawMouseInfo_ZoomView(sender, bufferedgraphic);
                //DrawFindAlignMark_ZoomView(sender, bufferedgraphic);

                DrawZoomInfo(sender, bufferedgraphic);

                // Draw Measure
                if (toolStripMenuItem_Measure.Checked == true)
                    DrawMouseMeasure_ZoomView(sender, bufferedgraphic);

                DrawCenterLine_ZoomView(sender, bufferedgraphic);
            }
            else
            {
                ScrollBarHide(true);
                DrawImage_FitMode(sender, bufferedgraphic);
                DrawMouseInfo_FitView(sender, bufferedgraphic);
                //DrawFindAlignMark_FitView(sender, bufferedgraphic);

                // Draw Measure
                if (toolStripMenuItem_Measure.Checked == true)
                    DrawMouseMeasure_FitView(sender, bufferedgraphic);

                DrawCenterLine_FitView(sender, bufferedgraphic);
            }

            //DrawAlignResult(sender, bufferedgraphic);
            DrawTitle(sender, bufferedgraphic);

            bufferedgraphic.Render(e.Graphics);
        }

        private void DrawImage_ZoomMode(object sender, BufferedGraphics e)
        {
            int nX = m_nCurrentViewPositionX;
            int nY = m_nCurrentViewPositionY;

            int nViewWidth = picNViewer.Width;
            int nViewHeight = picNViewer.Height;

            int nViewRealWidth = (int)(nViewWidth / m_dZoomRatio);
            int nViewRealHeight = (int)(nViewHeight / m_dZoomRatio);

            nViewRealWidth = ((int)(nViewRealWidth + 3) / 4) * 4;

            int size = nViewRealWidth * nViewRealHeight;

            if (NpBuffer == IntPtr.Zero)
                return;

            Bitmap Canvas = new Bitmap(nViewRealWidth, nViewRealHeight, PixelFormat.Format8bppIndexed);
            BitmapData CanvasData = Canvas.LockBits(new Rectangle(0, 0, nViewRealWidth, nViewRealHeight), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            unsafe
            {
                int nBuffer_TotalY = m_nFrameHeight;
                int nCopyWidth = (m_nFrameWidth < nViewRealWidth) ? m_nFrameWidth : nViewRealWidth;

                IntPtr Ptr = (IntPtr)CanvasData.Scan0.ToPointer();

                for (int i = 0; i < nViewRealHeight; i++)
                {
                    if ((i + nY) < 0 || nBuffer_TotalY <= (i + nY))
                        continue;

                    if (nX < 0)
                        nX = 0;

                    CopyMemory((byte*)(Ptr + (i * nViewRealWidth)), (byte*)(NpBuffer + ((i + nY) * m_nFrameWidth + nX)), nCopyWidth);
                }
                //Marshal.Copy(pBuffer + (i * nFrameWidth), nX, Ptr+(i * nViewWidth), nViewWidth);

                Canvas.UnlockBits(CanvasData);

                SetGrayscalePalette(Canvas);
            }

            Bitmap pImageBMP = Canvas;


            //this.pictureBox_BufferView.Image = pImageBMP;
            Rectangle rtView = new Rectangle(0, 0, nViewWidth, nViewHeight);
            e.Graphics.DrawImage(pImageBMP, rtView, 0, 0, nViewRealWidth, nViewRealHeight, GraphicsUnit.Pixel);
        }

        private void DrawImage_FitMode(object sender, BufferedGraphics e)
        {
            int nViewWidth = picNViewer.Width;
            int nViewHeight = picNViewer.Height;

            int nViewRealWidth = m_nFrameWidth; // (int)(nViewWidth / m_dZoomRatio);
            int nViewRealHeight = m_nFrameHeight; // (int)(nViewHeight / m_dZoomRatio);

            // nViewRealWidth = ((int)(nViewRealWidth + 3) / 4) * 4;

            int size = nViewRealWidth * nViewRealHeight;

            if (NpBuffer == IntPtr.Zero)
                return;

            Bitmap Canvas = new Bitmap(nViewRealWidth, nViewRealHeight, PixelFormat.Format8bppIndexed);
            BitmapData CanvasData = Canvas.LockBits(new Rectangle(0, 0, nViewRealWidth, nViewRealHeight), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            unsafe
            {
                int nBuffer_TotalY = m_nFrameHeight;
                int nCopyWidth = (m_nFrameWidth < nViewRealWidth) ? m_nFrameWidth : nViewRealWidth;

                IntPtr Ptr = (IntPtr)CanvasData.Scan0.ToPointer();

                for (int i = 0; i < nViewRealHeight; i++)
                {
                    if (nBuffer_TotalY <= i)
                        continue;

                    CopyMemory((byte*)(Ptr + (i * nViewRealWidth)), (byte*)(NpBuffer + (i * m_nFrameWidth)), nCopyWidth);
                }
                //Marshal.Copy(pBuffer + (i * nFrameWidth), nX, Ptr+(i * nViewWidth), nViewWidth);

                Canvas.UnlockBits(CanvasData);

                SetGrayscalePalette(Canvas);
            }

            Bitmap pImageBMP = Canvas;


            //this.picNViewer.Image = pImageBMP;
            Rectangle rtView = new Rectangle(0, 0, nViewWidth, nViewHeight);
            e.Graphics.DrawImage(pImageBMP, rtView, 0, 0, nViewRealWidth, nViewRealHeight, GraphicsUnit.Pixel);
        }

        //private void DrawFindAlignMark_ZoomView(object sender, BufferedGraphics e)
        //{
        //    Pen AlignAreaPen = new Pen(Color.SkyBlue, 1);
        //    SolidBrush AlignMarkInfoBrush = new SolidBrush(Color.SkyBlue);
        //    Font AlignMarkInfoFont = new Font("Arial", 10, FontStyle.Bold);

        //    string strTemp;

        //    for (int i = 0; i < ConstDefine.MAX_ALIGN_CAMERA_COUNT; i++)
        //    {
        //        int nStatus = m_ResultData.m_nMarkFind_Status[i];

        //        if (nStatus == 0)
        //            continue;

        //        if (m_nAlignPosIdx != -1 && m_nAlignPosIdx != i)
        //            continue;

        //        double dFindX_pxl = m_ResultData.m_dMarkFind_Result_X[i];
        //        double dFindY_pxl = m_ResultData.m_dMarkFind_Result_Y[i];
        //        double dMatchingRate = m_ResultData.m_dMarkFind_Matching_Rate[i];

        //        float fFindX_wnd = (float)((dFindX_pxl - (double) m_nCurrentViewPositionX) * m_dZoomRatio);
        //        float fFindY_wnd = (float)((dFindY_pxl - (double) m_nCurrentViewPositionY) * m_dZoomRatio);

        //        e.Graphics.DrawLine(AlignAreaPen, new PointF(fFindX_wnd - 30, fFindY_wnd), new PointF(fFindX_wnd + 30, fFindY_wnd));
        //        e.Graphics.DrawLine(AlignAreaPen, new PointF(fFindX_wnd, fFindY_wnd - 30), new PointF(fFindX_wnd, fFindY_wnd + 30));

        //        strTemp = string.Format("Pos X : {0} pxl", dFindX_pxl);
        //        // DrawOutlineText(e.Graphics, new Point(rtDraw.Right, rtDraw.Bottom), Color.Black, 1, Color.Pink, new FontFamily("Arial"), FontStyle.Bold, 11, strTemp);
        //        e.Graphics.DrawString(strTemp, AlignMarkInfoFont, AlignMarkInfoBrush, fFindX_wnd + 10, fFindY_wnd + 10);

        //        strTemp = string.Format("Pos Y : {0} pxl", dFindY_pxl);
        //        // DrawOutlineText(e.Graphics, new Point(rtDraw.Right, rtDraw.Bottom + 15), Color.Black, 1, Color.Pink, new FontFamily("Arial"), FontStyle.Bold, 11, strTemp);
        //        e.Graphics.DrawString(strTemp, AlignMarkInfoFont, AlignMarkInfoBrush, fFindX_wnd + 10, fFindY_wnd + 25);

        //        strTemp = string.Format("Rate : {0:0.00} %", dMatchingRate);
        //        // DrawOutlineText(e.Graphics, new Point(rtDraw.Right, rtDraw.Bottom + 30), Color.Black, 1, Color.Pink, new FontFamily("Arial"), FontStyle.Bold, 11, strTemp);
        //        e.Graphics.DrawString(strTemp, AlignMarkInfoFont, AlignMarkInfoBrush, fFindX_wnd + 10, fFindY_wnd + 40);
        //    }

        //    AlignAreaPen.Dispose();
        //    AlignMarkInfoBrush.Dispose();
        //    AlignMarkInfoFont.Dispose();
        //}

        //private void DrawFindAlignMark_FitView(object sender, BufferedGraphics e)
        //{
        //    Pen AlignAreaPen = new Pen(Color.SkyBlue, 1);
        //    SolidBrush AlignMarkInfoBrush = new SolidBrush(Color.SkyBlue);
        //    Font AlignMarkInfoFont = new Font("Arial", 10, FontStyle.Bold);

        //    string strTemp;

        //    for (int i=0; i<ConstDefine.MAX_ALIGN_CAMERA_COUNT; i++)
        //    {
        //        int nStatus = m_ResultData.m_nMarkFind_Status[i];

        //        if (nStatus == 0)
        //            continue;

        //        if (m_nAlignPosIdx != -1 && m_nAlignPosIdx != i)
        //            continue;

        //        double dFindX_pxl = m_ResultData.m_dMarkFind_Result_X[i];
        //        double dFindY_pxl = m_ResultData.m_dMarkFind_Result_Y[i];
        //        double dMatchingRate = m_ResultData.m_dMarkFind_Matching_Rate[i];

        //        float fFindX_wnd = (float) (dFindX_pxl / m_dFitViewRatio_X);
        //        float fFindY_wnd = (float) (dFindY_pxl / m_dFitViewRatio_Y);

        //        e.Graphics.DrawLine(AlignAreaPen, new PointF(fFindX_wnd - 30, fFindY_wnd), new PointF(fFindX_wnd + 30, fFindY_wnd));
        //        e.Graphics.DrawLine(AlignAreaPen, new PointF(fFindX_wnd, fFindY_wnd - 30), new PointF(fFindX_wnd, fFindY_wnd + 30));

        //        strTemp = string.Format("Pos X : {0} pxl", dFindX_pxl);
        //        // DrawOutlineText(e.Graphics, new Point(rtDraw.Right, rtDraw.Bottom), Color.Black, 1, Color.Pink, new FontFamily("Arial"), FontStyle.Bold, 11, strTemp);
        //        e.Graphics.DrawString(strTemp, AlignMarkInfoFont, AlignMarkInfoBrush, fFindX_wnd + 10, fFindY_wnd + 10);

        //        strTemp = string.Format("Pos Y : {0} pxl", dFindY_pxl);
        //        // DrawOutlineText(e.Graphics, new Point(rtDraw.Right, rtDraw.Bottom + 15), Color.Black, 1, Color.Pink, new FontFamily("Arial"), FontStyle.Bold, 11, strTemp);
        //        e.Graphics.DrawString(strTemp, AlignMarkInfoFont, AlignMarkInfoBrush, fFindX_wnd + 10, fFindY_wnd + 25);

        //        strTemp = string.Format("Rate : {0:0.00} %", dMatchingRate);
        //        // DrawOutlineText(e.Graphics, new Point(rtDraw.Right, rtDraw.Bottom + 30), Color.Black, 1, Color.Pink, new FontFamily("Arial"), FontStyle.Bold, 11, strTemp);
        //        e.Graphics.DrawString(strTemp, AlignMarkInfoFont, AlignMarkInfoBrush, fFindX_wnd + 10, fFindY_wnd + 40);
        //    }

        //    AlignAreaPen.Dispose();
        //    AlignMarkInfoBrush.Dispose();
        //    AlignMarkInfoFont.Dispose();
        //}

        private void DrawCenterLine_FitView(object sender, BufferedGraphics e)
        {
            Pen drawLinePen = new Pen(Color.Red, 1);

            float fWidth_wnd = picNViewer.Width;
            float fHeight_wnd = picNViewer.Height;
            float fWidth_half_wnd = fWidth_wnd / (float)2.0;
            float fHeight_half_wnd = fHeight_wnd / (float)2.0;

            e.Graphics.DrawLine(drawLinePen, new PointF(fWidth_half_wnd, 0), new PointF(fWidth_half_wnd, fHeight_wnd));
            e.Graphics.DrawLine(drawLinePen, new PointF(0, fHeight_half_wnd), new PointF(fWidth_wnd, fHeight_half_wnd));

            drawLinePen.Dispose();
        }

        private void DrawCenterLine_ZoomView(object sender, BufferedGraphics e)
        {
            Pen drawLinePen = new Pen(Color.Red, 1);

            float fFrameWidth = (float)m_nFrameWidth;
            float fFrameHeight = (float)m_nFrameHeight;
            float fFrameWidth_Half = (float)m_nFrameWidth / (float)2.0;
            float fFrameHeight_Half = (float)m_nFrameHeight / (float)2.0;

            float fFrameWidth_wnd = (float)((fFrameWidth - (float)m_nCurrentViewPositionX) * m_dZoomRatio);
            float fFrameHeight_wnd = (float)((fFrameHeight - (float)m_nCurrentViewPositionY) * m_dZoomRatio);
            float fFrameWidth_Half_wnd = (float)((fFrameWidth_Half - (float)m_nCurrentViewPositionX) * m_dZoomRatio);
            float fFrameHeight_Half_wnd = (float)((fFrameHeight_Half - (float)m_nCurrentViewPositionY) * m_dZoomRatio);

            e.Graphics.DrawLine(drawLinePen, new PointF(fFrameWidth_Half_wnd, 0), new PointF(fFrameWidth_Half_wnd, fFrameHeight_wnd));
            e.Graphics.DrawLine(drawLinePen, new PointF(0, fFrameHeight_Half_wnd), new PointF(fFrameWidth_wnd, fFrameHeight_Half_wnd));

            drawLinePen.Dispose();
        }

        private void DrawTitle(object sender, BufferedGraphics e)
        {
            SolidBrush TitleBrush = new SolidBrush(Color.Orange);

            Font TitleFont = new Font("Arial", 10, FontStyle.Bold);

            string strTemp = string.Format("[{0}]", m_strViewTitle);

            // DrawOutlineText(e.Graphics, new Point(10, 10), Color.Black, 1, Color.Orange, new FontFamily("Arial"), FontStyle.Bold, 11, strTemp);
            e.Graphics.DrawString(strTemp, TitleFont, TitleBrush, 10, 10);

            TitleBrush.Dispose();
            TitleFont.Dispose();
        }

        //private void DrawAlignResult(object sender, BufferedGraphics e)
        //{
        //    SolidBrush TitleBrush = new SolidBrush(Color.Yellow);

        //    Font TitleFont = new Font("Arial", 11, FontStyle.Bold);

        //    string strTemp = string.Format("Mark1 [{0:0},{1:0}] Mark2 [{2:0},{3:0}] Result [X:{4:0.000}mm Y:{5:0.000}mm T:{6:0.000}deg]", m_ResultData.m_dMarkFind_Result_X[0], m_ResultData.m_dMarkFind_Result_Y[0]
        //                                                                                        , m_ResultData.m_dMarkFind_Result_X[1], m_ResultData.m_dMarkFind_Result_Y[1]
        //                                                                                          , m_ResultData.m_Result_X_um/1000.0, m_ResultData.m_Result_Y_um/1000.0, m_ResultData.m_Result_T_degree);

        //    // DrawOutlineText(e.Graphics, new Point(10, 10), Color.Black, 1, Color.Orange, new FontFamily("Arial"), FontStyle.Bold, 11, strTemp);
        //    e.Graphics.DrawString(strTemp, TitleFont, TitleBrush, 220, 10);

        //    TitleBrush.Dispose();
        //    TitleFont.Dispose();
        //}

        private void DrawMouseInfo_ZoomView(object sender, BufferedGraphics e)
        {
            SolidBrush MouseInfoBrush = new SolidBrush(Color.Orange);

            Font MouseInfoFont = new Font("Arial", 10, FontStyle.Bold);

            Point ptMouseReal = m_ptMousePos;

            ptMouseReal.X = (int)(ptMouseReal.X / m_dZoomRatio);
            ptMouseReal.Y = (int)(ptMouseReal.Y / m_dZoomRatio);

            ptMouseReal.Offset(m_nCurrentViewPositionX, m_nCurrentViewPositionY);

            if (ptMouseReal.X < 0 || m_nFrameWidth <= ptMouseReal.X)
                return;

            if (ptMouseReal.Y < 0 || m_nFrameHeight <= ptMouseReal.Y)
                return;

            if (NpBuffer == IntPtr.Zero)
                return;

            Byte[] grayValue = new Byte[1];

            Marshal.Copy(NpBuffer + ptMouseReal.X, grayValue, 0, 1);

            string strTemp = string.Format("[X:{0} Y:{1} Gray:{2}]", ptMouseReal.X, ptMouseReal.Y, (int)grayValue[0]);

            // DrawOutlineText(e.Graphics, new Point(10, pictureBox_BufferView.Height - 20), Color.Black, 1, Color.Orange, new FontFamily("Arial"), FontStyle.Bold, 11, strTemp);
            e.Graphics.DrawString(strTemp, MouseInfoFont, MouseInfoBrush, 10, picNViewer.Height - 20);

            MouseInfoBrush.Dispose();
            MouseInfoFont.Dispose();
        }

        private void DrawMouseInfo_FitView(object sender, BufferedGraphics e)
        {
            SolidBrush MouseInfoBrush = new SolidBrush(Color.Orange);

            Font MouseInfoFont = new Font("Arial", 10, FontStyle.Bold);

            Point ptMouseReal = m_ptMousePos;

            ptMouseReal.X = (int)(ptMouseReal.X * m_dFitViewRatio_X);
            ptMouseReal.Y = (int)(ptMouseReal.Y * m_dFitViewRatio_Y);

            if (ptMouseReal.X < 0 || m_nFrameWidth <= ptMouseReal.X)
                return;

            if (ptMouseReal.Y < 0 || m_nFrameHeight <= ptMouseReal.Y)
                return;

            if (NpBuffer == IntPtr.Zero)
                return;

            Byte[] grayValue = new Byte[1];

            Marshal.Copy(NpBuffer + ptMouseReal.X, grayValue, 0, 1);

            string strTemp = string.Format("[X:{0} Y:{1} Gray:{2}]", ptMouseReal.X, ptMouseReal.Y, (int)grayValue[0]);

            // DrawOutlineText(e.Graphics, new Point(10, pictureBox_BufferView.Height - 20), Color.Black, 1, Color.Orange, new FontFamily("Arial"), FontStyle.Bold, 11, strTemp);
            e.Graphics.DrawString(strTemp, MouseInfoFont, MouseInfoBrush, 10, picNViewer.Height - 20);

            MouseInfoBrush.Dispose();
            MouseInfoFont.Dispose();
        }

        private void DrawMouseMeasure_ZoomView(object sender, BufferedGraphics e)
        {
            if (m_ptMeasureStartPos.X == -1 || m_ptMeasureStartPos.Y == -1)
                return;

            bool bDiectionX = (0 <= (m_ptMeasureStartPos.X - m_ptMeasureEndPos.X)) ? true : false;

            int nWidthPxl = Math.Abs(m_ptMeasureStartPos.X - m_ptMeasureEndPos.X);
            int nHeightPxl = Math.Abs(m_ptMeasureStartPos.Y - m_ptMeasureEndPos.Y);

            double dWidth = nWidthPxl * m_dPixelSizeX_um;
            double dHeight = nHeightPxl * m_dPixelSizeY_um;
            double dDiagonal = Math.Sqrt((dWidth * dWidth) + (dHeight * dHeight));

            double dAngle_degree = RadianToDegree(Math.Atan2(dWidth, dHeight)) * ((bDiectionX) ? 1.0 : -1.0);

            SolidBrush MouseMeasureBrush = new SolidBrush(Color.Orange);

            Pen MouseMeasurePen;
            if (m_bMeasureStart_New == true || m_bMeasure_Move_StartPos == true || m_bMeasure_Move_EndPos == true) MouseMeasurePen = new Pen(Color.Red, 1);
            else MouseMeasurePen = new Pen(Color.Cyan, 1);

            Pen MouseMeasurePen_Point = new Pen(Color.Yellow, 1);

            Font MouseMeasureFont = new Font("Arial", 10, FontStyle.Bold);

            Point ptMeasureStart = m_ptMeasureStartPos;
            Point ptMeasureEnd = m_ptMeasureEndPos;

            ptMeasureStart.Offset(-m_nCurrentViewPositionX, -m_nCurrentViewPositionY);
            ptMeasureStart.X = (int)(ptMeasureStart.X * m_dZoomRatio);
            ptMeasureStart.Y = (int)(ptMeasureStart.Y * m_dZoomRatio);

            ptMeasureEnd.Offset(-m_nCurrentViewPositionX, -m_nCurrentViewPositionY);
            ptMeasureEnd.X = (int)(ptMeasureEnd.X * m_dZoomRatio);
            ptMeasureEnd.Y = (int)(ptMeasureEnd.Y * m_dZoomRatio);

            // Diagonal
            e.Graphics.DrawLine(MouseMeasurePen, ptMeasureStart, ptMeasureEnd);

            // Start Point Line
            e.Graphics.DrawLine(MouseMeasurePen, new Point(ptMeasureStart.X, 0), new Point(ptMeasureStart.X, picNViewer.Size.Height));
            e.Graphics.DrawLine(MouseMeasurePen, new Point(0, ptMeasureStart.Y), new Point(picNViewer.Size.Width, ptMeasureStart.Y));

            // Start Point Cross Line
            e.Graphics.DrawLine(MouseMeasurePen_Point, new Point(ptMeasureStart.X - m_nPointSelectRange, ptMeasureStart.Y - m_nPointSelectRange), new Point(ptMeasureStart.X + m_nPointSelectRange, ptMeasureStart.Y + m_nPointSelectRange));
            e.Graphics.DrawLine(MouseMeasurePen_Point, new Point(ptMeasureStart.X + m_nPointSelectRange, ptMeasureStart.Y - m_nPointSelectRange), new Point(ptMeasureStart.X - m_nPointSelectRange, ptMeasureStart.Y + m_nPointSelectRange));

            // End Point Line
            e.Graphics.DrawLine(MouseMeasurePen, new Point(ptMeasureEnd.X, 0), new Point(ptMeasureEnd.X, picNViewer.Size.Height));
            e.Graphics.DrawLine(MouseMeasurePen, new Point(0, ptMeasureEnd.Y), new Point(picNViewer.Size.Width, ptMeasureEnd.Y));

            // End Point Cross Line
            e.Graphics.DrawLine(MouseMeasurePen_Point, new Point(ptMeasureEnd.X - m_nPointSelectRange, ptMeasureEnd.Y - m_nPointSelectRange), new Point(ptMeasureEnd.X + m_nPointSelectRange, ptMeasureEnd.Y + m_nPointSelectRange));
            e.Graphics.DrawLine(MouseMeasurePen_Point, new Point(ptMeasureEnd.X + m_nPointSelectRange, ptMeasureEnd.Y - m_nPointSelectRange), new Point(ptMeasureEnd.X - m_nPointSelectRange, ptMeasureEnd.Y + m_nPointSelectRange));

            // Center Line
            Rectangle rtDrawRect = new Rectangle(ptMeasureStart.X, ptMeasureStart.Y, ptMeasureEnd.X - ptMeasureStart.X, ptMeasureEnd.Y - ptMeasureStart.Y);
            int nCenterX = (ptMeasureStart.X + ptMeasureEnd.X) / 2;
            int nCenterY = (ptMeasureStart.Y + ptMeasureEnd.Y) / 2;

            e.Graphics.DrawLine(MouseMeasurePen, new Point(nCenterX, ptMeasureStart.Y), new Point(nCenterX, ptMeasureEnd.Y));
            e.Graphics.DrawLine(MouseMeasurePen, new Point(ptMeasureStart.X, nCenterY), new Point(ptMeasureEnd.X, nCenterY));


            string strTemp = string.Format("Width : {0:f3}um {1}pixel\nHeight : {2:f3}um {3}pixel\nDiagonal : {4:f3}um\nAngle : {5:f3}deg.", dWidth, nWidthPxl, dHeight, nHeightPxl, dDiagonal, dAngle_degree);

            // DrawOutlineText(e.Graphics, new Point(m_ptMousePos.X + 10, m_ptMousePos.Y + 10), Color.Black, 2, Color.Orange, new FontFamily("Arial"), FontStyle.Bold, 13, strTemp);
            e.Graphics.DrawString(strTemp, MouseMeasureFont, MouseMeasureBrush, m_ptMousePos.X + 10, m_ptMousePos.Y + 10);

            MouseMeasureBrush.Dispose();
            MouseMeasureFont.Dispose();
            MouseMeasurePen.Dispose();
            MouseMeasurePen_Point.Dispose();
        }

        private void DrawMouseMeasure_FitView(object sender, BufferedGraphics e)
        {
            if (m_ptMeasureStartPos.X == -1 || m_ptMeasureStartPos.Y == -1)
                return;

            bool bDiectionX = (0 <= (m_ptMeasureStartPos.X - m_ptMeasureEndPos.X)) ? true : false;

            int nWidthPxl = Math.Abs(m_ptMeasureStartPos.X - m_ptMeasureEndPos.X);
            int nHeightPxl = Math.Abs(m_ptMeasureStartPos.Y - m_ptMeasureEndPos.Y);

            double dWidth = nWidthPxl * m_dPixelSizeX_um;
            double dHeight = nHeightPxl * m_dPixelSizeY_um;
            double dDiagonal = Math.Sqrt((dWidth * dWidth) + (dHeight * dHeight));

            double dAngle_degree = RadianToDegree(Math.Atan2(dWidth, dHeight)) * ((bDiectionX) ? 1.0 : -1.0);

            SolidBrush MouseMeasureBrush = new SolidBrush(Color.Orange);

            Pen MouseMeasurePen;
            if (m_bMeasureStart_New == true || m_bMeasure_Move_StartPos == true || m_bMeasure_Move_EndPos == true) MouseMeasurePen = new Pen(Color.Red, 1);
            else MouseMeasurePen = new Pen(Color.Cyan, 1);

            Pen MouseMeasurePen_Point = new Pen(Color.Yellow, 1);

            Font MouseMeasureFont = new Font("Arial", 10, FontStyle.Bold);

            Point ptMeasureStart = m_ptMeasureStartPos;
            Point ptMeasureEnd = m_ptMeasureEndPos;

            ptMeasureStart.X = (int)(ptMeasureStart.X / m_dFitViewRatio_X);
            ptMeasureStart.Y = (int)(ptMeasureStart.Y / m_dFitViewRatio_Y);

            ptMeasureEnd.X = (int)(ptMeasureEnd.X / m_dFitViewRatio_X);
            ptMeasureEnd.Y = (int)(ptMeasureEnd.Y / m_dFitViewRatio_Y);

            // Diagonal
            e.Graphics.DrawLine(MouseMeasurePen, ptMeasureStart, ptMeasureEnd);

            // Start Point Line
            e.Graphics.DrawLine(MouseMeasurePen, new Point(ptMeasureStart.X, 0), new Point(ptMeasureStart.X, picNViewer.Size.Height));
            e.Graphics.DrawLine(MouseMeasurePen, new Point(0, ptMeasureStart.Y), new Point(picNViewer.Size.Width, ptMeasureStart.Y));

            // Start Point Cross Line
            e.Graphics.DrawLine(MouseMeasurePen_Point, new Point(ptMeasureStart.X - m_nPointSelectRange, ptMeasureStart.Y - m_nPointSelectRange), new Point(ptMeasureStart.X + m_nPointSelectRange, ptMeasureStart.Y + m_nPointSelectRange));
            e.Graphics.DrawLine(MouseMeasurePen_Point, new Point(ptMeasureStart.X + m_nPointSelectRange, ptMeasureStart.Y - m_nPointSelectRange), new Point(ptMeasureStart.X - m_nPointSelectRange, ptMeasureStart.Y + m_nPointSelectRange));

            // End Point Line
            e.Graphics.DrawLine(MouseMeasurePen, new Point(ptMeasureEnd.X, 0), new Point(ptMeasureEnd.X, picNViewer.Size.Height));
            e.Graphics.DrawLine(MouseMeasurePen, new Point(0, ptMeasureEnd.Y), new Point(picNViewer.Size.Width, ptMeasureEnd.Y));

            // End Point Cross Line
            e.Graphics.DrawLine(MouseMeasurePen_Point, new Point(ptMeasureEnd.X - m_nPointSelectRange, ptMeasureEnd.Y - m_nPointSelectRange), new Point(ptMeasureEnd.X + m_nPointSelectRange, ptMeasureEnd.Y + m_nPointSelectRange));
            e.Graphics.DrawLine(MouseMeasurePen_Point, new Point(ptMeasureEnd.X + m_nPointSelectRange, ptMeasureEnd.Y - m_nPointSelectRange), new Point(ptMeasureEnd.X - m_nPointSelectRange, ptMeasureEnd.Y + m_nPointSelectRange));

            // Center Line
            Rectangle rtDrawRect = new Rectangle(ptMeasureStart.X, ptMeasureStart.Y, ptMeasureEnd.X - ptMeasureStart.X, ptMeasureEnd.Y - ptMeasureStart.Y);
            int nCenterX = (ptMeasureStart.X + ptMeasureEnd.X) / 2;
            int nCenterY = (ptMeasureStart.Y + ptMeasureEnd.Y) / 2;

            e.Graphics.DrawLine(MouseMeasurePen, new Point(nCenterX, ptMeasureStart.Y), new Point(nCenterX, ptMeasureEnd.Y));
            e.Graphics.DrawLine(MouseMeasurePen, new Point(ptMeasureStart.X, nCenterY), new Point(ptMeasureEnd.X, nCenterY));


            string strTemp = string.Format("Width : {0:f3}um {1}pixel\nHeight : {2:f3}um {3}pixel\nDiagonal : {4:f3}um\nAngle : {5:f3}deg.", dWidth, nWidthPxl, dHeight, nHeightPxl, dDiagonal, dAngle_degree);

            // DrawOutlineText(e.Graphics, new Point(m_ptMousePos.X + 10, m_ptMousePos.Y + 10), Color.Black, 2, Color.Orange, new FontFamily("Arial"), FontStyle.Bold, 13, strTemp);
            e.Graphics.DrawString(strTemp, MouseMeasureFont, MouseMeasureBrush, m_ptMousePos.X + 10, m_ptMousePos.Y + 10);

            MouseMeasureBrush.Dispose();
            MouseMeasureFont.Dispose();
            MouseMeasurePen.Dispose();
            MouseMeasurePen_Point.Dispose();
        }

        private void DrawZoomInfo(object sender, BufferedGraphics e)
        {
            SolidBrush ZoomInfoBrush = new SolidBrush(Color.Orange);

            Font ZoomInfoFont = new Font("Arial", 10, FontStyle.Bold);

            string strTemp = string.Format("[Zoom X{0:0.000}]", m_dZoomRatio.ToString());

            int nPosition = picNViewer.Width - 100;

            // DrawOutlineText(e.Graphics, new Point(nPosition, 10), Color.Black, 1, Color.Orange, new FontFamily("Arial"), FontStyle.Bold, 11, strTemp);
            e.Graphics.DrawString(strTemp, ZoomInfoFont, ZoomInfoBrush, nPosition, 10);

            ZoomInfoBrush.Dispose();
            ZoomInfoFont.Dispose();
        }

        private void pictureBox_BufferView_MouseMove(object sender, MouseEventArgs e)
        {
            //if (!m_bPan)
            //{
            m_ptMousePos.X = e.X;
            m_ptMousePos.Y = e.Y;

            if (zoomViewToolStripMenuItem.Checked == true)
            {
                if (m_bMeasureStart_New == true || m_bMeasure_Move_StartPos == true || m_bMeasure_Move_EndPos == true)
                {
                    int nX = e.X;
                    int nY = e.Y;

                    if (m_bMeasure_Move_StartPos == true)
                        m_ptMeasureStartPos = new Point((int)(m_nCurrentViewPositionX + (nX / m_dZoomRatio)), (int)(m_nCurrentViewPositionY + (nY / m_dZoomRatio)));

                    if (m_bMeasure_Move_EndPos == true)
                        m_ptMeasureEndPos = new Point((int)(m_nCurrentViewPositionX + (nX / m_dZoomRatio)), (int)(m_nCurrentViewPositionY + (nY / m_dZoomRatio)));

                    if (m_bMeasureStart_New == true)
                        m_ptMeasureEndPos = new Point((int)(m_nCurrentViewPositionX + (nX / m_dZoomRatio)), (int)(m_nCurrentViewPositionY + (nY / m_dZoomRatio)));
                }
            }
            else
            {
                if (m_bMeasureStart_New == true || m_bMeasure_Move_StartPos == true || m_bMeasure_Move_EndPos == true)
                {
                    int nX = e.X;
                    int nY = e.Y;

                    if (m_bMeasure_Move_StartPos == true)
                        m_ptMeasureStartPos = new Point((int)(nX * m_dFitViewRatio_X), (int)(nY * m_dFitViewRatio_Y));

                    if (m_bMeasure_Move_EndPos == true)
                        m_ptMeasureEndPos = new Point((int)(nX * m_dFitViewRatio_X), (int)(nY * m_dFitViewRatio_Y));

                    if (m_bMeasureStart_New == true)
                        m_ptMeasureEndPos = new Point((int)(nX * m_dFitViewRatio_X), (int)(nY * m_dFitViewRatio_Y));
                }
            }
            //}
            //else
            //{
            //    MouseEventArgs mouse = e as MouseEventArgs;
            //    if (mouse.Button == MouseButtons.Left)
            //    {
            //        Point mousePosNow = mouse.Location;
            //        int deltaX = mousePosNow.X - m_ptMousePos.X;
            //        int deltaY = mousePosNow.Y - m_ptMousePos.Y;

            //        imgX = (int)(startPanX + (deltaX / m_dZoomRatio));
            //        imgY = (int)(startPanY + (deltaY / m_dZoomRatio));
            //    }
            //}

            ReDraw();
        }

        private void pictureBox_BufferView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (fitViewToolStripMenuItem.Checked == true)
                return;

            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                int nX = e.X;
                int nY = e.Y;

                Point ptMousePos_pxl = new Point((int)(m_nCurrentViewPositionX + (nX / m_dZoomRatio)), (int)(m_nCurrentViewPositionY + (nY / m_dZoomRatio)));

                if (e.Delta < 0)
                    ZoomIn();
                else
                    ZoomOut();

                int nView_x = (int)(nX / m_dZoomRatio);
                int nView_y = (int)(nY / m_dZoomRatio);

                // Mouse 위치가 Zoom 되도록..
                SetHorizontalScrollPos(ptMousePos_pxl.X - nView_x);
                SetVerticalScrollPos(ptMousePos_pxl.Y - nView_y);
            }
            else
            {
                const int nMovePxl = 128;       // 16배 줌까지 하니깐 16보다는 크게

                int nMoveStep = (int)(nMovePxl / m_dZoomRatio);

                if (e.Delta < 0)
                    SetVerticalScrollPos(m_nCurrentViewPositionY + nMoveStep);
                else
                    SetVerticalScrollPos(m_nCurrentViewPositionY - nMoveStep);
            }

            ReDraw();
        }

        private void hScrollBar_ValueChanged(object sender, EventArgs e)
        {
            this.picNViewer.Invalidate();
        }

        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            this.picNViewer.Invalidate();
        }

        private void hScrollBar_MouseWheel(object sender, MouseEventArgs e)
        {
            const int nMovePxl = 128;       // 16배 줌까지 하니깐 16보다는 크게

            int nMoveStep = (int)(nMovePxl / m_dZoomRatio);

            if (e.Delta < 0)
                SetHorizontalScrollPos(m_nCurrentViewPositionX + nMoveStep);
            else
                SetHorizontalScrollPos(m_nCurrentViewPositionX - nMoveStep);

            ReDraw();
        }

        private void vScrollBar_MouseWheel(object sender, MouseEventArgs e)
        {
            const int nMovePxl = 128;       // 16배 줌까지 하니깐 16보다는 크게

            int nMoveStep = (int)(nMovePxl / m_dZoomRatio);

            if (e.Delta < 0)
                SetVerticalScrollPos(m_nCurrentViewPositionY + nMoveStep);
            else
                SetVerticalScrollPos(m_nCurrentViewPositionY - nMoveStep);

            ReDraw();
        }

        private void pictureBox_BufferView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                MouseLeftClickEvent(sender, e);
        }

        private void MouseLeftClickEvent(object sender, MouseEventArgs e)
        {
            if (zoomViewToolStripMenuItem.Checked == true)
                MouseLeftClickEvent_Measure_ZoomView(sender, e);
            else
                MouseLeftClickEvent_Measure_FitView(sender, e);
        }

        private void MouseLeftClickEvent_Measure_ZoomView(object sender, MouseEventArgs e)
        {
            bool bChecked = toolStripMenuItem_Measure.Checked;

            if (bChecked != true)
                return;

            int nX = e.X;
            int nY = e.Y;

            Point ptMousePos_pxl = new Point((int)(m_nCurrentViewPositionX + (nX / m_dZoomRatio)), (int)(m_nCurrentViewPositionY + (nY / m_dZoomRatio)));
            int nPointSelectRange_pxl = (int)(m_nPointSelectRange / m_dZoomRatio);
            nPointSelectRange_pxl = (nPointSelectRange_pxl == 0) ? 3 : nPointSelectRange_pxl;

            if (m_bMeasureStart_New == false)
            {
                if (m_bMeasure_Move_StartPos == false)
                {
                    Rectangle rtStartPosArea_pxl = new Rectangle(m_ptMeasureStartPos.X - nPointSelectRange_pxl, m_ptMeasureStartPos.Y - nPointSelectRange_pxl, nPointSelectRange_pxl * 2, nPointSelectRange_pxl * 2);

                    if (rtStartPosArea_pxl.Contains(ptMousePos_pxl))
                    {
                        m_ptMeasureStartPos = ptMousePos_pxl;
                        m_bMeasure_Move_StartPos = true;
                        m_bMeasure_Move_EndPos = false;
                        return;
                    }
                }
                else
                {
                    m_ptMeasureStartPos = ptMousePos_pxl;
                    m_bMeasure_Move_StartPos = false;
                    m_bMeasure_Move_EndPos = false;
                    return;
                }
            }

            if (m_bMeasureStart_New == false)
            {
                if (m_bMeasure_Move_EndPos == false)
                {
                    Rectangle rtEndPosArea_pxl = new Rectangle(m_ptMeasureEndPos.X - nPointSelectRange_pxl, m_ptMeasureEndPos.Y - nPointSelectRange_pxl, nPointSelectRange_pxl * 2, nPointSelectRange_pxl * 2);

                    if (rtEndPosArea_pxl.Contains(ptMousePos_pxl))
                    {
                        m_ptMeasureEndPos = ptMousePos_pxl;
                        m_bMeasure_Move_StartPos = false;
                        m_bMeasure_Move_EndPos = true;
                        return;
                    }
                }
                else
                {
                    m_ptMeasureEndPos = ptMousePos_pxl;
                    m_bMeasure_Move_StartPos = false;
                    m_bMeasure_Move_EndPos = false;
                    return;
                }
            }

            if (m_bMeasureStart_New == false)
            {
                m_ptMeasureStartPos = new Point((int)(m_nCurrentViewPositionX + (nX / m_dZoomRatio)), (int)(m_nCurrentViewPositionY + (nY / m_dZoomRatio)));
                m_bMeasureStart_New = true;
                m_bMeasure_Move_StartPos = false;
                m_bMeasure_Move_EndPos = false;
            }
            else
            {
                m_ptMeasureEndPos = new Point((int)(m_nCurrentViewPositionX + (nX / m_dZoomRatio)), (int)(m_nCurrentViewPositionY + (nY / m_dZoomRatio)));
                m_bMeasureStart_New = false;
                m_bMeasure_Move_StartPos = false;
                m_bMeasure_Move_EndPos = false;
            }
        }

        private void MouseLeftClickEvent_Measure_FitView(object sender, MouseEventArgs e)
        {
            bool bChecked = toolStripMenuItem_Measure.Checked;

            if (bChecked != true)
                return;

            int nX = e.X;
            int nY = e.Y;

            Point ptMousePos_pxl = new Point((int)(nX * m_dFitViewRatio_X), (int)(nY * m_dFitViewRatio_Y));
            int nPointSelectRange_pxl = (int)(m_nPointSelectRange * m_dFitViewRatio_Y);
            nPointSelectRange_pxl = (nPointSelectRange_pxl == 0) ? 3 : nPointSelectRange_pxl;

            if (m_bMeasureStart_New == false)
            {
                if (m_bMeasure_Move_StartPos == false)
                {
                    Rectangle rtStartPosArea_pxl = new Rectangle(m_ptMeasureStartPos.X - nPointSelectRange_pxl, m_ptMeasureStartPos.Y - nPointSelectRange_pxl, nPointSelectRange_pxl * 2, nPointSelectRange_pxl * 2);

                    if (rtStartPosArea_pxl.Contains(ptMousePos_pxl))
                    {
                        m_ptMeasureStartPos = ptMousePos_pxl;
                        m_bMeasure_Move_StartPos = true;
                        m_bMeasure_Move_EndPos = false;
                        return;
                    }
                }
                else
                {
                    m_ptMeasureStartPos = ptMousePos_pxl;
                    m_bMeasure_Move_StartPos = false;
                    m_bMeasure_Move_EndPos = false;
                    return;
                }
            }

            if (m_bMeasureStart_New == false)
            {
                if (m_bMeasure_Move_EndPos == false)
                {
                    Rectangle rtEndPosArea_pxl = new Rectangle(m_ptMeasureEndPos.X - nPointSelectRange_pxl, m_ptMeasureEndPos.Y - nPointSelectRange_pxl, nPointSelectRange_pxl * 2, nPointSelectRange_pxl * 2);

                    if (rtEndPosArea_pxl.Contains(ptMousePos_pxl))
                    {
                        m_ptMeasureEndPos = ptMousePos_pxl;
                        m_bMeasure_Move_StartPos = false;
                        m_bMeasure_Move_EndPos = true;
                        return;
                    }
                }
                else
                {
                    m_ptMeasureEndPos = ptMousePos_pxl;
                    m_bMeasure_Move_StartPos = false;
                    m_bMeasure_Move_EndPos = false;
                    return;
                }
            }

            if (m_bMeasureStart_New == false)
            {
                m_ptMeasureStartPos = new Point((int)(nX * m_dFitViewRatio_X), (int)(nY * m_dFitViewRatio_Y));
                m_bMeasureStart_New = true;
                m_bMeasure_Move_StartPos = false;
                m_bMeasure_Move_EndPos = false;
            }
            else
            {
                m_ptMeasureEndPos = new Point((int)(nX * m_dFitViewRatio_X), (int)(nY * m_dFitViewRatio_Y));
                m_bMeasureStart_New = false;
                m_bMeasure_Move_StartPos = false;
                m_bMeasure_Move_EndPos = false;
            }
        }

        private void toolStripMenuItem_Measure_Click(object sender, EventArgs e)
        {
            bool bChecked = toolStripMenuItem_Measure.Checked;

            if (bChecked == true)
                toolStripMenuItem_Measure.Checked = false;
            else
            {
                toolStripMenuItem_Measure.Checked = true;
                blackDefectToolStripMenuItem.Checked = false;
                whiteDefectToolStripMenuItem.Checked = false;

                int nX = m_ptMousePos.X;
                int nY = m_ptMousePos.Y;
                m_ptMeasureStartPos = new Point((int)(m_nCurrentViewPositionX + (nX / m_dZoomRatio)), (int)(m_nCurrentViewPositionY + (nY / m_dZoomRatio)));
                m_ptMeasureEndPos = new Point((int)(m_nCurrentViewPositionX + (nX / m_dZoomRatio)), (int)(m_nCurrentViewPositionY + (nY / m_dZoomRatio)));
                m_bMeasureStart_New = true;
            }

            Refresh();
        }

        private void startendLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bChecked = startendLineToolStripMenuItem.Checked;

            if (bChecked == true)
                startendLineToolStripMenuItem.Checked = false;
            else
                startendLineToolStripMenuItem.Checked = true;

            Refresh();
        }

        private void sideLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bChecked = sideLineToolStripMenuItem.Checked;

            if (bChecked == true)
                sideLineToolStripMenuItem.Checked = false;
            else
                sideLineToolStripMenuItem.Checked = true;

            Refresh();
        }

        private void defectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bChecked = defectToolStripMenuItem.Checked;

            if (bChecked == true)
                defectToolStripMenuItem.Checked = false;
            else
                defectToolStripMenuItem.Checked = true;

            Refresh();
        }

        private void cornerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bChecked = cornerToolStripMenuItem.Checked;

            if (bChecked == true)
                cornerToolStripMenuItem.Checked = false;
            else
                cornerToolStripMenuItem.Checked = true;

            Refresh();
        }

        private void dimensionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bChecked = dimensionToolStripMenuItem.Checked;

            if (bChecked == true)
                dimensionToolStripMenuItem.Checked = false;
            else
                dimensionToolStripMenuItem.Checked = true;

            Refresh();
        }

        private void allDrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startendLineToolStripMenuItem.Checked = true;
            sideLineToolStripMenuItem.Checked = true;
            defectToolStripMenuItem.Checked = true;
            cornerToolStripMenuItem.Checked = true;
            alignMarkToolStripMenuItem.Checked = true;
            dimensionToolStripMenuItem.Checked = true;

            Refresh();
        }

        private void allClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startendLineToolStripMenuItem.Checked = false;
            sideLineToolStripMenuItem.Checked = false;
            defectToolStripMenuItem.Checked = false;
            cornerToolStripMenuItem.Checked = false;
            alignMarkToolStripMenuItem.Checked = false;
            dimensionToolStripMenuItem.Checked = false;

            Refresh();
        }

        private void alignMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bChecked = alignMarkToolStripMenuItem.Checked;

            if (bChecked == true)
                alignMarkToolStripMenuItem.Checked = false;
            else
                alignMarkToolStripMenuItem.Checked = true;

            Refresh();
        }

        public Rectangle GetUserMeasureRect()
        {
            int nWidthPxl = Math.Abs(m_ptMeasureStartPos.X - m_ptMeasureEndPos.X);
            int nHeightPxl = Math.Abs(m_ptMeasureStartPos.Y - m_ptMeasureEndPos.Y);

            int nLeft = (m_ptMeasureStartPos.X < m_ptMeasureEndPos.X) ? m_ptMeasureStartPos.X : m_ptMeasureEndPos.X;
            int nTop = (m_ptMeasureStartPos.Y < m_ptMeasureEndPos.Y) ? m_ptMeasureStartPos.Y : m_ptMeasureEndPos.Y;

            return new Rectangle(nLeft, nTop, nWidthPxl, nHeightPxl);
        }

        private void drawBlackDefectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bChecked = blackDefectToolStripMenuItem.Checked;

            if (bChecked == true)
                blackDefectToolStripMenuItem.Checked = false;
            else
            {
                blackDefectToolStripMenuItem.Checked = true;
                whiteDefectToolStripMenuItem.Checked = false;
                toolStripMenuItem_Measure.Checked = false;
            }

            Refresh();
        }

        private void drawWhiteDefectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bChecked = whiteDefectToolStripMenuItem.Checked;

            if (bChecked == true)
                whiteDefectToolStripMenuItem.Checked = false;
            else
            {
                whiteDefectToolStripMenuItem.Checked = true;
                blackDefectToolStripMenuItem.Checked = false;
                toolStripMenuItem_Measure.Checked = false;
            }

            Refresh();
        }

        #region 외곽선 텍스트 그리기 - DrawOutlineText(graphics, location, borderColor, borderWidth, fillColor, fontFamily, fontStyle, fontSize, text)

        /// <summary>
        /// 텍스트 그리기
        /// </summary>
        /// <param name="graphics">그래픽스 객체</param>
        /// <param name="location">위치</param>
        /// <param name="borderColor">테두리 색상</param>
        /// <param name="borderWidth">테두리 두께</param>
        /// <param name="fillColor">칠하기 색상</param>
        /// <param name="fontFamily">폰트 패밀리</param>
        /// <param name="fontStyle">폰트 스타일</param>
        /// <param name="fontSize">폰트 크기</param>
        /// <param name="text">텍스트</param>
        public void DrawOutlineText(Graphics graphics, Point location, Color borderColor, int borderWidth, Color fillColor, FontFamily fontFamily, FontStyle fontStyle, float fontSize, string text)
        {
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                using (GraphicsPath graphicsPath = new GraphicsPath())
                {
                    using (Brush fillBrush = new SolidBrush(fillColor))
                    {
                        graphicsPath.AddString(text, fontFamily, (int)fontStyle, fontSize, location, StringFormat.GenericTypographic);

                        graphics.DrawPath(pen, graphicsPath);

                        graphics.FillPath(fillBrush, graphicsPath);
                    }
                }
            }
        }

        #endregion

        private void fitViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fitViewToolStripMenuItem.Checked = true;
            zoomViewToolStripMenuItem.Checked = false;

            Refresh();
        }

        private void zoomViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fitViewToolStripMenuItem.Checked = false;
            zoomViewToolStripMenuItem.Checked = true;

            Refresh();
        }

        private void ScrollBarHide(bool bHide)
        {
            if (bHide == true)
            {
                hScrollBar.Hide();
                vScrollBar.Hide();
            }
            else
            {
                hScrollBar.Show();
                vScrollBar.Show();
            }
        }

        // KERNEL FUNCTIONS
        [DllImport("kernel32.dll")]
        public extern static unsafe void ZeroMemory(void* Destination, int Length);
        [DllImport("kernel32.dll")]
        public extern static unsafe void CopyMemory(void* Destination, void* Source, int Length);

        public static void SetGrayscalePalette(Bitmap Image)
        {
            ColorPalette GrayscalePalette = Image.Palette;

            for (int i = 0; i < 256; i++)
                GrayscalePalette.Entries[i] = Color.FromArgb(i, i, i);
            Image.Palette = GrayscalePalette;
        }

        //private void picNViewer_MouseDown(object sender, MouseEventArgs e)
        //{
        //    MouseEventArgs mouse = e as MouseEventArgs;
        //    if (mouse.Button == MouseButtons.Left)
        //    {
        //        if (m_bPan)
        //        {
        //            m_ptMousePos = mouse.Location;
        //            startPanX = imgX;
        //            startPanY = imgY;
        //        }
        //    }
        //}

    }
}
