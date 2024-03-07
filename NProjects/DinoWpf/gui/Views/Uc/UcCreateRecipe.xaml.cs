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
        public UcCreateRecipe()
        {
            InitializeComponent();

            this.DataContext = this;
            ucCreateRecipe.SettingLocator += UcCreateRecipe_SettingLocator;
            ucCreateRecipe.SettingROI += UcCreateRecipe_SettingROI;

            _xmlPath = CommonDefines.JobXmlPath + MainViewModel.Instance.JobSelected.Name + ".xml";
            _xmlManagement.Load(_xmlPath);
        }

        private void UcCreateRecipe_SettingLocator(object sender, RoutedEventArgs e)
        {
            NUcBufferViewer bufferView = sender as NUcBufferViewer;
            
            contentSetting.Content = new UcSettingLocatorTool();
            ToolSelected = ToolSelected.LocatorTool;

            XmlNode nodeRecipe = _xmlManagement.SelectSingleNode("//Job/Camera[@id='" + MainViewModel.Instance.CameraIdSelected + "']/Recipe");
            if(nodeRecipe != null)
            {
                XmlNode nodeLocator = _xmlManagement.AddChildNode(nodeRecipe, "LocatorTool");
                if(nodeLocator != null)
                {
                    _xmlManagement.AddAttributeToNode(nodeLocator, "id", "Loc" + _locatorIdx);
                    _xmlManagement.AddAttributeToNode(nodeLocator, "name", "locator" + _locatorIdx);
                    _xmlManagement.AddAttributeToNode(nodeLocator, "priority", "1");
                    _xmlManagement.AddAttributeToNode(nodeLocator, "hasChildren", "True");
                    _xmlManagement.AddAttributeToNode(nodeLocator, "children", "");

                    XmlNode nodeRectInside = _xmlManagement.AddChildNode(nodeLocator, "RectangleInSide");
                    _xmlManagement.AddAttributeToNode(nodeRectInside, "x", "0");
                    _xmlManagement.AddAttributeToNode(nodeRectInside, "y", "0");
                    _xmlManagement.AddAttributeToNode(nodeRectInside, "width", "0");
                    _xmlManagement.AddAttributeToNode(nodeRectInside, "height", "0");
                    XmlNode nodeRectOutside = _xmlManagement.AddChildNode(nodeLocator, "RectangleOutSide");
                    _xmlManagement.AddAttributeToNode(nodeRectOutside, "x", "0");
                    _xmlManagement.AddAttributeToNode(nodeRectOutside, "y", "0");
                    _xmlManagement.AddAttributeToNode(nodeRectOutside, "width", "0");
                    _xmlManagement.AddAttributeToNode(nodeRectOutside, "height", "0");

                    if (_xmlManagement.Save(_xmlPath))
                        _locatorIdx++;
                }
            }
        }
        private void UcCreateRecipe_SettingROI(object sender, RoutedEventArgs e)
        {
            contentSetting.Content = new UcSettingROITool();
            ToolSelected = ToolSelected.SelectROITool;

            XmlNode nodeRecipe = _xmlManagement.SelectSingleNode("//Job/Camera[@id='" + MainViewModel.Instance.CameraIdSelected + "']/Recipe");
            if (nodeRecipe != null)
            {
                XmlNode nodeSelectROI = _xmlManagement.AddChildNode(nodeRecipe, "SelectROITool");
                if (nodeSelectROI != null)
                {
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "id", "ROI"+_roiIdx);
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "name", "");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "type", "");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "algorithm", "");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "rotations", "");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "angleRotate", "0.0");
                    _xmlManagement.AddAttributeToNode(nodeSelectROI, "priority", "");

                    if (_xmlManagement.Save(_xmlPath))
                        _roiIdx++;
                }
            }
        }

        public ToolSelected ToolSelected
        {
            get { return _toolSelected; }
            set
            {
                if(SetProperty(ref _toolSelected, value))
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
