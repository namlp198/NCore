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
            int nCamCount = MainViewModel.Instance.SettingVM.CameraCount;

            switch (nCamCount)
            {
                case 1:
                    Sum1CameraViewModel sum1CamVM = MainViewModel.Instance.RunVM.SumCamVM as Sum1CameraViewModel;
                    MainViewModel.Instance.RunVM.RunView.contentCamView.Content = sum1CamVM.Sum1CameraView;
                    break;
                case 2:
                    Sum2CameraViewModel sum2CamVM = MainViewModel.Instance.RunVM.SumCamVM as Sum2CameraViewModel;
                    MainViewModel.Instance.RunVM.RunView.contentCamView.Content = sum2CamVM.Sum2CameraView;
                    break;
                case 3:
                    Sum3CameraViewModel sum3CamVM = MainViewModel.Instance.RunVM.SumCamVM as Sum3CameraViewModel;
                    MainViewModel.Instance.RunVM.RunView.contentCamView.Content = sum3CamVM.Sum3CameraView;
                    break;
                case 4:
                    Sum4CameraViewModel sum4CamVM = MainViewModel.Instance.RunVM.SumCamVM as Sum4CameraViewModel;
                    MainViewModel.Instance.RunVM.RunView.contentCamView.Content = sum4CamVM.Sum4CameraView;
                    break;
                case 5:
                    Sum5CameraViewModel sum5CamVM = MainViewModel.Instance.RunVM.SumCamVM as Sum5CameraViewModel;
                    MainViewModel.Instance.RunVM.RunView.contentCamView.Content = sum5CamVM.Sum5CameraView;
                    break;
                case 6:
                    Sum6CameraViewModel sum6CamVM = MainViewModel.Instance.RunVM.SumCamVM as Sum6CameraViewModel;
                    MainViewModel.Instance.RunVM.RunView.contentCamView.Content = sum6CamVM.Sum6CameraView;
                    break;
                case 7:
                    Sum7CameraViewModel sum7CamVM = MainViewModel.Instance.RunVM.SumCamVM as Sum7CameraViewModel;
                    MainViewModel.Instance.RunVM.RunView.contentCamView.Content = sum7CamVM.Sum7CameraView;
                    break;
                case 8:
                    Sum8CameraViewModel sum8CamVM = MainViewModel.Instance.RunVM.SumCamVM as Sum8CameraViewModel;
                    MainViewModel.Instance.RunVM.RunView.contentCamView.Content = sum8CamVM.Sum8CameraView;
                    break;
            }
        }
    }
}
