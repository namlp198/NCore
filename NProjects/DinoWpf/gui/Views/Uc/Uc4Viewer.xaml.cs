﻿using NCore.Wpf.UcZoomBoxViewer;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Uc4Viewer : UserControl
    {
        public Uc4Viewer()
        {
            InitializeComponent();

            ucView1.CameraIndex = 0;
            ucView2.CameraIndex = 1;
            ucView3.CameraIndex = 2;
            ucView4.CameraIndex = 3;

            ucView1.CreateRecipe += UcView1_CreateRecipe;
            ucView1.UpdateRecipe += UcView1_UpdateRecipe;
            ucView2.CreateRecipe += UcView2_CreateRecipe;
            ucView2.UpdateRecipe += UcView2_UpdateRecipe;
            ucView3.CreateRecipe += UcView3_CreateRecipe;
            ucView3.UpdateRecipe += UcView3_UpdateRecipe;
            ucView4.CreateRecipe += UcView4_CreateRecipe;
            ucView4.UpdateRecipe += UcView4_UpdateRecipe;
        }
        private void UcView4_UpdateRecipe(object sender, RoutedEventArgs e)
        {
            CreateRecipeView createRecipeView = new CreateRecipeView();
            createRecipeView.ShowDialog();
        }

        private void UcView4_CreateRecipe(object sender, RoutedEventArgs e)
        {
            EnterRecipeNameView recipeName = new EnterRecipeNameView();
            recipeName.ShowDialog();
        }

        private void UcView3_UpdateRecipe(object sender, RoutedEventArgs e)
        {
            CreateRecipeView createRecipeView = new CreateRecipeView();
            createRecipeView.ShowDialog();
        }

        private void UcView3_CreateRecipe(object sender, RoutedEventArgs e)
        {
            EnterRecipeNameView recipeName = new EnterRecipeNameView();
            recipeName.ShowDialog();
        }

        private void UcView2_UpdateRecipe(object sender, RoutedEventArgs e)
        {
            CreateRecipeView createRecipeView = new CreateRecipeView();
            createRecipeView.ShowDialog();
        }

        private void UcView2_CreateRecipe(object sender, RoutedEventArgs e)
        {
            EnterRecipeNameView recipeName = new EnterRecipeNameView();
            recipeName.ShowDialog();
        }
        private void UcView1_UpdateRecipe(object sender, RoutedEventArgs e)
        {
            CreateRecipeView createRecipeView = new CreateRecipeView();
            createRecipeView.ShowDialog();
        }
        private void UcView1_CreateRecipe(object sender, RoutedEventArgs e)
        {
            EnterRecipeNameView recipeName = new EnterRecipeNameView();
            recipeName.ShowDialog();
        }
    }
}
