
namespace NViewer
{
    partial class NBufferViewer
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picNViewer = new System.Windows.Forms.PictureBox();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.contextMenuStrip_ViewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_Measure = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Result = new System.Windows.Forms.ToolStripMenuItem();
            this.startendLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cornerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sideLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dimensionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.allDrawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allClearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeDefectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blackDefectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.whiteDefectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.picNViewer)).BeginInit();
            this.contextMenuStrip_ViewMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // picNViewer
            // 
            this.picNViewer.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.picNViewer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picNViewer.Location = new System.Drawing.Point(0, 0);
            this.picNViewer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picNViewer.Name = "picNViewer";
            this.picNViewer.Size = new System.Drawing.Size(141, 152);
            this.picNViewer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picNViewer.TabIndex = 0;
            this.picNViewer.TabStop = false;
            this.picNViewer.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_BufferView_Paint);
            this.picNViewer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_BufferView_MouseClick);
            this.picNViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_BufferView_MouseMove);
            this.picNViewer.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox_BufferView_MouseWheel);
            // 
            // hScrollBar
            // 
            this.hScrollBar.Location = new System.Drawing.Point(0, 156);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(141, 20);
            this.hScrollBar.TabIndex = 1;
            this.hScrollBar.ValueChanged += new System.EventHandler(this.hScrollBar_ValueChanged);
            this.hScrollBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.hScrollBar_MouseWheel);
            // 
            // vScrollBar
            // 
            this.vScrollBar.Location = new System.Drawing.Point(144, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(20, 152);
            this.vScrollBar.TabIndex = 2;
            this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBar_ValueChanged);
            this.vScrollBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.vScrollBar_MouseWheel);
            // 
            // contextMenuStrip_ViewMenu
            // 
            this.contextMenuStrip_ViewMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_ViewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Measure,
            this.toolStripMenuItem_Result,
            this.makeDefectToolStripMenuItem,
            this.fitViewToolStripMenuItem,
            this.zoomViewToolStripMenuItem});
            this.contextMenuStrip_ViewMenu.Name = "contextMenuStrip_ViewMenu";
            this.contextMenuStrip_ViewMenu.Size = new System.Drawing.Size(163, 124);
            // 
            // toolStripMenuItem_Measure
            // 
            this.toolStripMenuItem_Measure.Name = "toolStripMenuItem_Measure";
            this.toolStripMenuItem_Measure.Size = new System.Drawing.Size(162, 24);
            this.toolStripMenuItem_Measure.Text = "Measure";
            this.toolStripMenuItem_Measure.Click += new System.EventHandler(this.toolStripMenuItem_Measure_Click);
            // 
            // toolStripMenuItem_Result
            // 
            this.toolStripMenuItem_Result.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startendLineToolStripMenuItem,
            this.cornerToolStripMenuItem,
            this.sideLineToolStripMenuItem,
            this.alignMarkToolStripMenuItem,
            this.defectToolStripMenuItem,
            this.dimensionToolStripMenuItem,
            this.toolStripSeparator1,
            this.allDrawToolStripMenuItem,
            this.allClearToolStripMenuItem});
            this.toolStripMenuItem_Result.Name = "toolStripMenuItem_Result";
            this.toolStripMenuItem_Result.Size = new System.Drawing.Size(162, 24);
            this.toolStripMenuItem_Result.Text = "Result";
            // 
            // startendLineToolStripMenuItem
            // 
            this.startendLineToolStripMenuItem.Name = "startendLineToolStripMenuItem";
            this.startendLineToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.startendLineToolStripMenuItem.Text = "Start/End Line";
            this.startendLineToolStripMenuItem.Click += new System.EventHandler(this.startendLineToolStripMenuItem_Click);
            // 
            // cornerToolStripMenuItem
            // 
            this.cornerToolStripMenuItem.Name = "cornerToolStripMenuItem";
            this.cornerToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.cornerToolStripMenuItem.Text = "Corner";
            this.cornerToolStripMenuItem.Click += new System.EventHandler(this.cornerToolStripMenuItem_Click);
            // 
            // sideLineToolStripMenuItem
            // 
            this.sideLineToolStripMenuItem.Name = "sideLineToolStripMenuItem";
            this.sideLineToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.sideLineToolStripMenuItem.Text = "Side Line";
            this.sideLineToolStripMenuItem.Click += new System.EventHandler(this.sideLineToolStripMenuItem_Click);
            // 
            // alignMarkToolStripMenuItem
            // 
            this.alignMarkToolStripMenuItem.Name = "alignMarkToolStripMenuItem";
            this.alignMarkToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.alignMarkToolStripMenuItem.Text = "AlignMark";
            this.alignMarkToolStripMenuItem.Click += new System.EventHandler(this.alignMarkToolStripMenuItem_Click);
            // 
            // defectToolStripMenuItem
            // 
            this.defectToolStripMenuItem.Name = "defectToolStripMenuItem";
            this.defectToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.defectToolStripMenuItem.Text = "Defect";
            this.defectToolStripMenuItem.Click += new System.EventHandler(this.defectToolStripMenuItem_Click);
            // 
            // dimensionToolStripMenuItem
            // 
            this.dimensionToolStripMenuItem.Name = "dimensionToolStripMenuItem";
            this.dimensionToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.dimensionToolStripMenuItem.Text = "Dimension";
            this.dimensionToolStripMenuItem.Click += new System.EventHandler(this.dimensionToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(182, 6);
            // 
            // allDrawToolStripMenuItem
            // 
            this.allDrawToolStripMenuItem.Name = "allDrawToolStripMenuItem";
            this.allDrawToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.allDrawToolStripMenuItem.Text = "All Draw";
            this.allDrawToolStripMenuItem.Click += new System.EventHandler(this.allDrawToolStripMenuItem_Click);
            // 
            // allClearToolStripMenuItem
            // 
            this.allClearToolStripMenuItem.Name = "allClearToolStripMenuItem";
            this.allClearToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.allClearToolStripMenuItem.Text = "All Clear";
            this.allClearToolStripMenuItem.Click += new System.EventHandler(this.allClearToolStripMenuItem_Click);
            // 
            // makeDefectToolStripMenuItem
            // 
            this.makeDefectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blackDefectToolStripMenuItem,
            this.whiteDefectToolStripMenuItem});
            this.makeDefectToolStripMenuItem.Name = "makeDefectToolStripMenuItem";
            this.makeDefectToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
            this.makeDefectToolStripMenuItem.Text = "Make Defect";
            // 
            // blackDefectToolStripMenuItem
            // 
            this.blackDefectToolStripMenuItem.Name = "blackDefectToolStripMenuItem";
            this.blackDefectToolStripMenuItem.Size = new System.Drawing.Size(179, 26);
            this.blackDefectToolStripMenuItem.Text = "Black Defect";
            this.blackDefectToolStripMenuItem.Click += new System.EventHandler(this.drawBlackDefectToolStripMenuItem_Click);
            // 
            // whiteDefectToolStripMenuItem
            // 
            this.whiteDefectToolStripMenuItem.Name = "whiteDefectToolStripMenuItem";
            this.whiteDefectToolStripMenuItem.Size = new System.Drawing.Size(179, 26);
            this.whiteDefectToolStripMenuItem.Text = "White Defect";
            this.whiteDefectToolStripMenuItem.Click += new System.EventHandler(this.drawWhiteDefectToolStripMenuItem_Click);
            // 
            // fitViewToolStripMenuItem
            // 
            this.fitViewToolStripMenuItem.Name = "fitViewToolStripMenuItem";
            this.fitViewToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
            this.fitViewToolStripMenuItem.Text = "FitView";
            this.fitViewToolStripMenuItem.Click += new System.EventHandler(this.fitViewToolStripMenuItem_Click);
            // 
            // zoomViewToolStripMenuItem
            // 
            this.zoomViewToolStripMenuItem.Name = "zoomViewToolStripMenuItem";
            this.zoomViewToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
            this.zoomViewToolStripMenuItem.Text = "ZoomView";
            this.zoomViewToolStripMenuItem.Click += new System.EventHandler(this.zoomViewToolStripMenuItem_Click);
            // 
            // NBufferViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.hScrollBar);
            this.Controls.Add(this.picNViewer);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "NBufferViewer";
            this.Size = new System.Drawing.Size(168, 181);
            this.Resize += new System.EventHandler(this.EdgeBufferViewer_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picNViewer)).EndInit();
            this.contextMenuStrip_ViewMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picNViewer;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_ViewMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Measure;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Result;
        private System.Windows.Forms.ToolStripMenuItem startendLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sideLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cornerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem allDrawToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allClearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alignMarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dimensionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeDefectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blackDefectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem whiteDefectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomViewToolStripMenuItem;
    }
}
