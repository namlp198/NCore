using Microsoft.Win32;
using NpcCore.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NCore.Wpf.NUcBufferViewer
{
    public enum ModeTest
    {
        [Description("Null")]
        None = -1,
        [Description("Single Test")]
        SingleTest = 0,
        [Description("Continuous Test")]
        ContinuousTest = 1
    }
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NUcBufferViewer : UserControl, INotifyPropertyChanged
    {
        private int _camIdx = -1;
        private string _cameraName;
        private bool _isFakeCamera;
        private List<string> _cameraLst = new List<string>();

        private List<string> _modeTestsStr = new List<string>();
        private ModeTest _modeTestSelected = ModeTest.None;
        private int _modeTestSelectedIdx = -1;
        private const double _opacityClear = 1.0;
        private const double _opacityBlur = 0.3;

        // ROI and angle rotate
        private Rect _rectOutSide = new Rect();
        private Rect _rectInSide = new Rect();
        private Rect _roi = new Rect();
        private double _angleRotate = 0.0;

        public NUcBufferViewer()
        {
            InitializeComponent();

            CameraList.Add("Fake Camera");

            ModeTestsString = GetEnumDescriptionToListString();

            this.DataContext = this;
            scrollViewerExt.ImageExt = imageExt;
            scrollViewerExt.Grid = gridMain;

            imageExt.SelectedROI += ImageExt_SelectedROI;
            imageExt.TrainLocator += ImageExt_TrainLocator;
            imageExt.SaveImage += ImageExt_SaveImage;
        }

        private void ImageExt_SaveImage(object sender, RoutedEventArgs e)
        {
            ImageExt imageExt = (ImageExt)sender;
            ROISelected = imageExt.Rect;
            AngleRotate = imageExt.RectRotation;

            RaiseEvent(new RoutedEventArgs(UcSaveImageEvent, this));
        }

        private void ImageExt_TrainLocator(object sender, RoutedEventArgs e)
        {
            ImageExt imageExt = (ImageExt)sender;
            RectOutSide = imageExt.Rect;
            RectInSide = imageExt.RectInside;

            RaiseEvent(new RoutedEventArgs(UcTrainLocatorEvent, this));
        }

        private void ImageExt_SelectedROI(object sender, RoutedEventArgs e)
        {
            ImageExt imageExt = (ImageExt)sender;
            ROISelected = imageExt.Rect;
            AngleRotate = imageExt.RectRotation;

            RaiseEvent(new RoutedEventArgs(UcSelectedROIEvent, this));
        }

        #region Methods
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            if (fieldInfo != null)
            {
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                if (attribArray != null && attribArray.Length > 0 && attribArray[0] is DescriptionAttribute attrib)
                {
                    return attrib.Description;
                }
            }
            return enumObj.ToString();
        }
        private List<string> GetEnumDescriptionToListString()
        {
            List<string> modeTestString = new List<string>();
            List<ModeTest> modeTests = Enum.GetValues(typeof(ModeTest))
                                           .Cast<ModeTest>()
                                           .ToList();

            foreach (var item in modeTests)
            {
                string s = GetEnumDescription(item);
                //if (s.Equals("Null"))
                //    continue;
                modeTestString.Add(s);
            }

            return modeTestString;
        }
        #endregion

        #region event form
        private void btnLocatorTool_Click(object sender, RoutedEventArgs e)
        {
            if (imageExt.Source == null)
                return;

            imageExt.EnableLocatorTool = true;
            RaiseEvent(new RoutedEventArgs(SettingLocatorEvent, this));
        }
        private void btnSelectROITool_Click(object sender, RoutedEventArgs e)
        {
            if (imageExt.Source == null)
                return;

            imageExt.EnableSelectRoiTool = true;
            RaiseEvent(new RoutedEventArgs(SettingROIEvent, this));
        }
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
            if (openFileDialog.ShowDialog() == true)
            {
                imageExt.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
        #endregion

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
                    if(_isFakeCamera == true)
                    {
                        ModeTestSelected = ModeTest.None;
                        ModeTestSelectedIndex = 2;

                        cbbModeTest.Opacity = _opacityBlur;
                        btnGrab.Opacity = _opacityBlur;
                        btnStreamming.Opacity = _opacityBlur;

                        btnLoadImage.Opacity = _opacityClear;
                    }
                    else
                    {
                        cbbModeTest.Opacity = _opacityClear;
                        btnGrab.Opacity = _opacityClear;
                        btnStreamming.Opacity = _opacityClear;

                        btnLoadImage.Opacity = _opacityBlur;
                    }
                }
            }
        }
        public List<string> ModeTestsString
        {
            get => _modeTestsStr;
            set
            {
                if(SetProperty(ref _modeTestsStr, value))
                {

                }
            }
        }
        public ModeTest ModeTestSelected
        {
            get => _modeTestSelected;
            set
            {
                if (SetProperty(ref _modeTestSelected, value))
                {

                }
            }
        }
        public int ModeTestSelectedIndex
        {
            get => _modeTestSelectedIdx;
            set
            {
                if(SetProperty(ref _modeTestSelectedIdx, value))
                {
                    switch(_modeTestSelectedIdx)
                    {
                        case 0:
                            ModeTestSelected = ModeTest.SingleTest;
                            break;
                        case 1:
                            ModeTestSelected = ModeTest.ContinuousTest;
                            break;
                        default:
                            ModeTestSelected = ModeTest.None;
                            break;
                    }
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

        public Rect RectInSide
        {
            get => _rectInSide;
            set { if (SetProperty(ref _rectInSide, value)) { } }
        }
        public Rect RectOutSide
        {
            get => _rectOutSide;
            set { if (SetProperty(ref _rectOutSide, value)) { } }
        }
        public Rect ROISelected
        {
            get => _roi;
            set { if (SetProperty(ref _roi, value)) { } }
        }
        public double AngleRotate
        {
            get => _angleRotate;
            set { if(SetProperty(ref _angleRotate, value)) { } }
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

        public static readonly RoutedEvent SettingROIEvent = EventManager.RegisterRoutedEvent(
           "SettingROI",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(ImageExt));
        public event RoutedEventHandler SettingROI
        {
            add
            {
                base.AddHandler(SettingROIEvent, value);
            }
            remove
            {
                base.RemoveHandler(SettingROIEvent, value);
            }
        }

        public static readonly RoutedEvent UcSelectedROIEvent = EventManager.RegisterRoutedEvent(
            "UcSelectedROI",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));

        public static readonly RoutedEvent UcSaveImageEvent = EventManager.RegisterRoutedEvent(
            "UcSaveImage",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));

        public static readonly RoutedEvent UcTrainLocatorEvent = EventManager.RegisterRoutedEvent(
            "UcTrainLocator",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));

        public event RoutedEventHandler UcSelectedROI
        {
            add
            {
                base.AddHandler(UcSelectedROIEvent, value);
            }
            remove
            {
                base.RemoveHandler(UcSelectedROIEvent, value);
            }
        }
        public event RoutedEventHandler UcSaveImage
        {
            add
            {
                base.AddHandler(UcSaveImageEvent, value);
            }
            remove
            {
                base.RemoveHandler(UcSaveImageEvent, value);
            }
        }
        public event RoutedEventHandler UcTrainLocator
        {
            add
            {
                base.AddHandler(UcTrainLocatorEvent, value);
            }
            remove
            {
                base.RemoveHandler(UcTrainLocatorEvent, value);
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
       
    }
}
