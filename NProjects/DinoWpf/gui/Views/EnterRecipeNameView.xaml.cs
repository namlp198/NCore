using DinoWpf.Commons;
using DinoWpf.ViewModels;
using Kis.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace DinoWpf.Views
{
    /// <summary>
    /// Interaction logic for EnterRecipeNameView.xaml
    /// </summary>
    public partial class EnterRecipeNameView : Window
    {
        private int _camId;
        private XmlManagement _xmlManagement = new XmlManagement();
        public EnterRecipeNameView()
        {
            InitializeComponent();
        }
        public int CamId { get => _camId; set => _camId = value; }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRecipeName.Text.Trim()))
                return;

            string xmlPath = CommonDefines.JobXmlPath + MainViewModel.Instance.JobSelected.Name + ".xml";
            _xmlManagement.Load(xmlPath);

            // find the node camera have a id = camera index
            XmlNode nodeCam = _xmlManagement.SelectSingleNode("//Job/Camera[@id='" + _camId + "']");
            if(nodeCam!=null)
            {
                // add new Recipe node into Camera node
                XmlNode nodeRecipe = _xmlManagement.AddChildNode(nodeCam, "Recipe");
                if (nodeRecipe != null)
                {
                    // add attributes of Recipe into node Recipe
                    _xmlManagement.AddAttributeToNode(nodeRecipe, "id", "0");
                    _xmlManagement.AddAttributeToNode(nodeRecipe, "name", txtRecipeName.Text.Trim());
                    if(_xmlManagement.Save(xmlPath))
                    {
                        // when save success then re-load job
                        MainViewModel.Instance.LoadJob();

                        // close view
                        this.Close();
                        Thread.Sleep(1);

                        // show form recipe
                        CreateRecipeView createRecipeView = new CreateRecipeView();
                        createRecipeView.ShowDialog();
                    }
                }
            }
        }
    }
}
