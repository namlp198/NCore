using SealingInspectGUI.Manager;
using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SealingInspectGUI.Command.Cmd
{
    public class SaveRecipeCmd : CommandBase
    {
        int nFrameIdx = 0;
        public SaveRecipeCmd() { }
        public override void Execute(object parameter)
        {
            string btnName = parameter as string;
            int.TryParse(btnName.Substring(btnName.IndexOf("_") - 1, 1), out nFrameIdx);

            if (btnName != null)
            {
                if (string.Compare(btnName, "btnSaveFrame1_Top") == 0 ||
                   string.Compare(btnName, "btnSaveFrame2_Top") == 0)
                {
                    if (nFrameIdx == 1)
                        SetValueFrame1_TopCam();
                    else if (nFrameIdx == 2)
                        SetValueFrame2_TopCam();
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.
                        SaveRecipe(ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe, "TOPCAM", nFrameIdx);
                }

                else if (string.Compare(btnName, "btnSaveFrame1_Side") == 0 ||
                         string.Compare(btnName, "btnSaveFrame2_Side") == 0 ||
                         string.Compare(btnName, "btnSaveFrame3_Side") == 0 ||
                         string.Compare(btnName, "btnSaveFrame4_Side") == 0)
                {
                    SetValueFrame_SideCam();
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.
                        SaveRecipe(ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe, "SIDECAM", nFrameIdx);
                }
            }
        }
        private void SetValueFrame1_TopCam()
        {
            int nTopCam1 = 0;
            int nTopCam2 = 1;

            for (int i = 0; i < MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam.Count; i++)
            {
                int nIdx = MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].Index;
                switch (nIdx)
                {
                    case 1:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nDistanceMeasurementTolerance_Min);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nDistanceMeasurementTolerance_Min);
                        break;
                    case 2:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nDistanceMeasurementTolerance_Max);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nDistanceMeasurementTolerance_Max);
                        break;
                    case 3:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadius_Min);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadius_Min);
                        break;
                    case 4:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadius_Max);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadius_Max);
                        break;
                }
            }
        }
        private void SetValueFrame2_TopCam()
        {
            int nTopCam1 = 0;
            int nTopCam2 = 1;

            for (int i = 0; i < MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam.Count; i++)
            {
                int nIdx = MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].Index;
                switch (nIdx)
                {
                    case 1:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nDistanceMeasurementTolerance_Min);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nDistanceMeasurementTolerance_Min);
                        break;
                    case 2:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nDistanceMeasurementTolerance_Max);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nDistanceMeasurementTolerance_Max);
                        break;
                    case 3:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nRadius_Min);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nRadius_Min);
                        break;
                    case 4:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nRadius_Max);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nRadius_Max);
                        break;
                }
            }
        }
        private void SetValueFrame_SideCam()
        {
            if (nFrameIdx == 0)
                return;

            int nSideCam1 = 0;
            int nSideCam2 = 1;

            switch(nFrameIdx)
            {
                // frame 1
                case 1:
                    for(int i = 0; i < MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam.Count; i++)
                    {
                        int nIdx = MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].Index;
                        switch (nIdx)
                        {
                            case 1:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame1.m_nDistanceMeasurementTolerance_Min);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nDistanceMeasurementTolerance_Min);
                                break;
                            case 2:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame1.m_nDistanceMeasurementTolerance_Max);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nDistanceMeasurementTolerance_Max);
                                break;
                        }
                    }
                    break;

                // frame 2
                case 2:
                    for (int i = 0; i < MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam.Count; i++)
                    {
                        int nIdx = MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].Index;
                        switch (nIdx)
                        {
                            case 1:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame2.m_nDistanceMeasurementTolerance_Min);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nDistanceMeasurementTolerance_Min);
                                break;
                            case 2:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame2.m_nDistanceMeasurementTolerance_Max);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nDistanceMeasurementTolerance_Max);
                                break;
                        }
                    }
                    break;

                // frame 3
                case 3:
                    for (int i = 0; i < MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam.Count; i++)
                    {
                        int nIdx = MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].Index;
                        switch (nIdx)
                        {
                            case 1:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame3.m_nDistanceMeasurementTolerance_Min);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nDistanceMeasurementTolerance_Min);
                                break;
                            case 2:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame3.m_nDistanceMeasurementTolerance_Max);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nDistanceMeasurementTolerance_Max);
                                break;
                        }
                    }
                    break;

                // frame 4
                case 4:
                    for (int i = 0; i < MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam.Count; i++)
                    {
                        int nIdx = MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].Index;
                        switch (nIdx)
                        {
                            case 1:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame4.m_nDistanceMeasurementTolerance_Min);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nDistanceMeasurementTolerance_Min);
                                break;
                            case 2:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame4.m_nDistanceMeasurementTolerance_Max);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nDistanceMeasurementTolerance_Max);
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
