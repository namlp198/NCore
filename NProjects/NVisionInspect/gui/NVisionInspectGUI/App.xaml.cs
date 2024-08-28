using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NVisionInspectGUI.ViewModels;
using NVisionInspectGUI.Views;
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
            UcSum1CameraView ucSumCameraView = new UcSum1CameraView();
            Sum1CameraViewModel sumCamVM = new Sum1CameraViewModel(ucSumCameraView.Dispatcher, ucSumCameraView);
            ucSumCameraView.DataContext = sumCamVM;

            UcResultView resultView = new UcResultView();
            ResultViewModel resultVM = new ResultViewModel(resultView.Dispatcher, resultView);
            resultView.DataContext = resultVM;

            UcRunView runView = new UcRunView();
            runView.contentCamView.Content = ucSumCameraView;
            runView.contentResult.Content = resultView;
            RunViewModel runVM = new RunViewModel(runView.Dispatcher, runView, sumCamVM, resultVM);
            runView.DataContext = runVM;

            UcSettingView settingView = new UcSettingView();
            SettingViewModel settingVM = new SettingViewModel(settingView.Dispatcher, settingView);
            settingView.DataContext = settingVM;

            MainView mainView = new MainView();
            MainViewModel mainVM = new MainViewModel(mainView.Dispatcher, mainView, runVM, settingVM);

            mainView.contentMain.Content = runVM.RunView;
            mainView.DataContext = mainVM;
            mainView.Show();
        }
    }
}
