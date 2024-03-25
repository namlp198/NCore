using DinoWpf.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
using KisToolkit;
using DinoWpf.Command;
using Kis.Toolkit;
using System.Xml;
using System.Collections.ObjectModel;
using DinoWpf.Models;
using DinoWpf.ViewModels;

namespace DinoWpf.Views
{
    /// <summary>
    /// Interaction logic for NewJobView.xaml
    /// </summary>
    public partial class NewJobView : Window, INotifyPropertyChanged
    {
        public log4net.ILog Logger { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int _maxCamSupport = 8;
        private int[] _arrCbbCam = new int[_maxCamSupport] { 1, 2, 3, 4, 5, 6, 7, 8 };

        private string _numberOfCamInJob = string.Empty;
        private List<CameraModel> _cameraInfos = new List<CameraModel>();
        private CameraModel _cameraInfoSelected = new CameraModel();

        private XmlManagement _xmlManagement = new XmlManagement();
        public NewJobView()
        {
            InitializeComponent();

            cbbNumberOfCam.ItemsSource = _arrCbbCam;

            this.DataContext = this;
        }

        #region Properties
        public string NumberOfCameraInJob
        {
            get => _numberOfCamInJob;
            set
            {
                if (SetProperty(ref _numberOfCamInJob, value))
                {
                    CameraInfos.Clear();
                    int numberOfCam = Convert.ToInt32(_numberOfCamInJob);

                    List<CameraModel> cameraInfos = new List<CameraModel>();
                    for (int i = 0; i < numberOfCam; i++)
                    {
                        CameraModel cameraInfo = new CameraModel();
                        cameraInfo.Id = i;
                        cameraInfo.Name = string.Empty;
                        cameraInfo.InterfaceType = string.Empty;
                        cameraInfo.SensorType = string.Empty;
                        cameraInfo.Channels = 1;
                        cameraInfo.Manufacturer = string.Empty;
                        cameraInfo.FrameWidth = 0;
                        cameraInfo.FrameHeight = 0;
                        cameraInfo.SerialNumber = string.Empty;
                        cameraInfos.Add(cameraInfo);
                    }
                    CameraInfos = cameraInfos;
                    Logger.Info("Initialize camera list!");
                }
            }
        }
        public List<CameraModel> CameraInfos
        {
            get => _cameraInfos;
            set
            {
                if (SetProperty(ref _cameraInfos, value))
                {

                }
            }
        }

        public CameraModel CameraInfoSelected
        {
            get => _cameraInfoSelected;
            set
            {
                if(SetProperty(ref _cameraInfoSelected, value))
                {
                    CameraModel cameraInfoModel = CameraInfos.Where((x) => x.Id == _cameraInfoSelected.Id).ElementAt(0);
                    if (cameraInfoModel != null)
                    {
                        cameraInfoModel = _cameraInfoSelected;
                    }
                }
            }
        }
        #endregion

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

        private void cbbNumberOfCam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int index = (int)cbbNumberOfCam.SelectedItem;
            //NumberOfCameraInJob = index;

            //CameraInfos.Clear();

            //List<CameraInfo> cameraInfos = new List<CameraInfo>();
            //for (int i = 0; i < NumberOfCameraInJob; i++)
            //{
            //    CameraInfo cameraInfo = new CameraInfo();
            //    cameraInfo.CameraId = i;
            //    cameraInfo.CameraName = string.Empty;
            //    cameraInfo.CameraType = string.Empty;
            //    cameraInfo.Manufacturer = string.Empty;
            //    cameraInfo.FrameWidth = 0;
            //    cameraInfo.FrameHeight = 0;
            //    cameraInfo.SerialNumber = string.Empty;
            //    cameraInfos.Add(cameraInfo);
            //}
            //CameraInfos = cameraInfos;

            //dgParamCamera.ItemsSource = CameraInfos;
        }

        private void btnCreateJob_Click(object sender, RoutedEventArgs e)
        {
            if (txtJobName.Text == string.Empty)
                return;

            string jobName = txtJobName.Text + ".xml";
            string xmlFilePath = CommonDefines.JobXmlPath + jobName;
            if (_xmlManagement.Create(xmlFilePath, sStartElementName: "Job"))
            {
                _xmlManagement.Load(xmlFilePath);
                XmlNode nodeJob = _xmlManagement.SelectSingleNode("//Job");
                if (nodeJob != null)
                {
                    _xmlManagement.AddAttributeToNode(nodeJob, "id", "0");
                    _xmlManagement.AddAttributeToNode(nodeJob, "name", jobName.Substring(0, jobName.Length - 4));
                    _xmlManagement.AddAttributeToNode(nodeJob, "numberOfCamera", NumberOfCameraInJob);
                }

                foreach (var cameraInfo in CameraInfos)
                {
                    XmlNode nodeCam = _xmlManagement.AddChildNode(nodeJob, "Camera");
                    if (nodeCam != null)
                    {
                        _xmlManagement.AddAttributeToNode(nodeCam, "id", cameraInfo.Id + "");
                        _xmlManagement.AddAttributeToNode(nodeCam, "name", cameraInfo.Name);
                        _xmlManagement.AddAttributeToNode(nodeCam, "interfaceType", cameraInfo.InterfaceType);
                        _xmlManagement.AddAttributeToNode(nodeCam, "sensorType", cameraInfo.SensorType);
                        _xmlManagement.AddAttributeToNode(nodeCam, "channels", cameraInfo.Channels +"");
                        _xmlManagement.AddAttributeToNode(nodeCam, "manufacturer", cameraInfo.Manufacturer);
                        _xmlManagement.AddAttributeToNode(nodeCam, "frameWidth", cameraInfo.FrameWidth + "");
                        _xmlManagement.AddAttributeToNode(nodeCam, "frameHeight", cameraInfo.FrameHeight + "");
                        _xmlManagement.AddAttributeToNode(nodeCam, "serialNumber", cameraInfo.SerialNumber);
                    }
                }

                if(_xmlManagement.Save())
                {
                    MainViewModel.Instance.GetAllJobNames();
                    this.Close();
                }
            }
        }

        public ICommand CreateJob { get; }
    }
}
