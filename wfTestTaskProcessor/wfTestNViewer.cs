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
        public wfTestNViewer()
        {
            InitializeComponent();

            NBufferViewer nBufferViewer = new NBufferViewer(0);
            panel1.Controls.Add(nBufferViewer);
            nBufferViewer.Dock = DockStyle.Fill;
        }
    }
}
