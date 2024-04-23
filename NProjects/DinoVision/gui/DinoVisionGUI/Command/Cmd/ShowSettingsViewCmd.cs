using DinoVisionGUI.ViewModels;
using DinoVisionGUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;


namespace DinoVisionGUI.Command
{
    public class ShowSettingsViewCmd : CommandBase
    {
        private readonly MainViewModel _mainViewModel;
        public ShowSettingsViewCmd(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
        public override void Execute(object parameter)
        {
            SettingsView settingsView = new SettingsView();
            settingsView.ShowDialog();
        }
    }
}
