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
using NVisionInspectGUI.Views.CamView;
using NCore.Wpf.BufferViewerSimple.Model;

namespace NVisionInspectGUI.ViewModels
{
    public class Sum1CameraViewModel : ViewModelBase, ISumCamVM
    {
        #region variables
        private static readonly object _lockObj = new object();
        private int m_nIndex = 1;
        private readonly Dispatcher _dispatcher;
        private UcSum1CameraView m_ucSum1CameraView;
        #endregion

        #region Constructor
        public Sum1CameraViewModel(Dispatcher dispatcher, UcSum1CameraView sum1CameraView)
        {
            _dispatcher = dispatcher;
            m_ucSum1CameraView = sum1CameraView;

            int nWidthCam1 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[0].m_nFrameWidth;
            int nHeightCam1 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[0].m_nFrameHeight;

            // set rect outer draw to on image
            int[] rectOuter = new int[4]
            {
                MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_OuterX,
                MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_OuterY,
                MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Outer_Width,
                MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Outer_Height
            };

            m_ucSum1CameraView.buffCam.CameraIndex = 0;
            m_ucSum1CameraView.buffCam.ModeView = emModeView.Mono;
            m_ucSum1CameraView.buffCam.CameraName = "[Cam 1]";
            m_ucSum1CameraView.buffCam.SetParamsModeMono(nWidthCam1, nHeightCam1);
            m_ucSum1CameraView.buffCam.InitModels(Defines.MAX_COUNT_PIXEL_TOOL_COUNT_CAM1, rectOuter);
            m_ucSum1CameraView.buffCam.ShowDetail += BuffCam1_ShowDetail;

            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);
        }
        #endregion

        #region Properties
        public UcSum1CameraView Sum1CameraView { get { return m_ucSum1CameraView; } }
        #endregion

        #region Methods
        private async void InspectionComplete(int nCamIdx, int bSetting)
        {
            if (bSetting == 0)
            {
                int nCoreIdx = nCamIdx;
                int nBuff = nCamIdx;
                int nFrame = 0;

                switch (nCamIdx)
                {
                    /*case 0:
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetInspectionResult(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult);

                        Sum1CameraView.buffCam.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetResultBuffer(nBuff, nFrame);
                        await Sum1CameraView.buffCam.UpdateImage();

                        MainViewModel.Instance.RunVM.ResultVM.CountTotal++;

                        if (InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_bResultStatus == 1)
                        {
                            Sum1CameraView.buffCam.InspectResult = EInspectResult.InspectResult_OK;
                            MainViewModel.Instance.Plc_Delta_DVP.StartAddressBitM += 1; // Out Y3
                            MainViewModel.Instance.Plc_Delta_DVP.SetOutputPlc(true);
                            Thread.Sleep(5);
                            MainViewModel.Instance.Plc_Delta_DVP.StartAddressBitM = 2048; // reset bit M to init value

                            MainViewModel.Instance.RunVM.ResultVM.CountOK++;
                        }
                        else
                        {
                            Sum1CameraView.buffCam.InspectResult = EInspectResult.InspectResult_NG;
                            MainViewModel.Instance.Plc_Delta_DVP.StartAddressBitM += 0; // Out Y2
                            MainViewModel.Instance.Plc_Delta_DVP.SetOutputPlc(true);
                            Thread.Sleep(5);
                            MainViewModel.Instance.Plc_Delta_DVP.StartAddressBitM = 2048; // reset bit M to init value

                            MainViewModel.Instance.RunVM.ResultVM.CountNG++;
                        }
                        string resStr_1 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_sResultString;
                        if (!resStr_1.IsNullOrEmpty())
                        {
                            if (resStr_1.IndexOf(";") == -1)
                            {
                                // record to csv file
                                List<ExcelTemplateModel> excelTemplateModels = new List<ExcelTemplateModel>();
                                ExcelTemplateModel excelModel = new ExcelTemplateModel();

                                excelModel.Id = m_nIndex;
                                excelModel.ProductName = "PRODUCT_TEST";
                                excelModel.ProductCode = resStr_1;
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
                                string[] arrStr = resStr_1.Split(new char[] { ';' });
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
                        break;*/

                    case 0:
                        InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetInspectionResult(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult);

                        Sum1CameraView.buffCam.LocatorModel.CenterPt[0] = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_NVisonInspectResLocator.m_nCoordinateX;
                        Sum1CameraView.buffCam.LocatorModel.CenterPt[1] = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_NVisonInspectResLocator.m_nCoordinateY;
                        Sum1CameraView.buffCam.LocatorModel.Result = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_NVisonInspectResLocator.m_bResultStatus == 1 ? true : false;

                        for (int i = 0; i < InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_NVisonInspectResCntPxl.Length; i++)
                        {
                            Sum1CameraView.buffCam.CountPixelModels[i].ROI_CountPixel[0] = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_NVisonInspectResCntPxl[i].m_arrROICntPxl[0];
                            Sum1CameraView.buffCam.CountPixelModels[i].ROI_CountPixel[1] = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_NVisonInspectResCntPxl[i].m_arrROICntPxl[1];
                            Sum1CameraView.buffCam.CountPixelModels[i].ROI_CountPixel[2] = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_NVisonInspectResCntPxl[i].m_arrROICntPxl[2];
                            Sum1CameraView.buffCam.CountPixelModels[i].ROI_CountPixel[3] = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_NVisonInspectResCntPxl[i].m_arrROICntPxl[3];

                            Sum1CameraView.buffCam.CountPixelModels[i].NumberOfPixel = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_NVisonInspectResCntPxl[i].m_fNumberOfPixel;
                            Sum1CameraView.buffCam.CountPixelModels[i].Result = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_NVisonInspectResCntPxl[i].m_bResultStatus == 1? true : false;
                        }

                        Sum1CameraView.buffCam.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetResultBuffer(nBuff, nFrame);
                        await Sum1CameraView.buffCam.UpdateImage();

                        MainViewModel.Instance.RunVM.ResultVM.CountTotal++;

                        if (InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_bResultStatus == 1)
                        {
                            MainViewModel.Instance.RunVM.ResultVM.CountOK++;
                        }
                        else
                        {
                            MainViewModel.Instance.RunVM.ResultVM.CountNG++;
                        }
                        break;
                }
            }
        }
        private async void BuffCam1_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[0].m_nFrameWidth;
            int nHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[0].m_nFrameHeight;

            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = m_ucSum1CameraView.buffCam.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum1CameraView.buffCam.CameraName;
            ucShowDetail.buffVS.ModeView = emModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum1CameraView.buffCam.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum1CameraView.buffCam.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum1CameraView.buffCam.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }

        #endregion
    }
}
