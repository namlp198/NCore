using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NpcCore.Wpf.Core
{
    public class MouseWheelBehavior
    {
        public static double GetValue(Slider slider)
        {
            return (double)slider.GetValue(ValueProperty);
        }

        public static void SetValue(Slider slider, double value)
        {
            slider.SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached(
            "Value",
            typeof(double),
            typeof(MouseWheelBehavior),
            new UIPropertyMetadata(0.0, OnValueChanged));

        public static Slider GetSlider(UIElement parentElement)
        {
            return (Slider)parentElement.GetValue(SliderProperty);
        }

        public static void SetSlider(UIElement parentElement, Slider value)
        {
            parentElement.SetValue(SliderProperty, value);
        }

        public static readonly DependencyProperty SliderProperty =
            DependencyProperty.RegisterAttached(
            "Slider",
            typeof(Slider),
            typeof(MouseWheelBehavior));


        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // [NCS-2695] CID 171133 Unchecked dynamic_cast
            //Slider slider = d as Slider;
            //slider.Loaded += (ss, ee) =>
            //{
            //    //Window window = Window.GetWindow(slider);
            //    //if (window != null)
            //    //{
            //    //    SetSlider(window, slider);
            //    //    window.PreviewMouseWheel += Window_PreviewMouseWheel;
            //    //}
            //    slider.PreviewMouseWheel += OnPreviewMouseWheel;
            //};
            //slider.Unloaded += (ss, ee) =>
            //{
            //    //Window window = Window.GetWindow(slider);
            //    //if (window != null)
            //    //{
            //    //    SetSlider(window, null);
            //    //    window.PreviewMouseWheel -= Window_PreviewMouseWheel;
            //    //}
            //    slider.PreviewMouseWheel -= OnPreviewMouseWheel;
            //};
            if (d is Slider slider)
            {
                slider.Loaded += (ss, ee) =>
                {
                    slider.PreviewMouseWheel += OnPreviewMouseWheel;
                };

                slider.Unloaded += (ss, ee) =>
                {
                    slider.PreviewMouseWheel -= OnPreviewMouseWheel;
                };
            }
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Window window = sender as Window;
            if (sender is Slider slider)
            {
                double value = GetValue(slider);
                if (value != 0)
                {
                    slider.Value += slider.SmallChange * e.Delta / value;
                }
            }
        }
    }
}
