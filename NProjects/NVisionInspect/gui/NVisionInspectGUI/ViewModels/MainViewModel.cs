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
using NVisionInspectGUI.Views.CamView;
using NVisionInspectGUI.Views.UcViews;
using System.Security.Principal;

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
        public MainViewModel(Dispatcher dispatcher, MainView mainView, SettingViewModel settingVM)
        {
            // construct a instance of MainViewModel
            if (_instance == null) _instance = this;
            else return;

            _dispatcher = dispatcher;
            m_ucMainView = mainView;
            m_settingVM = settingVM;

            m_columnNameList = new List<string>() { "A", "B", "C", "D", "E", "F" };
            StartAppExcel();
            Csv_Manager.Instance.Initialize();

            this.SelectRunViewCmd = new SelectRunViewCmd();
            this.SelectSettingViewCmd = new SelectSettingViewCmd();
            this.SelectMachineModeCmd = new SelectMachineModeCmd();
            this.ShowLoginViewCmd = new ShowLoginViewCmd();
            this.ShowRecipeListCmd = new ShowRecipeListCmd();
            this.ShowReportViewCmd = new ShowReportViewCmd();
            this.ExportExcelFileCmd = new ExportExcelFileCmd();
            this.InitializeCmd = new InitializeCmd();

            InterfaceManager.Instance.m_processorManager.Initialize();

            // Load system setting
            SettingVM.LoadSystemSettings();
            if(InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sRole.CompareTo("superadmin") == 0)
            {
                LoginSystemEventHandle(emLoginStatus.LoginStatus_Success, emRole.Role_SuperAdmin);
            }


            // read number of camera inspect
            int nNumberOfCamInsp = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_nNumberOfInspectionCamera;
            SettingVM.CameraCount = nNumberOfCamInsp;

            // Read camera list from camera setting
            List<string> lstCameras = new List<string>();
            for (int nCamIdx = 0; nCamIdx < nNumberOfCamInsp; nCamIdx++)
            {
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadCameraSettings(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx], nCamIdx);
                SettingVM.LoadCamerasSetting(nCamIdx);

                string sCamType = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sManufacturer;

                string sCamera = "Cam " + (nCamIdx + 1) + " - " + sCamType;
                lstCameras.Add(sCamera);
            }

            AddSumCamViewToRunView(nNumberOfCamInsp);

            // Load Recipe
            SettingVM.LoadRecipe(nNumberOfCamInsp);

            // Load Recipe Fake Cam
            SettingVM.LoadRecipe_FakeCam();

            // Load Fake Camera Setting
            SettingVM.LoadFakeCameraSetting();

            // Add Fake Cam in the end of list camera
            lstCameras.Add("Fake Camera");

            // Load Camera List to Combobox
            SettingVM.SettingView.buffSettingPRO.CameraList = lstCameras;
            SettingVM.LoadPlcSettings();

            // Init PLC and start inspect
            if (InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSimulation == 0)
            {
                Plc_Delta_DVP.Initialize();
                SettingVM.SetAllParamPlcDelta();
                int nThreadCount = 1;
                if (InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.InspectStart(nThreadCount, nNumberOfCamInsp))
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

            LoginViewModel.LoginSystemSuccessEvent += new LoginViewModel.LoginSystemSuccess_Handler(LoginSystemEventHandle);
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

        private emMachineMode m_machineMode = emMachineMode.MachineMode_Auto;
        private emRole m_role = emRole.Role_Operator;
        private bool m_bInspRunning = false;
        private bool m_bEnableSetting = false;
        private string m_displayImage_MachineModePath = "/NpcCore.Wpf;component/Resources/Images/arrow_backward.png";
        private string m_displayImage_LoginStatusPath = "/NpcCore.Wpf;component/Resources/Images/account_2.png";
        private string m_sRecipeName = string.Empty;

        private ExcelParser m_excelParser = new ExcelParser();
        private List<string> m_columnNameList;

        private IOManager_PLC_LS m_Plc_LS = new IOManager_PLC_LS();
        private IOManager_PLC_Delta_DVP m_Plc_Delta = new IOManager_PLC_Delta_DVP();
        #endregion

        #region Properties
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
        public emMachineMode MachineMode
        {
            get => m_machineMode;
            set
            {
                if (SetProperty(ref m_machineMode, value))
                {
                }
            }
        }
        public emRole ROLE
        {
            get => m_role;
            set
            {
                if(SetProperty(ref m_role, value))
                {
                    if(m_role == emRole.Role_Operator)
                    {
                        LoginSystemEventHandle(emLoginStatus.LoginStatus_Success, emRole.Role_Operator);
                    }
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
        public string DisplayImage_LoginStatusPath
        {
            get => m_displayImage_LoginStatusPath;
            set
            {
                if (SetProperty(ref m_displayImage_LoginStatusPath, value))
                {

                }
            }
        }
        public string RecipeName
        {
            get => m_sRecipeName;
            set
            {
                if (SetProperty(ref m_sRecipeName, value))
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
                        m_ucMainView.btnSelectRecipe.Opacity = 0.3;
                        m_ucMainView.btnReport.Opacity = 0.3;
                        m_ucMainView.btnSettings.Opacity = 0.3;
                    }
                    else if (m_bInspRunning == false && (m_role == emRole.Role_Admin || m_role == emRole.Role_SuperAdmin))
                    {
                        EnableSetting = true;
                        m_ucMainView.btnSelectRecipe.Opacity = 1.0;
                        m_ucMainView.btnReport.Opacity = 1.0;
                        m_ucMainView.btnSettings.Opacity = 1.0;
                    }
                    else if (m_bInspRunning == false && m_role == emRole.Role_Operator)
                    {
                        EnableSetting = false;
                        m_ucMainView.btnSelectRecipe.Opacity = 0.3;
                        m_ucMainView.btnReport.Opacity = 0.3;
                        m_ucMainView.btnSettings.Opacity = 0.3;
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
        private RunViewModel<ISumCamVM> m_runVM;
        public RunViewModel<ISumCamVM> RunVM { get => m_runVM; set => m_runVM = value; }
        private SettingViewModel m_settingVM;
        public SettingViewModel SettingVM { get => m_settingVM; private set { } }
        #endregion

        #region Methods
        private void LoginSystemEventHandle (emLoginStatus eStatus, emRole eRole)
        {
            switch (eStatus)
            {
                case emLoginStatus.LoginStatus_Success:
                    m_role = eRole;

                    switch (eRole)
                    {
                        case emRole.Role_Operator:
                            MainView.tbLogin.Text = "LOGIN";
                            DisplayImage_LoginStatusPath = "/NpcCore.Wpf;component/Resources/Images/account_2.png";

                            EnableSetting = false;
                            m_ucMainView.btnSelectRecipe.Opacity = 0.3;
                            m_ucMainView.btnReport.Opacity = 0.3;
                            m_ucMainView.btnSettings.Opacity = 0.3;
                            break;

                        case emRole.Role_Engineer:
                        case emRole.Role_Admin:
                        case emRole.Role_SuperAdmin:
                            MainView.tbLogin.Text = "LOGOUT";
                            DisplayImage_LoginStatusPath = "/NpcCore.Wpf;component/Resources/Images/logout.png";

                            EnableSetting = true;
                            m_ucMainView.btnSelectRecipe.Opacity = 1.0;
                            m_ucMainView.btnReport.Opacity = 1.0;
                            m_ucMainView.btnSettings.Opacity = 1.0;
                            break;
                    }
                    break;
                case emLoginStatus.LoginStatus_Failed:
                    break;
            }
        }
        private void StartAppExcel()
        {
            KillAppExcel();
            OpenExcelResultFile();
        }
        private void KillAppExcel()
        {
            foreach (var process in Process.GetProcessesByName("EXCEL"))
            {
                process.Kill();
            }
        }
        private void OpenExcelResultFile()
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
        private void AddSumCamViewToRunView(int nCamCount)
        {
            if (nCamCount < 1)
                return;

            switch (nCamCount)
            {
                case 1:
                    {
                        UcSum1CameraView sum1CameraView = new UcSum1CameraView();
                        Sum1CameraViewModel sum1CameraVM = new Sum1CameraViewModel(sum1CameraView.Dispatcher, sum1CameraView);
                        sum1CameraView.DataContext = sum1CameraVM;

                        UcResultView resultView = new UcResultView();
                        ResultViewModel resultVM = new ResultViewModel(resultView.Dispatcher, resultView);
                        resultView.DataContext = resultVM;

                        UcRunView runView = new UcRunView();
                        RunViewModel<ISumCamVM> runVM = new RunViewModel<ISumCamVM>(runView.Dispatcher, runView);
                        runView.DataContext = runVM;

                        runVM.SumCamVM = sum1CameraVM;
                        runVM.ResultVM = resultVM;

                        runView.contentCamView.Content = sum1CameraView;
                        runView.contentResult.Content = resultView;

                        MainView.contentMain.Content = runView;
                        RunVM = runVM;
                    }
                    break;
                case 2:
                    {
                        UcSum2CameraView sum2CameraView = new UcSum2CameraView();
                        Sum2CameraViewModel sum2CameraVM = new Sum2CameraViewModel(sum2CameraView.Dispatcher, sum2CameraView);
                        sum2CameraView.DataContext = sum2CameraVM;

                        UcResultView resultView = new UcResultView();
                        ResultViewModel resultVM = new ResultViewModel(resultView.Dispatcher, resultView);
                        resultView.DataContext = resultVM;

                        UcRunView runView = new UcRunView();
                        RunViewModel<ISumCamVM> runVM = new RunViewModel<ISumCamVM>(runView.Dispatcher, runView);
                        runView.DataContext = runVM;

                        runVM.SumCamVM = sum2CameraVM;
                        runVM.ResultVM = resultVM;

                        runView.contentCamView.Content = sum2CameraView;
                        runView.contentResult.Content = resultView;

                        MainView.contentMain.Content = runView;
                        RunVM = runVM;
                    }
                    break;
                case 3:
                    {
                        UcSum3CameraView sum3CameraView = new UcSum3CameraView();
                        Sum3CameraViewModel sum3CameraVM = new Sum3CameraViewModel(sum3CameraView.Dispatcher, sum3CameraView);
                        sum3CameraView.DataContext = sum3CameraVM;

                        UcResultView resultView = new UcResultView();
                        ResultViewModel resultVM = new ResultViewModel(resultView.Dispatcher, resultView);
                        resultView.DataContext = resultVM;

                        UcRunView runView = new UcRunView();
                        RunViewModel<ISumCamVM> runVM = new RunViewModel<ISumCamVM>(runView.Dispatcher, runView);
                        runView.DataContext = runVM;

                        runVM.SumCamVM = sum3CameraVM;
                        runVM.ResultVM = resultVM;

                        runView.contentCamView.Content = sum3CameraView;
                        runView.contentResult.Content = resultView;

                        MainView.contentMain.Content = runView;
                        RunVM = runVM;
                    }
                    break;
                case 4:
                    {
                        UcSum4CameraView sum4CameraView = new UcSum4CameraView();
                        Sum4CameraViewModel sum4CameraVM = new Sum4CameraViewModel(sum4CameraView.Dispatcher, sum4CameraView);
                        sum4CameraView.DataContext = sum4CameraVM;

                        UcResultView resultView = new UcResultView();
                        ResultViewModel resultVM = new ResultViewModel(resultView.Dispatcher, resultView);
                        resultView.DataContext = resultVM;

                        UcRunView runView = new UcRunView();
                        RunViewModel<ISumCamVM> runVM = new RunViewModel<ISumCamVM>(runView.Dispatcher, runView);
                        runView.DataContext = runVM;

                        runVM.SumCamVM = sum4CameraVM;
                        runVM.ResultVM = resultVM;

                        runView.contentCamView.Content = sum4CameraView;
                        runView.contentResult.Content = resultView;

                        MainView.contentMain.Content = runView;
                        RunVM = runVM;
                    }
                    break;
                case 5:
                    {
                        UcSum5CameraView sum5CameraView = new UcSum5CameraView();
                        Sum5CameraViewModel sum5CameraVM = new Sum5CameraViewModel(sum5CameraView.Dispatcher, sum5CameraView);
                        sum5CameraView.DataContext = sum5CameraVM;

                        UcResultView resultView = new UcResultView();
                        ResultViewModel resultVM = new ResultViewModel(resultView.Dispatcher, resultView);
                        resultView.DataContext = resultVM;

                        UcRunView runView = new UcRunView();
                        RunViewModel<ISumCamVM> runVM = new RunViewModel<ISumCamVM>(runView.Dispatcher, runView);
                        runView.DataContext = runVM;

                        runVM.SumCamVM = sum5CameraVM;
                        runVM.ResultVM = resultVM;

                        runView.contentCamView.Content = sum5CameraView;
                        runView.contentResult.Content = resultView;

                        MainView.contentMain.Content = runView;
                        RunVM = runVM;
                    }
                    break;
                case 6:
                    {
                        UcSum6CameraView sum6CameraView = new UcSum6CameraView();
                        Sum6CameraViewModel sum6CameraVM = new Sum6CameraViewModel(sum6CameraView.Dispatcher, sum6CameraView);
                        sum6CameraView.DataContext = sum6CameraVM;

                        UcResultView resultView = new UcResultView();
                        ResultViewModel resultVM = new ResultViewModel(resultView.Dispatcher, resultView);
                        resultView.DataContext = resultVM;

                        UcRunView runView = new UcRunView();
                        RunViewModel<ISumCamVM> runVM = new RunViewModel<ISumCamVM>(runView.Dispatcher, runView);
                        runView.DataContext = runVM;

                        runVM.SumCamVM = sum6CameraVM;
                        runVM.ResultVM = resultVM;

                        runView.contentCamView.Content = sum6CameraView;
                        runView.contentResult.Content = resultView;

                        MainView.contentMain.Content = runView;
                        RunVM = runVM;
                    }
                    break;
                case 7:
                    {
                        UcSum7CameraView sum7CameraView = new UcSum7CameraView();
                        Sum7CameraViewModel sum7CameraVM = new Sum7CameraViewModel(sum7CameraView.Dispatcher, sum7CameraView);
                        sum7CameraView.DataContext = sum7CameraVM;

                        UcResultView resultView = new UcResultView();
                        ResultViewModel resultVM = new ResultViewModel(resultView.Dispatcher, resultView);
                        resultView.DataContext = resultVM;

                        UcRunView runView = new UcRunView();
                        RunViewModel<ISumCamVM> runVM = new RunViewModel<ISumCamVM>(runView.Dispatcher, runView);
                        runView.DataContext = runVM;

                        runVM.SumCamVM = sum7CameraVM;
                        runVM.ResultVM = resultVM;

                        runView.contentCamView.Content = sum7CameraView;
                        runView.contentResult.Content = resultView;

                        MainView.contentMain.Content = runView;
                        RunVM = runVM;
                    }
                    break;
                case 8:
                    {
                        UcSum8CameraView sum8CameraView = new UcSum8CameraView();
                        Sum8CameraViewModel sum8CameraVM = new Sum8CameraViewModel(sum8CameraView.Dispatcher, sum8CameraView);
                        sum8CameraView.DataContext = sum8CameraVM;

                        UcResultView resultView = new UcResultView();
                        ResultViewModel resultVM = new ResultViewModel(resultView.Dispatcher, resultView);
                        resultView.DataContext = resultVM;

                        UcRunView runView = new UcRunView();
                        RunViewModel<ISumCamVM> runVM = new RunViewModel<ISumCamVM>(runView.Dispatcher, runView);
                        runView.DataContext = runVM;

                        runVM.SumCamVM = sum8CameraVM;
                        runVM.ResultVM = resultVM;

                        runView.contentCamView.Content = sum8CameraView;
                        runView.contentResult.Content = resultView;

                        MainView.contentMain.Content = runView;
                        RunVM = runVM;
                    }
                    break;
                default:
                    break;
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
        public string ExcelFilePath
        {
            get;
            set;
        }
        #endregion

        #region Command
        public ICommand SelectRunViewCmd { get; }
        public ICommand SelectSettingViewCmd { get; }
        public ICommand SelectMachineModeCmd { get; }
        public ICommand ShowRecipeListCmd { get; }
        public ICommand ShowLoginViewCmd { get; }
        public ICommand ShowReportViewCmd { get; }
        public ICommand ExportExcelFileCmd {  get; }
        public ICommand InitializeCmd { get; }
        #endregion
    }
}
