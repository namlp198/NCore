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
    /// Interaction logic for Uc3Viewer.xaml
    /// </summary>
    public partial class Uc3Viewer : UserControl
    {
        public Uc3Viewer()
        {
            InitializeComponent();

            ucView1.CameraIndex = 0;
            ucView2.CameraIndex = 1;
            ucView3.CameraIndex = 2;

            ucView1.CreateRecipe += UcView1_CreateRecipe;
            ucView1.UpdateRecipe += UcView1_UpdateRecipe;
            ucView2.CreateRecipe += UcView2_CreateRecipe;
            ucView2.UpdateRecipe += UcView2_UpdateRecipe;
            ucView3.CreateRecipe += UcView3_CreateRecipe;
            ucView3.UpdateRecipe += UcView3_UpdateRecipe;
        }

        private void UcView3_UpdateRecipe(object sender, RoutedEventArgs e)
        {
            CreateRecipeView createRecipeView = new CreateRecipeView();
            MainViewModel.Instance.CameraIdSelected = ucView3.CameraIndex;
            createRecipeView.ShowDialog();
        }

        private void UcView3_CreateRecipe(object sender, RoutedEventArgs e)
        {
            EnterRecipeNameView recipeName = new EnterRecipeNameView();
            MainViewModel.Instance.CameraIdSelected = ucView3.CameraIndex;
            recipeName.ShowDialog();
        }

        private void UcView2_UpdateRecipe(object sender, RoutedEventArgs e)
        {
            CreateRecipeView createRecipeView = new CreateRecipeView();
            MainViewModel.Instance.CameraIdSelected = ucView2.CameraIndex;
            createRecipeView.ShowDialog();
        }

        private void UcView2_CreateRecipe(object sender, RoutedEventArgs e)
        {
            EnterRecipeNameView recipeName = new EnterRecipeNameView();
            MainViewModel.Instance.CameraIdSelected = ucView2.CameraIndex;
            recipeName.ShowDialog();
        }
        private void UcView1_UpdateRecipe(object sender, RoutedEventArgs e)
        {
            CreateRecipeView createRecipeView = new CreateRecipeView();
            MainViewModel.Instance.CameraIdSelected = ucView1.CameraIndex;
            createRecipeView.ShowDialog();
        }

        private void UcView1_CreateRecipe(object sender, RoutedEventArgs e)
        {
            EnterRecipeNameView recipeName = new EnterRecipeNameView();
            MainViewModel.Instance.CameraIdSelected = ucView1.CameraIndex;
            recipeName.ShowDialog();
        }
    }
}
