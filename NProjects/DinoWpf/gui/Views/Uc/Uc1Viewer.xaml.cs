using DinoWpf.ViewModels;
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

namespace DinoWpf.Views.Uc
{
    /// <summary>
    /// Interaction logic for Uc1Viewer.xaml
    /// </summary>
    public partial class Uc1Viewer : UserControl
    {
        public Uc1Viewer()
        {
            InitializeComponent();

            ucView1.CameraIndex = 0;
            ucView1.CreateRecipe += UcView1_CreateRecipe;
            ucView1.UpdateRecipe += UcView1_UpdateRecipe;
        }

        private void UcView1_UpdateRecipe(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.CameraIdSelected = ucView1.CameraIndex;
            CreateRecipeView createRecipeView = new CreateRecipeView();
            createRecipeView.ShowDialog();
        }

        private void UcView1_CreateRecipe(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.CameraIdSelected = ucView1.CameraIndex;
            EnterRecipeNameView recipeName = new EnterRecipeNameView();
            recipeName.CamId = ucView1.CameraIndex;
            recipeName.ShowDialog();
        }
    }
}
