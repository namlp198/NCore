using Microsoft.Win32;
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
using Npc.Foundation.Logger;

namespace wpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public log4net.ILog Logger { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        NLogger logger = new NLogger();
        public MainWindow()
        {
            InitializeComponent();

            logger.GenerateLogger();
            LogHub.SetLogger(logger);

            scrollViewerExt.ImageExt = imageExt;
            scrollViewerExt.Grid = gridMain;

            imageExt.GetROI += ImageExt_GetROI;

            LogHub.Write("This is NpcInfo", LogTypes.NpcInfo);
            LogHub.Write("This is NpcFatal", LogTypes.NpcFatal);
            LogHub.Write("This is NpcGUI", LogTypes.NpcGUI);
            LogHub.Write("This is NpcConnector", LogTypes.NpcDBConnector);
            LogHub.Write("This is NpcDebug", LogTypes.NpcDebug);
        }

        private void ImageExt_GetROI(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(sender.GetType().ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                imageExt.Source = new BitmapImage(new Uri(ofd.FileName));

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            imageExt.EnableGetRoiTool = true;
            imageExt.IsSelectingRoi = true;
            imageExt.EnableSelectRect = true;
            imageExt.EnableRotate = true;
            imageExt.Drag = true;
            imageExt.InvalidateVisual();
        }
    }
}
