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
            UcSumCameraView ucSumCameraView = new UcSumCameraView();
            SumCameraViewModel sumCamVM = new SumCameraViewModel(ucSumCameraView.Dispatcher, ucSumCameraView);
            ucSumCameraView.DataContext = sumCamVM;

            UcResultView resultView = new UcResultView();
            ResultViewModel resultVM = new ResultViewModel(resultView.Dispatcher, resultView);
            resultView.DataContext = resultVM;

            UcStatisticsView statisticsView = new UcStatisticsView();
            StatisticsViewModel statisticsVM = new StatisticsViewModel(statisticsView.Dispatcher, statisticsView);
            statisticsView.DataContext = statisticsVM;

            UcRunView runView = new UcRunView();
            runView.contentCamView.Content = ucSumCameraView;
            runView.contentResult.Content = resultView;
            runView.contentStatistics.Content = statisticsView;
            RunViewModel runVM = new RunViewModel(runView.Dispatcher, runView, sumCamVM, resultVM, statisticsVM);
            runView.DataContext = runVM;

            UcSettingView settingView = new UcSettingView();
            SettingViewModel settingVM = new SettingViewModel(settingView.Dispatcher, settingView);
            settingView.DataContext = settingVM;

            MainView mainView = new MainView();
            MainViewModel mainViewModel = new MainViewModel(mainView.Dispatcher, mainView, 
                                                            runVM, settingVM);

            mainView.contentMain.Content = runVM.RunView;
            mainView.DataContext = mainViewModel;
            mainView.Show();
        }
    }
}
