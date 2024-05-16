using SealingInspectGUI.ViewModels;
using SealingInspectGUI.Views;
using SealingInspectGUI.Views.UcViews;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SealingInspectGUI
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
            UcRunView runView = new UcRunView();
            RunViewModel runVM = new RunViewModel(runView.Dispatcher, runView);

            UcSettingView settingView = new UcSettingView();
            SettingViewModel settingVM = new SettingViewModel(settingView.Dispatcher, settingView);

            MainView mainView = new MainView();
            MainViewModel mainViewModel = new MainViewModel(mainView.Dispatcher, mainView, runVM, settingVM);

            mainView.contentMain.Content = runView;
            mainView.DataContext = mainViewModel;
            mainView.Show();
        }
    }
}
