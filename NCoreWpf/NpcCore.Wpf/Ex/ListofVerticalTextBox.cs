using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace NpcCore.Wpf.Ex
{
    public class ListofVerticalTextBox : Control
    {
        Grid ListText;

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(ListofVerticalTextBox), new PropertyMetadata((int)0, OnDatachanged));
        public static readonly DependencyProperty maxYLabelProperty =
          DependencyProperty.Register("maxYLabel", typeof(int), typeof(ListofVerticalTextBox), new PropertyMetadata((int)0, OnDatachanged));
        public static readonly DependencyProperty dividedIntervalProperty =
          DependencyProperty.Register("dividedInterval", typeof(int), typeof(ListofVerticalTextBox), new PropertyMetadata((int)0, OnDatachanged));
        public override void OnApplyTemplate()
        {

            ListText = GetTemplateChild("ListText") as Grid;
            OnMaxValueChange();
            base.OnApplyTemplate();
        }

        public int MaxValue
        {
            set
            {
                this.SetValue(MaxValueProperty, value);
            }
            get
            {
                return (int)this.GetValue(MaxValueProperty);
            }
        }

        public int maxYLabel
        {
            get
            {
                return (int)this.GetValue(maxYLabelProperty);
            }
            set
            {
                this.SetValue(maxYLabelProperty, value);
            }
        }

        public int dividedInterval
        {
            get
            {
                return (int)this.GetValue(dividedIntervalProperty);
            }
            set
            {
                this.SetValue(dividedIntervalProperty, value);
            }
        }


        public static void OnDatachanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ListofVerticalTextBox)d;
            if (obj != null)
            {
                obj.OnMaxValueChange();
            }

        }
        public void OnMaxValueChange()
        {

            if (ListText != null)
            {
                if ((MaxValue != 0) && (maxYLabel <= MaxValue) && dividedInterval >= 0)
                {
                    if (ListText != null)
                    {
                        ListText.RowDefinitions.Clear();
                        ListText.Children.Clear();
                        {
                            RowDefinition c1added = new RowDefinition();
                            double a = (double)(MaxValue - maxYLabel) / MaxValue;
                            c1added.Height = new GridLength(a, GridUnitType.Star);
                            ListText.RowDefinitions.Add(c1added);
                        }
                        for (int i = 1; i <= dividedInterval; i++)
                        {
                            RowDefinition c1 = new RowDefinition();
                            double a = (double)((double)maxYLabel / dividedInterval) / MaxValue;
                            c1.Height = new GridLength(a, GridUnitType.Star);
                            ListText.RowDefinitions.Add(c1);
                            TextBlock newText = new TextBlock();
                            double value = maxYLabel * ((double)(dividedInterval + 1 - i) / dividedInterval);
                            newText.Text = value.ToString();
                            newText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cccccc"));
                            newText.HorizontalAlignment = HorizontalAlignment.Right;
                            newText.VerticalAlignment = VerticalAlignment.Top;
                            newText.FontSize = 11;
                            newText.Margin = new Thickness(0, -6, 6, 0);
                            ListText.Children.Add(newText);
                            Grid.SetRow(newText, i);
                        }

                        {
                            TextBlock newText = new TextBlock();
                            newText.Text = "0";
                            newText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cccccc"));
                            newText.HorizontalAlignment = HorizontalAlignment.Right;
                            newText.VerticalAlignment = VerticalAlignment.Bottom;
                            newText.FontSize = 11;
                            newText.Margin = new Thickness(0, 6, 6, 0);
                            ListText.Children.Add(newText);
                            Grid.SetRow(newText, dividedInterval);
                        }

                    }
                }
            }
        }

    }
}
