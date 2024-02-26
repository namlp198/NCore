using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace NpcCore.Wpf.Controls
{
    public class SliderControl : Control
    {
        enum SliderThumb
        {
            None,
            Start,
            End
        }
        enum TypeSlider
        {
            OneSlider = 1,
            TwoSlider
        }
        #region Fields
        private FrameworkElement _sliderContainer;
        private FrameworkElement _borderContainer;
        private Thumb _startThumb, _endThumb;
        private TextBlock _startThumbValue, _endThumbValue;
        private Thumb _startThumbReverse, _endThumbReverse;
        private FrameworkElement _startArea;
        private FrameworkElement _endArea;
        private FrameworkElement _defaultValueTick;
        private FrameworkElement _centerOfRangeValueTick;
        private FrameworkElement _thumbRange;
        private Border _startLength;
        private Border _endLength;
        private Border _middleLength;
        private Border _startValue;
        private Border _endValue;
        private TextBoxControl _textboxStartValue;
        private ButtonControl _downButton;
        private ButtonControl _upButton;

        private Border _borderStartLength;
        private Border _borderEndLength;
        private Border _borderMiddleLength;
        private Border _border_InsideMiddleLength;


        #endregion
        #region Dependency Property
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(SliderControl),
            new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(SliderControl),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty MediumValueProperty = DependencyProperty.Register("MediumValue", typeof(double), typeof(SliderControl),
            new FrameworkPropertyMetadata(50d, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty StartProperty = DependencyProperty.Register("Start", typeof(int), typeof(SliderControl),
            new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty EndProperty = DependencyProperty.Register("End", typeof(int), typeof(SliderControl),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SliderControl),
            new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty IsMoveToPointEnabledProperty = DependencyProperty.Register("IsMoveToPointEnabled", typeof(bool), typeof(SliderControl), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty StartThumbToolTipProperty = DependencyProperty.Register("StartThumbToolTip", typeof(object), typeof(SliderControl));
        public static readonly DependencyProperty EndThumbToolTipProperty = DependencyProperty.Register("EndThumbToolTip", typeof(object), typeof(SliderControl));
        public static readonly DependencyProperty StartThumbStyleProperty = DependencyProperty.Register("StartThumbStyle", typeof(Style), typeof(SliderControl));
        public static readonly DependencyProperty EndThumbStyleProperty = DependencyProperty.Register("EndThumbStyle", typeof(Style), typeof(SliderControl));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(SliderControl));
        public static readonly DependencyProperty NumberSliderProperty = DependencyProperty.Register("NumberSlider", typeof(int), typeof(SliderControl), new PropertyMetadata(2));
        public static readonly DependencyProperty HighlightBackgroundProperty = DependencyProperty.Register("HighlightBackground", typeof(Brush), typeof(SliderControl), null);
        public static readonly DependencyProperty UnHighlightBackgroundProperty = DependencyProperty.Register("UnHighlightBackground", typeof(Brush), typeof(SliderControl), null);
        public static readonly DependencyProperty SliderProcessColorProperty = DependencyProperty.Register("SliderProcessColor", typeof(Color), typeof(SliderControl), null);
        public static readonly DependencyProperty IsReverseProperty = DependencyProperty.Register("IsReverse", typeof(bool), typeof(SliderControl), new PropertyMetadata((bool)false, OnReverseChange));

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(SliderControl),
            new FrameworkPropertyMetadata(5d));
        public static readonly DependencyProperty ThumbSizeProperty = DependencyProperty.Register("ThumbSize", typeof(double), typeof(SliderControl),
            new PropertyMetadata(18d));
        public static readonly DependencyProperty InputBackgroundProperty = DependencyProperty.Register("InputBackground", typeof(Brush), typeof(SliderControl), null);
        public static readonly DependencyProperty InputBorderBrushProperty = DependencyProperty.Register("InputBorderBrush", typeof(Brush), typeof(SliderControl), null);
        public static readonly DependencyProperty ShowInputValueProperty = DependencyProperty.Register("ShowInputValue", typeof(bool), typeof(SliderControl),
            new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty ThicknessThumbProperty = DependencyProperty.Register("ThicknessThumb", typeof(Thickness), typeof(SliderControl),
            new FrameworkPropertyMetadata(new Thickness(1, 1, 1, 1)));
        public static readonly DependencyProperty BorderControlSliderVisibleProperty = DependencyProperty.Register("BorderControlSliderVisible", typeof(bool), typeof(SliderControl), new FrameworkPropertyMetadata(false));


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

        }

        #endregion
        #region Constructor
        static SliderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderControl), new FrameworkPropertyMetadata(typeof(SliderControl)));
            EventManager.RegisterClassHandler(typeof(SliderControl), Thumb.DragStartedEvent, new DragStartedEventHandler(OnDragStartedEvent));
            EventManager.RegisterClassHandler(typeof(SliderControl), Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumbDragDelta));
            EventManager.RegisterClassHandler(typeof(SliderControl), Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnDragCompletedEvent));
        }
        #endregion
        #region Properties

        public int NumberSlider
        {
            get
            {
                return (int)GetValue(NumberSliderProperty);
            }
            set
            {
                SetValue(NumberSliderProperty, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        public double Maximum
        {
            get
            {
                return (double)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        public double Minimum
        {
            get
            {
                return (double)GetValue(MinimumProperty);
            }
            set
            {
                SetValue(MinimumProperty, value);
            }
        }

        public double MediumValue
        {
            get
            {
                return (double)GetValue(MediumValueProperty);
            }
            set
            {
                SetValue(MediumValueProperty, value);
            }
        }

        public int Start
        {
            get
            {
                return (int)GetValue(StartProperty);
            }
            set
            {
                SetValue(StartProperty, value);
            }
        }

        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(double), typeof(SliderControl), new FrameworkPropertyMetadata(50d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double DefaultValue
        {
            get
            {
                return (double)GetValue(DefaultValueProperty);
            }
            set
            {
                SetValue(DefaultValueProperty, value);
            }
        }

        public static readonly DependencyProperty ThumbRangeProperty = DependencyProperty.Register("ThumbRange", typeof(double?), typeof(SliderControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double? ThumbRange
        {
            get
            {
                return (double?)GetValue(ThumbRangeProperty);
            }
            set
            {
                SetValue(ThumbRangeProperty, value);
            }
        }

        public static readonly DependencyProperty ThumbRangeSizeProperty = DependencyProperty.Register("ThumbRangeSize", typeof(double), typeof(SliderControl), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double ThumbRangeSize
        {
            get
            {
                return (double)GetValue(ThumbRangeSizeProperty);
            }
            set
            {
                SetValue(ThumbRangeSizeProperty, value);
            }
        }

        public static readonly DependencyProperty MaxValueByDefProperty = DependencyProperty.Register("MaxValueByDef", typeof(double?), typeof(SliderControl), new PropertyMetadata(null));
        public double? MaxValueByDef
        {
            get
            {
                return (double?)GetValue(MaxValueByDefProperty);
            }
            set
            {
                SetValue(MaxValueByDefProperty, value);
            }
        }

        public static readonly DependencyProperty MinValueByDefProperty = DependencyProperty.Register("MinValueByDef", typeof(double?), typeof(SliderControl), new PropertyMetadata(null));
        public double? MinValueByDef
        {
            get
            {
                return (double?)GetValue(MinValueByDefProperty);
            }
            set
            {
                SetValue(MinValueByDefProperty, value);
            }
        }

        public static readonly DependencyProperty CurrentValueByDefProperty = DependencyProperty.Register("CurrentValueByDef", typeof(double?), typeof(SliderControl), new PropertyMetadata(null));
        public double? CurrentValueByDef
        {
            get
            {
                return (double?)GetValue(CurrentValueByDefProperty);
            }
            set
            {
                SetValue(CurrentValueByDefProperty, value);
            }
        }

        public static readonly DependencyProperty CenterOfRangeValueProperty = DependencyProperty.Register("CenterOfRangeValue", typeof(double), typeof(SliderControl), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double CenterOfRangeValue
        {
            get
            {
                return (double)GetValue(CenterOfRangeValueProperty);
            }
            set
            {
                SetValue(CenterOfRangeValueProperty, value);
            }
        }

        public int End
        {
            get
            {
                return (int)GetValue(EndProperty);
            }
            set
            {
                SetValue(EndProperty, value);
            }
        }

        /// <summary>
        /// DragCompletedEvent 에서 적용되는 값
        /// </summary>
        public int StartCompletedValue
        {
            get
            {
                return (int)GetValue(StartCompletedValueProperty);
            }
            set
            {
                SetValue(StartCompletedValueProperty, value);
            }
        }
        public static readonly DependencyProperty StartCompletedValueProperty = DependencyProperty.Register("StartCompletedValue", typeof(int), typeof(SliderControl),
            new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// DragCompletedEvent 에서 적용되는 값
        /// </summary>
        public int EndCompletedValue
        {
            get
            {
                return (int)GetValue(EndCompletedValueProperty);
            }
            set
            {
                SetValue(EndCompletedValueProperty, value);
            }
        }
        public static readonly DependencyProperty EndCompletedValueProperty = DependencyProperty.Register("EndCompletedValue", typeof(int), typeof(SliderControl),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsMoveToPointEnabled
        {
            get
            {
                return (bool)GetValue(IsMoveToPointEnabledProperty);
            }
            set
            {
                SetValue(IsMoveToPointEnabledProperty, value);
            }
        }

        public object StartThumbToolTip
        {
            get
            {
                return GetValue(StartThumbToolTipProperty);
            }
            set
            {
                SetValue(StartThumbToolTipProperty, value);
            }
        }

        public object EndThumbToolTip
        {
            get
            {
                return GetValue(EndThumbToolTipProperty);
            }
            set
            {
                SetValue(EndThumbToolTipProperty, value);
            }
        }

        public Style StartThumbStyle
        {
            get
            {
                return (Style)GetValue(StartThumbStyleProperty);
            }
            set
            {
                SetValue(StartThumbStyleProperty, value);

            }
        }

        public Style EndThumbStyle
        {
            get
            {
                return (Style)GetValue(EndThumbStyleProperty);
            }
            set
            {
                SetValue(EndThumbStyleProperty, value);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }
            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        public Brush HighlightBackground
        {
            get
            {
                return (Brush)GetValue(HighlightBackgroundProperty);
            }
            set
            {
                SetValue(HighlightBackgroundProperty, value);
            }
        }

        public Brush UnHighlightBackground
        {
            get
            {
                return (Brush)GetValue(UnHighlightBackgroundProperty);
            }
            set
            {
                SetValue(UnHighlightBackgroundProperty, value);
            }
        }

        public Color SliderProcessColor
        {
            get
            {
                return (Color)GetValue(SliderProcessColorProperty);
            }
            set
            {
                SetValue(SliderProcessColorProperty, value);
            }
        }

        public bool IsReverse
        {
            get
            {
                return (bool)GetValue(IsReverseProperty);
            }
            set
            {
                SetValue(IsReverseProperty, value);
            }
        }

        public Brush InputBackground
        {
            get
            {
                return (Brush)GetValue(InputBackgroundProperty);
            }
            set
            {
                SetValue(InputBackgroundProperty, value);
            }
        }
        public Brush InputBorderBrush
        {
            get
            {
                return (Brush)GetValue(InputBorderBrushProperty);
            }
            set
            {
                SetValue(InputBorderBrushProperty, value);
            }
        }

        public double Thickness
        {
            get
            {
                return (double)GetValue(ThicknessProperty);
            }
            set
            {
                SetValue(ThicknessProperty, value);
            }
        }

        public double ThumbSize
        {
            get
            {
                return (double)GetValue(ThumbSizeProperty);
            }
            set
            {
                SetValue(ThumbSizeProperty, value);
            }
        }

        public bool ShowInputValue
        {
            get
            {
                return (bool)GetValue(ShowInputValueProperty);
            }
            set
            {
                SetValue(ShowInputValueProperty, value);
            }
        }

        public bool BorderControlSliderVisible
        {
            get
            {
                return (bool)GetValue(BorderControlSliderVisibleProperty);
            }
            set
            {
                SetValue(BorderControlSliderVisibleProperty, value);
            }
        }

        public Thickness ThicknessThumb
        {
            get
            {
                return (Thickness)GetValue(ThicknessProperty);
            }
            set
            {
                SetValue(ThicknessProperty, value);
            }
        }
        #endregion

        #region Methods
        public override void OnApplyTemplate()
        {
            _sliderContainer = GetTemplateChild("PART_SliderContainer") as FrameworkElement;
            _borderContainer = GetTemplateChild("Border_SliderContainer") as FrameworkElement;

            if (_sliderContainer != null)
            {
                _sliderContainer.PreviewMouseDown += ViewBox_PreviewMouseDown;
            }
            if (NumberSlider == (int)TypeSlider.TwoSlider)
            {
                _endThumb = GetTemplateChild("PART_EndThumb") as Thumb;
                _endLength = GetTemplateChild("PART_EndLength") as Border;
                _middleLength = GetTemplateChild("PART_MiddleLength") as Border;
                _endValue = GetTemplateChild("PART_EndValueArea") as Border;

                _borderEndLength = GetTemplateChild("Border_EndLength") as Border;
                _borderMiddleLength = GetTemplateChild("Border_MiddleLength") as Border;
                _border_InsideMiddleLength = GetTemplateChild("Border_InsideMiddleLength") as Border;
            }
            else if (NumberSlider == (int)TypeSlider.OneSlider)
            {
                End = (int)Maximum;
            }
            _startArea = GetTemplateChild("PART_StartArea") as FrameworkElement;
            _endArea = GetTemplateChild("PART_EndArea") as FrameworkElement;
            _defaultValueTick = GetTemplateChild("PART_DefaultValueTick") as FrameworkElement;
            _centerOfRangeValueTick = GetTemplateChild("PART_CenterOfRangeValueTick") as FrameworkElement;
            _thumbRange = GetTemplateChild("OutBox") as FrameworkElement;
            _startThumb = GetTemplateChild("PART_StartThumb") as Thumb;
            _startThumbValue = GetTemplateChild("PART_StartThumbValue") as TextBlock;
            _endThumbValue = GetTemplateChild("PART_EndThumbValue") as TextBlock;
            _startLength = GetTemplateChild("PART_StartLength") as Border;
            _startValue = GetTemplateChild("PART_StartValueArea") as Border;
            _startThumbReverse = GetTemplateChild("PART_StartThumbReverse") as Thumb;
            _endThumbReverse = GetTemplateChild("PART_EndThumbReverse") as Thumb;


            _borderStartLength = GetTemplateChild("Border_StartLength") as Border;

            MediumValue = (Maximum + Minimum) * 0.5;
            /* if (!IsEnabled)
             {
                 if (NumberSlider == (int)TypeSlider.TwoSlider)
                 {
                     Start = (int)Minimum;
                     End = (int)Maximum;
                 }
                 else if (NumberSlider == (int)TypeSlider.OneSlider)
                 {
                     Start = (int)MediumValue;
                 }
             }*/
            _textboxStartValue = GetTemplateChild("PART_MinValue") as TextBoxControl;
            if (_textboxStartValue != null)
            {
                _textboxStartValue.TextChanged += new TextChangedEventHandler(UpdateText);
            }
            _downButton = GetTemplateChild("DownButton") as ButtonControl;
            if (_downButton != null)
            {
                _downButton.Click += buttonDownClick;
            }
            _upButton = GetTemplateChild("UpButton") as ButtonControl;
            if (_upButton != null)
            {
                _upButton.Click += buttonUpClick;
            }
            base.OnApplyTemplate();
        }

        public void UpdateText(object sender, TextChangedEventArgs e)
        {
            if (_textboxStartValue != null)
            {

            }
        }

        private void buttonUpClick(object sender, RoutedEventArgs e)
        {
            if (Start < Maximum)
            {
                Start += 1;
            }
        }

        private void buttonDownClick(object sender, RoutedEventArgs e)
        {
            if (Start > Minimum)
            {
                Start -= 1;
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var arrangeSize = base.ArrangeOverride(arrangeBounds);

            if (Maximum > Minimum)
            {
                var viewportSize = 0.0;
                var viewportBorderSize = 0.0;
                if (Orientation == Orientation.Horizontal)
                {
                    viewportSize = _sliderContainer != null ? _sliderContainer.ActualWidth : arrangeBounds.Width;
                    viewportBorderSize = _borderContainer != null ? _borderContainer.ActualWidth : arrangeBounds.Width;
                }

                else
                {
                    viewportSize = _sliderContainer != null ? _sliderContainer.ActualHeight : arrangeBounds.Height;
                    viewportBorderSize = _borderContainer != null ? _borderContainer.ActualHeight : arrangeBounds.Height;
                }

                if (NumberSlider == (int)TypeSlider.TwoSlider)
                {
                    if (End >= Start && Start >= Minimum && Start <= Maximum &&
                        End <= Maximum && End >= Minimum)
                    {
                        var start = Math.Max(Minimum, Math.Min(Maximum, Start));
                        var end = Math.Max(Minimum, Math.Min(Maximum, End));

                        var startPosition = 0.0;
                        var endPosition = 0.0;
                        var startBorderPosition = 0.0;
                        var endBorderPosition = 0.0;
                        if (Orientation == Orientation.Horizontal)
                        {
                            startPosition = (start - Minimum) / (Maximum - Minimum) * viewportSize;
                            endPosition = (end - Minimum) / (Maximum - Minimum) * viewportSize;
                            startBorderPosition = (start - Minimum) / (Maximum - Minimum) * viewportBorderSize;
                            endBorderPosition = (end - Minimum) / (Maximum - Minimum) * viewportBorderSize;
                            if (startPosition >= 0.0 && _startLength != null)
                            {
                                _startLength.Width = startPosition;
                            }
                            if (startBorderPosition >= 0.0 && _borderStartLength != null)
                            {
                                _borderStartLength.Width = startBorderPosition;
                            }

                            if (endPosition >= startPosition && _middleLength != null)
                            {
                                _middleLength.Width = endPosition - startPosition;
                            }

                            if (endBorderPosition <= viewportBorderSize && _borderEndLength != null)
                            {
                                _borderEndLength.Width = viewportBorderSize - endBorderPosition;
                            }

                            if (endBorderPosition >= startBorderPosition && _borderMiddleLength != null)
                            {
                                _borderMiddleLength.Width = endBorderPosition - startBorderPosition;
                                if ((_borderMiddleLength.Width <= 2) && (Maximum > 250))
                                {
                                    if (_borderMiddleLength.Width == 0)
                                    {
                                        _borderMiddleLength.Width = _borderMiddleLength.Width + 3;
                                    }
                                    else
                                    {
                                        _borderMiddleLength.Width = _borderMiddleLength.Width + 2;
                                    }

                                    // [NCS-2695] CID 171174 Dereference after null check
                                    //if (_borderStartLength.Width >= 1)
                                    if (_borderStartLength != null && _borderStartLength.Width >= 1)
                                    {
                                        _borderStartLength.Width = _borderStartLength.Width - 1;
                                    }

                                    // [NCS-2695] CID 171162 Dereference after null check
                                    //if (_borderEndLength.Width >= 1)
                                    //{
                                    //    _borderEndLength.Width = _borderEndLength.Width - 1;
                                    //}
                                    if (_borderEndLength != null && _borderEndLength.Width >= 1)
                                    {
                                        _borderEndLength.Width = _borderEndLength.Width - 1;
                                    }
                                }
                                else if ((_borderMiddleLength.Width <= 1) && (Maximum <= 120))
                                {
                                    _borderMiddleLength.Width = 2;

                                    // [NCS-2695] CID 171174 Dereference after null check
                                    //if (_borderStartLength.Width >= 1)
                                    //{
                                    //    _borderStartLength.Width = _borderStartLength.Width - 1;
                                    //}
                                    if (_borderStartLength != null && _borderStartLength.Width >= 1)
                                    {
                                        _borderStartLength.Width = _borderStartLength.Width - 1;
                                    }

                                    // [NCS-2695] CID 171162 Dereference after null check
                                    //if (_borderEndLength.Width >= 1)
                                    //{
                                    //    _borderEndLength.Width = _borderEndLength.Width - 1;
                                    //}
                                    if (_borderEndLength != null && _borderEndLength.Width >= 1)
                                    {
                                        _borderEndLength.Width = _borderEndLength.Width - 1;
                                    }
                                }
                            }

                            if (endPosition <= viewportSize && _endLength != null)
                            {
                                _endLength.Width = viewportSize - endPosition;
                            }
                        }
                        else
                        {
                            startPosition = (start - Minimum) / (Maximum - Minimum) * viewportSize;
                            endPosition = (end - Minimum) / (Maximum - Minimum) * viewportSize;
                        }

                        // [NCS-2695] CID 171134 Dereference after null check
                        //if (_startThumb != null)
                        //    _startThumb.Margin = new Thickness(_startLength.Width, 0, 0, 0);
                        if (_startThumb != null)
                        {
                            _startThumb.Margin = new Thickness(_startLength != null ? _startLength.Width : 0, 0, 0, 0);
                        }

                        // [NCS-2695] CID 171176 Dereference after null check
                        //if (_endThumb != null)
                        //    _endThumb.Margin = new Thickness(0, 0, _endLength.Width, 0);
                        if (_endThumb != null)
                        {
                            _endThumb.Margin = new Thickness(0, 0, _endLength != null ? _endLength.Width : 0, 0);
                        }

                        if (_startValue != null && _startLength != null)
                            _startValue.Margin = new Thickness(_startLength.Width, 0, 0, 0);
                        if (_endValue != null && _endLength != null)
                            _endValue.Margin = new Thickness(0, 0, _endLength.Width, 0);

                        // [NCS-2695] CID 171134 Dereference after null check
                        //double marginStart = _startLength.Width - 10;
                        //double marginStartRev = _startLength.Width - 4;
                        double marginStart = _startLength != null ? (_startLength.Width - 10) : 0;
                        double marginStartRev = _startLength != null ? (_startLength.Width - 4) : 0;

                        // [NCS-2695] CID 171176 Dereference after null check
                        //double marginEnd = _endLength.Width - 10;
                        //double marginEndRev = _endLength.Width - 4;
                        double marginEnd = _endLength != null ? (_endLength.Width - 10) : 0;
                        double marginEndRev = _endLength != null ? (_endLength.Width - 4) : 0;

                        double subValue = Math.Abs(start - end);
                        if (_startThumbValue != null)
                        {
                            _startThumbValue.Text = start.ToString();

                            if (subValue < 20)
                            {
                                marginStart = marginStart - 8;
                                marginStartRev = marginStartRev - 8;
                            }
                            if (!IsReverse)
                                _startThumbValue.Margin = new Thickness(marginStart, 0, 0, 0);
                            else
                                _startThumbValue.Margin = new Thickness(marginStartRev, 0, 0, 0);
                        }

                        if (_endThumbValue != null)
                        {
                            _endThumbValue.Text = end.ToString();
                            if (subValue < 20)
                            {
                                //if (subValue == 1)
                                //    _middleLength.Visibility = Visibility.Hidden;
                                //else
                                //{
                                //    _middleLength.Visibility = Visibility.Visible;
                                marginEnd = marginEnd - 8;
                                marginEndRev = marginEndRev - 8;
                                //}
                            }
                            //double temMid = start + (end - start) / 2;
                            //_middleLength.Margin = new Thickness(temMid, 0, 0, 0);
                            if (start == end)
                            {
                                _endThumbValue.Visibility = Visibility.Hidden;

                                // [NCS-2695] CID 171188 Dereference after null check
                                //if (!IsReverse)
                                //    _startThumbValue.Margin = new Thickness(marginStart + 11, 0, 0, 0);
                                //else
                                //    _startThumbValue.Margin = new Thickness(marginStartRev + 11, 0, 0, 0);
                                if (_startThumbValue != null)
                                {
                                    if (!IsReverse)
                                    {
                                        _startThumbValue.Margin = new Thickness(marginStart + 11, 0, 0, 0);
                                    }
                                    else
                                    {
                                        _startThumbValue.Margin = new Thickness(marginStartRev + 11, 0, 0, 0);
                                    }
                                }
                            }
                            else
                            {
                                _endThumbValue.Visibility = Visibility.Visible;
                            }

                            if (!IsReverse)
                                _endThumbValue.Margin = new Thickness(0, 0, marginEnd, 0);
                            else
                                _endThumbValue.Margin = new Thickness(0, 0, marginEndRev, 0);
                        }

                        if (IsReverse)
                        {
                            if (_startThumbReverse != null)
                                _startThumbReverse.Margin = new Thickness(_startLength.Width, 0, 0, 0);
                            if (_endThumbReverse != null)
                                _endThumbReverse.Margin = new Thickness(0, 0, _endLength.Width, 0);
                        }

                        IsReverseChange();


                    }
                }
                else if (NumberSlider == (int)TypeSlider.OneSlider)
                {
                    var startPosition = ((Start - Minimum) / (Maximum - Minimum)) * viewportSize;
                    if (Orientation == Orientation.Horizontal)
                    {
                        if (Start >= Minimum && Start <= Maximum)
                        {
                            if (_startArea != null && startPosition >= 0)
                                _startArea.Width = startPosition;
                            if (_endArea != null && (viewportSize - startPosition) >= 0)
                                _endArea.Width = viewportSize - startPosition;
                            if (_startThumb != null)
                                _startThumb.Margin = new Thickness(startPosition, 0, 0, 0);
                            if (_startValue != null)
                            {
                                double actualWidth = _startValue.ActualWidth;
                                _startValue.Margin = new Thickness(startPosition - actualWidth * 0.5, 0, 0, 0);
                            }
                        }
                    }
                    else
                    {
                        if (Start >= Minimum && Start <= Maximum)
                        {
                            if (_startArea != null && startPosition >= 0)
                                _startArea.Height = startPosition;
                            if (_endArea != null && (viewportSize - startPosition) >= 0)
                                _endArea.Height = viewportSize - startPosition;
                            if (_startThumb != null)
                                _startThumb.Margin = new Thickness(0, 0, 0, startPosition);
                            if (_startValue != null)
                            {
                                double actualHeight = _startValue.ActualHeight;
                                _startValue.Margin = new Thickness(0, 0, 0, startPosition - actualHeight * 0.5);
                            }
                        }

                        if (_defaultValueTick != null && DefaultValue >= Minimum && DefaultValue <= Maximum)
                        {
                            var defaultValueTickPosition = ((DefaultValue - Minimum) / (Maximum - Minimum)) * viewportSize;
                            _defaultValueTick.Margin = new Thickness(0, 0, 0, defaultValueTickPosition);
                        }

                        if (ThumbRange != null)
                        {
                            ThumbRangeSize = (double)ThumbRange / (Maximum - Minimum) * viewportSize;
                        }

                        if (_centerOfRangeValueTick != null && CenterOfRangeValue >= Minimum && CenterOfRangeValue <= Maximum)
                        {
                            var centerOfRangeValueTickPosition = ((CenterOfRangeValue - Minimum) / (Maximum - Minimum)) * viewportSize;
                            _centerOfRangeValueTick.Margin = new Thickness(0, 0, 0, centerOfRangeValueTickPosition);
                        }
                    }
                }
            }

            return arrangeSize;
        }

        private void ViewBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsReadOnly && IsMoveToPointEnabled)
            {
                if (NumberSlider == (int)TypeSlider.TwoSlider)
                {
                    if ((_startThumb != null && _startThumb.IsMouseOver) || (_endThumb != null && _endThumb.IsMouseOver))
                        return;
                }
                else if (NumberSlider == (int)TypeSlider.OneSlider)
                {
                    if (_startThumb != null && _startThumb.IsMouseOver)
                        return;
                }
                var point = e.GetPosition(_sliderContainer);
                if (e.ChangedButton == MouseButton.Left)
                {
                    MoveBlockTo(point, SliderThumb.Start);
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    MoveBlockTo(point, SliderThumb.End);
                }
                e.Handled = true;
            }
        }

        private void MoveBlockTo(Point point, SliderThumb block)
        {
            double position;
            if (Orientation == Orientation.Horizontal)
            {
                position = point.X;
            }
            else
            {
                position = point.Y;
            }

            double viewportSize = (Orientation == Orientation.Horizontal) ? _sliderContainer.ActualWidth : _sliderContainer.ActualHeight;
            if (!double.IsNaN(viewportSize) && viewportSize > 0)
            {
                var value = 0.0;
                if (Orientation == Orientation.Horizontal)
                    value = Math.Min(Maximum, Minimum + (position / viewportSize) * (Maximum - Minimum));
                else
                    value = Math.Min(Maximum, Maximum - (position / viewportSize) * (Maximum - Minimum));
                if (NumberSlider == (int)TypeSlider.TwoSlider)
                {
                    if (block == SliderThumb.Start)
                    {
                        Start = (int)Math.Min(End, value);
                    }
                    else if (block == SliderThumb.End)
                    {
                        End = (int)Math.Max(Start, value);
                    }
                }
                else if (NumberSlider == (int)TypeSlider.OneSlider)
                {
                    Start = (int)value;
                    End = (int)Maximum;
                }
            }
        }

        private static void OnDragStartedEvent(object sender, DragStartedEventArgs e)
        {
            if (sender is SliderControl rs)
            {
                rs.OnDragStartedEvent(e);
            }
        }

        private void OnDragStartedEvent(DragStartedEventArgs e)
        {
        }

        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is SliderControl rs)
            {
                rs.OnThumbDragDelta(e);
            }
        }

        private void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            if (!IsReadOnly && e.OriginalSource is Thumb thumb && _sliderContainer != null)
            {
                double change;
                if (Orientation == Orientation.Horizontal)
                {
                    change = e.HorizontalChange / _sliderContainer.ActualWidth * (Maximum - Minimum);
                }
                else
                {
                    change = (-1) * e.VerticalChange / _sliderContainer.ActualHeight * (Maximum - Minimum);
                }
                if (IsReverse)
                {
                    if (thumb == _startThumbReverse)
                    {
                        Start = (int)Math.Max(Minimum, Math.Min(End, Start + change));
                    }
                    else if (thumb == _endThumbReverse)
                    {
                        End = (int)Math.Min(Maximum, Math.Max(Start, End + change));
                    }
                }
                else
                {
                    if (thumb == _startThumb)
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            Start = (int)Math.Max(Minimum, Math.Min(End, Start + change));
                        }
                        else
                        {
                            Start = (int)Math.Max(Minimum, Start + change);
                        }
                    }
                    else if (thumb == _endThumb)
                    {
                        End = (int)Math.Min(Maximum, Math.Max(Start, End + change));
                    }
                }

            }
        }

        private static void OnDragCompletedEvent(object sender, DragCompletedEventArgs e)
        {
            if (sender is SliderControl rs)
            {
                rs.StartCompletedValue = rs.Start;
                rs.EndCompletedValue = rs.End;

                rs.OnDragCompletedEvent(e);
            }
        }

        private void OnDragCompletedEvent(DragCompletedEventArgs e)
        {

        }
        #endregion

        private static void OnReverseChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d is SliderControl slidercontrol)
                {
                    slidercontrol.IsReverseChange();
                }
            }
            catch (Exception ex)
            {
                // LogHelper.Fatal(ex);
            }
        }

        public void IsReverseChange()
        {
            if (IsReverse)
            {
                if (_borderStartLength != null)
                {
                    Panel.SetZIndex(_borderStartLength, 2);
                    _borderStartLength.BorderThickness = new Thickness(1, 1, 1, 1);
                }
                if ((_borderMiddleLength != null) && (_border_InsideMiddleLength != null))
                {
                    _borderMiddleLength.BorderThickness = new Thickness(0, 0, 0, 0);
                    _border_InsideMiddleLength.BorderBrush = Brushes.Transparent;
                    Panel.SetZIndex(_border_InsideMiddleLength, 1);
                    Panel.SetZIndex(_borderMiddleLength, 1);
                }
                if (_borderEndLength != null)
                {
                    Panel.SetZIndex(_borderEndLength, 2);
                    _borderEndLength.BorderThickness = new Thickness(1, 1, 1, 1);
                }
            }
            else
            {
                if (_borderStartLength != null)
                {
                    Panel.SetZIndex(_borderStartLength, 1);
                    _borderStartLength.BorderThickness = new Thickness(0, 0, 0, 0);
                }
                if ((_borderMiddleLength != null) && (_border_InsideMiddleLength != null))
                {
                    _borderMiddleLength.BorderThickness = new Thickness(1, 1, 1, 1);
                    _border_InsideMiddleLength.BorderBrush = Brushes.Aqua;
                    Panel.SetZIndex(_borderMiddleLength, 2);
                    Panel.SetZIndex(_border_InsideMiddleLength, 3);
                }
                if (_borderEndLength != null)
                {
                    Panel.SetZIndex(_borderEndLength, 1);
                    _borderEndLength.BorderThickness = new Thickness(0, 0, 0, 0);
                }
            }
        }

    }
}
