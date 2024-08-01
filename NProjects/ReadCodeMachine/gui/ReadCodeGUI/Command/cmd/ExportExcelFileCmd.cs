using ReadCodeGUI.Commons;
using ReadCodeGUI.Manager;
using ReadCodeGUI.Manager.Class;
using ReadCodeGUI.Models;
using ReadCodeGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReadCodeGUI.Command.Cmd
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
