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

namespace TemplateVisionWpf_Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.TempInspProcessorManager.Initialize();
        }

        private void btnInsp1_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.InspectStart(1, 0);
        }

        private void btnInsp2_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.InspectStart(1, 1);
        }
    }
}
