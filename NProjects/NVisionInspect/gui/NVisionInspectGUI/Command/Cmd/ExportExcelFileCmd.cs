using NVisionInspectGUI.Commons;
using NVisionInspectGUI.Manager;
using NVisionInspectGUI.Manager.Class;
using NVisionInspectGUI.Models;
using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
{
    public class ExportExcelFileCmd : CommandBase
    {
        public ExportExcelFileCmd() { }
        public override void Execute(object parameter)
        {
            //List<ExcelTemplateModel> listExcelModel = SQLite_Manager.Instance.SelectAllData("Test_Excel");

            List<ExcelTemplateModel> listExcelModel = Csv_Manager.Instance.ReadExcelTemplateModelFromCsv();

            MainViewModel.Instance.ExportData(listExcelModel, "TEST", 3);

            MainViewModel.Instance.OpenFolder(Defines.StartupProgPath + "\\Report");
        }
    }
}
