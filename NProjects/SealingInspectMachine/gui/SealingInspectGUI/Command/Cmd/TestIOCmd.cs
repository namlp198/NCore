using SealingInspectGUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SealingInspectGUI.Command.Cmd
{
    public class TestIOCmd : CommandBase
    {
        public TestIOCmd() { }
        public override void Execute(object parameter)
        {
            TestIOView view = new TestIOView();
            view.Show();
        }
    }
}
