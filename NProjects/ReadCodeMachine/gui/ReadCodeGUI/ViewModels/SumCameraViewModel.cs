using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Npc.Foundation.Base;
using ReadCodeGUI.Manager;
using ReadCodeGUI.Views.UcViews;
using NCore.Wpf.BufferViewerSimple;
using ReadCodeGUI.Commons;
using ReadCodeGUI.Models;
using OpenXMLParser;
using System.Diagnostics;
using System.Windows;
using System.IO;
using ReadCodeGUI.Manager.Class;

namespace ReadCodeGUI.ViewModels
{
    public class SumCameraViewModel : ViewModelBase
    {
        #region variables
        private static readonly object _lockObj = new object();
        private readonly Dispatcher _dispatcher;
        private UcSumCameraView _sumCameraView;
        private int m_nIndex = 1;
        #endregion

        #region Constructor
        public SumCameraViewModel(Dispatcher dispatcher, UcSumCameraView sumCameraView)
        {
            _dispatcher = dispatcher;
            _sumCameraView = sumCameraView;

            _sumCameraView.buffCam.CameraIndex = 0;
            _sumCameraView.buffCam.ModeView = ModeView.Color;
            _sumCameraView.buffCam.CameraName = "[Cam 1 - Read Code]";
            _sumCameraView.buffCam.SetParamsModeColor(Defines.FRAME_WIDTH, Defines.FRAME_HEIGHT);
            //_sumCameraView.buffCam.ShowDetail += BuffCam_ShowDetail;

            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);
        }
        #endregion

        #region Properties
        public UcSumCameraView SumCameraView { get { return _sumCameraView; } }
        #endregion

        #region Methods
        private async void InspectionComplete(int bSetting)
        {
            InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.GetInspectionResult(0, ref InterfaceManager.Instance.m_processorManager.m_readCodeResult[0]);

            SumCameraView.buffCam.BufferView = InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.GetResultBuffer(0, 0);
            await SumCameraView.buffCam.UpdateImage();

            if (InterfaceManager.Instance.m_processorManager.m_readCodeResult[0].m_bResultStatus == 1) SumCameraView.buffCam.InspectResult = EInspectResult.InspectResult_OK;
            else SumCameraView.buffCam.InspectResult = EInspectResult.InspectResult_NG;

            string resStr = InterfaceManager.Instance.m_processorManager.m_readCodeResult[0].m_sResultString;
            List<ResultStringMapToDataGridModel> listResStrMapToDataGrid = new List<ResultStringMapToDataGridModel>();
            if (!resStr.IsNullOrEmpty())
            {
                if (resStr.IndexOf(";") == -1)
                {
                    ResultStringMapToDataGridModel resStrMapToDg = new ResultStringMapToDataGridModel();

                    resStrMapToDg.Index = 1;
                    resStrMapToDg.CodeName = "Code 1";
                    resStrMapToDg.Code = resStr;

                    listResStrMapToDataGrid.Add(resStrMapToDg);

                    // record to database
                    ExcelTemplateModel excelModel = new ExcelTemplateModel();
                    excelModel.Id = m_nIndex;
                    excelModel.ProductName = "PRODUCT_TEST";
                    excelModel.ProductCode = resStr;
                    excelModel.Date = DateTime.Now.ToString("dd-MM-yyyy HH:mm::ss");
                    excelModel.Judgement = "OK_TEST";
                    excelModel.Note = "TEST";

                    SQLite_Manager.Instance.InsertData(excelModel, "Test_Excel");

                    m_nIndex++;
                }
                else
                {
                    string[] arrStr = resStr.Split(new char[] { ';' });
                    for (int i = 0; i < arrStr.Length; i++)
                    {
                        ResultStringMapToDataGridModel resStrMapToDg = new ResultStringMapToDataGridModel();
                        resStrMapToDg.Index = i + 1;
                        resStrMapToDg.CodeName = "Code " + i + 1;
                        resStrMapToDg.Code = arrStr[i];

                        listResStrMapToDataGrid.Add(resStrMapToDg);

                        // record to database
                        ExcelTemplateModel excelModel = new ExcelTemplateModel();
                        excelModel.Id = m_nIndex;
                        excelModel.ProductName = "PRODUCT_TEST";
                        excelModel.ProductCode = arrStr[i];
                        excelModel.Date = DateTime.Now.ToString("dd-MM-yyyy HH:mm::ss");
                        excelModel.Judgement = "OK_TEST";
                        excelModel.Note = "TEST";

                        SQLite_Manager.Instance.InsertData(excelModel, "Test_Excel");

                        m_nIndex++;
                    }
                }
            }

            MainViewModel.Instance.ResultVM.ListResultStringMapToDataGrid = listResStrMapToDataGrid;
        }
        //private async void BuffCam_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    UcShowDetail ucShowDetail = new UcShowDetail();
        //    ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam1_Frame1.CameraIndex;
        //    ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam1_Frame1.CameraName;
        //    ucShowDetail.buffVS.ModeView = ModeView.Color;
        //    ucShowDetail.buffVS.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
        //    MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
        //    if (_sumCameraView.buffTopCam1_Frame1.BufferView != IntPtr.Zero)
        //    {
        //        ucShowDetail.buffVS.BufferView = _sumCameraView.buffTopCam1_Frame1.BufferView;
        //        ucShowDetail.buffVS.InspectResult = _sumCameraView.buffTopCam1_Frame1.InspectResult;
        //        await ucShowDetail.buffVS.UpdateImage();
        //    }
        //}

        #endregion
    }
}
