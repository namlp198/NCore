using Npc.Foundation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace NpcCore.Wpf.Ex
{
    /// <summary>
    /// TextRange TextBlock Control
    /// </summary>
    public class TextRangeTextBlock : TextBlock
    {
        /// <summary>
        /// TextRange Background Brush (Default:Blue)
        /// </summary>
        public Brush RangeBackground
        {
            get { return (Brush)GetValue(RangeBackgroundProperty); }
            set { SetValue(RangeBackgroundProperty, value); }
        }
        /// <summary>
        /// TextRange Background Brush (Default:Blue)
        /// </summary>
        public static readonly DependencyProperty RangeBackgroundProperty = DependencyProperty.Register("RangeBackground", typeof(Brush), typeof(TextRangeTextBlock), new UIPropertyMetadata(Brushes.Blue));

        /// <summary>
        /// TextRange Foreground Brush (Default:White)
        /// </summary>
        public Brush RangeForeground
        {
            get { return (Brush)GetValue(RangeForegroundProperty); }
            set { SetValue(RangeForegroundProperty, value); }
        }
        /// <summary>
        /// TextRange Foreground Brush (Default:White)
        /// </summary>
        public static readonly DependencyProperty RangeForegroundProperty = DependencyProperty.Register("RangeForeground", typeof(Brush), typeof(TextRangeTextBlock), new UIPropertyMetadata(Brushes.White));

        /// <summary>
        /// Range Text
        /// </summary>
        public string RangeText
        {
            get { return (string)GetValue(RangeTextProperty); }
            set { SetValue(RangeTextProperty, value); }
        }
        /// <summary>
        /// Range Text
        /// </summary>
        public static readonly DependencyProperty RangeTextProperty = DependencyProperty.Register("RangeText", typeof(string), typeof(TextRangeTextBlock), new UIPropertyMetadata(""));

        /// <summary>
        /// Constructor
        /// </summary>
        public TextRangeTextBlock()
        {
            this.Loaded += this.TextRangeTextBlock_Loaded;
        }

        /// <summary>
        /// Loaded Event Action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextRangeTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var textRange = this.FindTextRange(RangeText);
            if (textRange != null)
            {
                textRange.ApplyPropertyValue(TextElement.BackgroundProperty, RangeBackground);
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, RangeForeground);
            }
        }
    }
}
