﻿using System;
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

namespace DinoWpf.Views
{
    /// <summary>
    /// Interaction logic for CreateRecipeView.xaml
    /// </summary>
    public partial class CreateRecipeView : Window
    {
        public log4net.ILog Logger { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public CreateRecipeView()
        {
            InitializeComponent();
        }
    }
}
