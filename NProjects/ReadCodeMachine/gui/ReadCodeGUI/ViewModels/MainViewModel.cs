using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Npc.Foundation.Base;
using ReadCodeGUI.Commons;
using ReadCodeGUI.Views;
using ReadCodeGUI.Command.Cmd;
using ReadCodeGUI.Manager;
using OpenXMLParser;
using System.Diagnostics;
using System.Windows;
using System.IO;
using ReadCodeGUI.Models;
using System.Threading;

namespace ReadCodeGUI.ViewModels
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
                             RunViewModel runVM, SettingViewModel settingVM, ResultViewModel resultVM)
        {
            // construct a instance of MainViewModel
            if (_instance == null) _instance = this;
            else return;

            _dispatcher = dispatcher;
            _mainView = mainView;
            _runVM = runVM;
            _settingVM = settingVM;
            _resultVM = resultVM;

            m_columnNameList = new List<string>() { "A", "B", "C", "D", "E", "F" };
            StartAppExcel();

            this.SelectRunViewCmd = new SelectRunViewCmd();
            this.SelectSettingViewCmd = new SelectSettingViewCmd();
            this.SelectMachineModeCmd = new SelectMachineModeCmd();
            this.ExportExcelFileCmd = new ExportExcelFileCmd();
            this.InitializeCmd = new InitializeCmd();

            InterfaceManager.Instance.m_processorManager.Initialize();

            InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.LoadSystemSettings(ref InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings);
            SettingVM.LoadSystemSettings();

            SettingVM.LoadPlcSettings();

            InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.LoadRecipe(ref InterfaceManager.Instance.m_processorManager.m_readCodeRecipe);
            SettingVM.LoadRecipe();

            if (InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSimulation == 0)
            {
                if (InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.InspectStart(0))
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
        ~MainViewModel() { }
        #endregion

        #region variables
        private readonly Dispatcher _dispatcher;
        private MainView _mainView;
        public MainView MainView { get => _mainView; private set { } }

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
                        _mainView.btnExportExcel.Opacity = 0.3;
                        _mainView.btnSettings.Opacity = 0.3;
                    }
                    else if (m_bInspRunning == false)
                    {
                        EnableSetting = true;
                        _mainView.btnExportExcel.Opacity = 1.0;
                        _mainView.btnSettings.Opacity = 1.0;
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
        private RunViewModel _runVM;
        public RunViewModel RunVM { get => _runVM; private set { } }

        private SettingViewModel _settingVM;
        public SettingViewModel SettingVM { get => _settingVM; private set { } }

        private ResultViewModel _resultVM;
        public ResultViewModel ResultVM { get => _resultVM; private set { } }

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
                string currentDir = Environment.CurrentDirectory + "\\TemplateExcel\\" + "Report_Template.xlsx";
                string currentDailyData = Environment.CurrentDirectory + "\\Report\\" + DateTime.Now.ToString("dd-MM-yyyy") + "_Report" + ".xlsx";
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
