using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
{
    public class InitializeCmd : CommandBase
    {
        public InitializeCmd() { }
        public override void Execute(object parameter)
        {
            if (MessageBox.Show("Do you want to Initialize?", "INFORM", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MessageBox.Show("Initialize Success!");
            }
        }
    }
}
