using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace NpcCore.Wpf.Ex
{

    public class ListofHorizotalTextBox : Control
    {
        Grid ListText;

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(ListofHorizotalTextBox), new PropertyMetadata((int)0,OnDatachanged));
        public override void OnApplyTemplate()
        {
           
            ListText = GetTemplateChild("ListText") as Grid;
            OnMaxValueChange();
            base.OnApplyTemplate();
        }

        public  int MaxValue
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


        public static void OnDatachanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ListofHorizotalTextBox)d;
            if (obj != null )
            {
                obj.OnMaxValueChange();
            }

        }
        public void OnMaxValueChange()
        {
            
            if (ListText !=null)
            {
                if (MaxValue !=0)
                {
                    if (ListText != null)
                    {
                        ListText.ColumnDefinitions.Clear();
                        int dividednumber = (int) MaxValue / 5;
                        for (int i=0;i<=dividednumber;i++)
                        {
                            ColumnDefinition c1 = new ColumnDefinition();
                            c1.Width = new GridLength(1, GridUnitType.Star);
                            ListText.ColumnDefinitions.Add(c1);
                            TextBlock newText = new TextBlock();
                            
                            newText.Text = (5 * i).ToString();
                            newText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cccccc"));
                            newText.HorizontalAlignment = HorizontalAlignment.Left;
                            newText.FontSize = 11;
                            newText.Margin = new Thickness(-1,0,0,0);
                            ListText.Children.Add(newText);
                            Grid.SetColumn(newText, i);
                        }
                       
                    }
                }
            }
        }
    }
}
