using NVisionInspectGUI.ViewModels;
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

namespace NVisionInspectGUI.Views.UcViews
{
    /// <summary>
    /// Interaction logic for UcViewDetail.xaml
    /// </summary>
    public partial class UcShowDetail : UserControl
    {
        public UcShowDetail()
        {
            InitializeComponent();

            this.DataContext = this;

            buffVS.ShowDetail += BuffVS_ShowDetail;
        }

        private void BuffVS_ShowDetail(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = MainViewModel.Instance.RunVM.SumCamVM.Sum1CameraView;
        }
    }
}
