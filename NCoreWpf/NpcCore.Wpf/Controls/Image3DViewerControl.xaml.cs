using Npc.Foundation.Logger;
using NpcCore.Wpf.Core;
using NpcCore.Wpf.Enums;
using NpcCore.Wpf.Extensions;
using NpcCore.Wpf.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NpcCore.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for Image3DViewerControl.xaml
    /// </summary>
    public partial class Image3DViewerControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #region Constructor
        public Image3DViewerControl()
        {
            InitializeComponent();
            if (MainGrid != null)
            {
                MainGrid.MouseWheel += OnMouseWheel;
                MainGrid.MouseDown += OnMouseDown;
            }
        }
        #endregion

        #region Fields
        private double _cameraPhi = Math.PI / 3.0;
        private double _cameraTheta = Math.PI / 2.0;
        private double _cameraR = 512;
        private long _mouseClickTime = 0;
        private Point _mousePoint;
        private Point _mouseLastPos;
        private ScaleTransform3D _scale;
        private RotateTransform3D _rotate = new RotateTransform3D();
        private int _imageWidth = 0;
        private int _imageLength = 0;
        private float _maxHeight;
        private float _minHeight;
        private string _drawDataDD;
        private float[] _image3d;
        private byte[] _image2d;
        private GeometryModel3D _defectModel;
        private GeometryModel3D _boundingModel;
        public GeometryModel3D _defectColorModel;
        private float[] _heightMap;
        private bool _isNoDataToDisplay = true;
        private Dictionary<string, PointCollection> _dataCollection = new Dictionary<string, PointCollection>();
        public event Action<List<PointCollection>> DataCollectionChanged;
        public event Action<double> ScaleValueChanged;
        BasePlaneSection _baseLine = null;
        #endregion
        #region Properties
        public bool IsNoDataToDisplay
        {
            get
            {
                return _isNoDataToDisplay;
            }
            set
            {
                _isNoDataToDisplay = value;
                OnPropertyRaised(nameof(IsNoDataToDisplay));
            }
        }

        public string Unit
        {
            get
            {
                return (string)GetValue(UnitProperty);
            }
            set
            {
                SetValue(UnitProperty, value);
            }
        }
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(Image3DViewerControl), new FrameworkPropertyMetadata("mm"));
        
        public TextureType TextureType
        {
            get
            {
                return (TextureType)GetValue(TextureTypeProperty);
            }
            set
            {
                SetValue(TextureTypeProperty, value);
            }
        }

        public static readonly DependencyProperty TextureTypeProperty = DependencyProperty.Register("TextureType", typeof(TextureType),
              typeof(Image3DViewerControl), new PropertyMetadata(default(TextureType), OnTextureTypePropertyChangedCallback));

        public double PalletHeight
        {
            get
            {
                return (double)GetValue(PalletHeightProperty);
            }
            set
            {
                SetValue(PalletHeightProperty, value);
            }
        }

        public static readonly DependencyProperty PalletHeightProperty = DependencyProperty.Register("PalletHeight", typeof(double), typeof(Image3DViewerControl),
            new PropertyMetadata(-1.0, OnPalletHeightPropertyChangedCallback));

        private List<CrossSection> _crossSections = new List<CrossSection>();
        public List<CrossSection> CrossSections
        {
            get
            {
                return _crossSections;
            }
            set
            {
                _crossSections = value;
            }
        }

        public bool ShowCrossSection
        {
            get
            {
                return (bool)GetValue(ShowCrossSectionProperty);
            }
            set
            {
                SetValue(ShowCrossSectionProperty, value);
            }
        }

        public static readonly DependencyProperty ShowCrossSectionProperty = DependencyProperty.Register("ShowCrossSection", typeof(bool), typeof(Image3DViewerControl),
            new PropertyMetadata(false, OnShowCrossSectionPropertyChangedCallback));

        public List<PointCollection> DataCollection
        {
            get
            {
                return (List<PointCollection>)GetValue(DataCollectionProperty);
            }
            set
            {
                SetValue(DataCollectionProperty, value);
            }
        }

        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(List<PointCollection>),
            typeof(Image3DViewerControl), new FrameworkPropertyMetadata(null));


        public Color PinColor
        {
            get
            {
                return (Color)GetValue(PinColorProperty);
            }
            set
            {
                SetValue(PinColorProperty, value);
            }
        }
        public static readonly DependencyProperty PinColorProperty = DependencyProperty.Register("PinColor", typeof(Color), typeof(Image3DViewerControl), null);

        public int NumberCrossSection
        {
            get
            {
                return (int)GetValue(NumberCrossSectionProperty);
            }
            set
            {
                SetValue(NumberCrossSectionProperty, value);
            }
        }
        public static readonly DependencyProperty NumberCrossSectionProperty = DependencyProperty.Register("NumberCrossSection", typeof(int), typeof(Image3DViewerControl),
            new FrameworkPropertyMetadata(1));

        public bool IsNegative
        {
            get
            {
                return (bool)GetValue(IsNegativeProperty);
            }
            set
            {
                SetValue(IsNegativeProperty, value);
            }
        }
        public static readonly DependencyProperty IsNegativeProperty = DependencyProperty.Register("IsNegative", typeof(bool), typeof(Image3DViewerControl),
            new FrameworkPropertyMetadata(false));

        private double _scaleX = 1.0;
        public double ScaleX
        {
            get
            {
                return _scaleX;
            }
            set
            {
                _scaleX = value;
            }
        }

        private double _scaleY = 1.0;
        public double ScaleY
        {
            get
            {
                return _scaleY;
            }
            set
            {
                _scaleY = value;
            }
        }
        private double _scaleZ = 1.0;
        public double ScaleZ
        {
            get
            {
                return _scaleZ;
            }
            set
            {
                _scaleZ = value;
            }
        }

        public static readonly DependencyProperty BaseWidthExtendProperty = DependencyProperty.Register("BaseWidthExtend", typeof(double), typeof(Image3DViewerControl),
            new FrameworkPropertyMetadata(10.0d));
        public double BaseWidthExtend
        {
            get
            {
                return (double)GetValue(BaseWidthExtendProperty);
            }
            set
            {
                SetValue(BaseWidthExtendProperty, value);
            }
        }

        public static readonly DependencyProperty BaseLengthExtendProperty = DependencyProperty.Register("BaseLengthExtend", typeof(double), typeof(Image3DViewerControl),
            new FrameworkPropertyMetadata(10.0d));
        public double BaseLengthExtend
        {
            get
            {
                return (double)GetValue(BaseLengthExtendProperty);
            }
            set
            {
                SetValue(BaseLengthExtendProperty, value);
            }
        }

        public static readonly DependencyProperty CrossSectionExtendProperty = DependencyProperty.Register("CrossSectionExtend", typeof(double), typeof(Image3DViewerControl),
            new FrameworkPropertyMetadata(10.0d));
        public double CrossSectionExtend
        {
            get
            {
                return (double)GetValue(CrossSectionExtendProperty);
            }
            set
            {
                SetValue(CrossSectionExtendProperty, value);
            }
        }

        public double ScaleMax
        {
            get
            {
                return (double)GetValue(ScaleMaxProperty);
            }
            set
            {
                SetValue(ScaleMaxProperty, value);
            }
        }
        public static readonly DependencyProperty ScaleMaxProperty = DependencyProperty.Register("ScaleMax", typeof(double), typeof(Image3DViewerControl), new FrameworkPropertyMetadata(17.0d));

        public double ScaleMin
        {
            get
            {
                return (double)GetValue(ScaleMinProperty);
            }
            set
            {
                SetValue(ScaleMinProperty, value);
            }
        }
        public static readonly DependencyProperty ScaleMinProperty = DependencyProperty.Register("ScaleMin", typeof(double), typeof(Image3DViewerControl), new FrameworkPropertyMetadata(0.1d));

        #endregion

        #region Methods

        private static void OnPalletHeightPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d != null && d is Image3DViewerControl)
                {
                    ((Image3DViewerControl)d).OnPalletHeightChanged(e);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
            }
            
        }

        private void OnPalletHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            try
            {
                PalletHeightChanged();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private void PalletHeightChanged()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (_defectModel != null && TextureType == TextureType.Pallet)
                    {
                        //_defectModel.ApplyPalletMaterial(GetMaxHeight() - GetMinHeight(), (float)PalletHeight);
                    }
                }));
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
            }
        }
        

        private static void OnTextureTypePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Image3DViewerControl viewer)
            {
                viewer.OnTextureTypeChanged();
            }
        }

        private void OnTextureTypeChanged()
        {
            if (TextureType != TextureType.Default)
            {
                UpdateMeshSurface();
            }
        }

        private double RoundedValueByUnit(double value)
        {
            if (Unit.ToLower() == "mm")
                return Math.Round(value, 1);
            else if ((Unit.ToLower() == "um") || (Unit.ToLower() == "μm"))
                return Math.Round(value, 2);
            else
                return Math.Round(value, 0);
        }

        public void SetData(DataTable imageData)
        {
            //Data가 존재하는지 체크
            bool isExistData = false;

            try
            {
                ClearData();
                if (ScaleX <= 0)
                    ScaleX = 1.0;
                if (ScaleY <= 0)
                    ScaleY = 1.0;
                if (ScaleZ <= 0)
                    ScaleZ = 1.0;
                
                if (imageData != null)
                {
                    foreach (DataRow dr in imageData.Rows)
                    {
                        _imageWidth = dr.FieldOrDefault<int>("Image3DWidth");
                        _imageLength = dr.FieldOrDefault<int>("Image3DHeight");
                        _drawDataDD = dr.FieldOrDefault<string>("DrawDataDD");
                        _image2d = dr.FieldOrDefault<byte[]>("Image2D");

                        var bytes = dr.FieldOrDefault<byte[]>("Image3D");
                        _image3d = ArrayHelper.ConvertToFloatArray(bytes);
                        if (_image3d != null)
                            IsNoDataToDisplay = _image3d.Length == 0;

                        ShowImage(_image3d, _imageWidth, _imageLength, _image2d);

                        isExistData = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
            }

            //KHK : [NCS-3447] 코팅 미도포 상태 검사 시 3d두께 표기 됨(3D thick. is present when insected by insufficient coating)
            //Data가 없는 경우 외부에 알림 (ex. 외부에서 No Data 표시 등등)
            if (!isExistData)
            {
                _dataCollection.Clear();
                DataCollection = new List<PointCollection>();
                if (DataCollectionChanged != null)
                {
                    DataCollectionChanged(DataCollection);
                }
            }
        }

        private void ClearData()
        {
            MainModelGroup?.Children?.Clear();
            _crossSections?.Clear();
            _image2d = null;
            _image3d = null;
            _imageWidth = 0;
            _imageLength = 0;
            _maxHeight = 0;
            _minHeight = 0;
            _drawDataDD = null;
            _defectModel = null;
            _boundingModel = null;
        }

        private void ShowImage(float[] sdata, int imageWidth, int imageLength, byte[] image2d)
        {
            try
            {
                if (sdata == null || sdata.Length == 0 || MainModelGroup == null || imageWidth == 0 || imageLength == 0)
                    return;
                if (ScaleX <= 0)
                    ScaleX = 1.0;
                if (ScaleY <= 0)
                    ScaleY = 1.0;
                if (ScaleZ <= 0)
                    ScaleZ = 1.0;
                _heightMap = sdata;
                float yMax = sdata.Max();
                float yMin = sdata.Min();
                SetMaxHeight(yMax);
                SetMinHeight(yMin);
                double height = 0.0;
                if (IsNegative)
                    height = yMax - yMin;
                else
                    height = yMax;
                height *= ScaleZ;
                _cameraR = Math.Max(Math.Max(imageWidth, imageLength), height / 15) * 3.0;

                double actualWidth = this.ActualWidth;
                double actualHeight = this.ActualHeight;
                double scaleTemp = 1;
                double ratio = height / Math.Max(imageLength, imageWidth);
                if (ratio > 2)
                    scaleTemp = 0.1;
                else if (ratio > 1 && ratio < 2)
                    scaleTemp = 0.5;
                else
                    scaleTemp = 1.0;
                _scale = new ScaleTransform3D();
                _scale.ScaleX = scaleTemp;
                _scale.ScaleY = scaleTemp;
                _scale.ScaleZ = scaleTemp;
                _rotate = D3.Rotate(new Vector3D(0, 1, 0), 90);
                ShowImageOnPerspectiveView();
                // Clean up
                Transform3DGroup group = new Transform3DGroup();
                group.Children.Add(_scale);
                group.Children.Add(_rotate);

                MainModelGroup.Transform = group;

                // [NCS-2695] CID 171145 Dereference null return (stat)
                //MainModelGroup.Children.Clear();
                MainModelGroup.Children?.Clear();

                PositionCamera();
                // Define lights.
                DefineLights(MainModelGroup);
                _defectModel = MeshCreator.DefineModel(sdata, imageWidth, imageLength, _minHeight, _maxHeight, ScaleX, ScaleY, ScaleZ, image2d, TextureType);
                _boundingModel = MeshCreator.DefineBoundingModel(sdata, imageWidth, imageLength, _minHeight, _maxHeight, image2d, TextureType);
                _defectColorModel = MeshCreator.DefineModel(sdata, GetImageWidth(), GetImageLength(), _minHeight, _maxHeight, ScaleX, ScaleY, ScaleZ, _image2d, TextureType.Default);

                // [NCS-2695] CID 171145 Dereference null return (stat)
                //if (_defectModel != null)
                //    MainModelGroup.Children.Add(_defectModel);
                //if (_boundingModel != null)
                //    MainModelGroup.Children.Add(_boundingModel);
                if (MainModelGroup.Children != null)
                {
                    if (_defectModel != null)
                    {
                        MainModelGroup.Children.Add(_defectModel);
                    }

                    if (_boundingModel != null)
                    {
                        MainModelGroup.Children.Add(_boundingModel);
                    }
                }

                _dataCollection.Clear();
                if (ShowCrossSection)
                {
                    CreateCrossSection();
                }
                if (IsNegative)
                {
                    CreateBaseLine();
                }
                else
                {
                    CreateBasePlane();
                }
                UpdateScaleToFitView(imageWidth, imageLength, height);
                UpdatePositionForCrossSectionLabel(_scale);
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
            }
        }

        protected void UpdateScaleDefault(double imageWidth, double imageLength, double height)
        {
            
        }

        protected void UpdateScaleToFitView(double imageWidth, double imageLength, double height)
        {
            double imageWidthScale = imageWidth * ScaleX;
            double imageLengthScale = imageLength * ScaleY;
            Point3D pointTop = new Point3D(imageWidthScale * 0.5, height * 0.5, imageLengthScale * 0.5);
            Point3D pointBottom = new Point3D((-1) * imageWidthScale * 0.5, height * 0.5, (-1) * imageLengthScale * 0.5);
            Point3D pointBottomDown = pointBottom;
            pointBottomDown.Y = (-1) * height * 0.5;
            Point3D pointLeft = new Point3D(imageWidthScale * 0.5, height * 0.5, (-1) * imageLengthScale * 0.5);
            Point3D pointRight = new Point3D((-1) * imageWidthScale * 0.5, height * 0.5, imageLengthScale * 0.5);

            pointTop.X = pointTop.X * _scale.ScaleX;
            pointTop.Y = pointTop.Y * _scale.ScaleY;
            pointTop.Z = pointTop.Z * _scale.ScaleZ;
            pointBottom.X = pointBottom.X * _scale.ScaleX;
            pointBottom.Y = pointBottom.Y * _scale.ScaleY;
            pointBottom.Z = pointBottom.Z * _scale.ScaleZ;
            pointBottomDown.X = pointBottomDown.X * _scale.ScaleX;
            pointBottomDown.Y = pointBottomDown.Y * _scale.ScaleY;
            pointBottomDown.Z = pointBottomDown.Z * _scale.ScaleZ;
            pointLeft.X = pointLeft.X * _scale.ScaleX;
            pointLeft.Y = pointLeft.Y * _scale.ScaleY;
            pointLeft.Z = pointLeft.Z * _scale.ScaleZ;
            pointRight.X = pointRight.X * _scale.ScaleX;
            pointRight.Y = pointRight.Y * _scale.ScaleY;
            pointRight.Z = pointRight.Z * _scale.ScaleZ;
            Point pointTop2D = Panel3DMath.Get2DPoint(pointTop, MainViewport3D);
            Point pointBottom2D = Panel3DMath.Get2DPoint(pointBottom, MainViewport3D);
            Point pointBottomDown2D = Panel3DMath.Get2DPoint(pointBottomDown, MainViewport3D);
            Point pointTop2DProject = new Point(pointBottom2D.X, pointTop2D.Y);
            Point pointBottom2DPriject = new Point(pointBottomDown2D.X, pointBottom2D.Y);
            Point pointLeft2D = Panel3DMath.Get2DPoint(pointLeft, MainViewport3D);
            Point pointRight2D = Panel3DMath.Get2DPoint(pointRight, MainViewport3D);
            Point pointRight2DProject = new Point(pointRight2D.X, pointLeft2D.Y);
            double length1 = (pointTop2DProject - pointBottom2D).Length;
            double length2 = (pointBottom2DPriject - pointBottomDown2D).Length;
            double heightIn2D = length1 + length2;
            double widthIn2D = (pointRight2DProject - pointLeft2D).Length;
            double dScale = 0.0;
            double actualWidth = this.ActualWidth;
            double actualHeight = this.ActualHeight;
            double spacing = 10;
            if (heightIn2D > 0.0 && widthIn2D > 0.0 && actualWidth > 0.0 && actualHeight > 0.0 && spacing > 0.0)
            {
                if (!Double.IsNaN(actualWidth) && !Double.IsNaN(actualHeight) && actualHeight != 0.0 && actualWidth != 0.0)
                {
                    if (widthIn2D / actualWidth > heightIn2D / actualHeight)
                    {
                        dScale = (actualWidth - 2 * spacing * actualWidth / 100) / widthIn2D;
                    }
                    else
                    {
                        dScale = (actualHeight - 2 * spacing * actualHeight / 100) / heightIn2D;
                    }
                    _scale.ScaleZ *= dScale;
                    _scale.ScaleX *= dScale;
                    _scale.ScaleY *= dScale;
                    if (ScaleValueChanged != null)
                        ScaleValueChanged(_scale.ScaleX);
                }
            }
        }

        public void ApplyGridMaskMaterial(double lowerValue, double upperValue)
        {
            try
            {
                if (_defectModel != null && _defectColorModel != null && _image3d !=null)
                {
                    _defectModel.ApplyMaterialByPath(_image3d, GetImageWidth(), GetImageLength());
                    _defectColorModel.CreateMapImageByRange(_image3d, GetImageWidth(), GetImageLength(), lowerValue, upperValue);

                    // [NCS-2695] CID 171190 Dereference null return (stat)
                    //MainModelGroup.Children.Add(_defectColorModel);
                    MainModelGroup.Children?.Add(_defectColorModel);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
            }

        }

        /// <summary>
        /// Define the lights.
        /// </summary>
        private void DefineLights(Model3DGroup group)
        {
            AmbientLight ambientLight = new AmbientLight(Colors.Gray);

            // [NCS-2695] CID 171202 Dereference null return (stat)
            //group.Children.Add(ambientLight);
            //group.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(-1, -3, -2)));
            //group.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(1, -3, 2)));
            if (group.Children != null)
            {
                group.Children.Add(ambientLight);
                group.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(-1, -3, -2)));
                group.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(1, -3, 2)));
            }
        }

        // Position the camera.
        private void PositionCamera()
        {
            // Calculate the camera's position in Cartesian coordinates.
            double y = _cameraR * Math.Sin(_cameraPhi);
            double hyp = _cameraR * Math.Cos(_cameraPhi);
            double x = hyp * Math.Cos(_cameraTheta);
            double z = hyp * Math.Sin(_cameraTheta);
            MainPerspectiveCamera.Position = new Point3D(x, y, z);

            // Look toward the origin.
            MainPerspectiveCamera.LookDirection = new Vector3D(-x, -y, -z);

            // Set the Up direction.
            MainPerspectiveCamera.UpDirection = new Vector3D(0, 1, 0);
        }

#pragma warning disable S1172 // Unused method parameters should be removed
        private void RotateTransform(MouseEventArgs e)
#pragma warning restore S1172 // Unused method parameters should be removed
        {
            Point actualPos = GetMousePosition();
            double dx = actualPos.X - _mouseLastPos.X;
            double dy = actualPos.Y - _mouseLastPos.Y;

            if (dy != 0)
            {
                var cameraDPhi = 0.002 * Math.Abs(dy);
                if (Math.Sign(dy) < 0)
                {
                    _cameraPhi += cameraDPhi;
                    if (_cameraPhi > Math.PI / 2.0)
                        _cameraPhi = Math.PI / 2.0;
                }
                else
                {
                    _cameraPhi -= cameraDPhi;
                    if (_cameraPhi < 0)
                        _cameraPhi = 0;
                }
            }

            if (dx != 0)
            {
                var cameraDTheta = 0.003 * Math.Abs(dx);
                if (Math.Sign(dx) < 0)
                {
                    _cameraTheta -= cameraDTheta;
                }
                else
                {
                    _cameraTheta += cameraDTheta;
                }
            }
            _mouseLastPos = actualPos;

            PositionCamera();
        }

        private void AutoRotation()
        {
            /*float mouseDragTime = (DateTime.UtcNow.Ticks - _mouseClickTime) / (float)Math.Pow(10, 7);

            if (mouseDragTime > 0.0f && mouseDragTime < 0.5f)
            {
                double mouseDragDistance = GetMousePosition().X - _mousePoint.X;
                if (Math.Abs(mouseDragDistance) > 100)
                {
                    float rotateSpeed = (float)(mouseDragDistance / mouseDragTime);
                    rotateSpeed /= 200.0f;
                    SetAutoRotation(true, rotateSpeed);
                    return;
                }
            }

            SetAutoRotation(false, 0);*/
        }

        private void SetAutoRotation(bool bStart, float rotationSpeed)
        {
            AxisAngleRotation3D rotation = _rotate.Rotation as AxisAngleRotation3D;
            if (rotation == null)
                return;

            double angle = rotation.Angle;
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
            rotation.Angle = angle;

            if (bStart)
            {
                var duration = Math.Ceiling(13 / Math.Abs(rotationSpeed) * Math.PI);
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = angle,
                    To = angle + (360 * Math.Sign(rotationSpeed)),
                    Duration = new Duration(TimeSpan.FromSeconds(duration)),
                    RepeatBehavior = RepeatBehavior.Forever,
                };
                rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
            }
        }

        private Point GetMousePosition()
        {
            Point pos = Mouse.GetPosition(MainViewport3D);
            Point actualPos = new Point(pos.X - MainViewport3D.ActualWidth / 2, MainViewport3D.ActualHeight / 2 - pos.Y);

            return actualPos;
        }

        private void UpdateMeshSurface()
        {
            if (_defectModel != null && _image3d !=null)
            {
                _defectModel.ApplyMaterial(_image3d, GetImageWidth(), GetImageLength(), _image2d, TextureType, (float)PalletHeight);
            }
        }

        private int MapPosition(int imgW, int imgH, double ptX, double ptY)
        {
            try
            {
                double width = ((double)imgW) / 2;
                double height = ((double)imgH) / 2;

                double xTopLeft = (-1) * width;
                double yTopLeft = height;
                double xTopRight = width;
                double yTopRight = height;
                double xBottomRight = width;
                double yBottomRight = (-1) * height;
                double xBottomLeft = (-1) * width;
                double yBottomLeft = (-1) * height;

                double distanceToLeft = (xTopLeft - xBottomLeft) * (ptY - yTopLeft) - (ptX - xTopLeft) * (yTopLeft - yBottomLeft);
                double distanceToRight = (xTopRight - xBottomRight) * (ptY - yTopRight) - (ptX - xTopRight) * (yTopRight - yBottomRight);
                double distanceToTop = (xTopLeft - xTopRight) * (ptY - yTopLeft) - (ptX - xTopLeft) * (yTopLeft - yTopRight);
                double distanceToBottom = (xBottomLeft - xBottomRight) * (ptY - yBottomRight) - (ptX - xBottomRight) * (yBottomLeft - yBottomRight);

                int retPoint = 0;

                if (distanceToLeft <= 0.0 && distanceToRight >= 0 && distanceToTop >= 0 && distanceToBottom <= 0)
                {
                    ptX += width;
                    ptY += height;
                    int pintX = (int)ptX;
                    int pIntY = (int)ptY;
                    if (pintX >= imgW)
                        pintX -= 1;
                    if (pIntY >= imgH)
                        pIntY -= 1;
                    retPoint = (pIntY * imgW + pintX);
                    if (retPoint > imgW * imgH)
                        retPoint = imgW * imgH - 1;
                }
                else
                    retPoint = -1;
                return retPoint;
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
                return 0;
            }


          
        }
        private double LineLength(double ptX1, double ptY1, double ptX2, double ptY2, out double xDelta, out double yDelta)
        {
           
            double width = Math.Abs(ptX2 - ptX1);
            double height = Math.Abs(ptY2 - ptY1);

            double len = Math.Sqrt(width * width + height * height);
            xDelta = width / len;
            yDelta = height / len;

            if (ptX1 > ptX2)
                xDelta = -xDelta;
            if (ptY1 > ptY2)
                yDelta = -yDelta;
            return len;
            
          
        }

        private void OnShowCrossSectionChanged(DependencyPropertyChangedEventArgs e)
        {
            CreateCrossSection();
            UpdatePositionForCrossSectionLabel(_scale);
        }

        private void UpdatePositionForCrossSectionLabel(ScaleTransform3D scaleTransform3D = null)
        {
            try
            {
                if (NumberCrossSection == 2 && _crossSections?.Count == 2 && ShowCrossSection)
                {
                    Point3D startCrossSectionFirst = _crossSections.ElementAt(0).StartPoint;
                    Point3D startCrossSectionSecond = _crossSections.ElementAt(1).StartPoint;
                    if (scaleTransform3D != null)
                    {
                        startCrossSectionFirst.X = startCrossSectionFirst.X * scaleTransform3D.ScaleX;
                        startCrossSectionFirst.Y = startCrossSectionFirst.Y * scaleTransform3D.ScaleY;
                        startCrossSectionFirst.Z = startCrossSectionFirst.Z * scaleTransform3D.ScaleZ;

                        startCrossSectionSecond.X = startCrossSectionSecond.X * scaleTransform3D.ScaleX;
                        startCrossSectionSecond.Y = startCrossSectionSecond.Y * scaleTransform3D.ScaleY;
                        startCrossSectionSecond.Z = startCrossSectionSecond.Z * scaleTransform3D.ScaleZ;
                    }
                    if (MainViewportCanvas != null)
                    {
                        MainViewportCanvas.Visibility = Visibility.Visible;
                        SetUIElement(CrossSectionFirstLabel, MainViewport3D, startCrossSectionFirst, "1");
                        SetUIElement(CrossSectionSecondLabel, MainViewport3D, startCrossSectionSecond, "2");
                    }
                }
                else
                {
                    if (MainViewportCanvas != null)
                    {
                        MainViewportCanvas.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
            }

        
        }

        private static void OnShowCrossSectionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null && d is Image3DViewerControl)
            {
                ((Image3DViewerControl)d).OnShowCrossSectionChanged(e);
            }
        }
        #endregion

        #region Public Methods
        public GeometryModel3D GetDefectModel()
        {
            return _defectModel;
        }

        public float GetMaxHeight()
        {
            return _maxHeight;
        }

        public void SetMaxHeight(float maxHeight)
        {
            _maxHeight = maxHeight;
        }

        public float GetMinHeight()
        {
            return _minHeight;
        }

        public void SetMinHeight(float minHeight)
        {
            _minHeight = minHeight;
        }

        public void SetImageWidth(int imageWidth)
        {
            _imageWidth = imageWidth;
        }
        public int GetImageWidth()
        {
            return _imageWidth;
        }

        public void SetImageLength(int imageLength)
        {
            _imageLength = imageLength;
        }

        public int GetImageLength()
        {
            return _imageLength;
        }

        public void UpdatePositionCrossSection(Point3D point, int position = 0, int crossElement = 0)
        {
            if (CrossSections != null && CrossSections.Count > crossElement && _scale != null && MainViewport3D != null)
            {
                CrossSection crossSection = CrossSections.ElementAt(crossElement);
                if (crossSection != null)
                {
                    crossSection.UpdateCrossSectionByPoint(point, position);
                }
            }
        }

        public void UpdateCrossSectionPosition(CrossectionViewOptions viewOption)
        {
            if (CrossSections != null && CrossSections.Count > 0 && _scale != null && MainViewport3D != null)
            {
                CrossSection crossSectionFirst = CrossSections.ElementAt(0);
                if (crossSectionFirst != null)
                {
                    Point3D oldStartPoint = crossSectionFirst.StartPoint;
                    Point3D oldEndPoint = crossSectionFirst.EndPoint;
                    Point3D newStartPoint = oldStartPoint;
                    Point3D newEndPoint = oldEndPoint;
                    bool bContinue = false;
                    if (viewOption == CrossectionViewOptions.Defect_View)
                    {
                        float maxValue = _image3d.Max();
                        float minValue = _image3d.Min();
                        int indexMaxValue = _image3d.ToList().IndexOf(maxValue);
                        int indexMinValue = _image3d.ToList().IndexOf(minValue);
                        MeshGeometry3D meshGeometryModel = (MeshGeometry3D)_defectModel.Geometry;
                        if (meshGeometryModel != null && meshGeometryModel.TextureCoordinates != null)
                        {
                            Point maxPoint = meshGeometryModel.TextureCoordinates.ElementAt(indexMaxValue);
                            Point minPoint = meshGeometryModel.TextureCoordinates.ElementAt(indexMinValue);
                            newStartPoint.X = maxPoint.X - GetImageWidth() * 0.5;
                            newStartPoint.Z = maxPoint.Y - GetImageLength() * 0.5;
                            newEndPoint.X = minPoint.X - GetImageWidth() * 0.5;
                            newEndPoint.Z = minPoint.Y - GetImageLength() * 0.5;
                            double height = Math.Abs(newStartPoint.X - newEndPoint.X);
                            double width = Math.Abs(newEndPoint.Z - newEndPoint.Z);
                            double anpha = Math.Atan(height / width);
                            Vector3D direction = newStartPoint - newEndPoint;
                            direction.Normalize();
                            double lengthVerStart = GetImageWidth() * 0.5 - Math.Abs(newStartPoint.X);
                            double distanceStart = (lengthVerStart + CrossSectionExtend) / Math.Sin(anpha);
                            newStartPoint = newStartPoint + direction * distanceStart;
                            double lengthVerEnd = GetImageWidth() * 0.5 - Math.Abs(newEndPoint.X);
                            double distanceEnd = (lengthVerEnd + CrossSectionExtend) / Math.Sin(anpha);
                            newEndPoint = newEndPoint - direction * distanceEnd;
                            bContinue = true;
                        }
                    }
                    else if (viewOption == CrossectionViewOptions.Width_View)
                    {
                        newStartPoint.X = 0;
                        newStartPoint.Z = (-1) * GetImageLength() * 0.5 - CrossSectionExtend;
                        newEndPoint.X = 0;
                        newEndPoint.Z = GetImageLength() * 0.5 + CrossSectionExtend;
                        bContinue = true;
                    }
                    else if (viewOption == CrossectionViewOptions.Height_View)
                    {
                        newStartPoint.X = (-1) * GetImageWidth() * 0.5 - CrossSectionExtend;
                        newStartPoint.Z = 0;
                        newEndPoint.X = GetImageWidth() * 0.5 + CrossSectionExtend;
                        newEndPoint.Z = 0;
                        bContinue = true;
                    }
                    if (bContinue)
                    {
                        crossSectionFirst.UpdateCrossSectionByPoint(newStartPoint, 0);
                        crossSectionFirst.UpdateCrossSectionByPoint(newEndPoint, 1);
                    }
                }
            }
        }

        public void UpdateCrossectionFollowWidth(double ratio)
        {
            try
            {
                if (CrossSections != null && CrossSections.Count > 0 && MainViewport3D != null)
                {
                    CrossSection crossSectionFirst = CrossSections.ElementAt(0);
                    if (crossSectionFirst != null)
                    {
                        Point3D oldStartPoint = crossSectionFirst.StartPoint;
                        Point3D oldEndPoint = crossSectionFirst.EndPoint;
                        Point3D newStartPoint = oldStartPoint;
                        Point3D newEndPoint = oldEndPoint;
                        newStartPoint.X = (ratio - 0.5)*GetImageWidth();
                        newStartPoint.Z = (-1) * GetImageLength() * 0.5 - CrossSectionExtend;
                        newEndPoint.X = (ratio - 0.5) * GetImageWidth();
                        newEndPoint.Z = GetImageLength() * 0.5 + CrossSectionExtend;
                        crossSectionFirst.UpdateCrossSectionByPoint(newStartPoint, 0);
                        crossSectionFirst.UpdateCrossSectionByPoint(newEndPoint, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
            }
        }

        public void UpdateCrossectionFollowHeight(double ratio)
        {
            try
            {
                if (CrossSections != null && CrossSections.Count > 0 && MainViewport3D != null)
                {
                    CrossSection crossSectionFirst = CrossSections.ElementAt(0);
                    if (crossSectionFirst != null)
                    {
                        Point3D oldStartPoint = crossSectionFirst.StartPoint;
                        Point3D oldEndPoint = crossSectionFirst.EndPoint;
                        Point3D newStartPoint = oldStartPoint;
                        Point3D newEndPoint = oldEndPoint;
                        newStartPoint.X = (-1) * GetImageWidth() * 0.5 - CrossSectionExtend;
                        newStartPoint.Z = (ratio - 0.5) * GetImageLength();
                        newEndPoint.X = GetImageWidth() * 0.5 + CrossSectionExtend;
                        newEndPoint.Z = (ratio - 0.5) * GetImageLength();
                        crossSectionFirst.UpdateCrossSectionByPoint(newStartPoint, 0);
                        crossSectionFirst.UpdateCrossSectionByPoint(newEndPoint, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
            }
        }

        public void CreateCrossSection(int position = 0)
        {
            if (ShowCrossSection && _defectModel != null)
            {
                bool keepContent = false;
                Point3D startPointFirstSection = new Point3D();
                Point3D endPointFirstSection = new Point3D();
                Point3D startPointSecondSection = new Point3D();
                Point3D endPointSecondSection = new Point3D();
                if (CrossSections?.Count > 0)
                {
                    keepContent = true;
                    startPointFirstSection = CrossSections.ElementAt(0).StartPoint;
                    endPointFirstSection = CrossSections.ElementAt(0).EndPoint;
                    if (NumberCrossSection == 2 && CrossSections.Count > 1)
                    {
                        startPointSecondSection = CrossSections.ElementAt(1).StartPoint;
                        endPointSecondSection = CrossSections.ElementAt(1).EndPoint;
                    }
                }
                CrossSections.Clear();
                float height = 0;
                if (IsNegative)
                    height = GetMaxHeight() - GetMinHeight();
                else
                    height = GetMaxHeight();
                var crossSectionHorizontal = new CrossSection(MainViewport3D, (int)(GetImageWidth() * ScaleX), (int)(GetImageLength() * ScaleY), (float)(height * ScaleZ), PinColor, CrossSectionExtend, 0);
                if (crossSectionHorizontal != null)
                {
                    crossSectionHorizontal.KeepContent = keepContent;
                    crossSectionHorizontal.StartPoint = startPointFirstSection;
                    crossSectionHorizontal.EndPoint = endPointFirstSection;
                    crossSectionHorizontal.CrossPointChanged += CrossSection_CrossPointChanged;
                    crossSectionHorizontal.Show();
                    CrossSections.Add(crossSectionHorizontal);
                }

                if (NumberCrossSection == 2)
                {
                    var crossSectionVertical = new CrossSection(MainViewport3D, (int)(GetImageWidth() * ScaleX), (int)(GetImageLength() * ScaleY), (float)(height * ScaleZ), PinColor, CrossSectionExtend, 1);
                    if (crossSectionVertical != null)
                    {
                        crossSectionVertical.KeepContent = keepContent;
                        crossSectionVertical.StartPoint = startPointSecondSection;
                        crossSectionVertical.EndPoint = endPointSecondSection;
                        crossSectionVertical.CrossPointChanged += CrossSection_CrossPointChanged;
                        crossSectionVertical.Show();
                        CrossSections.Add(crossSectionVertical);
                    }
                }
            }
            else
            {
                foreach (var crossSection in CrossSections)
                {
                    crossSection.Hide();
                }
                _dataCollection.Clear();
            }
        }

        private void CreateBasePlane()
        {
            float height = 0;
            float minHeight = 0;
            if (IsNegative)
            {
                height = GetMaxHeight() - GetMinHeight();
                minHeight = GetMinHeight();
            }
            else
            {
                height = GetMaxHeight();
            }
            var basePlane = new BasePlaneSection(MainViewport3D, (int)(GetImageWidth() * ScaleX), (int)(GetImageLength() * ScaleY), (float)(height * ScaleZ), (float)(minHeight * ScaleZ), BaseModes.BasePlane);
            if (basePlane != null)
            {
                basePlane.Show();
            }
        }

        private void CreateBaseLine()
        {
            float height = 0;
            float minHeight = 0;
            if (IsNegative)
            {
                height = GetMaxHeight() - GetMinHeight();
                minHeight = GetMinHeight();
            }
            else
            {
                height = GetMaxHeight();
            }
            _baseLine = new BasePlaneSection(MainViewport3D, (int)(GetImageWidth() * ScaleX), (int)(GetImageLength() * ScaleY), (float)(height * ScaleZ), (float)(minHeight * ScaleZ), BaseModes.BaseLine);
            if (_baseLine != null)
            {
                _baseLine.Show();
            }
        }

        private void CrossSection_CrossPointChanged(object sender, EventArgs e)
        {
            if (sender is CrossSection c)
            {
                GetDataCrossSection(c);
            }
        }

        private void GetDataCrossSection(CrossSection cross)
        {
            try
            {
                if (cross.IsVisible)
                {
                    PointCollection points = null;
                    if (_dataCollection.ContainsKey(cross.Uid))
                    {
                        points = _dataCollection[cross.Uid];
                        points.Clear();
                    }
                    else
                    {
                        points = new PointCollection();
                        _dataCollection[cross.Uid] = points;
                    }

                    //Step1 . Calculate Line Div and tick interval
                    double dX = 0, dY = 0;
                    double lenDiv = LineLength(cross.StartPoint.X / ScaleX, cross.StartPoint.Z / ScaleY, cross.EndPoint.X / ScaleX, cross.EndPoint.Z / ScaleY, out dX, out dY);

                    //Step2. Set Start Point
                    double sX = cross.StartPoint.X / ScaleX;
                    double sY = cross.StartPoint.Z / ScaleY;

                    for (int i = 0; i < lenDiv; i++)
                    {
                        //Step3. 3D location convert to height map array index
                        int mapPos = MapPosition(GetImageWidth(), GetImageLength(), sX, sY);
                        float y = 0;
                        if (mapPos >= 0)
                        {
                            //Step4 . get value
                            y = _heightMap[mapPos];
                        }
                        else
                        {
                            if (IsNegative)
                                y = GetMinHeight();
                            else
                                y = 0;
                        }



                        //Step5 Draw Information.
                        points.Add(new Point(i, y));

                        //Step6. Move next point
                        sX += dX;
                        sY += dY;
                    }

                    // [NCS-2695] CID 171171 Dereference before null check
                    //if (_dataCollection != null)
                    //{
                    //    DataCollection = _dataCollection.Values.ToList();
                    //    if (DataCollectionChanged != null)
                    //        DataCollectionChanged(DataCollection);
                    //}
                    DataCollection = _dataCollection.Values.ToList();
                    if (DataCollectionChanged != null)
                    {
                        DataCollectionChanged(DataCollection);
                    }
                    UpdatePositionForCrossSectionLabel(_scale);
                }
                else
                {
                    _dataCollection.Remove(cross.Uid);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
            }
        }

        #endregion

        #region event interaction

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IsNoDataToDisplay = true;
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_scale != null)
            {
                if ((e.Delta < 0 && _scale.ScaleX < ScaleMin)
                || (e.Delta > 0 && _scale.ScaleX > ScaleMax))
                    return;

                double delta = e.Delta / 120.0;
                double scale = Math.Exp(delta / 31.4);

                _scale.ScaleX *= scale;
                _scale.ScaleY *= scale;
                _scale.ScaleZ *= scale;
                if (this.ScaleValueChanged != null)
                    this.ScaleValueChanged(_scale.ScaleX);
                UpdatePositionForCrossSectionLabel(_scale);
            }
        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            _mouseClickTime = DateTime.UtcNow.Ticks;
            _mousePoint = GetMousePosition();
            _mouseLastPos = GetMousePosition();
            MainGrid.CaptureMouse();
            MainGrid.MouseMove += OnMouseMove;
            MainGrid.MouseUp += OnMouseUp;
            foreach (var crossSection in CrossSections)
            {
                crossSection.OnMouseDown(e);
                crossSection.OnMouseDown(e);
            }
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (CrossSections.Count(m => m.IsDragging) == 0)
            {
                RotateTransform(e);
            }
            else
            {
                foreach (var crossSection in CrossSections)
                {
                    crossSection.OnMouseMove(e);
                }
            }
            UpdatePositionForCrossSectionLabel(_scale);
        }
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MainGrid.ReleaseMouseCapture();
            MainGrid.MouseMove -= OnMouseMove;
            MainGrid.MouseUp -= OnMouseUp;
            foreach (var crossSection in CrossSections)
            {
                if (crossSection.IsDragging)
                {
                    crossSection.OnMouseUp();
                    SetAutoRotation(false, 0);
                }
                else
                    AutoRotation();
            }
            if (CrossSections.Count == 0)
            {
                AutoRotation();
            }
            UpdatePositionForCrossSectionLabel(_scale);
            if (Math.Abs(_cameraPhi - Math.PI*0.5) > 0.001 && Math.Abs(_cameraTheta - Math.PI*0.5) > 0.001)
            {
                if (_baseLine != null)
                    _baseLine.Hide();
                if (IsNegative)
                    CreateBaseLine();
            }
        }

        public void ShowImageOnTopView()
        {
            _cameraPhi = Math.PI * 0.5;
            _cameraTheta = Math.PI * 0.5;
            double y = _cameraR * Math.Sin(_cameraPhi);
            double hyp = _cameraR * Math.Cos(_cameraPhi);
            double x = hyp * Math.Cos(_cameraTheta);
            double z = hyp * Math.Sin(_cameraTheta);
            MainPerspectiveCamera.Position = new Point3D(x, y, z);
            // Look toward the origin.
            MainPerspectiveCamera.LookDirection = new Vector3D(-x, -y, -z);
            // Set the Up direction.
            MainPerspectiveCamera.UpDirection = new Vector3D(0, 0, -1);
            if (_baseLine != null)
            {
                _baseLine.Hide();
            }
        }

        public void ShowImageOnPerspectiveView()
        {
            _cameraPhi = Math.PI / 4;
            _cameraTheta = Math.PI / 4;
            double y = _cameraR * Math.Sin(_cameraPhi);
            double hyp = _cameraR * Math.Cos(_cameraPhi);
            double x = hyp * Math.Cos(_cameraTheta);
            double z = hyp * Math.Sin(_cameraTheta);
            MainPerspectiveCamera.Position = new Point3D(x, y, z);
            // Look toward the origin.
            MainPerspectiveCamera.LookDirection = new Vector3D(-x, -y, -z);
            // Set the Up direction.
            MainPerspectiveCamera.UpDirection = new Vector3D(-1, -1, -1);
            if (_baseLine != null)
                _baseLine.Hide();
            if (IsNegative)
                CreateBaseLine();
        }

        internal void RemoveGrid3DControl()
        {
            if (_defectColorModel != null)
            {
                // [NCS-2695] CID 171183 Dereference null return (stat)
                //MainModelGroup.Children.Remove(_defectColorModel);
                MainModelGroup.Children?.Remove(_defectColorModel);
            }
        }

        private void SetUIElement(Border borderText, Viewport3D mainViewport3D, Point3D point3d, String content)
        {
            if (mainViewport3D != null)
            {
                Point p2d = Panel3DMath.Get2DPoint(point3d, mainViewport3D);
                Canvas.SetTop(borderText, p2d.Y + 0);
                Canvas.SetLeft(borderText, p2d.X + 10);
            }
        }

        public void SetScaleValue(double scale)
        {
            try
            {
                if (_scale != null)
                {
                    _scale.ScaleX = scale;
                    _scale.ScaleY = scale;
                    _scale.ScaleZ = scale;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
            }
        }

        #endregion
    }

    public class Panel3DMath
    {
        #region  Helpers
        public static Vector3D Transform3DVector(Transform3D transform, Vector3D vector)
        {
            Point3D input = new Point3D(vector.X, vector.Y, vector.Z);
            Point3D output;
            if (transform.TryTransform(input, out output))
            {
                return new Vector3D(output.X, output.Y, output.Z);
            }
            return vector;
        }
        public static Point Get2DPoint(Point3D p3d, Viewport3D vp)
        {
            // [NCS-2695] CID 171129 Unchecked dynamic_cast
            //bool TransformationResultOK;
            //Viewport3DVisual vp3Dv = VisualTreeHelper.GetParent(vp.Children[0]) as Viewport3DVisual;
            //Matrix3D m = MathUtils.TryWorldToViewportTransform(vp3Dv, out TransformationResultOK);
            //if (!TransformationResultOK)
            //    return new Point(0, 0);
            //Point3D pb = m.Transform(p3d);
            //Point p2d = new Point(pb.X, pb.Y);
            //return p2d;

            bool TransformationResultOK = false;
            if (VisualTreeHelper.GetParent(vp?.Children?[0]) is Viewport3DVisual vp3Dv)
            {
                if (null == vp3Dv)
                {
                    return new Point(0, 0);
                }

                Matrix3D m = BasicMath.TryWorldToViewportTransform(vp3Dv, out TransformationResultOK);

                if (TransformationResultOK == false)
                {
                    return new Point(0, 0);
                }

                Point3D pb = m.Transform(p3d);
                return new Point(pb.X, pb.Y);
            }
            else
            {
                return new Point(0, 0);
            }
        }
        #endregion
    }
}
