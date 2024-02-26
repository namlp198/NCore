using Npc.Foundation.Helper;
using Npc.Foundation.Logger;
using NpcCore.Wpf.Core;
using NpcCore.Wpf.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
namespace NpcCore.Wpf.Core
{
    public class CrossSection
    {
        #region Fields

        private const double yScale = 0.05;

        private double _width;
        private double _length;
        private double _height;
        private double _extendWidth = 0;
        private double _extendLength = 0;
        private double _extendHeight = 5;
        private double _extendHeightColumn = 5;
        private double _heightPyramid = 5;
        private double _radiusPyramid = 4;
        private double _columnRadius = 0.7;

        private Viewport3D _viewport;
        private PerspectiveCamera _camera;
        private ModelVisual3D _visual3d;
        private Model3DGroup _group;
        private Grid _mainGrid;

        // Variables used to drag.
        private Viewport3D _dragViewport;
        private PerspectiveCamera _dragCamera;
        private Model3DGroup _dragGroup;
        private Point3D _dragPoint;

        // The currently selected model and mesh.
        private MeshGeometry3D _selectedMesh = null;
        private Model3DEnum _meshNameSelected = Model3DEnum.Unknown;

        private Point3D _startPoint;
        private Point3D _endPoint;
        private GeometryModel3D _crossPlaneModel = null;

        private List<Model3D> _models = new List<Model3D>();

        protected int _dataPointIndex;

        private int _position;
        private bool _isBase;

        private bool _keepContent = false;

        private Color _pinColor;
        #endregion

        #region Properties
        public Point3D StartPoint
        {
            get
            {
                return _startPoint;
            }
            set
            {
                _startPoint = value;
            }
        }

        public Point3D EndPoint
        {
            get
            {
                return _endPoint;
            }
            set
            {
                _endPoint = value;
            }
        }

        public bool KeepContent
        {
            get
            {
                return _keepContent;
            }
            set
            {
                _keepContent = value;
            }
        }

        public string Uid { get; private set; }

        public Visibility Visibility { get; private set; }

        public bool IsVisible { get => Visibility == Visibility.Visible; }

        public bool IsDragging { get => _selectedMesh != null; }

        #endregion

        #region event

        public event EventHandler CrossPointChanged;

        #endregion

        #region Constructors
        public CrossSection()
        {
            Uid = Guid.NewGuid().ToString();
            _models = new List<Model3D>();

            Visibility = Visibility.Collapsed;
        }

        public CrossSection(Viewport3D viewport, int width, int length, float height, Color pinColor, double extend, int position = 0, bool isBase = false) : this()
        {
            _viewport = viewport;
            _camera = viewport?.Camera as PerspectiveCamera;
            _visual3d = viewport?.Children?.FirstOrDefault(m => m is ModelVisual3D) as ModelVisual3D;

            // [NCS-2695] CID 171164 Unchecked dynamic_cast
            //_group = _visual3d.Content as Model3DGroup;
            if (_visual3d != null)
            {
                _group = _visual3d.Content as Model3DGroup;
            }
            _mainGrid = _viewport.Parent as Grid;
            _extendWidth = extend;
            _extendLength = extend;
            _width = width + _extendWidth;
            _length = length + _extendLength;
            _height = height + _extendHeight;

            _position = position;
            _pinColor = pinColor;
            _isBase = isBase;
        }
        #endregion

        #region Methods

        public void Show()
        {
            if (_viewport == null)
                return;

            if (_models.Any())
                Hide(KeepContent);

            if (!KeepContent)
            {
                // TODO
                if (_position == 0)
                {
                    StartPoint = new Point3D(0, (_height + _extendHeight) * 0.5, (-1)*_length*0.5);
                    EndPoint = new Point3D(0, (_height + _extendHeight) * 0.5, _length*0.5);
                }
                else if (_position == 1)
                {
                    StartPoint = new Point3D((-1)*_width*0.5, (_height + _extendHeight) * 0.5, 0);
                    EndPoint = new Point3D(_width * 0.5, (_height + _extendHeight) * 0.5, 0);
                }
            }
            _crossPlaneModel = MeshCreator.DrawCrossPlane(StartPoint , EndPoint, _extendHeight);
            _models.Add(_crossPlaneModel);
            if (!_isBase)
            {
                double adjustHeightColumn = _extendHeightColumn - 1;
                Point3D startPyramidLeft = new Point3D(StartPoint.X, StartPoint.Y + _extendHeightColumn, StartPoint.Z);
                Point3D startPyramidRight = new Point3D(EndPoint.X, EndPoint.Y + _extendHeightColumn, EndPoint.Z);
                Point3D startColumnLeft = new Point3D(StartPoint.X, StartPoint.Y + _extendHeightColumn - adjustHeightColumn, StartPoint.Z);
                Point3D startColumnRight = new Point3D(EndPoint.X, EndPoint.Y + _extendHeightColumn - adjustHeightColumn, EndPoint.Z);
                Vector3D axis = new Vector3D(0, (-1) * _heightPyramid, 0);
                _models.Add(MeshCreator.DrawPyramid(startPyramidLeft, axis, _radiusPyramid, Colors.Transparent, nameof(Model3DEnum.StartPointModel), MeshCreator.LeftSide, _position));
                _models.Add(MeshCreator.DrawPyramid(startPyramidRight, axis, _radiusPyramid, _pinColor, nameof(Model3DEnum.EndPointModel), MeshCreator.RightSide, _position));
                _models.Add(MeshCreator.DrawColumn(startColumnLeft, new Vector3D(0, (-1) * (_height + _extendHeightColumn - adjustHeightColumn), 0), _columnRadius, _pinColor, nameof(Model3DEnum.StartColumn)));
                _models.Add(MeshCreator.DrawColumn(startColumnRight, new Vector3D(0, (-1) * (_height + _extendHeightColumn - adjustHeightColumn), 0), _columnRadius, _pinColor, nameof(Model3DEnum.EndColumn)));
                _models.Add(MeshCreator.DrawPyramid(startPyramidLeft, axis, _radiusPyramid, _pinColor, nameof(Model3DEnum.Wrireframe), MeshCreator.LeftSide, _position));
            }

            // [NCS-2695] CID 171180 Dereference null return (stat)
            //foreach (var model in _models)
            //{
            //    _group.Children.Add(model);
            //}
            if (_group.Children != null)
            {
                foreach (var model in _models)
                {
                    _group.Children.Add(model);
                }
            }

            // Build the drag environment.
            MakeDragEnvironment();

            Visibility = Visibility.Visible;
            CrossPointChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Hide(bool keepContent = false)
        {
            if (_viewport == null)
                return;

            // [NCS-2695] CID 171179 Dereference null return (stat)
            //foreach (var model in _models)
            //{
            //    _group.Children.Remove(model);
            //}
            if (_group.Children != null)
            {
                foreach (var model in _models)
                {
                    _group.Children.Remove(model);
                }
            }

            _models.Clear();

            _selectedMesh = null;

            KeepContent = keepContent;
            Visibility = Visibility.Collapsed;

            CrossPointChanged?.Invoke(this, EventArgs.Empty);
        }

        // TODO
        public List<Point> Get2DIntersections()
        {
            List<Point> intersections2D = new List<Point>();

            return intersections2D;
        }

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            if (_viewport == null
                || this.IsVisible == false)
                return;

            // Deselect any previously selected mesh.
            _selectedMesh = null;

            // Get the mouse's position relative to the viewport.
            Point mousePos = e.GetPosition(_viewport);

            // See where the ray from the camera to the new point hits the drag sphere.
            var rayMesh = CastRay(_viewport, mousePos);
            if (rayMesh.Count == 0)
                return;

            RayMeshGeometry3DHitTestResult meshResult = null;
            if (rayMesh.Count > 0)
                meshResult = rayMesh[0];

            var modelHit = meshResult.ModelHit as GeometryModel3D;
            if (_models.Contains(modelHit) == false)
                return;

            var name = AutomationProperties.GetName(modelHit).ToEnum(default(Model3DEnum));

            if (name == Model3DEnum.StartPointModel ||
                name == Model3DEnum.EndPointModel)
            {
                _dataPointIndex = name == Model3DEnum.StartPointModel ? 0 : 1;

                // Record the model, mesh, and point that were hit.
                _selectedMesh = meshResult.MeshHit;
                _meshNameSelected = name;

                _dragPoint = _dataPointIndex == 0 ? StartPoint : EndPoint;
                //DragPoint = meshResult.PointHit;

                // Initialize the drag environment
                // Make the drag viewport fit the visible one.
                _dragViewport.Width = _viewport.ActualWidth;
                _dragViewport.Height = _viewport.ActualHeight;

                // Make the camera match the main viewport's camera.
                _dragCamera.FieldOfView = _camera.FieldOfView;
                _dragCamera.UpDirection = _camera.UpDirection;
                _dragCamera.LookDirection = _camera.LookDirection;
                _dragCamera.Position = _camera.Position;

                //DragViewport.Visibility = Visibility.Visible;
            }
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            if (_viewport == null
                || !IsDragging
                || !IsVisible)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                OnMouseUp();
                return;
            }

            // Get the mouse's new position.
            Point newPoint = e.GetPosition(_mainGrid);

            // See where the ray from the camera to the new point hits the drag sphere.
            var rayMesh = CastRay(_dragViewport, newPoint);
            if (rayMesh.Count == 0)
                return;
            RayMeshGeometry3DHitTestResult meshResult = rayMesh[0];

            var pointHit = GetPointHit(meshResult);
            pointHit = ApplyReverseTransformation(pointHit, _group.Transform as Transform3DGroup);
            pointHit = ValidatePointHit(pointHit);


            var transform = new TranslateTransform3D(pointHit - _dragPoint);

            // Translate the selected model.
            _selectedMesh.ApplyTransformation(transform);
            if (_models.Count == 6)
            {
                GeometryModel3D columnModel = null;
                GeometryModel3D wireframeModel = null;
                if (_meshNameSelected == Model3DEnum.StartPointModel)
                {
                     columnModel = _models.ElementAt((int)Model3DEnum.StartColumn) as GeometryModel3D;
                    wireframeModel = _models.ElementAt((int)Model3DEnum.Wrireframe) as GeometryModel3D;
                }
                else if (_meshNameSelected== Model3DEnum.EndPointModel)
                {
                    columnModel = _models.ElementAt((int)Model3DEnum.EndColumn) as GeometryModel3D;
                }
                if (columnModel != null)
                {
                    MeshGeometry3D meshGeometry = columnModel.Geometry as MeshGeometry3D;
                    if (meshGeometry != null)
                    {
                        meshGeometry.ApplyTransformation(transform);
                    }
                }
                if (wireframeModel != null)
                {
                    MeshGeometry3D meshWireframeGeometry = wireframeModel.Geometry as MeshGeometry3D;
                    if (meshWireframeGeometry != null)
                    {
                        meshWireframeGeometry.ApplyTransformation(transform);
                    }
                }
            }
            ApplyTransformCrossSection(_crossPlaneModel.Geometry as MeshGeometry3D, transform, _dataPointIndex);
            _dragPoint = pointHit;
        }

        public void OnMouseUp()
        {
            _dragViewport.Visibility = Visibility.Hidden;

            _selectedMesh = null;
        }


        /// <summary>
        /// Make the drag environment.
        /// </summary>
        private void MakeDragEnvironment()
        {
            _dragViewport = new Viewport3D();
            _dragViewport.Visibility = Visibility.Hidden;
            _mainGrid.Children.Add(_dragViewport);

            ModelVisual3D visual3d = new ModelVisual3D();
            _dragViewport.Children.Add(visual3d);

            AmbientLight light = new AmbientLight(Colors.Gray);

            _dragGroup = new Model3DGroup();

            // [NCS-2695] CID 171161 Dereference null return (stat)
            //_dragGroup.Children.Add(light);
            _dragGroup.Children?.Add(light);

            visual3d.Content = _dragGroup;

            _dragCamera = new PerspectiveCamera();
            _dragViewport.Camera = _dragCamera;

            var model = MeshCreator.DrawDragSurface(100000, 100000, (_height + _extendHeight)*0.5);

            // [NCS-2695] CID 171161 Dereference null return (stat)
            //_dragGroup.Children.Add(model);
            _dragGroup.Children?.Add(model);

            _dragGroup.Transform = _group.Transform;
        }

        //  This will cast a ray from the point (on _viewport) along the direction that the camera is looking, and returns hits
        private List<RayMeshGeometry3DHitTestResult> CastRay(Viewport3D viewport, Point clickPoint, IEnumerable<Visual3D> ignoreVisuals = null)
        {
            List<RayMeshGeometry3DHitTestResult> retVal = new List<RayMeshGeometry3DHitTestResult>();

            //  This gets called every time there is a hit
            HitTestResultCallback resultCallback = delegate (HitTestResult result)
            {
                if (result is RayMeshGeometry3DHitTestResult)       //  It could also be a RayHitTestResult, which isn't as exact as RayMeshGeometry3DHitTestResult
                {
                    RayMeshGeometry3DHitTestResult resultCast = (RayMeshGeometry3DHitTestResult)result;
                    if (ignoreVisuals == null || !ignoreVisuals.Any(o => o == resultCast.VisualHit))
                    {
                        retVal.Add(resultCast);
                    }
                }

                return HitTestResultBehavior.Continue;
            };

            //  Get hits against existing models
            VisualTreeHelper.HitTest(viewport, null, resultCallback, new PointHitTestParameters(clickPoint));

            //  Exit Function
            return retVal;
        }

        private Point3D GetPointHit(RayMeshGeometry3DHitTestResult hit)
        {
            if (hit.VisualHit.Transform != null)
            {
                return hit.VisualHit.Transform.Transform(hit.PointHit);
            }
            else
            {
                return hit.PointHit;
            }
        }

        private Point3D ApplyReverseTransformation(Point3D point, Transform3DGroup transformGroup)
        {
            if (transformGroup != null)
            {
                foreach (var transform in transformGroup.Children)
                {
                    if (transform is ScaleTransform3D scale)
                    {
                        point = point.Transform(D3.InverseTransform(scale));
                    }
                    else if (transform is RotateTransform3D rotate)
                    {
                        point = point.Transform(D3.InverseTransform(rotate));
                    }
                }
            }

            return point.Round();
        }

        private Point3D ValidatePointHit(Point3D point)
        {
            double halfSizeX = _width / 2.0;
            double halfSizeZ = _length / 2.0;

            if (Math.Abs(point.X) > halfSizeX)
                point.X = halfSizeX * Math.Sign(point.X);

            if (Math.Abs(point.Z) > halfSizeZ)
                point.Z = halfSizeZ * Math.Sign(point.Z);

            point.Y = (_height + _extendHeight)*0.5;

            return point;
        }

        public void UpdateCrossSectionByPoint(Point3D point, int position)
        {
            try
            {
                if (_models != null && _models.Count == 6 && _crossPlaneModel != null)
                {
                    _dragPoint = position == 0 ? StartPoint : EndPoint;
                    Model3DEnum positionEnum = position == 0 ? Model3DEnum.StartPointModel : Model3DEnum.EndPointModel;
                    var transform = new TranslateTransform3D(point - _dragPoint);
                    GeometryModel3D model = null;
                    GeometryModel3D wireframeModel = null;
                    GeometryModel3D selectedModel = null;
                    if (positionEnum == Model3DEnum.StartPointModel)
                    {
                        model = _models.ElementAt((int)Model3DEnum.StartColumn) as GeometryModel3D;
                        wireframeModel = _models.ElementAt((int)Model3DEnum.Wrireframe) as GeometryModel3D;
                        selectedModel = _models.ElementAt((int)Model3DEnum.StartPointModel) as GeometryModel3D;
                    }
                    else if (positionEnum == Model3DEnum.EndPointModel)
                    {
                        model = _models.ElementAt((int)Model3DEnum.EndColumn) as GeometryModel3D;
                        selectedModel = _models.ElementAt((int)Model3DEnum.EndPointModel) as GeometryModel3D;
                    }
                    if (selectedModel != null)
                    {
                        _selectedMesh = selectedModel.Geometry as MeshGeometry3D;
                        if (_selectedMesh != null)
                        {
                            _selectedMesh.ApplyTransformation(transform);
                        }
                    }
                    if (model != null)
                    {
                        MeshGeometry3D meshGeometry = model.Geometry as MeshGeometry3D;
                        if (meshGeometry != null)
                        {
                            meshGeometry.ApplyTransformation(transform);
                        }
                    }
                    if (wireframeModel != null)
                    {
                        MeshGeometry3D meshWireframeGeometry = wireframeModel.Geometry as MeshGeometry3D;
                        if (meshWireframeGeometry != null)
                        {
                            meshWireframeGeometry.ApplyTransformation(transform);
                        }
                    }
                    ApplyTransformCrossSection(_crossPlaneModel.Geometry as MeshGeometry3D, transform, position);
                    _dragPoint = point;
                    if (_dragViewport != null)
                        _dragViewport.Visibility = Visibility.Hidden;
                    _selectedMesh = null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Fatal(ex);
            }
        }

        private void ApplyTransformCrossSection(MeshGeometry3D mesh, Transform3D transformation, int index)
        {
            bool pointUpdated = false;
            if (index == 0)
            {
                StartPoint = transformation.Transform(StartPoint);
                pointUpdated = true;
            }
            else if (index == 1)
            {
                EndPoint = transformation.Transform(EndPoint);
                pointUpdated = true;
            }

            MeshCreator.UpdateCrossPlane(mesh, StartPoint, EndPoint, _extendHeight);

            if (pointUpdated)
            {
                CrossPointChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
