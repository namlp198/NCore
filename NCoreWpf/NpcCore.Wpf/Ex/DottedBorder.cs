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
    /// <summary>
    /// DottedBorder Control
    /// </summary>
    public class DottedBorder : ContentControl
    {

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
        public static readonly DependencyProperty StrokeBrushProperty = DependencyProperty.Register("StrokeBrush", typeof(SolidColorBrush), typeof(DottedBorder), new PropertyMetadata(null));

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
        public static readonly DependencyProperty DashedStrokeProperty = DependencyProperty.Register("DashedStroke", typeof(DoubleCollection), typeof(DottedBorder), new PropertyMetadata(null));

        /// <summary>
        /// DashedStroke
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(DottedBorder), new PropertyMetadata(1.0));

        /// <summary>
        /// Constructor
        /// </summary>
        public DottedBorder()
        {
            this.DefaultStyleKey = typeof(DottedBorder);
        }
    }
}
