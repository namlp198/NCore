using ReadCodeGUI.ViewModels;
using ReadCodeGUI.Views;
using ReadCodeGUI.Views.UcViews;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace ReadCodeGUI
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
            UcSumCameraView ucSumCameraView = new UcSumCameraView();
            SumCameraViewModel sumCamVM = new SumCameraViewModel(ucSumCameraView.Dispatcher, ucSumCameraView);
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
            //mainView.btnExportExcel.Opacity = 0.3;
            //mainView.btnSettings.Opacity = 0.3;
            MainViewModel mainVM = new MainViewModel(mainView.Dispatcher, mainView, runVM, settingVM, resultVM);

            mainView.contentMain.Content = runVM.RunView;
            mainView.DataContext = mainVM;
            mainView.Show();
        }
    }
}
