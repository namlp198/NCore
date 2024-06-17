using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Npc.Foundation.Base;
using SealingInspectGUI.Command.Cmd;
using SealingInspectGUI.Manager;
using SealingInspectGUI.Models;
using SealingInspectGUI.Properties;
using SealingInspectGUI.Views;

namespace SealingInspectGUI.ViewModels
{
    public class RecipeListViewModel : ViewModelBase
    {
        private readonly Dispatcher _dispatcher;
        private RecipeListView _recipeListView;

        private RecipeList_MapToDataGrid_Model m_recipeSelected = new RecipeList_MapToDataGrid_Model();
        private List<RecipeList_MapToDataGrid_Model> m_recipeListMapToDataGrid_List = new List<RecipeList_MapToDataGrid_Model>();
        private List<string> m_recipeList = new List<string>();

        public RecipeListViewModel(Dispatcher dispatcher, RecipeListView recipeListView)
        {
            _dispatcher = dispatcher;
            _recipeListView = recipeListView;

            GetRecipeList();
        }

        private void GetRecipeList()
        {
            string sRecipeList = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting.m_sModelList;
            string sModelName = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting.m_sModelName;
            string[] arrRecipe = sRecipeList.Split(',');

            List<RecipeList_MapToDataGrid_Model> recipeList = new List<RecipeList_MapToDataGrid_Model>();
            for (int i = 0; i < arrRecipe.Length; i++)
            {
                m_recipeList.Add(arrRecipe[i]);

                RecipeList_MapToDataGrid_Model recipeList_MapToDataGrid_Model = new RecipeList_MapToDataGrid_Model();
                recipeList_MapToDataGrid_Model.Index = i + 1;
                recipeList_MapToDataGrid_Model.RecipeName = arrRecipe[i];
                if (string.Compare(arrRecipe[i], sModelName) == 0)
                {
                    recipeList_MapToDataGrid_Model.SelectState = true;
                }
                else
                {
                    recipeList_MapToDataGrid_Model.SelectState = false;
                }

                recipeList.Add(recipeList_MapToDataGrid_Model);
            }

            RecipeListMapToDataGrid_List = recipeList;
        }
        public RecipeListView RecipeListView { get { return _recipeListView; } }

        public List<RecipeList_MapToDataGrid_Model> RecipeListMapToDataGrid_List
        {
            get => m_recipeListMapToDataGrid_List;
            set
            {
                if(SetProperty(ref m_recipeListMapToDataGrid_List, value))
                {

                }
            }
        }
        public RecipeList_MapToDataGrid_Model RecipeSelected
        {
            get => m_recipeSelected;
            set
            {
                if(SetProperty(ref m_recipeSelected, value))
                {
                    RecipeSelected.SelectState = true;
                    foreach (var recipe in RecipeListMapToDataGrid_List)
                    {
                        if (recipe != null && recipe.Index != RecipeSelected.Index)
                        {
                            recipe.SelectState = false;
                        }
                    }

                    InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                              m_sealingInspectSysSetting.m_sModelName = RecipeSelected.RecipeName;

                    InterfaceManager.Instance.m_sealingInspectProcessorManager.
                        m_sealingInspProcessorDll.SaveSystemSetting(ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting);

                    // Reload system setting and recipe vision core
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.
                       m_sealingInspProcessorDll.ReloadSystenSettings();
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.
                       m_sealingInspProcessorDll.ReloadRecipe();

                    // Reload system setting and recipe GUI
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.
                       m_sealingInspProcessorDll.LoadSystemSettings(ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting);
                    MainViewModel.Instance.SettingVM.LoadSystemSettings();
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.
                       m_sealingInspProcessorDll.LoadRecipe(ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe);
                    MainViewModel.Instance.SettingVM.LoadRecipe();

                    MainViewModel.Instance.RecipeName = RecipeSelected.RecipeName;
                    MessageBox.Show("Load Recipe Success!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
