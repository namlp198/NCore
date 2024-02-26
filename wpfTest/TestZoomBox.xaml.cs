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
using System.Windows.Shapes;

namespace wpfTest
{
    /// <summary>
    /// Interaction logic for TestZoomBox.xaml
    /// </summary>
    public partial class TestZoomBox : Window
    {
        public TestZoomBox()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                imgTest.Source = new BitmapImage(new Uri(ofd.FileName));
            }
        }
    }
}
