using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Npc.Foundation.Base;
using NVisionInspectGUI.Manager;
using NVisionInspectGUI.Views.UcViews;
using NVisionInspectGUI.Commons;
using NVisionInspectGUI.Models;
using NVisionInspectGUI.Manager.Class;
using NVisionInspectGUI.ViewModels;
using OpenXMLParser;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Threading;
using NCore.Wpf.BufferViewerSimple;

namespace NVisionInspectGUI.ViewModels
{
    public class Sum1CameraViewModel : ViewModelBase
    {
        #region variables
        private static readonly object _lockObj = new object();
        private readonly Dispatcher _dispatcher;
        private UcSum1CameraView m_ucSum1CameraView;
        private int m_nIndex = 1;
        private IOManager_PLC_LS m_Plc_LS = new IOManager_PLC_LS();
        private IOManager_PLC_Delta_DVP m_Plc_Delta = new IOManager_PLC_Delta_DVP();
        #endregion

        #region Constructor
        public Sum1CameraViewModel(Dispatcher dispatcher, UcSum1CameraView sum1CameraView)
        {
            _dispatcher = dispatcher;
            m_ucSum1CameraView = sum1CameraView;

            m_ucSum1CameraView.buffCam.CameraIndex = 0;
            m_ucSum1CameraView.buffCam.ModeView = ModeView.Color;
            m_ucSum1CameraView.buffCam.CameraName = "[Cam 1 - Read Code]";
            m_ucSum1CameraView.buffCam.SetParamsModeColor(Defines.FRAME_WIDTH, Defines.FRAME_HEIGHT);
            //_sumCameraView.buffCam.ShowDetail += BuffCam_ShowDetail;

            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);
        }
        #endregion

        #region Properties
        public UcSum1CameraView Sum1CameraView { get { return m_ucSum1CameraView; } }

        public IOManager_PLC_LS Plc_LS
        {
            get => m_Plc_LS;
            set => m_Plc_LS = value;
        }
        public IOManager_PLC_Delta_DVP Plc_Delta_DVP
        {
            get => m_Plc_Delta;
            set => m_Plc_Delta = value;
        }
        #endregion

        #region Methods
        private async void InspectionComplete(int nCamIdx, int bSetting)
        {
            if (bSetting == 0)
            {
                int nCoreIdx = nCamIdx;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetInspectionResult(nCoreIdx, ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult);

                Sum1CameraView.buffCam.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetResultBuffer(0, 0);
                await Sum1CameraView.buffCam.UpdateImage();

                MainViewModel.Instance.RunVM.ResultVM.CountTotal++;

                switch (nCamIdx)
                {
                    case 1:
                        if (InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_bResultStatus == 1)
                        {
                            Sum1CameraView.buffCam.InspectResult = EInspectResult.InspectResult_OK;
                            m_Plc_Delta.StartAddressBitM += 1; // Out Y3
                            m_Plc_Delta.SetOutputPlc(true);
                            Thread.Sleep(5);
                            m_Plc_Delta.StartAddressBitM = 2048; // reset bit M to init value

                            MainViewModel.Instance.RunVM.ResultVM.CountOK++;
                        }
                        else
                        {
                            Sum1CameraView.buffCam.InspectResult = EInspectResult.InspectResult_NG;
                            m_Plc_Delta.StartAddressBitM += 0; // Out Y2
                            m_Plc_Delta.SetOutputPlc(true);
                            Thread.Sleep(5);
                            m_Plc_Delta.StartAddressBitM = 2048; // reset bit M to init value

                            MainViewModel.Instance.RunVM.ResultVM.CountNG++;
                        }
                        string resStr = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_sResultString;
                        if (!resStr.IsNullOrEmpty())
                        {
                            if (resStr.IndexOf(";") == -1)
                            {
                                // record to csv file
                                List<ExcelTemplateModel> excelTemplateModels = new List<ExcelTemplateModel>();
                                ExcelTemplateModel excelModel = new ExcelTemplateModel();

                                excelModel.Id = m_nIndex;
                                excelModel.ProductName = "PRODUCT_TEST";
                                excelModel.ProductCode = resStr;
                                excelModel.Date = DateTime.Now.ToString("dd-MM-yyyy HH:mm::ss");
                                excelModel.Judgement = "OK_TEST";
                                excelModel.Note = "TEST";

                                excelTemplateModels.Add(excelModel);
                                Csv_Manager.Instance.WriteNewModelToCsv(excelTemplateModels);

                                //SQLite_Manager.Instance.InsertData(excelModel, "Test_Excel");

                                m_nIndex++;
                            }
                            else
                            {
                                string[] arrStr = resStr.Split(new char[] { ';' });
                                for (int i = 0; i < arrStr.Length; i++)
                                {
                                    // record to database
                                    List<ExcelTemplateModel> excelTemplateModels = new List<ExcelTemplateModel>();
                                    ExcelTemplateModel excelModel = new ExcelTemplateModel();
                                    excelModel.Id = m_nIndex;
                                    excelModel.ProductName = "PRODUCT_TEST";
                                    excelModel.ProductCode = arrStr[i];
                                    excelModel.Date = DateTime.Now.ToString("dd-MM-yyyy HH:mm::ss");
                                    excelModel.Judgement = "OK_TEST";
                                    excelModel.Note = "TEST";

                                    excelTemplateModels.Add(excelModel);
                                    Csv_Manager.Instance.WriteNewModelToCsv(excelTemplateModels);

                                    //SQLite_Manager.Instance.InsertData(excelModel, "Test_Excel");

                                    m_nIndex++;
                                }
                            }
                        }
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                }
            }
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
