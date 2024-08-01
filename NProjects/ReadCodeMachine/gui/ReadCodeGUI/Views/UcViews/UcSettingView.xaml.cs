using ReadCodeGUI.Manager.Class;
using ReadCodeGUI.Models;
using ReadCodeGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReadCodeGUI.Views.UcViews
{
    /// <summary>
    /// Interaction logic for UcSettingView.xaml
    /// </summary>
    public partial class UcSettingView : UserControl
    {
        public UcSettingView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM += 1; // Out Y3
            MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.SetOutputPlc(true);
            MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM = 2048; // reset bit M to init value
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM += 0; // Out Y2
            MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.SetOutputPlc(true);
            MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM = 2048; // reset bit M to init value
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // record to database
            List<ExcelTemplateModel> excelTemplateModels = new List<ExcelTemplateModel>();

            ExcelTemplateModel excelModel = new ExcelTemplateModel();
            excelModel.Id = 4;
            excelModel.ProductName = "PRODUCT_TEST";
            excelModel.ProductCode = "abcdefgh_4";
            excelModel.Date = DateTime.Now.ToString("dd-MM-yyyy HH:mm::ss");
            excelModel.Judgement = "OK_TEST";
            excelModel.Note = "TEST_4";

            excelTemplateModels.Add(excelModel);
            Csv_Manager.Instance.WriteNewModelToCsv(excelTemplateModels);
            //SQLite_Manager.Instance.InsertData(excelModel, "Test_Excel");

            List<ResultStringMapToDataGridModel> listResStrMapToDataGrid = new List<ResultStringMapToDataGridModel>();
            ResultStringMapToDataGridModel resStrMapToDg = new ResultStringMapToDataGridModel();
            resStrMapToDg.Index = 4;
            resStrMapToDg.CodeName = "Code " + 4;
            resStrMapToDg.Code = "abcdefgh_4";

            listResStrMapToDataGrid.Add(resStrMapToDg);

            MainViewModel.Instance.ResultVM.ListResultStringMapToDataGrid = listResStrMapToDataGrid;
        }
    }
}
