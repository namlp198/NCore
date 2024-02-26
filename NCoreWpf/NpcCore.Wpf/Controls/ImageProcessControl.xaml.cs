using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NpcCore.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for ImageProcessControl.xaml
    /// </summary>
    public partial class ImageProcessControl : UserControl
    {
        public const string CHANGE_LIGHT_INTENSITY = "Change_Light_Intensity";
        public const string CHANGE_BRIGHTNESS = "Change_Brightness";
        public const string CHANGE_CONSTRAST = "Change_Constrast";
        public const string CHANGE_COLOR_PICKER_SIZE = "Change_color_picker_size";
        public const string CHANGE_COLOR_PICKER_MODE = "Change_color_picker_mode";
        #region DependencyProperty
        public static readonly DependencyProperty LightIntensityProperty = DependencyProperty.Register("LightIntensity", typeof(int), typeof(ImageProcessControl),
            new PropertyMetadata(30, OnLightIntensityChanged));
        public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register("Brightness", typeof(int), typeof(ImageProcessControl),
            new PropertyMetadata(0, OnBrightnessChanged));
        public static readonly DependencyProperty ConstrastProperty = DependencyProperty.Register("Constrast", typeof(int), typeof(ImageProcessControl),
            new PropertyMetadata(0, OnConstrastChanged));
        public static readonly DependencyProperty HueMinProperty = DependencyProperty.Register("HueMin", typeof(int), typeof(ImageProcessControl),
            new PropertyMetadata(50, OnHueMinValueChanged));
        public static readonly DependencyProperty HueMaxProperty = DependencyProperty.Register("HueMax", typeof(int), typeof(ImageProcessControl),
            new PropertyMetadata(300, OnHueMaxValueChanged));
        public static readonly DependencyProperty SaturationMinProperty = DependencyProperty.Register("SaturationMin", typeof(int), typeof(ImageProcessControl),
            new PropertyMetadata(30, OnSaturationMinValueChanged));
        public static readonly DependencyProperty SaturationMaxProperty = DependencyProperty.Register("SaturationMax", typeof(int), typeof(ImageProcessControl),
            new PropertyMetadata(70, OnSaturationMaxValueChanged));
        public static readonly DependencyProperty ValueMinProperty = DependencyProperty.Register("ValueMin", typeof(int), typeof(ImageProcessControl),
            new PropertyMetadata(30, OnValueMinChanged));
        public static readonly DependencyProperty ValueMaxProperty = DependencyProperty.Register("ValueMax", typeof(int), typeof(ImageProcessControl),
            new PropertyMetadata(70, OnValueMaxChanged));
        public static readonly DependencyProperty ImagesSourceProperty = DependencyProperty.Register("ImagesSource", typeof(ObservableCollection<DataImage>), typeof(ImageProcessControl),
            new PropertyMetadata(null));
        public static readonly DependencyProperty ImageSelectedProperty = DependencyProperty.Register("ImageSelected", typeof(DataImage), typeof(ImageProcessControl),
            new PropertyMetadata(null, OnImageSelectedChanged));

        public static readonly DependencyProperty ColorPickerSizeProperty = DependencyProperty.Register("ColorPickerSize", typeof(int), typeof(ImageProcessControl),
           new PropertyMetadata(5, OnColorPickerSizeChanged));
        public static readonly DependencyProperty SelectedPickerColorProperty= DependencyProperty.Register("SelectedPickerColor",typeof(string),typeof(ImageProcessControl),new PropertyMetadata("#00ffffff", OnPickerColorChanged));
        public static readonly DependencyProperty SliderProcessColorProperty = DependencyProperty.Register("SliderProcessColor", typeof(string), typeof(ImageProcessControl), new PropertyMetadata("#00ffffff", OnPickerColorChanged));
        public static readonly DependencyProperty ColorRangeValueChangeProperty = DependencyProperty.Register("ColorRangeValueChange", typeof(int), typeof(ImageProcessControl), new PropertyMetadata(1));

        public static readonly DependencyProperty ColorPickerModeProperty = DependencyProperty.Register("ColorPickerMode", typeof(bool), typeof(ImageProcessControl), new PropertyMetadata((bool)false,ColorPickerModeChange));

        #endregion
        #region Events
        public event Action<string, object> ImageParameterChanged;
        public event Action<DataImage> ImageSourceChanged;
        #endregion
        #region Propeties
        public int LightIntensity
        {
            get
            {
                return (int)GetValue(LightIntensityProperty);
            }
            set
            {
                SetValue(LightIntensityProperty, value);
            }
        }

        public int Brightness
        {
            get
            {
                return (int)GetValue(BrightnessProperty);
            }
            set
            {
                SetValue(BrightnessProperty, value);
            }
        }

        public int Constrast
        {
            get
            {
                return (int)GetValue(ConstrastProperty);
            }
            set
            {
                SetValue(ConstrastProperty, value);
            }
        }

        public int ColorPickerSize
        {
            get
            {
                return (int)GetValue(ColorPickerSizeProperty);
            }
            set
            {
                SetValue(ColorPickerSizeProperty, value);
            }
        }

        public string SelectedPickerColor
        {
            get
            {
                return (string)GetValue(SelectedPickerColorProperty);
            }
            set
            {
                SetValue(SelectedPickerColorProperty, value);
            }
        }

        public string SliderProcessColor
        {
            get
            {
                return (string)GetValue(SliderProcessColorProperty);
            }
            set
            {
                SetValue(SliderProcessColorProperty, value);
            }
        }

        public bool ColorPickerMode
        {
            get
            {
                return (bool)GetValue(ColorPickerModeProperty);
            }
            set
            {
                SetValue(ColorPickerModeProperty, value);
            }
        }

        ObservableCollection<DataImage> ImagesSource
        {
            get
            {
                return (ObservableCollection<DataImage>)GetValue(ImagesSourceProperty);
            }
            set
            {
                SetValue(ImagesSourceProperty, value);
            }
        }
        DataImage ImageSelected
        {
            get
            {
                return (DataImage)GetValue(ImageSelectedProperty);
            }
            set
            {
                SetValue(ImageSelectedProperty, value);
            }
        }


        public int HueMin
        {
            get
            {
                return (int)GetValue(HueMinProperty);
            }
            set
            {
                SetValue(HueMinProperty, value);
            }
        }

        public int HueMax
        {
            get
            {
                return (int)GetValue(HueMaxProperty);
            }
            set
            {
                SetValue(HueMaxProperty, value);
            }
        }

        public int SaturationMin
        {
            get
            {
                return (int)GetValue(SaturationMinProperty);
            }
            set
            {
                SetValue(SaturationMinProperty, value);
            }
        }

        public int SaturationMax
        {
            get
            {
                return (int)GetValue(SaturationMaxProperty);
            }
            set
            {
                SetValue(SaturationMaxProperty, value);
            }
        }

        public int ValueMin
        {
            get
            {
                return (int)GetValue(ValueMinProperty);
            }
            set
            {
                SetValue(ValueMinProperty, value);
            }
        }

        public int ValueMax
        {
            get
            {
                return (int)GetValue(ValueMaxProperty);
            }
            set
            {
                SetValue(ValueMaxProperty, value);
            }
        }

        public int ColorRangeValueChange
        {
            get
            {
                return (int)GetValue(ColorRangeValueChangeProperty);
            }
            set
            {
                SetValue(ColorRangeValueChangeProperty, value);
            }
        }
        #endregion
        #region Methods
        private static void OnLightIntensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null)
            {
                if (target.ImageParameterChanged != null)
                {
                    if (target.ImageSelected != null)
                        target.ImageSelected.LightIntensity = target.LightIntensity;
                    target.ImageParameterChanged(CHANGE_LIGHT_INTENSITY, target.LightIntensity);
                }
            }
        }

        private static void OnBrightnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null)
            {
                if (target.ImageParameterChanged != null)
                {
                    if (target.ImageSelected != null)
                        target.ImageSelected.Brightness = target.Brightness;
                    target.ImageParameterChanged(CHANGE_BRIGHTNESS, target.Brightness);
                }
            }
        }

        private static void OnConstrastChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null)
            {
                if (target.ImageParameterChanged != null)
                {
                    if (target.ImageSelected != null)
                        target.ImageSelected.Constrast = target.Constrast;
                    target.ImageParameterChanged(CHANGE_CONSTRAST, target.Constrast);
                }
            }
        }


        private static void OnColorPickerSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null)
            {
                if (target.ImageParameterChanged != null)
                {
                    if (target.ImageSelected != null)
                        target.ImageSelected.ColorPickerSize = target.ColorPickerSize;
                    target.ImageParameterChanged(CHANGE_COLOR_PICKER_SIZE, target.ColorPickerSize);
                }
            }
        }

        private static void OnPickerColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null)
            {
                if (target.ImageParameterChanged != null)
                {
                    if (target.ImageSelected != null)
                        target.ImageSelected.SelectedPickerColor = target.SelectedPickerColor;
                    target.ImageParameterChanged(CHANGE_COLOR_PICKER_SIZE, target.ColorPickerSize);
                }
            }
        }


        private static void ColorPickerModeChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null)
            {
                if (target.ImageParameterChanged != null)
                {
                    if (target.ImageSelected != null)
                        target.ImageSelected.ColorPickerMode = target.ColorPickerMode;
                    target.ImageParameterChanged(CHANGE_COLOR_PICKER_MODE, target.ColorPickerMode);
                }
            }
        }
        

        private static void OnImageSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null && target.ImageSelected != null)
            {
                target.Brightness = target.ImageSelected.Brightness;
                target.Constrast = target.ImageSelected.Constrast;
                target.LightIntensity = target.ImageSelected.LightIntensity;
                target.HueMin = target.ImageSelected.HueMinValue;
                target.HueMax = target.ImageSelected.HueMaxValue;
                target.SaturationMin = target.ImageSelected.SaturationMinValue;
                target.SaturationMax = target.ImageSelected.SaturationMaxValue;
                target.ValueMin = target.ImageSelected.ValueMin;
                target.ValueMax = target.ImageSelected.ValueMax;
                target.ColorPickerSize = target.ImageSelected.ColorPickerSize;
                target.SelectedPickerColor = target.ImageSelected.SelectedPickerColor;
                if (target.ImageParameterChanged != null)
                {
                    target.ImageSourceChanged(target.ImageSelected);
                }
            }
        }

        private static void OnHueMinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null && target.ImageSelected != null)
            {
                target.ImageSelected.HueMinValue = target.HueMin;
            }
        }
        private static void OnHueMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null && target.ImageSelected != null)
            {
                target.ImageSelected.HueMaxValue = target.HueMax;
            }
        }

        private static void OnSaturationMinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null && target.ImageSelected != null)
            {
                target.ImageSelected.SaturationMinValue = target.SaturationMin;
            }
        }

        private static void OnSaturationMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null && target.ImageSelected != null)
            {
                target.ImageSelected.SaturationMaxValue = target.SaturationMax;
            }
        }

        private static void OnValueMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null && target.ImageSelected != null)
            {
                target.ImageSelected.ValueMin = target.ValueMin;
            }
        }

        private static void OnValueMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageProcessControl)d;
            if (target != null && target.ImageSelected != null)
            {
                target.ImageSelected.ValueMax = target.ValueMax;
            }
        }

        public ImageProcessControl()
        {
            InitializeComponent();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton bnt)
            {
                if (bnt.Name == "TeachingButton")
                {
                    TeachingView.Visibility = Visibility.Visible;
                    VerifycationView.Visibility = Visibility.Collapsed;
                }
                else if (bnt.Name == "VerifycationButton")
                {
                    TeachingView.Visibility = Visibility.Collapsed;
                    VerifycationView.Visibility = Visibility.Visible;
                }
            }
        }
        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Filter = "All Files (*.*)|*.*";
            fDialog.FilterIndex = 1;

            fDialog.Multiselect = true;
            if (fDialog.ShowDialog() == true)
            {
                string[] imagespath = fDialog.FileNames;
                if (imagespath != null && imagespath.Count() > 0)
                {
                    if (ImagesSource == null)
                        ImagesSource = new ObservableCollection<DataImage>();
                    foreach (string image in imagespath)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(image));
                        if (bitmapImage != null)
                        {
                            DataImage imageData = new DataImage();
                            if (imageData != null)
                            {
                                imageData.ImageSource = bitmapImage;
                                imageData.ImageIndex = (ImagesSource.Count + 1).ToString();
                                imageData.Brightness = Brightness;
                                imageData.Constrast = Constrast;
                                imageData.LightIntensity = LightIntensity;
                                imageData.ColorPickerSize = ColorPickerSize;
                                imageData.ColorPickerMode = ColorPickerMode;
                                ImagesSource.Add(imageData);
                                if (ImageSelected == null)
                                    ImageSelected = imageData;
                            }
                        }
                    }
                }
            }
        }

        private void ButtonAddedControl_Click(object sender, RoutedEventArgs e)
        {
            if (HueMin - ColorRangeValueChange >= 0)
                HueMin -= ColorRangeValueChange;
            if (HueMax + ColorRangeValueChange <= 360)
                HueMax += ColorRangeValueChange;

            if (SaturationMin - ColorRangeValueChange >= 0)
                SaturationMin -= ColorRangeValueChange;
            if (SaturationMax + ColorRangeValueChange <= 100)
                SaturationMax += ColorRangeValueChange;

            if (ValueMin - ColorRangeValueChange >= 0)
                ValueMin -= ColorRangeValueChange;
            if (ValueMax + ColorRangeValueChange <= 100)
                ValueMax += ColorRangeValueChange;
        }

        private void ButtonSubtractControl_Click(object sender, RoutedEventArgs e)
        {
            if (HueMin + ColorRangeValueChange <= HueMax)
                HueMin += ColorRangeValueChange;
            if (HueMax - ColorRangeValueChange >= HueMin)
                HueMax -= ColorRangeValueChange;

            if (SaturationMin + ColorRangeValueChange <= SaturationMax)
                SaturationMin += ColorRangeValueChange;
            if (SaturationMax + ColorRangeValueChange >= SaturationMin)
                SaturationMax -= ColorRangeValueChange;

            if (ValueMin + ColorRangeValueChange <= ValueMax)
                ValueMin += ColorRangeValueChange;
            if (ValueMax - ColorRangeValueChange >= ValueMin)
                ValueMax -= ColorRangeValueChange;
        }
        #endregion

        private void CbHue_Click(object sender, RoutedEventArgs e)
        {
            // [NCS-2695] CID 171199 Unchecked dynamic_cast
            //bool? isChecked = (sender as CheckBox).IsChecked;
            //if(isChecked != null)
            //    SliderControlHue.IsReverse = (bool) isChecked;
            if (sender is CheckBox checkBox)
            {
                bool? isChecked = checkBox.IsChecked;
                if (isChecked != null)
                {
                    SliderControlHue.IsReverse = (bool)isChecked;
                }
            }
        }

        private void CbStaturation_Click(object sender, RoutedEventArgs e)
        {
            // [NCS-2695] CID 171148 Unchecked dynamic_cast
            //bool? isChecked = (sender as CheckBox).IsChecked;
            //if (isChecked != null)
            //    SliderControlSaturation.IsReverse = (bool)isChecked;
            if (sender is CheckBox checkBox)
            {
                bool? isChecked = checkBox.IsChecked;
                if (isChecked != null)
                {
                    SliderControlSaturation.IsReverse = (bool)isChecked;
                }
            }
        }

        private void PickerColorClick(object sender, RoutedEventArgs e)
        {
            ColorPickerMode = true;
        }
    }

    public class DataImage
    {
        public string ImageIndex
        {
            get
            {
                return _imageIndex;
            }
            set
            {
                _imageIndex = value;
            }
        }
        private string _imageIndex = String.Empty;
        public BitmapImage ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                _imageSource = value;
            }
        }
        private BitmapImage _imageSource = null;

        public int Brightness
        {
            get
            {
                return _brightness;
            }
            set
            {
                _brightness = value;
            }
        }
        private int _brightness = 0;

        public int Constrast
        {
            get
            {
                return _constrast;
            }
            set
            {
                _constrast = value;
            }
        }
        private int _constrast = 0;

        public int LightIntensity
        {
            get
            {
                return _lightIntensity;
            }
            set
            {
                _lightIntensity = value;
            }
        }
        private int _lightIntensity = 0;

        public int HueMinValue
        {
            get
            {
                return _hueMinValue;
            }
            set
            {
                _hueMinValue = value;
            }
        }
        private int _hueMinValue = 0;

        public int HueMaxValue
        {
            get
            {
                return _hueMaxValue;
            }
            set
            {
                _hueMaxValue = value;
            }
        }
        private int _hueMaxValue = 0;

        public int SaturationMinValue
        {
            get
            {
                return _saturationMinValue;
            }
            set
            {
                _saturationMinValue = value;
            }
        }
        private int _saturationMinValue = 0;

        public int SaturationMaxValue
        {
            get
            {
                return _saturationMaxValue;
            }
            set
            {
                _saturationMaxValue = value;
            }
        }
        private int _saturationMaxValue = 0;

        public int ValueMin
        {
            get
            {
                return _valueMin;
            }
            set
            {
                _valueMin = value;
            }
        }
        private int _valueMin = 0;

        public int ValueMax
        {
            get
            {
                return _valueMax;
            }
            set
            {
                _valueMax = value;
            }
        }
        private int _valueMax = 0;

        public int ColorPickerSize
        {
            get
            {
                return _colorPickerSize;
            }
            set
            {
                _colorPickerSize = value;
            }
        }
        private int _colorPickerSize = 3;

        public string SelectedPickerColor
        {
            get
            {
                return _selectedPickerColor;
            }
            set
            {
                _selectedPickerColor = value;
            }
        }
        private string _selectedPickerColor = string.Empty;

        public bool ColorPickerMode
        {
            get
            {
                return _colorPickerMode;
            }
            set
            {
                _colorPickerMode = value;
            }
        }
        private bool _colorPickerMode = false;
    }
}
