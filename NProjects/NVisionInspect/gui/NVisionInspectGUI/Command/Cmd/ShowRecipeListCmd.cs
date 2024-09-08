﻿using NVisionInspectGUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Command.Cmd
{
    public class ShowRecipeListCmd : CommandBase
    {
        public ShowRecipeListCmd() { }
        public override void Execute(object parameter)
        {
            RecipeListView recipeListView = new RecipeListView();
            recipeListView.ShowDialog();
        }
    }
}