using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NVisionInspectGUI.ViewModels;
using NVisionInspectGUI.Views;
using NVisionInspectGUI.Views.CamView;
using NVisionInspectGUI.Views.UcViews;

namespace NVisionInspectGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            UcSettingView settingView = new UcSettingView();
            SettingViewModel settingVM = new SettingViewModel(settingView.Dispatcher, settingView);
            settingView.DataContext = settingVM;

            MainView mainView = new MainView();
            MainViewModel mainVM = new MainViewModel(mainView.Dispatcher, mainView, settingVM);

            mainView.DataContext = mainVM;
            mainView.Show();
        }
    }
}
