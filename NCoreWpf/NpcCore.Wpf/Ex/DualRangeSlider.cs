
using Npc.Foundation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NpcCore.Wpf.Ex
{
    [TemplatePart(Name = PART_Slider1, Type = typeof(Slider))]
    [TemplatePart(Name = PART_Slider2, Type = typeof(Slider))]
    public class DualRangeSlider : Control
    {
        
        private const string PART_Slider1 = "PART_Slider1";
        private const string PART_Slider2 = "PART_Slider2";

        Slider _slider1;
        Slider _slider2;

        public override void OnApplyTemplate()
        {
            _slider1 = GetTemplateChild(PART_Slider1) as Slider;
            _slider2 = GetTemplateChild(PART_Slider2) as Slider;

            UpdateSlider();

            //_slider1.UseLayoutRounding = true;
            //_slider2.UseLayoutRounding = true;

            _slider1.ValueChanged += _slider1_ValueChanged;
            _slider2.ValueChanged += _slider2_ValueChanged;

            this.AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(RangeSlider_DragStarted));
            this.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(RangeSlider_DragCompleted));
            
            base.OnApplyTemplate();
        }

        public static readonly RoutedEvent ApplyValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ApplyValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<DoubleRangeValue>), typeof(DualRangeSlider));

        public event RoutedPropertyChangedEventHandler<DoubleRangeValue> ApplyValueChanged
        {
            add { AddHandler(ApplyValueChangedEvent, value); }
            remove { RemoveHandler(ApplyValueChangedEvent, value); }
        }

        Point _beforeValue;

        private void RangeSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            _beforeValue = new Point(_slider1.Value, _slider2.Value);
        }

        private void RangeSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Point afterValue = new Point(_slider1.Value, _slider2.Value);

            if (_beforeValue != afterValue)
            {
                var applyValuechangedEventArgs = new RoutedPropertyChangedEventArgs<DoubleRangeValue>(
                    new DoubleRangeValue(_beforeValue.X, _beforeValue.Y),
                    new DoubleRangeValue(afterValue.X, afterValue.Y),
                    DualRangeSlider.ApplyValueChangedEvent);
                RaiseEvent(applyValuechangedEventArgs);
            }
        }

        private void _slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_slider1.Value > _slider2.Value)
            {
                _slider1.Value = _slider2.Value;
            }
            else
            {
                this.Value1 = _slider1.Value;
            }
        }

        private void _slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_slider2.Value < _slider1.Value)
            {
                _slider2.Value = _slider1.Value;
            }
            else
            {
                this.Value2 = _slider2.Value;
            }
        }

        public double? Value1
        {
            get { return (double?)GetValue(Value1Property); }
            set { SetValue(Value1Property, value); }
        }
        public static readonly DependencyProperty Value1Property =
            DependencyProperty.Register("Value1", typeof(double?), typeof(DualRangeSlider), new PropertyMetadata(new PropertyChangedCallback(OnValue1Changed)));

        private static void OnValue1Changed(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            DualRangeSlider view = dobj as DualRangeSlider;
            if (view != null && e.NewValue is double?)
            {
                if (view._slider1 != null)
                {
                    double? value = (double?)e.NewValue;
                    view._slider1.Value = value == null ? view.Minimum : (double)value;
                }
            }
        }

        public double? Value2
        {
            get { return (double?)GetValue(Value2Property); }
            set { SetValue(Value2Property, value); }
        }
        public static readonly DependencyProperty Value2Property =
            DependencyProperty.Register("Value2", typeof(double?), typeof(DualRangeSlider), new PropertyMetadata(new PropertyChangedCallback(OnValue2Changed)));

        private static void OnValue2Changed(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            DualRangeSlider view = dobj as DualRangeSlider;
            if (view != null && e.NewValue is double?)
            {
                if (view._slider2 != null)
                {
                    double? value = (double?)e.NewValue;
                    view._slider2.Value = value == null ? view.Maximum : (double)value;
                }
            }
        }

        private void UpdateSlider()
        {
            var a = Value1 == null ? Minimum : (double)Value1;
            var b = Value2 == null ? Maximum : (double)Value2;

            if (Value1 != null)
            {
                this.SetValue(DualRangeSlider.Value1Property, a);
            }

            if (Value2 != null)
            {
                this.SetValue(DualRangeSlider.Value2Property, b);
            }

            _slider1.Value = (double)a;
            _slider2.Value = (double)b;
        }



        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(DualRangeSlider), new PropertyMetadata(100d));


        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(DualRangeSlider), new PropertyMetadata(0d));

        public DualRangeSlider()
        {

        }
    }
}
