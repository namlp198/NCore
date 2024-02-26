using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NpcCore.Wpf.Ex
{
    public class CircleNumber : ContentControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CircleNumber()
        {
            this.DefaultStyleKey = typeof(CircleNumber);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CircleNumber), new PropertyMetadata(string.Empty));

        public bool IsCompleted
        {
            get { return (bool)GetValue(IsCompletedProperty); }
            set { SetValue(IsCompletedProperty, value); }
        }

        public static readonly DependencyProperty IsCompletedProperty = DependencyProperty.Register("IsCompleted", typeof(bool), typeof(CircleNumber), new PropertyMetadata(false));

        /// <summary>
        /// StrokeBrush
        /// </summary>
        public SolidColorBrush StrokeBrush
        {
            get { return (SolidColorBrush)GetValue(StrokeBrushProperty); }
            set { SetValue(StrokeBrushProperty, value); }
        }

        /// <summary>
        /// StrokeBrushProperty
        /// </summary>
        public static readonly DependencyProperty StrokeBrushProperty = DependencyProperty.Register("StrokeBrush", typeof(SolidColorBrush), typeof(CircleNumber), new PropertyMetadata(null));

        /// <summary>
        /// DashedStroke
        /// </summary>
        public DoubleCollection DashedStroke
        {
            get { return (DoubleCollection)GetValue(DashedStrokeProperty); }
            set { SetValue(DashedStrokeProperty, value); }
        }

        /// <summary>
        /// DashedStrokeProperty
        /// </summary>
        public static readonly DependencyProperty DashedStrokeProperty = DependencyProperty.Register("DashedStroke", typeof(DoubleCollection), typeof(CircleNumber), new PropertyMetadata(null));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(CircleNumber), new PropertyMetadata(new Thickness(0)));
    }
}
