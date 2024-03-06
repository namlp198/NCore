using System;
using System.Collections.Generic;
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

namespace DinoWpf.Views.UcSetting
{
    /// <summary>
    /// Interaction logic for UcSettingCalculateArea.xaml
    /// </summary>
    public partial class UcSettingCalculateArea : UserControl
    {
        public log4net.ILog Logger { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public UcSettingCalculateArea()
        {
            InitializeComponent();
        }
    }
}
