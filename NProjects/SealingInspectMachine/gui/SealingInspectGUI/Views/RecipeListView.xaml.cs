﻿using SealingInspectGUI.ViewModels;
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
using System.Windows.Shapes;

namespace SealingInspectGUI.Views
{
    /// <summary>
    /// Interaction logic for RecipeListView.xaml
    /// </summary>
    public partial class RecipeListView : Window
    {
        public RecipeListView()
        {
            InitializeComponent();

            RecipeListViewModel recipeListViewModel = new RecipeListViewModel(this.Dispatcher, this);
            this.DataContext = recipeListViewModel;
        }
    }
}