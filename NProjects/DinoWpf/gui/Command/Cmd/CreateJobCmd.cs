using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kis.Toolkit;
using System.Xml;
using DinoWpf.Commons;

namespace DinoWpf.Command
{
    public class CreateJobCmd : CommandBase
    {
        public log4net.ILog Logger { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        XmlManagement xmlManagement;
        public CreateJobCmd()
        {
            xmlManagement = new XmlManagement();
        }
        public override void Execute(object parameter)
        {

        }
    }
}
