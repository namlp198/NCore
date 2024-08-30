using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Npc.Foundation.Base;
using NVisionInspectGUI.Commons;
using NVisionInspectGUI.Views;
using NVisionInspectGUI.Command.Cmd;
using NVisionInspectGUI.Manager;
using NVisionInspectGUI.Models;
using NVisionInspectGUI.Manager.Class;
using OpenXMLParser;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Threading;

namespace NVisionInspectGUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region SingleTon
        private static MainViewModel _instance;
        public static MainViewModel Instance
        {
            get { return _instance; }
            private set { }
        }
        #endregion

        #region Constructor
        public MainViewModel(Dispatcher dispatcher, MainView mainView, 
                             RunViewModel runVM, SettingViewModel settingVM)
        {
            // construct a instance of MainViewModel
            if (_instance == null) _instance = this;
            else return;

            _dispatcher = dispatcher;
            m_ucMainView = mainView;
            m_runVM = runVM;
            m_settingVM = settingVM;

            m_columnNameList = new List<string>() { "A", "B", "C", "D", "E", "F" };
            StartAppExcel();
            Csv_Manager.Instance.Initialize();

            this.SelectRunViewCmd = new SelectRunViewCmd();
            this.SelectSettingViewCmd = new SelectSettingViewCmd();
            this.SelectMachineModeCmd = new SelectMachineModeCmd();
            this.ExportExcelFileCmd = new ExportExcelFileCmd();
            this.InitializeCmd = new InitializeCmd();

            InterfaceManager.Instance.m_processorManager.Initialize();

            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadSystemSettings(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings);
            SettingVM.LoadSystemSettings();

            int nNumberOfCamInsp = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_nInspectCameraCount;
            List<string> lstCameras = new List<string>();
            for (int nCamIdx = 0; nCamIdx < nNumberOfCamInsp; nCamIdx++)
            {
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadCameraSettings(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx], nCamIdx);
                SettingVM.LoadCamerasSetting(nCamIdx);

                string sCamera = "Cam " + (nCamIdx + 1) + "";
                lstCameras.Add(sCamera);
            }

            SettingVM.SettingView.buffSettingPRO.CameraList = lstCameras;

            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadRecipe(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe);
            SettingVM.LoadRecipe();

            SettingVM.LoadPlcSettings();

            if (InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSimulation == 0)
            {
                RunVM.SumCamVM.Plc_Delta_DVP.Initialize();
                SettingVM.SetAllParamPlcDelta();
                if (InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.InspectStart(1,0))
                {
                    InspectRunning = true;
                }
                else
                {
                    InspectRunning = false;
                }
            }
            else
            {
                InspectRunning = true;
                Thread.Sleep(2);
                InspectRunning = false;
            }
        }
        #endregion

        #region Destructor
        ~MainViewModel() 
        {
        }
        #endregion

        #region variables
        private readonly Dispatcher _dispatcher;
        private MainView m_ucMainView;
        public MainView MainView { get => m_ucMainView; private set { } }

        private eMachineMode m_machineMode = eMachineMode.MachineMode_Auto;
        private bool m_bInspRunning = false;
        private bool m_bEnableSetting = false;
        private string m_displayImage_MachineModePath = "/NpcCore.Wpf;component/Resources/Images/arrow_backward.png";

        private ExcelParser m_excelParser = new ExcelParser();
        private List<string> m_columnNameList;
        #endregion

        #region Properties
        public eMachineMode MachineMode
        {
            get => m_machineMode;
            set
            {
                if (SetProperty(ref m_machineMode, value))
                {
                }
            }
        }
        public string DisplayImage_MachineModePath
        {
            get => m_displayImage_MachineModePath;
            set
            {
                if (SetProperty(ref m_displayImage_MachineModePath, value))
                {

                }
            }
        }
        public bool InspectRunning
        {
            get => m_bInspRunning;
            set
            {
                if (SetProperty(ref m_bInspRunning, value))
                {
                    if (m_bInspRunning == true)
                    {
                        EnableSetting = false;
                        m_ucMainView.btnExportExcel.Opacity = 0.3;
                        m_ucMainView.btnSettings.Opacity = 0.3;
                    }
                    else if (m_bInspRunning == false)
                    {
                        EnableSetting = true;
                        m_ucMainView.btnExportExcel.Opacity = 1.0;
                        m_ucMainView.btnSettings.Opacity = 1.0;
                    }
                }
            }
        }
        public bool EnableSetting
        {
            get => m_bEnableSetting;
            set
            {
                if (SetProperty(ref m_bEnableSetting, value))
                {

                }
            }
        }
        #endregion

        #region AllViewModel
        private RunViewModel m_runVM;
        public RunViewModel RunVM { get => m_runVM; private set { } }

        private SettingViewModel m_settingVM;
        public SettingViewModel SettingVM { get => m_settingVM; private set { } }
        #endregion

        #region Methods
        void StartAppExcel()
        {
            KillAppExcel();
            OpenExcelResultFile();
        }
        void KillAppExcel()
        {
            foreach (var process in Process.GetProcessesByName("EXCEL"))
            {
                process.Kill();
            }
        }
        public string ExcelFilePath
        {
            get;
            set;
        }
        void OpenExcelResultFile()
        {
            try
            {
                string currentDir = Environment.CurrentDirectory + "\\VisionSettings\\TemplateExcel\\" + "Report_Template.xlsx";
                string currentDailyData = Environment.CurrentDirectory + "\\VisionSettings\\Report\\" + DateTime.Now.ToString("dd-MM-yyyy") + "_Report" + ".xlsx";
                if (!File.Exists(currentDailyData))
                {
                    File.Copy(@currentDir, @currentDailyData);
                }
                if (m_excelParser.OpenFile(currentDailyData, true))
                {
                    ExcelFilePath = currentDailyData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private Task ExcelTemplateInput(List<ExcelTemplateModel> excelTemplateModels, string sheetName, int cellStartWrite)
        {
            int cell = cellStartWrite;
            int index = 1;
            string addressName = string.Empty;
            string value = string.Empty;
            return Task.Run(() =>
            {

                foreach (var excelModel in excelTemplateModels)
                {

                    if (excelModel == null) return;

                    addressName = m_columnNameList[0] + cell;
                    value = index + "";
                    if (!m_excelParser.SetCellValue(sheetName, addressName, value))
                    {
                        m_excelParser.AddCell(sheetName, addressName, value);
                    }

                    addressName = m_columnNameList[1] + cell;
                    value = excelModel.ProductName;
                    if (!m_excelParser.SetCellValue(sheetName, addressName, value))
                    {
                        m_excelParser.AddCell(sheetName, addressName, value);
                    }

                    addressName = m_columnNameList[2] + cell;
                    value = excelModel.ProductCode;
                    if (!m_excelParser.SetCellValue(sheetName, addressName, value))
                    {
                        m_excelParser.AddCell(sheetName, addressName, value);
                    }

                    addressName = m_columnNameList[3] + cell;
                    value = excelModel.Date;
                    if (!m_excelParser.SetCellValue(sheetName, addressName, value))
                    {
                        m_excelParser.AddCell(sheetName, addressName, value);
                    }

                    addressName = m_columnNameList[4] + cell;
                    value = excelModel.Judgement;
                    if (!m_excelParser.SetCellValue(sheetName, addressName, value))
                    {
                        m_excelParser.AddCell(sheetName, addressName, value);
                    }

                    addressName = m_columnNameList[5] + cell;
                    value = excelModel.Note;
                    if (!m_excelParser.SetCellValue(sheetName, addressName, value))
                    {
                        m_excelParser.AddCell(sheetName, addressName, value);
                    }

                    m_excelParser.Save();

                    cell++;
                    index++;
                }
                m_excelParser.Close();
            });
        }
        public Task ExportData(List<ExcelTemplateModel> excelTemplateModels, string sheetName, int cellStartWrite)
        {
            return Task.Run(async () =>
            {
                await ExcelTemplateInput(excelTemplateModels, sheetName, cellStartWrite);
            });
        }
        public void OpenFolder(string folderPath)
        {
            System.Diagnostics.Process.Start("explorer.exe", @folderPath);
        }
        #endregion

        #region Command
        public ICommand SelectRunViewCmd { get; }
        public ICommand SelectSettingViewCmd { get; }
        public ICommand SelectMachineModeCmd { get; }
        public ICommand ExportExcelFileCmd {  get; }
        public ICommand InitializeCmd { get; }
        #endregion
    }
}
