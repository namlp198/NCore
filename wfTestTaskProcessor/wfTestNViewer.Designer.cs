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
            this.btnFindLine = new System.Windows.Forms.Button();
            this.btnTrigger = new System.Windows.Forms.Button();
            this.btnCameraLive = new System.Windows.Forms.Button();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.btnInit = new System.Windows.Forms.Button();
            this.timer_Cam_Live = new System.Windows.Forms.Timer(this.components);
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
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1271, 775);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(947, 771);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnInit);
            this.panel2.Controls.Add(this.btnFindLine);
            this.panel2.Controls.Add(this.btnTrigger);
            this.panel2.Controls.Add(this.btnCameraLive);
            this.panel2.Controls.Add(this.btnLoadImage);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(956, 2);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(312, 771);
            this.panel2.TabIndex = 1;
            // 
            // btnFindLine
            // 
            this.btnFindLine.Enabled = false;
            this.btnFindLine.Location = new System.Drawing.Point(37, 357);
            this.btnFindLine.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnFindLine.Name = "btnFindLine";
            this.btnFindLine.Size = new System.Drawing.Size(235, 58);
            this.btnFindLine.TabIndex = 1;
            this.btnFindLine.Text = "Find Line";
            this.btnFindLine.UseVisualStyleBackColor = true;
            this.btnFindLine.Click += new System.EventHandler(this.btnFindLine_Click);
            // 
            // btnTrigger
            // 
            this.btnTrigger.Enabled = false;
            this.btnTrigger.Location = new System.Drawing.Point(37, 255);
            this.btnTrigger.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTrigger.Name = "btnTrigger";
            this.btnTrigger.Size = new System.Drawing.Size(235, 44);
            this.btnTrigger.TabIndex = 0;
            this.btnTrigger.Text = "Trigger";
            this.btnTrigger.UseVisualStyleBackColor = true;
            // 
            // btnCameraLive
            // 
            this.btnCameraLive.Enabled = false;
            this.btnCameraLive.Location = new System.Drawing.Point(37, 192);
            this.btnCameraLive.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCameraLive.Name = "btnCameraLive";
            this.btnCameraLive.Size = new System.Drawing.Size(235, 44);
            this.btnCameraLive.TabIndex = 0;
            this.btnCameraLive.Text = "Camera Live";
            this.btnCameraLive.UseVisualStyleBackColor = true;
            this.btnCameraLive.Click += new System.EventHandler(this.btnCameraLive_Click);
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.Enabled = false;
            this.btnLoadImage.Location = new System.Drawing.Point(37, 129);
            this.btnLoadImage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(235, 44);
            this.btnLoadImage.TabIndex = 0;
            this.btnLoadImage.Text = "Load Image";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(37, 46);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(235, 48);
            this.btnInit.TabIndex = 2;
            this.btnInit.Text = "Initialize All";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // timer_Cam_Live
            // 
            this.timer_Cam_Live.Tick += new System.EventHandler(this.timer_Cam_Live_Tick);
            // 
            // wfTestNViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1271, 775);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
    }
}