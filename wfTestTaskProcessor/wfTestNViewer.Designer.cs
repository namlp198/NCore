namespace wfTestTaskProcessor
{
    partial class wfTestNViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnLiveBaslerCam = new System.Windows.Forms.Button();
            this.btnInit = new System.Windows.Forms.Button();
            this.btnFindLine = new System.Windows.Forms.Button();
            this.btnTrigger = new System.Windows.Forms.Button();
            this.btnCameraLive = new System.Windows.Forms.Button();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.timer_Cam_Live = new System.Windows.Forms.Timer(this.components);
            this.btnSingleGrab = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(953, 630);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(710, 626);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSingleGrab);
            this.panel2.Controls.Add(this.btnLiveBaslerCam);
            this.panel2.Controls.Add(this.btnInit);
            this.panel2.Controls.Add(this.btnFindLine);
            this.panel2.Controls.Add(this.btnTrigger);
            this.panel2.Controls.Add(this.btnCameraLive);
            this.panel2.Controls.Add(this.btnLoadImage);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(716, 2);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(235, 626);
            this.panel2.TabIndex = 1;
            // 
            // btnLiveBaslerCam
            // 
            this.btnLiveBaslerCam.Enabled = false;
            this.btnLiveBaslerCam.Location = new System.Drawing.Point(28, 391);
            this.btnLiveBaslerCam.Margin = new System.Windows.Forms.Padding(2);
            this.btnLiveBaslerCam.Name = "btnLiveBaslerCam";
            this.btnLiveBaslerCam.Size = new System.Drawing.Size(176, 51);
            this.btnLiveBaslerCam.TabIndex = 3;
            this.btnLiveBaslerCam.Text = "Live Basler Camera";
            this.btnLiveBaslerCam.UseVisualStyleBackColor = true;
            this.btnLiveBaslerCam.Click += new System.EventHandler(this.btnLiveBaslerCam_Click);
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(28, 37);
            this.btnInit.Margin = new System.Windows.Forms.Padding(2);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(176, 39);
            this.btnInit.TabIndex = 2;
            this.btnInit.Text = "Initialize All";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // btnFindLine
            // 
            this.btnFindLine.Enabled = false;
            this.btnFindLine.Location = new System.Drawing.Point(28, 290);
            this.btnFindLine.Name = "btnFindLine";
            this.btnFindLine.Size = new System.Drawing.Size(176, 47);
            this.btnFindLine.TabIndex = 1;
            this.btnFindLine.Text = "Find Line";
            this.btnFindLine.UseVisualStyleBackColor = true;
            this.btnFindLine.Click += new System.EventHandler(this.btnFindLine_Click);
            // 
            // btnTrigger
            // 
            this.btnTrigger.Enabled = false;
            this.btnTrigger.Location = new System.Drawing.Point(28, 207);
            this.btnTrigger.Margin = new System.Windows.Forms.Padding(2);
            this.btnTrigger.Name = "btnTrigger";
            this.btnTrigger.Size = new System.Drawing.Size(176, 36);
            this.btnTrigger.TabIndex = 0;
            this.btnTrigger.Text = "Trigger";
            this.btnTrigger.UseVisualStyleBackColor = true;
            // 
            // btnCameraLive
            // 
            this.btnCameraLive.Enabled = false;
            this.btnCameraLive.Location = new System.Drawing.Point(28, 156);
            this.btnCameraLive.Margin = new System.Windows.Forms.Padding(2);
            this.btnCameraLive.Name = "btnCameraLive";
            this.btnCameraLive.Size = new System.Drawing.Size(176, 36);
            this.btnCameraLive.TabIndex = 0;
            this.btnCameraLive.Text = "Camera Live";
            this.btnCameraLive.UseVisualStyleBackColor = true;
            this.btnCameraLive.Click += new System.EventHandler(this.btnCameraLive_Click);
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.Enabled = false;
            this.btnLoadImage.Location = new System.Drawing.Point(28, 105);
            this.btnLoadImage.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(176, 36);
            this.btnLoadImage.TabIndex = 0;
            this.btnLoadImage.Text = "Load Image";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
            // 
            // timer_Cam_Live
            // 
            this.timer_Cam_Live.Interval = 50;
            this.timer_Cam_Live.Tick += new System.EventHandler(this.timer_Cam_Live_Tick);
            // 
            // btnSingleGrab
            // 
            this.btnSingleGrab.Location = new System.Drawing.Point(28, 462);
            this.btnSingleGrab.Name = "btnSingleGrab";
            this.btnSingleGrab.Size = new System.Drawing.Size(176, 46);
            this.btnSingleGrab.TabIndex = 4;
            this.btnSingleGrab.Text = "Single Grab Usb Camera";
            this.btnSingleGrab.UseVisualStyleBackColor = true;
            this.btnSingleGrab.Click += new System.EventHandler(this.btnSingleGrab_Click);
            // 
            // wfTestNViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(953, 630);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "wfTestNViewer";
            this.Text = "wfTestNViewer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.wfTestNViewer_FormClosed);
            this.Load += new System.EventHandler(this.wfTestNViewer_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnTrigger;
        private System.Windows.Forms.Button btnCameraLive;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.Button btnFindLine;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Timer timer_Cam_Live;
        private System.Windows.Forms.Button btnLiveBaslerCam;
        private System.Windows.Forms.Button btnSingleGrab;
    }
}