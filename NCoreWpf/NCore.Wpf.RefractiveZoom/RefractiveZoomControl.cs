using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NCore.Wpf.RefractiveZoom
{
    public delegate void RefractiveZoomControlMouseDownEventHandler(object sender, Point point, MouseEventArgs e);
    public delegate void RefractiveZoomControlMouseUpEventHandler(object sender, Point point, bool isClicked, MouseEventArgs e);
    public delegate void RefractiveZoomControlMouseMoveEventHandler(object sender, Point point, MouseEventArgs e);

    /// <summary>
    /// [NCS-3103] : Refractive index 창에서 scan image zoom in/out 기능 구현되지 않음
    /// </summary>
    [TemplatePart(Name = PART_Presenter, Type = typeof(RefractiveZoomContentPresenter))]
    public class RefractiveZoomControl : ContentControl
    {
        public event RefractiveZoomControlMouseDownEventHandler ZoomControlMouseDownEvent;
        public event RefractiveZoomControlMouseUpEventHandler ZoomControlMouseUpEvent;
        public event RefractiveZoomControlMouseMoveEventHandler ZoomControlMouseMoveEvent;

        private const string PART_Presenter = "PART_Presenter";

        public static readonly DependencyProperty HideZoomProperty = DependencyProperty.Register("HideZoom", typeof(Visibility), typeof(RefractiveZoomControl), new UIPropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty AnimationLengthProperty = DependencyProperty.Register("AnimationLength", typeof(TimeSpan), typeof(RefractiveZoomControl), new UIPropertyMetadata(TimeSpan.FromMilliseconds(0)));
        public static readonly DependencyProperty MaxZoomDeltaProperty = DependencyProperty.Register("MaxZoomDelta", typeof(double), typeof(RefractiveZoomControl), new UIPropertyMetadata(5.0));
        public static readonly DependencyProperty MaxZoomProperty = DependencyProperty.Register("MaxZoom", typeof(double), typeof(RefractiveZoomControl), new UIPropertyMetadata(100.0));
        public static readonly DependencyProperty MinZoomProperty = DependencyProperty.Register("MinZoom", typeof(double), typeof(RefractiveZoomControl), new UIPropertyMetadata(0.01));
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(RefractiveZoomControlModes), typeof(RefractiveZoomControl), new UIPropertyMetadata(RefractiveZoomControlModes.Custom, Mode_PropertyChanged));
        public static readonly DependencyProperty ModifierModeProperty = DependencyProperty.Register("ModifierMode", typeof(RefractiveZoomViewModifierMode), typeof(RefractiveZoomControl), new UIPropertyMetadata(RefractiveZoomViewModifierMode.None));
        public static readonly DependencyProperty TranslateXProperty = DependencyProperty.Register("TranslateX", typeof(double), typeof(RefractiveZoomControl), new UIPropertyMetadata(0.0, TranslateX_PropertyChanged, TranslateX_Coerce));
        public static readonly DependencyProperty TranslateYProperty = DependencyProperty.Register("TranslateY", typeof(double), typeof(RefractiveZoomControl), new UIPropertyMetadata(0.0, TranslateY_PropertyChanged, TranslateY_Coerce));
        public static readonly DependencyProperty ZoomBoxBackgroundProperty = DependencyProperty.Register("ZoomBoxBackground", typeof(Brush), typeof(RefractiveZoomControl), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ZoomBoxBorderBrushProperty = DependencyProperty.Register("ZoomBoxBorderBrush", typeof(Brush), typeof(RefractiveZoomControl), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ZoomBoxBorderThicknessProperty = DependencyProperty.Register("ZoomBoxBorderThickness", typeof(Thickness), typeof(RefractiveZoomControl), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ZoomBoxOpacityProperty = DependencyProperty.Register("ZoomBoxOpacity", typeof(double), typeof(RefractiveZoomControl), new UIPropertyMetadata(0.5));
        public static readonly DependencyProperty ZoomBoxProperty = DependencyProperty.Register("ZoomBox", typeof(Rect), typeof(RefractiveZoomControl), new UIPropertyMetadata(new Rect()));
        public static readonly DependencyProperty ZoomDeltaMultiplierProperty = DependencyProperty.Register("ZoomDeltaMultiplier", typeof(double), typeof(RefractiveZoomControl), new UIPropertyMetadata(20.0));
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(RefractiveZoomControl), new UIPropertyMetadata(1.0, Zoom_PropertyChanged));

        private static void Mode_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zc = (RefractiveZoomControl)d;
            var mode = (RefractiveZoomControlModes)e.NewValue;
            switch (mode)
            {
                case RefractiveZoomControlModes.Fill:
                    zc.DoZoomToFill();
                    break;
                case RefractiveZoomControlModes.Original:
                    zc.DoZoomToOriginal();
                    break;
                case RefractiveZoomControlModes.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static object TranslateX_Coerce(DependencyObject d, object basevalue)
        {
            var zc = (RefractiveZoomControl)d;
            return zc.GetCoercedTranslateX((double)basevalue, zc.Zoom);
        }

        private static object TranslateY_Coerce(DependencyObject d, object basevalue)
        {
            var zc = (RefractiveZoomControl)d;
            return zc.GetCoercedTranslateY((double)basevalue, zc.Zoom);
        }

        private static void TranslateX_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zc = (RefractiveZoomControl)d;
            if (zc._translateTransform == null)
                return;
            zc._translateTransform.X = (double)e.NewValue;

            if (!zc._isZooming)
                zc.Mode = RefractiveZoomControlModes.Custom;
        }

        private static void TranslateY_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zc = (RefractiveZoomControl)d;
            if (zc._translateTransform == null)
                return;
            zc._translateTransform.Y = (double)e.NewValue;

            if (!zc._isZooming)
                zc.Mode = RefractiveZoomControlModes.Custom;
        }

        private static void Zoom_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zc = (RefractiveZoomControl)d;

            if (zc._scaleTransform == null)
                return;

            double zoom = (double)e.NewValue;
            zc._scaleTransform.ScaleX = zoom;
            zc._scaleTransform.ScaleY = zoom;
            if (!zc._isZooming)
            {
                double delta = (double)e.NewValue / (double)e.OldValue;
                zc.TranslateX *= delta;
                zc.TranslateY *= delta;
                zc.Mode = RefractiveZoomControlModes.Custom;
            }
        }

        private Point _mouseDownPos;

        private RefractiveZoomContentPresenter _presenter;

        /// <summary>
        /// Applied to the presenter.
        /// </summary>
        private ScaleTransform _scaleTransform;
        private Vector _startTranslate;
        private TransformGroup _transformGroup;

        /// <summary>
        /// Applied to the scrollviewer.
        /// </summary>
        private TranslateTransform _translateTransform;

        private int _zoomAnimCount;
        private bool _isZooming = false;

        
        public Brush ZoomBoxBackground
        {
            get { return (Brush)GetValue(ZoomBoxBackgroundProperty); }
            set { SetValue(ZoomBoxBackgroundProperty, value); }
        }

        public Brush ZoomBoxBorderBrush
        {
            get { return (Brush)GetValue(ZoomBoxBorderBrushProperty); }
            set { SetValue(ZoomBoxBorderBrushProperty, value); }
        }

        public Thickness ZoomBoxBorderThickness
        {
            get { return (Thickness)GetValue(ZoomBoxBorderThicknessProperty); }
            set { SetValue(ZoomBoxBorderThicknessProperty, value); }
        }

        public double ZoomBoxOpacity
        {
            get { return (double)GetValue(ZoomBoxOpacityProperty); }
            set { SetValue(ZoomBoxOpacityProperty, value); }
        }

        public Rect ZoomBox
        {
            get { return (Rect)GetValue(ZoomBoxProperty); }
            set { SetValue(ZoomBoxProperty, value); }
        }

        public Point OrigoPosition
        {
            get { return new Point(ActualWidth / 2, ActualHeight / 2); }
        }

        public double TranslateX
        {
            get { return (double)GetValue(TranslateXProperty); }
            set
            {
                BeginAnimation(TranslateXProperty, null);
                SetValue(TranslateXProperty, value);
            }
        }

        public double TranslateY
        {
            get { return (double)GetValue(TranslateYProperty); }
            set
            {
                BeginAnimation(TranslateYProperty, null);
                SetValue(TranslateYProperty, value);
            }
        }

        public TimeSpan AnimationLength
        {
            get { return (TimeSpan)GetValue(AnimationLengthProperty); }
            set { SetValue(AnimationLengthProperty, value); }
        }

        public double MinZoom
        {
            get { return (double)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        public double MaxZoom
        {
            get { return (double)GetValue(MaxZoomProperty); }
            set { SetValue(MaxZoomProperty, value); }
        }

        public double MaxZoomDelta
        {
            get { return (double)GetValue(MaxZoomDeltaProperty); }
            set { SetValue(MaxZoomDeltaProperty, value); }
        }

        public double ZoomDeltaMultiplier
        {
            get { return (double)GetValue(ZoomDeltaMultiplierProperty); }
            set { SetValue(ZoomDeltaMultiplierProperty, value); }
        }

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set
            {
                if (value == (double)GetValue(ZoomProperty))
                    return;
                BeginAnimation(ZoomProperty, null);
                SetValue(ZoomProperty, value);
            }
        }

        protected RefractiveZoomContentPresenter Presenter
        {
            get { return _presenter; }
            set
            {
                _presenter = value;
                if (_presenter == null)
                    return;

                //add the ScaleTransform to the presenter
                _transformGroup = new TransformGroup();
                _scaleTransform = new ScaleTransform();
                _translateTransform = new TranslateTransform();
                _transformGroup.Children.Add(_scaleTransform);
                _transformGroup.Children.Add(_translateTransform);
                _presenter.RenderTransform = _transformGroup;
                _presenter.RenderTransformOrigin = new Point(0.5, 0.5);
            }
        }

        /// <summary>
        /// Gets or sets the active modifier mode.
        /// </summary>
        public RefractiveZoomViewModifierMode ModifierMode
        {
            get { return (RefractiveZoomViewModifierMode)GetValue(ModifierModeProperty); }
            set { SetValue(ModifierModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the mode of the zoom control.
        /// </summary>
        public RefractiveZoomControlModes Mode
        {
            get { return (RefractiveZoomControlModes)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        static RefractiveZoomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RefractiveZoomControl), new FrameworkPropertyMetadata(typeof(RefractiveZoomControl)));
        }

        public RefractiveZoomControl()
        {
            this.PreviewMouseWheel += this.RefractiveZoomControl_MouseWheel;
            this.MouseWheel += this.RefractiveZoomControl_MouseWheel;
            this.PreviewMouseDown += this.RefractiveZoomControl_PreviewMouseDown;
            this.MouseDown += this.RefractiveZoomControl_MouseDown;
            this.MouseUp += this.RefractiveZoomControl_MouseUp;
            this.MouseMove += this.RefractiveZoomControl_MouseMove;
        }

        private void RefractiveZoomControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool handle = ModifierMode == RefractiveZoomViewModifierMode.None;
            if (!handle) { return; }

            e.Handled = true;

            Point origoPosition = new Point(ActualWidth / 2, ActualHeight / 2);
            Point mousePosition = e.GetPosition(this);

            this.DoZoom(Math.Max(1 / MaxZoomDelta, Math.Min(MaxZoomDelta, e.Delta / 10000.0 * ZoomDeltaMultiplier + 1))
                      , origoPosition
                      , mousePosition
                      , mousePosition);
        }

        private void RefractiveZoomControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.OnMouseDown(e, true);
        }

        private void RefractiveZoomControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.OnMouseDown(e, false);
        }

        private void RefractiveZoomControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (ModifierMode)
            {
                case RefractiveZoomViewModifierMode.None:
                    {
                        if (this.ZoomControlMouseUpEvent != null)
                        {
                            var p = e.GetPosition(this.Presenter);
                            this.ZoomControlMouseUpEvent(this, p, true, e);
                        }
                    }
                    return;
                case RefractiveZoomViewModifierMode.Pan:
                    break;
                case RefractiveZoomViewModifierMode.ZoomIn:
                    break;
                case RefractiveZoomViewModifierMode.ZoomOut:
                    break;
                case RefractiveZoomViewModifierMode.ZoomBox:
                    this.ZoomTo(ZoomBox);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.ModifierMode = RefractiveZoomViewModifierMode.None;
            this.PreviewMouseMove -= ZoomControl_PreviewMouseMove;
            this.ReleaseMouseCapture();
        }

        private void RefractiveZoomControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.ZoomControlMouseUpEvent != null)
            {
                var p = e.GetPosition(Presenter);
                this.ZoomControlMouseUpEvent(this, p, false, e);
            }

            this.ZoomControl_PreviewMouseMove(sender, e);
        }

        private void ZoomControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            switch (ModifierMode)
            {
                case RefractiveZoomViewModifierMode.None:
                    if (this.ZoomControlMouseMoveEvent != null)
                    {
                        this.ZoomControlMouseMoveEvent(this, e.GetPosition(this.Presenter), e);
                    }
                    return;
                case RefractiveZoomViewModifierMode.Pan:
                    var translate = _startTranslate + (e.GetPosition(this) - _mouseDownPos);
                    TranslateX = translate.X;
                    TranslateY = translate.Y;
                    break;
                case RefractiveZoomViewModifierMode.ZoomIn:
                    break;
                case RefractiveZoomViewModifierMode.ZoomOut:
                    break;
                case RefractiveZoomViewModifierMode.ZoomBox:
                    var pos = e.GetPosition(this);
                    var x = Math.Min(_mouseDownPos.X, pos.X);
                    var y = Math.Min(_mouseDownPos.Y, pos.Y);
                    var sizeX = Math.Abs(_mouseDownPos.X - pos.X);
                    var sizeY = Math.Abs(_mouseDownPos.Y - pos.Y);
                    ZoomBox = new Rect(x, y, sizeX, sizeY);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private double GetCoercedTranslateX(double baseValue, double zoom)
        {
            if (_presenter == null)
                return 0.0;

            return GetCoercedTranslate(baseValue, zoom,
                                       _presenter.ContentSize.Width,
                                       _presenter.DesiredSize.Width,
                                       ActualWidth);
        }

        private double GetCoercedTranslateY(double baseValue, double zoom)
        {
            if (_presenter == null)
                return 0.0;

            return GetCoercedTranslate(baseValue, zoom,
                                       _presenter.ContentSize.Height,
                                       _presenter.DesiredSize.Height,
                                       ActualHeight);
        }

        /// <summary>
        /// Coerces the translation.
        /// </summary>
        /// <param name="translate">The desired translate.</param>
        /// <param name="zoom">The factor of the zoom.</param>
        /// <param name="contentSize">The size of the content inside the zoomed ContentPresenter.</param>
        /// <param name="desiredSize">The desired size of the zoomed ContentPresenter.</param>
        /// <param name="actualSize">The size of the RefractiveZoomControl.</param>
        /// <returns>The coerced translation.</returns>
        private double GetCoercedTranslate(double translate, double zoom, double contentSize, double desiredSize, double actualSize)
        {
            return translate;
        }

        public void ZoomTo(Rect rect)
        {
            var deltaZoom = Math.Min(
                ActualWidth / rect.Width,
                ActualHeight / rect.Height);

            var startHandlePosition = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            DoZoom(deltaZoom, OrigoPosition, startHandlePosition, OrigoPosition);
            ZoomBox = new Rect();
        }

        private void OnMouseDown(MouseButtonEventArgs e, bool isPreview)
        {
            if (ModifierMode != RefractiveZoomViewModifierMode.None)
                return;

            switch (Keyboard.Modifiers)
            {
                case ModifierKeys.None:
                    if (!isPreview)
                    {
                        if (e.MiddleButton == MouseButtonState.Pressed)
                        {
                            ModifierMode = RefractiveZoomViewModifierMode.Pan;
                        }
                    }
                    break;
                case ModifierKeys.Alt:
                    ModifierMode = RefractiveZoomViewModifierMode.ZoomBox;
                    break;
                case ModifierKeys.Control:
                    break;
                case ModifierKeys.Shift:
                    ModifierMode = RefractiveZoomViewModifierMode.Pan;
                    break;
                case ModifierKeys.Windows:
                    break;
                default:
                    return;
            }

            if (this.ZoomControlMouseDownEvent != null)
            {
                this.ZoomControlMouseDownEvent(this, e.GetPosition(this.Presenter), e);
            }

            if (ModifierMode == RefractiveZoomViewModifierMode.None)
                return;

            _mouseDownPos = e.GetPosition(this);
            _startTranslate = new Vector(TranslateX, TranslateY);
            Mouse.Capture(this);
            PreviewMouseMove += ZoomControl_PreviewMouseMove;
        }

        private void DoZoom(double deltaZoom, Point origoPosition, Point startHandlePosition, Point targetHandlePosition)
        {
            double startZoom = Zoom;
            double currentZoom = startZoom * deltaZoom;
            currentZoom = Math.Max(MinZoom, Math.Min(MaxZoom, currentZoom));

            var fillZoom = Math.Min(ActualWidth / _presenter.ContentSize.Width, ActualHeight / _presenter.ContentSize.Height);
            if (currentZoom < fillZoom) { currentZoom = fillZoom; }

            var startTranslate = new Vector(TranslateX, TranslateY);

            var v = (startHandlePosition - origoPosition);
            var vTarget = (targetHandlePosition - origoPosition);

            var targetPoint = (v - startTranslate) / startZoom;
            var zoomedTargetPointPos = targetPoint * currentZoom + startTranslate;
            var endTranslate = vTarget - zoomedTargetPointPos;

            double transformX = GetCoercedTranslateX(TranslateX + endTranslate.X, currentZoom);
            double transformY = GetCoercedTranslateY(TranslateY + endTranslate.Y, currentZoom);

            DoZoomAnimation(currentZoom, transformX, transformY);
            Mode = RefractiveZoomControlModes.Custom;
        }

        private void DoZoomAnimation(double targetZoom, double transformX, double transformY)
        {
            _isZooming = true;
            var duration = new Duration(AnimationLength);
            StartAnimation(TranslateXProperty, transformX, duration);
            StartAnimation(TranslateYProperty, transformY, duration);
            StartAnimation(ZoomProperty, targetZoom, duration);
        }

        private void StartAnimation(DependencyProperty dp, double toValue, Duration duration)
        {
            if (double.IsNaN(toValue) || double.IsInfinity(toValue))
            {
                if (dp == ZoomProperty)
                {
                    _isZooming = false;
                }
                return;
            }
            var animation = new DoubleAnimation(toValue, duration);
            if (dp == ZoomProperty)
            {
                _zoomAnimCount++;
                animation.Completed += (s, args) =>
                                           {
                                               _zoomAnimCount--;
                                               if (_zoomAnimCount > 0)
                                                   return;
                                               var zoom = Zoom;
                                               BeginAnimation(ZoomProperty, null);
                                               SetValue(ZoomProperty, zoom);
                                               _isZooming = false;
                                           };
            }
            BeginAnimation(dp, animation, HandoffBehavior.Compose);
        }

        public void ZoomToOriginal()
        {
            Mode = RefractiveZoomControlModes.Original;
        }

        private void DoZoomToOriginal()
        {
            if (_presenter == null)
                return;

            var initialTranslate = GetInitialTranslate();
            DoZoomAnimation(1.0, initialTranslate.X, initialTranslate.Y);
        }

        private Vector GetInitialTranslate()
        {
            if (_presenter == null)
                return new Vector(0.0, 0.0);
            var w = _presenter.ContentSize.Width - _presenter.DesiredSize.Width;
            var h = _presenter.ContentSize.Height - _presenter.DesiredSize.Height;
            var tX = -w / 2.0;
            var tY = -h / 2.0;

            return new Vector(tX, tY);
            //return new Vector(0, 0);
        }
        public void ZoomToFill()
        {
            Mode = RefractiveZoomControlModes.Fill;
        }

        private void DoZoomToFill()
        {
            if (_presenter == null)
                return;

            var deltaZoom = Math.Min(
                ActualWidth / _presenter.ContentSize.Width,
                ActualHeight / _presenter.ContentSize.Height);

            var initialTranslate = GetInitialTranslate();
            DoZoomAnimation(deltaZoom, initialTranslate.X * deltaZoom, initialTranslate.Y * deltaZoom);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //get the presenter, and initialize
            Presenter = GetTemplateChild(PART_Presenter) as RefractiveZoomContentPresenter;
            if (Presenter != null)
            {
                Presenter.SizeChanged += (s, a) =>
                                             {
                                                 if (Mode == RefractiveZoomControlModes.Fill)
                                                     DoZoomToFill();
                                             };
                Presenter.ContentSizeChanged += (s, a) =>
                {
                    if (Mode == RefractiveZoomControlModes.Fill)
                        DoZoomToFill();
                };
            }
            ZoomToFill();
        }
    }

    public enum RefractiveZoomControlModes
    {
        /// <summary>
        /// The content should fill the given space.
        /// </summary>
        Fill,

        /// <summary>
        /// The content will be represented in its original size.
        /// </summary>
        Original,

        /// <summary>
        /// The content will be zoomed with a custom percent.
        /// </summary>
        Custom
    }

    public enum RefractiveZoomViewModifierMode
    {
        /// <summary>
        /// It does nothing at all.
        /// </summary>
        None,

        /// <summary>
        /// You can pan the view with the mouse in this mode.
        /// </summary>
        Pan,

        /// <summary>
        /// You can zoom in with the mouse in this mode.
        /// </summary>
        ZoomIn, 

        /// <summary>
        /// You can zoom out with the mouse in this mode.
        /// </summary>
        ZoomOut,

        /// <summary>
        /// Zooming after the user has been selected the zooming box.
        /// </summary>
        ZoomBox
    }
}
