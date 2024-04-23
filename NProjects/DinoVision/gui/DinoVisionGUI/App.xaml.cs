using DinoVisionGUI.ViewModels;
using DinoVisionGUI.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DinoVisionGUI
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
            // init views
            MainView mainView = new MainView();

            // init viewmodels
            SettingsViewModel settingsViewModel = new SettingsViewModel();
            LoginViewModel loginViewModel = new LoginViewModel();
            MainViewModel mainViewModel = new MainViewModel(mainView.Dispatcher, mainView,
                                                            settingsViewModel, loginViewModel);

            mainView.DataContext = mainViewModel;
            mainView.Show();
        }
    }
}
