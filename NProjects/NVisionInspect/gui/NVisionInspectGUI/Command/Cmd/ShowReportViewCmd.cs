using NVisionInspectGUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Command.Cmd
{
    public class ShowReportViewCmd : CommandBase
    {
        public ShowReportViewCmd() { }
        public override void Execute(object parameter)
        {
            ReportView reportView = new ReportView();
            reportView.ShowDialog();
        }
    }
}
