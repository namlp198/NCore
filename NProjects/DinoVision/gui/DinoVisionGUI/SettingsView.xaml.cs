using NpcCore.Wpf.Controls;
using NpcCore.Wpf.Struct_Vision;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using System.Windows.Interop;
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
        private int _frameWidth = 640;
        private int _frameHeight = 480;

        CLocatorToolResult_Simple locToolRes_Simple = new CLocatorToolResult_Simple();
        CROIGenAuto[] lstROIGenAuto = new CROIGenAuto[ConstDefine.MAX_ROI_AUTO];

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

        #region Recipe Config
        private string m_sRecipeName_Recipe;
        private string m_sAlgorithm;
        private int m_nRectX;
        private int m_nRectY;
        private int m_nRectWidth;
        private int m_nRectHeight;
        private double m_dMatchingRate;
        private int m_nCenterX;
        private int m_nCenterY;
        private string m_sImageTemplate;
        private int m_nOffsetROI0_X;
        private int m_nOffsetROI0_Y;
        private int m_nOffsetROI1_X;
        private int m_nOffsetROI1_Y;
        private int m_nROIWidth;
        private int m_nROIHeight;
        private int m_nNumberOfArray;
        private int m_nThresholdHeightMin;
        private int m_nThresholdHeightMax;

        public string RecipeName_Recipe
        {
            get { return m_sRecipeName_Recipe; }
            set
            {
                if (SetProperty(ref m_sRecipeName_Recipe, value)) { }
            }
        }
        public string Algorithm
        {
            get { return m_sAlgorithm; }
            set
            {
                if (SetProperty(ref m_sAlgorithm, value))
                {

                }
            }
        }
        public int RectX
        {
            get { return m_nRectX; }
            set
            {
                if (SetProperty(ref m_nRectX, value))
                {

                }
            }
        }
        public int RectY
        {
            get { return m_nRectY; }
            set
            {
                if (SetProperty(ref m_nRectY, value))
                {

                }
            }
        }
        public int RectWidth
        {
            get { return m_nRectWidth; }
            set
            {
                if (SetProperty(ref m_nRectWidth, value))
                {

                }
            }
        }
        public int RectHeight
        {
            get { return m_nRectHeight; }
            set
            {
                if (SetProperty(ref m_nRectHeight, value))
                {

                }
            }
        }
        public double MatchingRate
        {
            get { return m_dMatchingRate; }
            set
            {
                if (SetProperty(ref m_dMatchingRate, value))
                {

                }
            }
        }
        public int CenterX
        {
            get { return m_nCenterX; }
            set
            {
                if (SetProperty(ref m_nCenterX, value))
                {

                }
            }
        }
        public int CenterY
        {
            get { return m_nCenterY; }
            set
            {
                if (SetProperty(ref m_nCenterY, value))
                {

                }
            }
        }
        public string ImageTemplate
        {
            get { return m_sImageTemplate; }
            set
            {
                if (SetProperty(ref m_sImageTemplate, value))
                {

                }
            }
        }
        public int OffsetROI0_X
        {
            get { return m_nOffsetROI0_X; }
            set
            {
                if (SetProperty(ref m_nOffsetROI0_X, value))
                {

                }
            }
        }
        public int OffsetROI0_Y
        {
            get { return m_nOffsetROI0_Y; }
            set
            {
                if (SetProperty(ref m_nOffsetROI0_Y, value))
                {

                }
            }
        }
        public int OffsetROI1_X
        {
            get { return m_nOffsetROI1_X; }
            set
            {
                if (SetProperty(ref m_nOffsetROI1_X, value))
                {

                }
            }
        }
        public int OffsetROI1_Y
        {
            get { return m_nOffsetROI1_Y; }
            set
            {
                if (SetProperty(ref m_nOffsetROI1_Y, value))
                {

                }
            }
        }
        public int ROIWidth
        {
            get { return m_nROIWidth; }
            set
            {
                if (SetProperty(ref m_nROIWidth, value))
                {

                }
            }
        }
        public int ROIHeight
        {
            get { return m_nROIHeight; }
            set
            {
                if (SetProperty(ref m_nROIHeight, value))
                {

                }
            }
        }
        public int NumberOfArray
        {
            get { return m_nNumberOfArray; }
            set
            {
                if (SetProperty(ref m_nNumberOfArray, value))
                {

                }
            }
        }
        public int ThresholdHeightMin
        {
            get { return m_nThresholdHeightMin; }
            set
            {
                if (SetProperty(ref m_nThresholdHeightMin, value))
                {

                }
            }
        }
        public int ThresholdHeightMax
        {
            get { return m_nThresholdHeightMax; }
            set
            {
                if (SetProperty(ref m_nThresholdHeightMax, value))
                {

                }
            }
        }
        #endregion
        public SettingsView()
        {
            InitializeComponent();

            this.DataContext = this;

            scrollViewerExt.ImageExt = imgView;
            scrollViewerExt.Grid = gridMain;

            imgView.EnableRotate = false;
            imgView.SelectedROI += ImgView_SelectedROI;

            RefeshSetting_SysConfig();

            RefeshSetting_CamConfig();

            RefeshSetting_Recipe();
        }
        void RefeshSetting_SysConfig()
        {
            RecipePath = InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_sRecipePath;
            Model = InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_sModel;
            COMPort = InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_sCOMPort;
            UsePCControl = InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs.m_bUsePCControl;
        }
        void RefeshSetting_CamConfig()
        {
            for (int i = 0; i < ConstDefine.MAX_CAMERA_INSP_COUNT; i++)
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
        void RefeshSetting_Recipe()
        {
            for (int i = 0; i < ConstDefine.MAX_CAMERA_INSP_COUNT; i++)
            {
                RecipeName_Recipe = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_sRecipeName;
                Algorithm = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_sAlgorithm;
                RectX = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nRectX;
                RectY = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nRectY;
                RectWidth = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nRectWidth;
                RectHeight = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nRectHeight;
                MatchingRate = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_dMatchingRate;
                CenterX = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nCenterX;
                CenterY = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nCenterY;
                ImageTemplate = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_sImageTemplate;
                OffsetROI0_X = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nOffsetROI0_X;
                OffsetROI0_Y = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nOffsetROI0_Y;
                OffsetROI1_X = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nOffsetROI1_X;
                OffsetROI1_Y = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nOffsetROI1_Y;
                ROIWidth = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nROIWidth;
                ROIHeight = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nROIHeight;
                NumberOfArray = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nNumberOfArray;
                ThresholdHeightMin = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nThresholdHeightMin;
                ThresholdHeightMax = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[i].m_nThresholdHeightMax;
            }
        }
        void UpdateROIGenAuto()
        {
            lstROIGenAuto[0].m_nNumberOfArray = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nNumberOfArray;
            lstROIGenAuto[0].m_nROI_X = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nCenterX - InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nOffsetROI0_X;
            lstROIGenAuto[0].m_nROI_Y = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nCenterY - InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nOffsetROI0_Y;
            lstROIGenAuto[0].m_nROI_Width = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nROIWidth;
            lstROIGenAuto[0].m_nROI_Height = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nROIHeight;

            lstROIGenAuto[1].m_nNumberOfArray = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nNumberOfArray;
            lstROIGenAuto[1].m_nROI_X = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nCenterX + InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nOffsetROI1_X;
            lstROIGenAuto[1].m_nROI_Y = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nCenterY - InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nOffsetROI1_Y;
            lstROIGenAuto[1].m_nROI_Width = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nROIWidth;
            lstROIGenAuto[1].m_nROI_Height = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nROIHeight;
        }
        private void ImgView_SelectedROI(object sender, RoutedEventArgs e)
        {
            ImageExt imageExt = (ImageExt)sender;
            ROISelected = imageExt.RectReal;
            AngleRotate = imageExt.RectRotation;

            RectX = (int)ROISelected.X;
            RectY = (int)ROISelected.Y;
            RectWidth = (int)ROISelected.Width;
            RectHeight = (int)ROISelected.Height;

            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nRectX = RectX;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nRectY = RectY;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nRectWidth = RectWidth;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nRectHeight = RectHeight;

            InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.LocatorTrain(0, ref InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0]);

            InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.LoadRecipe(0, ref InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0]);

            RefeshSetting_Recipe();

            locToolRes_Simple.m_nCenterX = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nCenterX;
            locToolRes_Simple.m_nCenterY = InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nCenterY;

            UpdateROIGenAuto();

            imgView.ArrROIGenAuto = lstROIGenAuto;
            imgView.LocToolRes_Simple = locToolRes_Simple;
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
            if (InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.SaveSysConfigurations(ref InterfaceManager.Instance.JigInspProcessorManager.SystemConfigs))
            {
                MessageBox.Show("saved successfully!");
            }
        }

        private void btnSaveCamSettings_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0].m_sName = CameraName;
            InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0].m_sInterfaceType = InterfaceType;
            InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0].m_sSensorType = SensorType;
            InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0].m_nChannels = Channels;
            InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0].m_sManufacturer = Manufacturer;
            InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0].m_nFrameWidth = FrameWidth;
            InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0].m_nFrameHeight = FrameHeight;
            InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0].m_sSerialNumber = SerialNumber;
            InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0].m_sImageSavePath = ImageSavePath;
            InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0].m_sImageTemplatePath = ImageTemplatePath;
            InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0].m_sRecipeName = RecipeName;
            if (InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.SaveCamConfigurations(0, ref InterfaceManager.Instance.JigInspProcessorManager.CameraConfigs[0]))
            {
                MessageBox.Show("saved successfully!");
            }
        }

        private void btnSaveRecipe_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_sRecipeName = RecipeName_Recipe;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_sAlgorithm = Algorithm;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nRectX = RectX;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nRectY = RectY;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nRectWidth = RectWidth;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nRectHeight = RectHeight;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_dMatchingRate = MatchingRate;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nCenterX = CenterX;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nCenterY = CenterY;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_sImageTemplate = ImageTemplate;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nOffsetROI0_X = OffsetROI0_X;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nOffsetROI0_Y = OffsetROI0_Y;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nOffsetROI1_X = OffsetROI1_X;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nOffsetROI1_Y = OffsetROI1_Y;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nROIWidth = ROIWidth;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nROIHeight = ROIHeight;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nNumberOfArray = NumberOfArray;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nThresholdHeightMin = ThresholdHeightMin;
            InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0].m_nThresholdHeightMax = ThresholdHeightMax;
            if (InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.SaveRecipe(0, ref InterfaceManager.Instance.JigInspProcessorManager.RecipeConfigs[0]))
            {
                MessageBox.Show("saved successfully!");
            }
        }

        private async void btnLocatorTool_Click(object sender, RoutedEventArgs e)
        {
            imgView.ViewMode = ViewMode.ViewMode_CreateRecipe;
            //InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.ConnectDinoCam(0);
            InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.GrabImageForLocatorTool(0);

            _bufferView = InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.GetResultBufferImageDinoCam(0);

            await UpdateImage();
            //InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.DisconnectDinoCam(0);
        }

        private IntPtr _bufferView = IntPtr.Zero;
        public IntPtr BufferView
        {
            get { return _bufferView; }
            set
            {
                if (SetProperty(ref _bufferView, value))
                {

                }
            }
        }

        private Rect _roi = new Rect();
        public Rect ROISelected
        {
            get => _roi;
            set { if (SetProperty(ref _roi, value)) { } }
        }
        private double _angleRotate = 0.0;
        public double AngleRotate
        {
            get => _angleRotate;
            set { if (SetProperty(ref _angleRotate, value)) { } }
        }

        public async Task UpdateImage()
        {
            Task task = new Task(() =>
            {
                if (_bufferView == IntPtr.Zero)
                    return;


                // create "empty" all zeros 24bpp bitmap object
                Bitmap bmp = new Bitmap(_frameWidth, _frameHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                // create rectangle and lock bitmap into system memory
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, _frameWidth, _frameHeight);

                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

                unsafe
                {
                    IntPtr Ptr = (IntPtr)bmpData.Scan0.ToPointer();

                    for (int i = 0; i < _frameHeight; i++)
                    {

                        CopyMemory((byte*)(Ptr + (i * _frameWidth)), (byte*)(_bufferView + (i * _frameWidth)), _frameWidth);
                    }

                    bmp.UnlockBits(bmpData);

                    SetGrayscalePalette(bmp);
                }

                Bitmap pImageBMP = bmp;
                BitmapSource bmpSrc = BitmapToImageSource(pImageBMP);
                bmpSrc.Freeze();
                imgView.Dispatcher.Invoke(() => imgView.Source = bmpSrc);

            });

            task.Start();
            await task;
        }
        public void SetGrayscalePalette(Bitmap Image)
        {
            ColorPalette GrayscalePalette = Image.Palette;

            for (int i = 0; i < 256; i++)
                GrayscalePalette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            Image.Palette = GrayscalePalette;
        }
        // KERNEL FUNCTIONS
        [DllImport("kernel32.dll")]
        public extern static unsafe void ZeroMemory(void* Destination, int Length);
        [DllImport("kernel32.dll")]
        public extern static unsafe void CopyMemory(void* Destination, void* Source, int Length);

        #region Convert Bitmap to ImageSource
        /// <summary>
        /// convert Bitmap to ImageSource show on Image WPF
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// Convert Bitmap to ImageSource
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                ImageSource newSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(handle);
                return newSource;
            }
            catch (Exception ex)
            {
                DeleteObject(handle);
                return null;
            }
        }

        public BitmapSource ToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            BitmapSource bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Gray8, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        #endregion

        private void btnApplyRecipe_Click(object sender, RoutedEventArgs e)
        {
            UpdateROIGenAuto();

            imgView.ArrROIGenAuto = lstROIGenAuto;
            imgView.LocToolRes_Simple = locToolRes_Simple;
            imgView.InvalidateVisual();
        }
    }
}
