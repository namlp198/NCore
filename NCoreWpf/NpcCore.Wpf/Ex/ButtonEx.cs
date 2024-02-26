using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace NpcCore.Wpf.Ex
{
    public class ButtonEx
    {
        public static readonly DependencyProperty UseAutoImaeSizeProperty =
             DependencyProperty.RegisterAttached("UseAutoImaeSize", typeof(bool), typeof(ButtonEx), new FrameworkPropertyMetadata(false));

        public static bool GetUseAutoImaeSize(ButtonBase obj)
        {
            return (bool)obj.GetValue(UseAutoImaeSizeProperty);
        }

        public static void SetUseAutoImaeSize(ButtonBase obj, bool value)
        {
            obj.SetValue(UseAutoImaeSizeProperty, value);
        }

        public static readonly DependencyProperty OptionProperty =
             DependencyProperty.RegisterAttached("Option", typeof(string), typeof(ButtonEx), new FrameworkPropertyMetadata((string)null));

        public static string GetOption(ButtonBase obj)
        {
            return (string)obj.GetValue(OptionProperty);
        }

        public static void SetOption(ButtonBase obj, string value)
        {
            obj.SetValue(OptionProperty, value);
        }


        #region Image Button
        public static readonly DependencyProperty ImageWidthProperty =
             DependencyProperty.RegisterAttached("ImageWidth", typeof(double), typeof(ButtonEx), new FrameworkPropertyMetadata(double.NaN));

        public static double GetImageWidth(ButtonBase obj)
        {
            return (double)obj.GetValue(ImageWidthProperty);
        }

        public static void SetImageWidth(ButtonBase obj, double value)
        {
            obj.SetValue(ImageWidthProperty, value);
        }

        public static readonly DependencyProperty ImageHeightProperty =
             DependencyProperty.RegisterAttached("ImageHeight", typeof(double), typeof(ButtonEx), new FrameworkPropertyMetadata(double.NaN));

        public static double GetImageHeight(ButtonBase obj)
        {
            return (double)obj.GetValue(ImageHeightProperty);
        }

        public static void SetImageHeight(ButtonBase obj, double value)
        {
            obj.SetValue(ImageHeightProperty, value);
        }


        public static readonly DependencyProperty NormalImageProperty =
             DependencyProperty.RegisterAttached("NormalImage", typeof(ImageSource), typeof(ButtonEx), new FrameworkPropertyMetadata((ImageSource)null, new PropertyChangedCallback(OnNormalImageChanged)));

        private static void OnNormalImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var useAutoImageSize = d.GetValue(ButtonEx.UseAutoImaeSizeProperty);
            if (useAutoImageSize != null && (bool)useAutoImageSize == true)
            {
                ImageSource image = e.NewValue as ImageSource;
                if (image != null)
                {
                    d.SetValue(ButtonEx.ImageWidthProperty, image.Width);
                    d.SetValue(ButtonEx.ImageHeightProperty, image.Height);
                }
            }
        }

        public static ImageSource GetNormalImage(ButtonBase obj)
        {
            return (ImageSource)obj.GetValue(NormalImageProperty);
        }

        public static void SetNormalImage(ButtonBase obj, ImageSource value)
        {
            obj.SetValue(NormalImageProperty, value);
        }

        public static readonly DependencyProperty OverImageProperty =
             DependencyProperty.RegisterAttached("OverImage", typeof(ImageSource), typeof(ButtonEx), new FrameworkPropertyMetadata((ImageSource)null));

        public static ImageSource GetOverImage(ButtonBase obj)
        {
            return (ImageSource)obj.GetValue(OverImageProperty);
        }

        public static void SetOverImage(ButtonBase obj, ImageSource value)
        {
            obj.SetValue(OverImageProperty, value);
        }

        public static readonly DependencyProperty PressedImageProperty =
             DependencyProperty.RegisterAttached("PressedImage", typeof(ImageSource), typeof(ButtonEx), new FrameworkPropertyMetadata((ImageSource)null));

        public static ImageSource GetPressedImage(ButtonBase obj)
        {
            return (ImageSource)obj.GetValue(PressedImageProperty);
        }

        public static void SetPressedImage(ButtonBase obj, ImageSource value)
        {
            obj.SetValue(PressedImageProperty, value);
        }

        public static readonly DependencyProperty DisabledImageProperty =
             DependencyProperty.RegisterAttached("DisabledImage", typeof(ImageSource), typeof(ButtonEx), new FrameworkPropertyMetadata((ImageSource)null));

        public static ImageSource GetDisabledImage(ButtonBase obj)
        {
            return (ImageSource)obj.GetValue(DisabledImageProperty);
        }

        public static void SetDisabledImage(ButtonBase obj, ImageSource value)
        {
            obj.SetValue(DisabledImageProperty, value);
        }


        public static readonly DependencyProperty CheckedNormalImageProperty =
            DependencyProperty.RegisterAttached("CheckedNormalImage", typeof(ImageSource), typeof(ButtonEx), new FrameworkPropertyMetadata((ImageSource)null));

        public static ImageSource GetCheckedNormalImage(ButtonBase obj)
        {
            return (ImageSource)obj.GetValue(CheckedNormalImageProperty);
        }

        public static void SetCheckedNormalImage(ButtonBase obj, ImageSource value)
        {
            obj.SetValue(CheckedNormalImageProperty, value);
        }

        public static readonly DependencyProperty CheckedOverImageProperty =
            DependencyProperty.RegisterAttached("CheckedOverImage", typeof(ImageSource), typeof(ButtonEx), new FrameworkPropertyMetadata((ImageSource)null));

        public static ImageSource GetCheckedOverImage(ButtonBase obj)
        {
            return (ImageSource)obj.GetValue(CheckedOverImageProperty);
        }

        public static void SetCheckedOverImage(ButtonBase obj, ImageSource value)
        {
            obj.SetValue(CheckedOverImageProperty, value);
        }

        public static readonly DependencyProperty CheckedPressedImageProperty =
             DependencyProperty.RegisterAttached("CheckedPressedImage", typeof(ImageSource), typeof(ButtonEx), new FrameworkPropertyMetadata((ImageSource)null));

        public static ImageSource GetCheckedPressedImage(ButtonBase obj)
        {
            return (ImageSource)obj.GetValue(CheckedPressedImageProperty);
        }

        public static void SetCheckedPressedImage(ButtonBase obj, ImageSource value)
        {
            obj.SetValue(CheckedPressedImageProperty, value);
        }

        public static readonly DependencyProperty CheckedDisabledImageProperty =
             DependencyProperty.RegisterAttached("CheckedDisabledImage", typeof(ImageSource), typeof(ButtonEx), new FrameworkPropertyMetadata((ImageSource)null));

        public static ImageSource GetCheckedDisabledImage(ButtonBase obj)
        {
            return (ImageSource)obj.GetValue(CheckedDisabledImageProperty);
        }

        public static void SetCheckedDisabledImage(ButtonBase obj, ImageSource value)
        {
            obj.SetValue(CheckedDisabledImageProperty, value);
        } 
        #endregion


        public static readonly DependencyProperty OverBrushProperty =
            DependencyProperty.RegisterAttached("OverBrush", typeof(Brush), typeof(ButtonEx), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static Brush GetOverBrush(ButtonBase obj)
        {
            return (Brush)obj.GetValue(OverBrushProperty);
        }

        public static void SetOverBrush(ButtonBase obj, Brush value)
        {
            obj.SetValue(OverBrushProperty, value);
        }

        public static readonly DependencyProperty PressedBrushProperty =
            DependencyProperty.RegisterAttached("PressedBrush", typeof(Brush), typeof(ButtonEx), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static Brush GetPressedBrush(ButtonBase obj)
        {
            return (Brush)obj.GetValue(PressedBrushProperty);
        }

        public static void SetPressedBrush(ButtonBase obj, Brush value)
        {
            obj.SetValue(PressedBrushProperty, value);
        }
    }
}