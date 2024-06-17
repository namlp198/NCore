using SealingInspectGUI.ViewModels;
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

namespace SealingInspectGUI.Views.UcViews
{
    /// <summary>
    /// Interaction logic for UcSettingView.xaml
    /// </summary>
    public partial class UcSettingView : UserControl
    {
        public UcSettingView()
        {
            InitializeComponent();
        }

        private void btnSetLight255_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.RunVM.SumCamVM.LightController_PD3.Set_3_Bar_Light_255();
        }

        private void btnSetLight0_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.RunVM.SumCamVM.LightController_PD3.Set_4_Light_0();
        }
    }
}
