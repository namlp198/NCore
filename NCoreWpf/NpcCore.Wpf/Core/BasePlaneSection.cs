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
    public class BasePlaneSection
    {
        #region Fields

        private const double yScale = 0.05;
        private double _width;
        private double _length;
        private double _height;
        private double _minHeight;
        private double _extendWidth = 20;
        private double _extendLength = 20;

        private Viewport3D _viewport;
        private PerspectiveCamera _camera;
        private ModelVisual3D _visual3d;
        private Model3DGroup _group;
        private Grid _mainGrid;

        // Variables used to drag.
        private Viewport3D _dragViewport;
        private PerspectiveCamera _dragCamera;
        private Model3DGroup _dragGroup;
        private GeometryModel3D _crossPlaneModel = null;

        private List<Model3D> _models = new List<Model3D>();
        private BaseModes _baseMode = BaseModes.BasePlane;
        #endregion

        #region Properties

        public string Uid { get; private set; }

        public Visibility Visibility { get; private set; }

        public bool IsVisible { get => Visibility == Visibility.Visible; }
        

        #endregion

        #region Constructors
        public BasePlaneSection()
        {
            Uid = Guid.NewGuid().ToString();
            _models = new List<Model3D>();
            Visibility = Visibility.Collapsed;
        }

        public BasePlaneSection(Viewport3D viewport, int width, int length, float height, float minHeight, BaseModes baseMode = BaseModes.BasePlane) : this()
        {
            _viewport = viewport;
            _camera = viewport.Camera as PerspectiveCamera;

            // [NCS-2695] CID 171156 Unchecked dynamic_cast
            //_visual3d = viewport.Children.FirstOrDefault(m => m is ModelVisual3D) as ModelVisual3D;
            //_group = _visual3d.Content as Model3DGroup;
            _visual3d = viewport.Children?.FirstOrDefault(m => m is ModelVisual3D) as ModelVisual3D;
            _group = _visual3d?.Content as Model3DGroup;

            _mainGrid = _viewport.Parent as Grid;

            _width = width + _extendWidth;
            _length = length + _extendLength;
            _height = height;
            _minHeight = minHeight;
            _baseMode = baseMode;
        }
        #endregion

        #region Methods

        public void Show()
        {
            if (_viewport == null)
                return;
            double height = 0.0;
            if (_baseMode == BaseModes.BasePlane)
            {
                height = (-1)*_height * 0.5;
            }
            else
            {
                height = (-1) * _height * 0.5 + Math.Abs(_minHeight);
            }
            Point3D point1 = new Point3D((-1) * _width * 0.5, height, _length * 0.5);
            Point3D point2 = new Point3D(_width * 0.5, height, _length * 0.5);
            Point3D point3 = new Point3D(_width * 0.5, height, (-1) * _length * 0.5);
            Point3D point4 = new Point3D((-1) * _width * 0.5, height, (-1) * _length * 0.5);
            if (_baseMode == BaseModes.BasePlane)
                _crossPlaneModel = MeshCreator.DrawBasePlane(point1, point2, point3, point4);
            else
                _crossPlaneModel = MeshCreator.DrawBaseLine(point1, point2, point3, point4);
            if (_crossPlaneModel != null)
            {
                _models.Add(_crossPlaneModel);

                // [NCS-2695] CID 171158 Dereference null return (stat)
                if (_group.Children != null)
                {
                    foreach (var model in _models)
                    {
                        _group.Children.Add(model);
                    }
                }
            }
            // Build the drag environment.
            MakeDragEnvironment();
            Visibility = Visibility.Visible;
        }

        public void Hide(bool keepContent = false)
        {
            if (_viewport == null)
                return;

            // [NCS-2695] CID 171139 Dereference null return (stat)
            //foreach (var model in _models)
            //{
            //    _group.Children.Remove(model);
            //}
            if (_group != null && _group.Children != null && _group.Children.Any())
            {
                foreach (var model in _models)
                {
                    _group.Children.Remove(model);
                }
            }

            _models.Clear();
            
            Visibility = Visibility.Collapsed;
        }

        // TODO
        public List<Point> Get2DIntersections()
        {
            List<Point> intersections2D = new List<Point>();

            return intersections2D;
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
            // [NCS-2695] CID 171203 Dereference null return (stat)
            //_dragGroup.Children.Add(light);
            _dragGroup.Children?.Add(light);

            visual3d.Content = _dragGroup;

            _dragCamera = new PerspectiveCamera();
            _dragViewport.Camera = _dragCamera;

            var model = MeshCreator.DrawDragSurface(100000, 100000, (_height)*0.5);
            // [NCS-2695] CID 171203 Dereference null return (stat)
            //_dragGroup.Children.Add(model);
            _dragGroup.Children?.Add(model);

            _dragGroup.Transform = _group.Transform;
        }
        #endregion
    }

    public enum BaseModes
    {
        BasePlane,
        BaseLine
    }
}
