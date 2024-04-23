using DinoVisionGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;


namespace DinoVisionGUI.Command
{
    public class ShowLoginViewCmd : CommandBase
    {
        private readonly MainViewModel _mainViewModel;
        public ShowLoginViewCmd(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
        public override void Execute(object parameter)
        {

        }
    }
}
