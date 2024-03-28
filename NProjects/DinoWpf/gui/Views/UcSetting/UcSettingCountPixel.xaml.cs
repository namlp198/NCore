using DinoWpf.Models.ParameterModel;
using NpcCore.Wpf.Struct_Vision;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DinoWpf.Views.UcSetting
{
    public struct ROIInfos
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Algorithm { get; set; }
        public bool Rotations { get; set; }
        public int Priority { get; set; }
    }
    /// <summary>
    /// Interaction logic for UcSettingCountPixel.xaml
    /// </summary>
    public partial class UcSettingCountPixel : UserControl, INotifyPropertyChanged
    {
        public log4net.ILog Logger { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<ROIInfos> _lstRoiInfos = new List<ROIInfos>();
        private ROIInfos _roiInfosSelected = new ROIInfos();
        private List<CParamCntPxlAlgorithm> _lstParamCntPxl = new List<CParamCntPxlAlgorithm>();
        private CParamCntPxlAlgorithm _paramCntPxlSelected = new CParamCntPxlAlgorithm();
        public UcSettingCountPixel()
        {
            InitializeComponent();
            this.DataContext = this;

            _lstRoiInfos.Clear();
            List<ROIInfos> rOIInfos = new List<ROIInfos>();
            _roiInfosSelected.Id = "ROI";
            _roiInfosSelected.Name = "count_pixel";
            _roiInfosSelected.Type = "rectangle";
            _roiInfosSelected.Algorithm = "CountPixel";
            _roiInfosSelected.Rotations = true;
            _roiInfosSelected.Priority = 2;
            rOIInfos.Add(_roiInfosSelected);
            ListROIInfos = rOIInfos;

            _lstParamCntPxl.Clear();
            List<CParamCntPxlAlgorithm> cParamCntPxlAlgorithms = new List<CParamCntPxlAlgorithm>();
            _paramCntPxlSelected.m_nThresholdGrayMin = 0;
            _paramCntPxlSelected.m_nThresholdGrayMax = 0;
            _paramCntPxlSelected.m_nNumberOfPxlMin = 0;
            _paramCntPxlSelected.m_nThresholdGrayMax = 0;
            _paramCntPxlSelected.m_nROIX = 0;
            _paramCntPxlSelected.m_nROIY = 0;
            _paramCntPxlSelected.m_nROIWidth = 0;
            _paramCntPxlSelected.m_nROIHeight = 0;
            _paramCntPxlSelected.m_dROIAngleRotate = 0.0;
            cParamCntPxlAlgorithms.Add(_paramCntPxlSelected);
            ListParamCntPxl = cParamCntPxlAlgorithms;
        }

        #region Properties
        public List<ROIInfos> ListROIInfos
        {
            get => _lstRoiInfos;
            set
            {
                if(SetProperty(ref _lstRoiInfos, value))
                {

                }
            }
        }
        public ROIInfos ROIInfosSelected
        {
            get => _roiInfosSelected;
            set
            {
                if(SetProperty(ref _roiInfosSelected, value))
                {

                }
            }
        }
        public List<CParamCntPxlAlgorithm> ListParamCntPxl
        {
            get => _lstParamCntPxl;
            set => _lstParamCntPxl = value;
        }
        public CParamCntPxlAlgorithm ParamCntPxlSelected
        {
            get => _paramCntPxlSelected;
            set => _paramCntPxlSelected = value;
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
    }
}
