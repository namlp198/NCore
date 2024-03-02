using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NCore.Wpf.NUcBufferViewer
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NUcBufferViewer : UserControl
    {
        public NUcBufferViewer()
        {
            InitializeComponent();

            scrollViewerExt.ImageExt = imageExt;
            scrollViewerExt.Grid = gridMain;
        }

        private void btnLocatorTool_Click(object sender, RoutedEventArgs e)
        {
            if (imageExt.Source == null)
                return;

            imageExt.EnableLocatorTool = true;
        }
    }
}
