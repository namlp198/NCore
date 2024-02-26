using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Npc.Foundation.Helper;

namespace NpcCore.Wpf.Controls
{
    public class ColourSlider : Slider
    {
        #region Fields
        private Canvas _verticalLabel;
        private Border _colorColumn;
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
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(ColourSlider), new FrameworkPropertyMetadata("mm"));
        #endregion
        #region Constructor
        static ColourSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColourSlider), new FrameworkPropertyMetadata(typeof(ColourSlider)));
        }
        #endregion
        #region Events
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _verticalLabel = GetTemplateChild("PART_VerticalLabels") as Canvas;
            _colorColumn = GetTemplateChild("PART_ColorColumn") as Border;
        }
        public ColourSlider()
        {
            this.Background = new LinearGradientBrush(new GradientStopCollection() {
                new GradientStop(Colors.Red, 0.1),
                new GradientStop(Colors.Yellow, 0.4),
                new GradientStop(Colors.Lime, 0.6),
                new GradientStop(Colors.Aqua, 0.8),
                new GradientStop(Colors.Blue, 1.0),
            });
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

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            UpdateSlider();
        }

        public double RoundByTen(double value)
        {
            return (double)(Math.Ceiling(value / 10.0d) * 10);
        }

        public double RoundByFifty(double value)
        {
            return (double)(Math.Ceiling(value / 50.0d) * 50);
        }

        public double RoundByOneHundred(double value)
        {
            return (double)(Math.Ceiling(value / 100.0d) * 100);
        }

        public double RoundByFiveHundred(double value)
        {
            return (double)(Math.Ceiling(value / 500.0d) * 500);
        }

        public double RoundByThousand(double value)
        {
            return (double)(Math.Ceiling(value / 1000.0d) * 1000);
        }

        public double RoundByTenThousand(double value)
        {
            return (double)(Math.Ceiling(value / 10000.0d) * 10000);
        }

        public bool UpdateSlider()
        {
            bool bResult = false;
            if (_verticalLabel != null && _colorColumn != null)
            {
                _verticalLabel.Children.Clear();
                bool bStop = false;
                double changeValue = 0;
                double height = _colorColumn.ActualHeight;
                if (height > 0 && Maximum > Minimum)
                {
                    bResult = true;
                    double tickFrequency = (Maximum - Minimum) / 6;
                    if (tickFrequency > 0 && tickFrequency <= 1)
                        TickFrequency = RoundedValueByUnit(tickFrequency);
                    else if (tickFrequency > 1 && tickFrequency <= 100)
                        TickFrequency = RoundByTen(tickFrequency);
                    else if (tickFrequency > 100 && tickFrequency <= 1000)
                        TickFrequency = RoundByFifty(tickFrequency);
                    else if (tickFrequency > 1000 && tickFrequency <= 4000)
                        TickFrequency = RoundByOneHundred(tickFrequency);
                    else if (tickFrequency > 4000 && tickFrequency <= 10000)
                        TickFrequency = RoundByFiveHundred(tickFrequency);
                    else if (tickFrequency > 10000 && tickFrequency <= 100000)
                        TickFrequency = RoundByThousand(tickFrequency);
                    else if (tickFrequency > 1000000 && tickFrequency <= 1000000)
                        TickFrequency = RoundByTenThousand(tickFrequency);
                    else if (tickFrequency > 1000000 && tickFrequency <= 2000000)
                        TickFrequency = (double)(Math.Ceiling(tickFrequency / 100000d) * 100000);
                    else if (tickFrequency > 2000000 && tickFrequency <= 10000000)
                        TickFrequency = (double)(Math.Ceiling(tickFrequency / 1000000d) * 1000000);
                    else
                        TickFrequency = (double)(Math.Ceiling(tickFrequency / 10000000d) * 10000000);
                    bool hasZeroValue = false;
                    while (!bStop)
                    {
                        TextBlock verticalValue = new TextBlock();
                        if (verticalValue != null)
                        {
                            verticalValue.VerticalAlignment = VerticalAlignment.Top;
                            verticalValue.Style = GetStyleForLabel();
                            double value = this.Minimum + changeValue;
                            if (this.Maximum - value < TickFrequency * 0.5)
                            {
                                value = this.Maximum;
                                bStop = true;
                            }
                            if (value == 0.0)
                                hasZeroValue = true;
                            verticalValue.Text = UnitHelper.ConvertToStringByUnit(value, Unit);
                            double distanceToTop = height - (value - Minimum) * height / (Maximum - Minimum) + 6 - verticalValue.FontSize * 0.5;
                            Canvas.SetTop(verticalValue, distanceToTop);
                            _verticalLabel.Children.Add(verticalValue);
                            changeValue += TickFrequency;
                            if (value > this.Maximum)
                                bStop = true;
                        }
                    }
                    if (!hasZeroValue && this.Maximum > 0 && this.Minimum <= 0)
                    {
                        TextBlock zeroLabel = new TextBlock();
                        if (zeroLabel != null)
                        {
                            zeroLabel.VerticalAlignment = VerticalAlignment.Top;
                            zeroLabel.Style = GetStyleForLabel();
                            zeroLabel.Text = UnitHelper.ConvertToStringByUnit(0.0, Unit);
                            double distanceToTop = height - (0.0 - Minimum) * height / (Maximum - Minimum) + 6 - zeroLabel.FontSize * 0.5;
                            Canvas.SetTop(zeroLabel, distanceToTop);
                            _verticalLabel.Children.Add(zeroLabel);
                        }
                    }
                }
            }
            return bResult;
        }


        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            newValue = RoundedValueByUnit(newValue);
            Value = newValue;
        }
        #endregion

        #region Private Methods

        private Style GetStyleForLabel()
        {
            Style style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.ForegroundProperty, this.Foreground));
            style.Setters.Add(new Setter(TextBlock.FontSizeProperty, this.FontSize));
            style.Setters.Add(new Setter(TextBlock.FontWeightProperty, this.FontWeight));
            style.Setters.Add(new Setter(TextBlock.FontStyleProperty, this.FontStyle));
            style.Setters.Add(new Setter(TextBlock.FontFamilyProperty, this.FontFamily));
            return style;
        }
        #endregion
    }
}
