using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window, INotifyPropertyChanged
    {
        #region Sys Config
        private string m_sRecipePath;
        private string m_sModel;
        private string m_sCOMPort;
        private int m_nUsePCControl;

        public string RecipePath
        {
            get { return m_sRecipePath; }
            set
            {
                if (SetProperty(ref m_sRecipePath, value))
                {

                }
            }
        }
        public string Model
        {
            get { return m_sModel; }
            set
            {
                if (SetProperty(ref m_sModel, value))
                {

                }
            }
        }
        public string COMPort
        {
            get { return m_sCOMPort; }
            set
            {
                if (SetProperty(ref m_sCOMPort, value))
                {

                }
            }
        }
        public int UsePCControl
        {
            get { return m_nUsePCControl; }
            set
            {
                if (SetProperty(ref m_nUsePCControl, value))
                {

                }
            }
        }

        #endregion

        #region Cam Config
        private string m_sCameraName;
        private string m_sInterfaceType;
        private string m_sSensorType;
        private int m_nChannels;
        private string m_sManufacturer;
        private int m_nFrameWidth;
        private int m_nFrameHeight;
        private string m_sSerialNumber;
        private string m_sImageSavePath;
        private string m_sImageTemplatePath;
        private string m_sRecipeName;

        public string CameraName
        {
            get { return m_sCameraName; }
            set
            {
                if (SetProperty(ref m_sCameraName, value))
                {

                }
            }
        }
        public string InterfaceType
        {
            get { return m_sInterfaceType; }
            set
            {
                if (SetProperty(ref m_sInterfaceType, value))
                {

                }
            }
        }
        public string SensorType
        {
            get { return m_sSensorType; }
            set
            {
                if (SetProperty(ref m_sSensorType, value))
                {

                }
            }
        }
        public int Channels
        {
            get { return m_nChannels; }
            set
            {
                if (SetProperty(ref m_nChannels, value))
                {

                }
            }
        }
        public string Manufacturer
        {
            get { return m_sManufacturer; }
            set
            {
                if (SetProperty(ref m_sManufacturer, value))
                {

                }
            }
        }
        public int FrameWidth
        {
            get { return m_nFrameWidth; }
            set
            {
                if (SetProperty(ref m_nFrameWidth, value))
                {

                }
            }
        }
        public int FrameHeight
        {
            get { return m_nFrameHeight; }
            set
            {
                if (SetProperty(ref m_nFrameHeight, value))
                {

                }
            }
        }
        public string SerialNumber
        {
            get { return m_sSerialNumber; }
            set
            {
                if (SetProperty(ref m_sSerialNumber, value))
                {

                }
            }
        }
        public string ImageSavePath
        {
            get { return m_sImageSavePath; }
            set
            {
                if (SetProperty(ref m_sImageSavePath, value))
                {

                }
            }
        }
        public string ImageTemplatePath
        {
            get { return m_sImageTemplatePath; }
            set
            {
                if (SetProperty(ref m_sImageTemplatePath, value))
                {

                }
            }
        }
        public string RecipeName
        {
            get { return m_sRecipeName; }
            set
            {
                if (SetProperty(ref m_sRecipeName, value))
                {

                }
            }
        }
        #endregion
        public SettingsView()
        {
            InitializeComponent();

            this.DataContext = this;

            RecipePath = InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_sRecipePath;
            Model = InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_sModel;
            COMPort = InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_sCOMPort;
            UsePCControl = InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_bUsePCControl;

            for(int i = 0; i < ConstDefine.MAX_CAMERA_INSP_COUNT; i++)
            {
                CameraName = InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i].m_sName;
                InterfaceType = InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i].m_sInterfaceType;
                SensorType = InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i].m_sSensorType;
                Channels = InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i].m_nChannels;
                Manufacturer = InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i].m_sManufacturer;
                FrameWidth = InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i].m_nFrameWidth;
                FrameHeight = InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i].m_nFrameHeight;
                SerialNumber = InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i].m_sSerialNumber;
                ImageSavePath = InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i].m_sImageSavePath;
                ImageTemplatePath = InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i].m_sImageTemplatePath;
                RecipeName = InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[i].m_sRecipeName;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
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

        private void btnSaveSysConfig_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_sRecipePath = RecipePath;
            InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_sModel = Model;
            InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_sCOMPort = COMPort;
            InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_bUsePCControl = UsePCControl;
            if(InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.SaveSysConfigurations(ref InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs))
            {
                MessageBox.Show("saved successfully!");
            }
        }
    }
}
