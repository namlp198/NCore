using ReadCodeGUI.Commons;
using ReadCodeGUI.Manager;
using ReadCodeGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReadCodeGUI.Command.Cmd
{
    public class SaveRecipeCmd : CommandBase
    {
        public SaveRecipeCmd() { }
        public override void Execute(object parameter)
        {
            SetValueRecipe();
            InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.
                SaveRecipe(ref InterfaceManager.Instance.m_processorManager.m_readCodeRecipe);
        }
        private void SetValueRecipe()
        {

            for (int i = 0; i < MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels.Count; i++)
            {
                int nIdx = MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Index;
                switch (nIdx)
                {
                    case 1:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value, out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nMaxCodeCount);
                        break;
                }
            }
        }
    }
}
