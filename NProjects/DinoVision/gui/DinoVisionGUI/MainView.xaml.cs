#define GUI_DRAW
#undef GUI_DRAW

using NCore.Wpf.UcZoomBoxViewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DinoVisionGUI
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window, INotifyPropertyChanged
    {
        private static readonly object objLock = new object();

        private CameraStreamingController m_CameraStreaming;
        private IOManagement_PLC_Pana m_IOManagement;

        private int m_nCamIdx;
        private bool m_bResultOKNG;
        private int m_nCountOK;
        private int m_nCountNG;
        private int m_nCountTotal;
        private double m_dProcessTime;
        public MainView()
        {
            InitializeComponent();

            this.DataContext = this;

            // init Inspect Processor
            InterfaceManager.Instance.JigInspProcessorManager.Initialize();
            // read config
            InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.LoadSysConfigurations(ref InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs);
            // init a IO manager
            m_IOManagement = new IOManagement_PLC_Pana(m_nCamIdx);
            m_IOManagement.NewImageUpdate += M_IOManagement_NewImageUpdate;
            InterfaceManager.InspectionComplete += InterfaceManager_InspectionComplete;

            ucZoomBoxViewer.CameraIndex = 0;
            ucZoomBoxViewer.IsVisibleRecipeButton = false;
            ucZoomBoxViewer.UcSingleGrab += UcZoomBoxViewer_UcSingleGrab;
            ucZoomBoxViewer.UcContinuousGrab += UcZoomBoxViewer_UcContinuousGrab;
            ucZoomBoxViewer.UcLoadImage += UcZoomBoxViewer_UcLoadImage;
            ucZoomBoxViewer.SwitchMachineMode += UcZoomBoxViewer_SwitchMachineMode;

            // init a view for show image result
            m_nCamIdx = ucZoomBoxViewer.CameraIndex;
            m_CameraStreaming = new CameraStreamingController(640, 480, ucZoomBoxViewer, m_nCamIdx, ModeView.Color);

            // 0: inspect, 1: Live, 2: Manual, 3: Simulator
            // select inspect mode - view mode will be from color to mono
            ucZoomBoxViewer.MachineModeSelected = ucZoomBoxViewer.MachineModeList[0];

            for(int i = 0; i < ConstDefine.MAX_CAMERA_INSP_COUNT; i++)
            {
                InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.LoadCamConfigurations(i, ref InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i]);
            }
            for (int i = 0; i < ConstDefine.MAX_CAMERA_INSP_COUNT; i++)
            {
                InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.LoadRecipe(i, ref InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i]);
            }

            LoginView.LoginSystemSuccess += LoginView_LoginSystemSuccess;
        }

        private void LoginView_LoginSystemSuccess(LoginView login)
        {
            MessageBox.Show("Login success.");
        }

        private async void InterfaceManager_InspectionComplete()
        {
            InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.GetInspectionResult(m_nCamIdx,
                                   ref InterfaceManager.Instance.JigInspProcessorManager.JigInspResults);

            m_IOManagement.IsInspectCompleted = InterfaceManager.Instance.JigInspProcessorManager.JigInspResults.m_bInspectCompleted == 1? true : false;
            m_IOManagement.IsJudgeOKNG = m_bResultOKNG = InterfaceManager.Instance.JigInspProcessorManager.JigInspResults.m_bResultOKNG == 1? true : false;

            CountTotal++;
            await this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (m_bResultOKNG)
                {
                    ucZoomBoxViewer.IsAllInspectionOK = true;
                    lbResult.Content = "OK";
                    lbResult.Background = Brushes.Green;
                    CountOK++;
                }
                else
                {
                    ucZoomBoxViewer.IsAllInspectionOK = false;
                    lbResult.Content = "NG";
                    lbResult.Background = Brushes.Red;
                    CountNG++;
                }
            }));
            ucZoomBoxViewer.TemplateMatchingResult = InterfaceManager.Instance.JigInspProcessorManager.JigInspResults.m_TemplateMatchingResult;
#if GUI_DRAW
            ucZoomBoxViewer.BufferView = InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.GetResultBufferImageDinoCam(m_nCamIdx);
#else
            ucZoomBoxViewer.BufferView = InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.GetResultBufferImageDinoCam_BGR(m_nCamIdx);
#endif
            await ucZoomBoxViewer.UpdateImage();
        }

        private async void M_IOManagement_NewImageUpdate(IOManagement_PLC_Pana sender)
        {
            ucZoomBoxViewer.BufferView = InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.GetBufferDinoCam(m_nCamIdx);
            await ucZoomBoxViewer.UpdateImage();

            // simulator inspection completed
            Thread.Sleep(100);
            lock (objLock)
            {
                m_IOManagement.IsInspectCompleted = true;
                m_IOManagement.IsJudgeOKNG = true;
            }
        }

        private void UcZoomBoxViewer_SwitchMachineMode(object sender, RoutedEventArgs e)
        {
            SwitchMachineMode();
        }

        private void SwitchMachineMode()
        {
            if (ucZoomBoxViewer.MachineMode == NCore.Wpf.UcZoomBoxViewer.EMachineMode.EMachineMode_Inspect)
            {
                m_IOManagement.ResetAllInOut();
                Thread.Sleep(100);

#if GUI_DRAW
                ucZoomBoxViewer.ModeView = ModeView.Mono;
#else
                ucZoomBoxViewer.ModeView = ModeView.Color;
#endif
                Task.Run(new Action(() => m_IOManagement.StartInspect()));
                return;
            }

            ucZoomBoxViewer.ModeView = ModeView.Color;
            Task.Run( new Action(() => m_IOManagement.StopInspect()));

            Thread.Sleep(100);
            m_IOManagement.ResetAllInOut();
            m_IOManagement.ManipulateWithPLCContact(EIOMode.IOMode_Write, IOManagement_PLC_Pana._dataSendToWrite,
                                     contactName: "R", contactIdx: "2", level: "1");
            Thread.Sleep(100);
        }

        private void UcZoomBoxViewer_UcLoadImage(object sender, RoutedEventArgs e)
        {

        }

        private async void UcZoomBoxViewer_UcContinuousGrab(object sender, RoutedEventArgs e)
        {
            if (!ucZoomBoxViewer.IsStreamming)
            {
                ucZoomBoxViewer.IsStreamming = true;
                await m_CameraStreaming.ContinuousGrab(CameraType.UsbCam);
            }
            else
            {
                ucZoomBoxViewer.IsStreamming = false;
                await m_CameraStreaming.StopGrab(CameraType.UsbCam);
            }
        }

        private async void UcZoomBoxViewer_UcSingleGrab(object sender, RoutedEventArgs e)
        {
            await m_CameraStreaming.SingleGrab();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            InterfaceManager.Instance.JigInspProcessorManager.Destroy();
        }

        #region Implement INotifyPropertyChanged
        //
        // Summary:
        //     Occurs when a property value changes.
        public event PropertyChangedEventHandler PropertyChanged;

        //
        // Summary:
        //     Checks if a property already matches a desired value. Sets the property and notifies
        //     listeners only when necessary.
        //
        // Parameters:
        //   storage:
        //     Reference to a property with both getter and setter.
        //
        //   value:
        //     Desired value for the property.
        //
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support CallerMemberName.
        //
        // Type parameters:
        //   T:
        //     Type of the property.
        //
        // Returns:
        //     True if the value was changed, false if the existing value matched the desired
        //     value.
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
        //
        // Summary:
        //     Checks if a property already matches a desired value. Sets the property and notifies
        //     listeners only when necessary.
        //
        // Parameters:
        //   storage:
        //     Reference to a property with both getter and setter.
        //
        //   value:
        //     Desired value for the property.
        //
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support CallerMemberName.
        //
        //   onChanged:
        //     Action that is called after the property value has been changed.
        //
        // Type parameters:
        //   T:
        //     Type of the property.
        //
        // Returns:
        //     True if the value was changed, false if the existing value matched the desired
        //     value.
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);
            return true;
        }
        //
        // Summary:
        //     Raises this object's PropertyChanged event.
        //
        // Parameters:
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support System.Runtime.CompilerServices.CallerMemberNameAttribute.
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        //
        // Summary:
        //     Raises this object's PropertyChanged event.
        //
        // Parameters:
        //   args:
        //     The PropertyChangedEventArgs
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }
        #endregion

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            if(ucZoomBoxViewer.MachineMode == EMachineMode.EMachineMode_Inspect)
            {
                MessageBox.Show("Could not setting when the machine is in Inspect Mode!");
                return;
            }
            SettingsView settingsView = new SettingsView();
            settingsView.ShowDialog();
        }

        #region Properties
        public int CountOK
        {
            get => m_nCountOK;
            set
            {
                if(SetProperty(ref  m_nCountOK, value)) { }
            }
        }
        public int CountNG
        {
            get => m_nCountNG;
            set
            {
                if (SetProperty(ref m_nCountNG, value)) { }
            }
        }
        public int CountTotal
        {
            get => m_nCountTotal;
            set
            {
                if (SetProperty(ref m_nCountTotal, value)) { }
            }
        }
        public double ProcessTime
        {
            get => m_dProcessTime;
            set
            {
                if (SetProperty(ref m_dProcessTime, value)) { }
            }
        }
        #endregion

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginView loginView = new LoginView();
            loginView.ShowDialog();
        }
    }
}
