using DinoWpf.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

namespace DinoWpf.Views.UcSetting
{
    /// <summary>
    /// Interaction logic for UcSettingROITool.xaml
    /// </summary>
    public partial class UcSettingROITool : UserControl, INotifyPropertyChanged
    {
        public log4net.ILog Logger { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<Algorithms> _algorithms = new List<Algorithms>();
        private List<string> _algorithmsDes = new List<string>();
        private int _algorithmSelectedIdx = -1;
        private Algorithms _algorithmSelected = Algorithms.None;

        // Declare Settings View (current just testing with 2 views)
        UcSettingCountPixel _ucSettingCountPixel; 
        UcSettingCalculateArea _ucSettingCalculateArea;

        public UcSettingROITool()
        {
            InitializeComponent();

            _algorithms = Enum.GetValues(typeof(Algorithms))
                          .Cast<Algorithms>()
                          .ToList();

            foreach (var item in _algorithms)
            {
                string s =GetEnumDescription(item);
                if (s.Equals("Null"))
                    continue;
                _algorithmsDes.Add(s);
            }

            this.DataContext = this;
        }

        #region Properties
        public List<string> AlgorithmsDes
        {
            get => _algorithmsDes;
            set
            {
                if(SetProperty(ref _algorithmsDes, value))
                {

                }
            }
        }

        public int AlgorithmSelectedIndex
        {
            get => _algorithmSelectedIdx;
            set
            {
                if(SetProperty(ref _algorithmSelectedIdx, value))
                {
                    switch(_algorithmSelectedIdx)
                    {
                        case 0:
                            AlgorithmSelected = Algorithms.CountPixel; break;
                        case 1:
                            AlgorithmSelected = Algorithms.CalculateArea; break;
                        case 2:
                            AlgorithmSelected = Algorithms.CalculateCoordinate; break;
                        case 3:
                            AlgorithmSelected = Algorithms.CountBlob; break;
                        case 4:
                            AlgorithmSelected = Algorithms.FindLine; break;
                        case 5:
                            AlgorithmSelected = Algorithms.FindCircle; break;
                        case 6:
                            AlgorithmSelected = Algorithms.OCR; break;
                    }
                }
            }
        }

        public Algorithms AlgorithmSelected
        {
            get => _algorithmSelected;
            set
            {
                if(SetProperty(ref _algorithmSelected, value))
                {
                    switch (_algorithmSelected)
                    {
                        case Algorithms.CountPixel:
                            _ucSettingCountPixel = new UcSettingCountPixel();
                            contentSetting.Content = _ucSettingCountPixel;
                            break;
                        case Algorithms.CalculateArea:
                            _ucSettingCalculateArea = new UcSettingCalculateArea();
                            contentSetting.Content = _ucSettingCalculateArea;
                            break;
                    }
                }
            }
        }
        #endregion

        #region Implement INotifyPropertyChanged
        //
        // Summary:
        //     Occurs when a property value changes.
        public event PropertyChangedEventHandler PropertyChanged;

        //
        // Summary:
        //     Checks if a property already matches a desired value. Sets the property and notifies
        //     listeners only when necessary.
        //
        // Parameters:
        //   storage:
        //     Reference to a property with both getter and setter.
        //
        //   value:
        //     Desired value for the property.
        //
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support CallerMemberName.
        //
        // Type parameters:
        //   T:
        //     Type of the property.
        //
        // Returns:
        //     True if the value was changed, false if the existing value matched the desired
        //     value.
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
        //
        // Summary:
        //     Checks if a property already matches a desired value. Sets the property and notifies
        //     listeners only when necessary.
        //
        // Parameters:
        //   storage:
        //     Reference to a property with both getter and setter.
        //
        //   value:
        //     Desired value for the property.
        //
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support CallerMemberName.
        //
        //   onChanged:
        //     Action that is called after the property value has been changed.
        //
        // Type parameters:
        //   T:
        //     Type of the property.
        //
        // Returns:
        //     True if the value was changed, false if the existing value matched the desired
        //     value.
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);
            return true;
        }
        //
        // Summary:
        //     Raises this object's PropertyChanged event.
        //
        // Parameters:
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support System.Runtime.CompilerServices.CallerMemberNameAttribute.
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        //
        // Summary:
        //     Raises this object's PropertyChanged event.
        //
        // Parameters:
        //   args:
        //     The PropertyChangedEventArgs
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }
        #endregion

        #region Methods
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            // [NCS-2695]
            //  - CID 171132 Unchecked dynamic_cast
            //  - CID 171151 Dereference null return value
            //object[] attribArray = fieldInfo.GetCustomAttributes(false);
            //if (attribArray.Length == 0)
            //{
            //    return enumObj.ToString();
            //}
            //else
            //{
            //    DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
            //    return attrib.Description;
            //}
            if (fieldInfo != null)
            {
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                if (attribArray != null && attribArray.Length > 0 && attribArray[0] is DescriptionAttribute attrib)
                {
                    return attrib.Description;
                }
            }
            return enumObj.ToString();
        }
        #endregion

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Save into job file at here
        }
    }
}
