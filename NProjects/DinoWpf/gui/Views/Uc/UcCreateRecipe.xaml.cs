using DinoWpf.Commons;
using DinoWpf.ViewModels;
using DinoWpf.Views.UcSetting;
using Kis.Toolkit;
using NCore.Wpf.NUcBufferViewer;
using Npc.Foundation.Base;
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
using System.Xml;
using NpcCore.Wpf.Struct_Vision;
using CLocatorToolResult = NpcCore.Wpf.Struct_Vision.CLocatorToolResult;
using RectForTrainLocTool = NpcCore.Wpf.Struct_Vision.RectForTrainLocTool;
using System.Xml.Linq;

namespace DinoWpf.Views.Uc
{
    /// <summary>
    /// Interaction logic for UcCreateRecipe.xaml
    /// </summary>
    public partial class UcCreateRecipe : UserControl, INotifyPropertyChanged
    {
        private ToolSelected _toolSelected = ToolSelected.None;
        private XmlManagement _xmlManagement = new XmlManagement();
        private string _xmlPath = string.Empty;
        private int _locatorIdx = 0;
        private int _roiIdx = 0;
        private CameraStreamingController _cameraStreamingController;

        //Declare views
        private UcSettingLocatorTool _ucSettingLocator;
        private UcSettingROITool _ucSettingROITool;
        public UcCreateRecipe()
        {
            InitializeComponent();

            this.DataContext = this;
            ucCreateRecipe.SettingLocator += UcCreateRecipe_SettingLocator;
            ucCreateRecipe.SettingROI += UcCreateRecipe_SettingROI;
            ucCreateRecipe.UcTrainLocator += UcCreateRecipe_UcTrainLocator;
            ucCreateRecipe.UcSelectedROI += UcCreateRecipe_UcSelectedROI;
            ucCreateRecipe.UcSaveImage += UcCreateRecipe_UcSaveImage;

            ucCreateRecipe.UcContinuousGrab += UcCreateRecipe_UcContinuousGrab;
            ucCreateRecipe.UcSingleGrab += UcCreateRecipe_UcSingleGrab;

            _xmlPath = CommonDefines.JobXmlPath + MainViewModel.Instance.JobSelectedItem + ".xml";
            _xmlManagement.Load(_xmlPath);

            // add camera index into Camera List:
            ucCreateRecipe.CameraIndex = MainViewModel.Instance.CameraIdSelected;
            if (ucCreateRecipe.CameraIndex == -1)
                return;
            ucCreateRecipe.CameraList.Add("Cam " + MainViewModel.Instance.CameraIdSelected);
            ucCreateRecipe.CameraName = ucCreateRecipe.CameraList[1];

            // read info from job selected
            int frameWidth = MainViewModel.Instance.JobSelected.Cameras[ucCreateRecipe.CameraIndex].FrameWidth;
            int frameHeight = MainViewModel.Instance.JobSelected.Cameras[ucCreateRecipe.CameraIndex].FrameHeight;
            ModeView modeView = MainViewModel.Instance.JobSelected.Cameras[ucCreateRecipe.CameraIndex].SensorType == "color" ? ModeView.Color : ModeView.Mono;

            // initialize CameraStreamingController object
            _cameraStreamingController = new CameraStreamingController(frameWidth, frameHeight, ucCreateRecipe, ucCreateRecipe.CameraIndex, modeView);
        }

        private async void UcCreateRecipe_UcSingleGrab(object sender, RoutedEventArgs e)
        {
            ucCreateRecipe.ViewMode = NpcCore.Wpf.Controls.ViewMode.ViewMode_CreateRecipe;
            // set trigger mode and trigger source
            InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.SetTriggerModeHikCam(ucCreateRecipe.CameraIndex, (int)ucCreateRecipe.TriggerMode);
            InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.SetTriggerSourceHikCam(ucCreateRecipe.CameraIndex, (int)ucCreateRecipe.TriggerSoure);

            InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.ContinuousGrabHikCam(ucCreateRecipe.CameraIndex);
            // send single grab cmd to back end
            if (InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.SingleGrabHikCam(ucCreateRecipe.CameraIndex))
                // start aync method for get buffer 
                await _cameraStreamingController.SingleGrab();
        }

        private async void UcCreateRecipe_UcContinuousGrab(object sender, RoutedEventArgs e)
        {
            ucCreateRecipe.ViewMode = NpcCore.Wpf.Controls.ViewMode.ViewMode_CreateRecipe;
            // set trigger mode and trigger source
            InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.SetTriggerModeHikCam(ucCreateRecipe.CameraIndex, (int)ucCreateRecipe.TriggerMode);
            InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.SetTriggerSourceHikCam(ucCreateRecipe.CameraIndex, (int)ucCreateRecipe.TriggerSoure);

            // send continuous grab cmd to back end
            if (ucCreateRecipe.CamState != ECamState.Started)
            {
                if (InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.ContinuousGrabHikCam(ucCreateRecipe.CameraIndex))
                {
                    ucCreateRecipe.CamState = ECamState.Started;
                    // start asyn method for get buffer
                    await _cameraStreamingController.ContinuousGrab(CameraType.Hik);
                }
                else
                    ucCreateRecipe.CamState = ECamState.Stoped;
            }
            else
            {
                if (InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.StopGrabHikCam(ucCreateRecipe.CameraIndex))
                {
                    ucCreateRecipe.CamState = ECamState.Stoped;
                    await _cameraStreamingController.StopGrab(CameraType.Hik);
                }
                else
                    ucCreateRecipe.CamState = ECamState.Started;
            }
        }

        private void UcCreateRecipe_UcSaveImage(object sender, RoutedEventArgs e)
        {
        }

        private void UcCreateRecipe_UcSelectedROI(object sender, RoutedEventArgs e)
        {
            switch (_ucSettingROITool.AlgorithmSelected)
            {
                case Algorithms.CountPixel:
                    CParamCntPxlAlgorithm cParamCntPxlAlgorithm = new CParamCntPxlAlgorithm();
                    cParamCntPxlAlgorithm.m_nROIX = (int)ucCreateRecipe.ROISelected.X;
                    cParamCntPxlAlgorithm.m_nROIY = (int)ucCreateRecipe.ROISelected.Y;
                    cParamCntPxlAlgorithm.m_nROIWidth = (int)ucCreateRecipe.ROISelected.Width;
                    cParamCntPxlAlgorithm.m_nROIHeight = (int)ucCreateRecipe.ROISelected.Height;
                    cParamCntPxlAlgorithm.m_dROIAngleRotate = ucCreateRecipe.AngleRotate;
                    cParamCntPxlAlgorithm.m_nThresholdGrayMin = 100;
                    cParamCntPxlAlgorithm.m_nThresholdGrayMax = 200;
                    cParamCntPxlAlgorithm.m_nNumberOfPxlMin = 3000;
                    cParamCntPxlAlgorithm.m_nNumberOfPxlMax = 8000;
                    InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.CountPixelAlgorithm_Train(ucCreateRecipe.CameraIndex, ref cParamCntPxlAlgorithm);
                    break;
                case Algorithms.CalculateArea:
                    break;
            }
        }

        private void UcCreateRecipe_UcTrainLocator(object sender, RoutedEventArgs e)
        {
            ucCreateRecipe.ListLocToolRes.Clear();

            RectForTrainLocTool rectForTrainLocTool = new RectForTrainLocTool();
            rectForTrainLocTool.m_nRectIn_X = (int)ucCreateRecipe.RectInSide.X;
            rectForTrainLocTool.m_nRectIn_Y = (int)ucCreateRecipe.RectInSide.Y;
            rectForTrainLocTool.m_nRectIn_Width = (int)ucCreateRecipe.RectInSide.Width;
            rectForTrainLocTool.m_nRectIn_Height = (int)ucCreateRecipe.RectInSide.Height;
            rectForTrainLocTool.m_nRectOut_X = (int)ucCreateRecipe.RectOutSide.X;
            rectForTrainLocTool.m_nRectOut_Y = (int)ucCreateRecipe.RectOutSide.Y;
            rectForTrainLocTool.m_nRectOut_Width = (int)ucCreateRecipe.RectOutSide.Width;
            rectForTrainLocTool.m_nRectOut_Height = (int)ucCreateRecipe.RectOutSide.Height;
            rectForTrainLocTool.m_dMatchingRateLimit = double.Parse(_ucSettingLocator.txtMatchingRate.Text.Trim());

            if (_ucSettingLocator != null)
            {
                List<RectForTrainLocTool> lstRectForTrain = new List<RectForTrainLocTool>();
                lstRectForTrain.Add(rectForTrainLocTool);
                _ucSettingLocator.ListRectForTrainLocTool = lstRectForTrain;
            }

            if (InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.TrainLocator_TemplateMatching(ucCreateRecipe.CameraIndex, ref rectForTrainLocTool))
            {
                CLocatorToolResult cLocatorToolResult = new CLocatorToolResult();
                List<CLocatorToolResult> lstLocatorToolResult = new List<CLocatorToolResult>();
                if(InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.GetDataTrained_TemplateMatching(ucCreateRecipe.CameraIndex, ref cLocatorToolResult))
                {
                    //_cameraStreamingController.GetTemplateImage();
                    ucCreateRecipe.DataTrain[0] = cLocatorToolResult.m_nX;
                    ucCreateRecipe.DataTrain[1] = cLocatorToolResult.m_nY;

                    lstLocatorToolResult.Add(cLocatorToolResult);
                    ucCreateRecipe.ListLocToolRes = lstLocatorToolResult;

                    if (_ucSettingLocator != null)
                    {
                        _ucSettingLocator.ListLocToolRes = lstLocatorToolResult;
                    }
                }
            }
        }

        private void UcCreateRecipe_SettingLocator(object sender, RoutedEventArgs e)
        {
            _ucSettingLocator = new UcSettingLocatorTool();
            _ucSettingLocator.LocatorId = "Loc" + _locatorIdx;
            _ucSettingLocator.SaveParamLocatorTool += UcSettingLocator_SaveParam;
            contentSetting.Content = _ucSettingLocator;

            ToolSelected = ToolSelected.LocatorTool;
        }

        private void UcSettingLocator_SaveParam(object sender, RoutedEventArgs e)
        {
            if(_ucSettingLocator.txtName.Text == string.Empty || _ucSettingLocator.txtPriority.Text == string.Empty 
                || _ucSettingLocator.txtChildren.Text == string.Empty || _ucSettingLocator.txtMatchingRate.Text == string.Empty)
            {
                MessageBox.Show("Has the field is empty!");
            }
            XmlNode nodeRectLoc = _xmlManagement.SelectSingleNode("//Job/Camera[@id='" + MainViewModel.Instance.CameraIdSelected + "']/Recipe/LocatorTool[@id='" + _ucSettingLocator.LocatorId + "']");
            if(nodeRectLoc != null)
            {
                _xmlManagement.AddAttributeToNode(nodeRectLoc, "id", _ucSettingLocator.LocatorId);
                _xmlManagement.AddAttributeToNode(nodeRectLoc, "name", _ucSettingLocator.txtName.Text.Trim());
                _xmlManagement.AddAttributeToNode(nodeRectLoc, "priority", _ucSettingLocator.txtPriority.Text.Trim());
                _xmlManagement.AddAttributeToNode(nodeRectLoc, "hasChildren", _ucSettingLocator.cbbHasChildren.Text);
                _xmlManagement.AddAttributeToNode(nodeRectLoc, "children", _ucSettingLocator.txtChildren.Text.Trim());
                _xmlManagement.AddAttributeToNode(nodeRectLoc, "matchingRate", _ucSettingLocator.txtMatchingRate.Text.Trim());
            }

            XmlNode nodeRectInside = _xmlManagement.SelectSingleNode("//Job/Camera[@id='" + MainViewModel.Instance.CameraIdSelected + "']/Recipe/LocatorTool[@id='" + _ucSettingLocator.LocatorId + "']/RectangleInSide");
            if (nodeRectInside != null)
            {
                _xmlManagement.AddAttributeToNode(nodeRectInside, "x", (int)ucCreateRecipe.RectInSide.X + "");
                _xmlManagement.AddAttributeToNode(nodeRectInside, "y", (int)ucCreateRecipe.RectInSide.Y + "");
                _xmlManagement.AddAttributeToNode(nodeRectInside, "width", (int)ucCreateRecipe.RectInSide.Width + "");
                _xmlManagement.AddAttributeToNode(nodeRectInside, "height", (int)ucCreateRecipe.RectInSide.Height + "");
            }
            XmlNode nodeRectOutside = _xmlManagement.SelectSingleNode("//Job/Camera[@id='" + MainViewModel.Instance.CameraIdSelected + "']/Recipe/LocatorTool[@id='" + _ucSettingLocator.LocatorId + "']/RectangleOutSide");
            if (nodeRectOutside != null)
            {
                _xmlManagement.AddAttributeToNode(nodeRectOutside, "x", (int)ucCreateRecipe.RectOutSide.X + "");
                _xmlManagement.AddAttributeToNode(nodeRectOutside, "y", (int)ucCreateRecipe.RectOutSide.Y + "");
                _xmlManagement.AddAttributeToNode(nodeRectOutside, "width", (int)ucCreateRecipe.RectOutSide.Width + "");
                _xmlManagement.AddAttributeToNode(nodeRectOutside, "height", (int)ucCreateRecipe.RectOutSide.Height + "");
            }
            XmlNode nodeDataTrain = _xmlManagement.SelectSingleNode("//Job/Camera[@id='" + MainViewModel.Instance.CameraIdSelected + "']/Recipe/LocatorTool[@id='" + _ucSettingLocator.LocatorId + "']/DataTrain");
            if (nodeDataTrain != null)
            {
                _xmlManagement.AddAttributeToNode(nodeDataTrain, "x", (int)ucCreateRecipe.DataTrain[0] + "");
                _xmlManagement.AddAttributeToNode(nodeDataTrain, "y", (int)ucCreateRecipe.DataTrain[1] + "");
            }

           if( _xmlManagement.Save(_xmlPath))
            {
                MessageBox.Show("Save Recipe successfully!");
            }
        }

        private void UcCreateRecipe_SettingROI(object sender, RoutedEventArgs e)
        {
            _ucSettingROITool = new UcSettingROITool();
            contentSetting.Content = _ucSettingROITool;
            _ucSettingROITool.ROIId = "ROI" + _roiIdx;
            _ucSettingROITool.SaveParamROITool += _ucSettingROITool_SaveParam;
            ToolSelected = ToolSelected.SelectROITool;

            XmlNode nodeRecipe = _xmlManagement.SelectSingleNode("//Job/Camera[@id='" + MainViewModel.Instance.CameraIdSelected + "']/Recipe");
            if (nodeRecipe != null)
            {
                XmlNode nodeSelectROI = _xmlManagement.AddChildNode(nodeRecipe, "SelectROITool");
                if (nodeSelectROI != null)
                {
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "id", "ROI" + _roiIdx);
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "name", "");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "type", "");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "algorithm", "");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "rotations", "");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "priority", "");

                    if (_xmlManagement.Save(_xmlPath))
                        _roiIdx++;
                }
            }
        }

        private void _ucSettingROITool_SaveParam(object sender, RoutedEventArgs e)
        {
            switch (_ucSettingROITool.AlgorithmSelected)
            {
                case Algorithms.CountPixel:
                    XmlNode nodeSelectROI_CountPixel = _xmlManagement.SelectSingleNode("//Job/Camera[@id='" + MainViewModel.Instance.CameraIdSelected + "']/Recipe/SelectROITool[@id='" + _ucSettingROITool.ROIId + "']");
                    if (nodeSelectROI_CountPixel == null) return;
                    _xmlManagement.AddAttributeToNode(nodeSelectROI_CountPixel, "name", "count_pixel");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI_CountPixel, "type", "rectangle");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI_CountPixel, "algorithm", "CountPixel");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI_CountPixel, "rotations", "True");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI_CountPixel, "priority", "2");

                    XmlNode nodeParams = null;
                    XmlNode nodeROI_CountPxl = null;
                    XmlNode nodeThresholdGray = null;
                    XmlNode nodeNumberOfPixel = null;
                    if (nodeSelectROI_CountPixel.SelectSingleNode("//Parameters") == null)
                    {
                        nodeParams = _xmlManagement.AddChildNode(nodeSelectROI_CountPixel, "Parameters");
                        nodeROI_CountPxl = _xmlManagement.AddChildNode(nodeParams, "ROI");
                        nodeThresholdGray = _xmlManagement.AddChildNode(nodeParams, "ThresholdGray");
                        nodeNumberOfPixel = _xmlManagement.AddChildNode(nodeParams, "NumberOfPixel");
                    }

                    _xmlManagement.AddAttributeToNode(nodeROI_CountPxl, "x", (int)ucCreateRecipe.ROISelected.X + "");
                    _xmlManagement.AddAttributeToNode(nodeROI_CountPxl, "y", (int)ucCreateRecipe.ROISelected.Y + "");
                    _xmlManagement.AddAttributeToNode(nodeROI_CountPxl, "width", (int)ucCreateRecipe.ROISelected.Width + "");
                    _xmlManagement.AddAttributeToNode(nodeROI_CountPxl, "height", (int)ucCreateRecipe.ROISelected.Height + "");
                    _xmlManagement.AddAttributeToNode(nodeROI_CountPxl, "angleRotate", ucCreateRecipe.AngleRotate + "");

                    _xmlManagement.AddAttributeToNode(nodeThresholdGray, "min", _ucSettingROITool.ucSettingCountPixel.ThresholdGrayMin);
                    _xmlManagement.AddAttributeToNode(nodeThresholdGray, "max", _ucSettingROITool.ucSettingCountPixel.ThresholdGrayMax);

                    _xmlManagement.AddAttributeToNode(nodeNumberOfPixel, "min", _ucSettingROITool.ucSettingCountPixel.MinPixel);
                    _xmlManagement.AddAttributeToNode(nodeNumberOfPixel, "max", _ucSettingROITool.ucSettingCountPixel.MaxPixel);

                    _xmlManagement.Save(_xmlPath);

                    break;
                case Algorithms.CalculateArea:
                    XmlNode nodeSelectROI_CalArea = _xmlManagement.SelectSingleNode("//Job/Camera[@id='" + MainViewModel.Instance.CameraIdSelected + "']/Recipe/SelectROITool[@id='" + _ucSettingROITool.ROIId + "']");
                    if (nodeSelectROI_CalArea == null) return;
                    _xmlManagement.AddAttributeToNode(nodeSelectROI_CalArea, "name", "cal_area");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI_CalArea, "type", "rectangle");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI_CalArea, "algorithm", "CalculateArea");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI_CalArea, "rotations", "True");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI_CalArea, "priority", "2");

                    XmlNode nodeROI_CalArea = nodeSelectROI_CalArea.SelectSingleNode("//Parameters/ROI");
                    if (nodeROI_CalArea == null) return;
                    _xmlManagement.AddAttributeToNode(nodeROI_CalArea, "x", (int)ucCreateRecipe.ROISelected.X + "");
                    _xmlManagement.AddAttributeToNode(nodeROI_CalArea, "y", (int)ucCreateRecipe.ROISelected.Y + "");
                    _xmlManagement.AddAttributeToNode(nodeROI_CalArea, "width", (int)ucCreateRecipe.ROISelected.Width + "");
                    _xmlManagement.AddAttributeToNode(nodeROI_CalArea, "height", (int)ucCreateRecipe.ROISelected.Height + "");
                    _xmlManagement.AddAttributeToNode(nodeROI_CalArea, "angleRotate", ucCreateRecipe.AngleRotate + "");

                    XmlNode nodeThreshold = nodeSelectROI_CalArea.SelectSingleNode("//Parameters/Threshold");
                    if (nodeThreshold == null) return;
                    _xmlManagement.AddAttributeToNode(nodeThreshold, "value", _ucSettingROITool.ucSettingCalculateArea.Threshold + "");

                    XmlNode nodeArea = nodeSelectROI_CalArea.SelectSingleNode("//Parameters/Area");
                    if (nodeArea == null) return;
                    _xmlManagement.AddAttributeToNode(nodeArea, "min", _ucSettingROITool.ucSettingCalculateArea.MinArea + "");
                    _xmlManagement.AddAttributeToNode(nodeArea, "max", _ucSettingROITool.ucSettingCalculateArea.MaxArea + "");

                    _xmlManagement.Save(_xmlPath);

                    break;
            }
        }

        public ToolSelected ToolSelected
        {
            get { return _toolSelected; }
            set
            {
                if (SetProperty(ref _toolSelected, value))
                {

                }
            }
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
