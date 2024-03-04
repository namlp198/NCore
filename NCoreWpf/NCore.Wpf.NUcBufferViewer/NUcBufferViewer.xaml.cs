using Microsoft.Win32;
using NpcCore.Wpf.Controls;
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

namespace NCore.Wpf.NUcBufferViewer
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NUcBufferViewer : UserControl, INotifyPropertyChanged
    {
        private int _camIdx = -1;
        private string _cameraName;
        private bool _isFakeCamera;
        private List<string> _cameraLst = new List<string>();

        public NUcBufferViewer()
        {
            InitializeComponent();

            CameraList.Add("Fake Camera");

            this.DataContext = this;
            scrollViewerExt.ImageExt = imageExt;
            scrollViewerExt.Grid = gridMain;
        }

        private void btnLocatorTool_Click(object sender, RoutedEventArgs e)
        {
            if (imageExt.Source == null)
                return;

            imageExt.EnableLocatorTool = true;
            RaiseEvent(new RoutedEventArgs(SettingLocatorEvent, this));
        }
        #region Properties
        public int CameraIndex
        {
            get => _camIdx;
            set
            {
                if(SetProperty(ref _camIdx, value))
                {

                }
            }
        }
        public bool IsFakeCamera
        {
            get => _isFakeCamera;
            set
            {
                if(SetProperty(ref _isFakeCamera, value))
                {

                }
            }
        }

        public List<string> CameraList
        {
            get => _cameraLst;
            set
            {
                if(SetProperty(ref _cameraLst, value))
                {

                }
            }
        }
        public string CameraName
        {
            get => _cameraName;
            set
            {
                if(SetProperty(ref _cameraName, value))
                {
                    if(string.Compare(_cameraName, "Fake Camera") == 0)
                    {
                        IsFakeCamera = true;
                        CameraIndex = 100; // set index camera for show imageExt
                    }
                    else IsFakeCamera = false;
                }
            }
        }
        #endregion
        #region Event
        public static readonly RoutedEvent SettingLocatorEvent = EventManager.RegisterRoutedEvent(
            "SettingLocator",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));
        public event RoutedEventHandler SettingLocator
        {
            add
            {
                base.AddHandler(SettingLocatorEvent, value);
            }
            remove
            {
                base.RemoveHandler(SettingLocatorEvent, value);
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //if(CameraIndex == -1)
            //{
            //    TextBlock textBlock = new TextBlock();
            //    textBlock.Text = "No Camera";
            //    textBlock.Foreground = new SolidColorBrush(Colors.Orange);
            //    textBlock.FontSize = 14;
            //    textBlock.FontWeight = FontWeights.SemiBold;
            //    textBlock.TextAlignment = TextAlignment.Center;
            //    textBlock.VerticalAlignment = VerticalAlignment.Center;
            //    Canvas.SetLeft(textBlock, 15);
            //    Canvas.SetTop(textBlock, 10);
            //    canvasImage.Children.Add(textBlock);
            //}
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                imageExt.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
    }
}
