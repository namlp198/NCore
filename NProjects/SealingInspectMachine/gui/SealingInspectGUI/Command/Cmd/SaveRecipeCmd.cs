using SealingInspectGUI.Commons;
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
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nThresholdBinaryMinEnclosing);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nThresholdBinaryMinEnclosing);
                        break;
                    case 2:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nThresholdBinaryCannyHoughCircle);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nThresholdBinaryCannyHoughCircle);
                        break;
                    case 3:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nDistanceRadiusDiffMin);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nDistanceRadiusDiffMin);
                        break;
                    case 4:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer);
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer);
                        break;
                    case 5:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min);
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min);
                        break;
                    case 6:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max);
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max);
                        break;
                    case 7:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusInner_Min);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusInner_Min);
                        break;
                    case 8:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusInner_Max);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusInner_Max);
                        break;
                    case 9:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusOuter_Min);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusOuter_Min);
                        break;
                    case 10:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nRadiusOuter_Max);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nRadiusOuter_Max);
                        break;
                    case 11:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nDeltaRadiusOuterInner);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nDeltaRadiusOuterInner);
                        break;
                    case 12:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIWidth_Hor);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIWidth_Hor);
                        break;
                    case 13:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIHeight_Hor);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIHeight_Hor);
                        break;
                    case 14:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIWidth_Ver);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIWidth_Ver);
                        break;
                    case 15:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROIHeight_Ver);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROIHeight_Ver);
                        break;
                    case 16:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI12H_OffsetX);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI12H_OffsetX);
                        break;
                    case 17:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI12H_OffsetY);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI12H_OffsetY);
                        break;
                    case 18:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI3H_OffsetX);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI3H_OffsetX);
                        break;
                    case 19:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI3H_OffsetY);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI3H_OffsetY);
                        break;
                    case 20:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI6H_OffsetX);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI6H_OffsetX);
                        break;
                    case 21:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI6H_OffsetY);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI6H_OffsetY);
                        break;
                    case 22:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI9H_OffsetX);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI9H_OffsetX);
                        break;
                    case 23:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nROI9H_OffsetY);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nROI9H_OffsetY);
                        break;
                    case 24:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_bUseAdvancedAlgorithms);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_bUseAdvancedAlgorithms);
                        break;
                    case 25:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Min);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Min);
                        break;
                    case 26:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Max);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Max);
                        break;
                    case 27:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dIncrementAngle);
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dIncrementAngle);
                        break;
                    case 28:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dThresholdCanny1_MakeROI);
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dThresholdCanny1_MakeROI);
                        break;
                    case 29:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_dThresholdCanny2_MakeROI);
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_dThresholdCanny2_MakeROI);
                        break;
                    case 30:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nDelayTimeGrab);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nDelayTimeGrab);
                        break;
                    case 31:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
                        break;
                    case 32:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nHoughCircleParam1);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nHoughCircleParam1);
                        break;
                    case 33:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame1.m_nHoughCircleParam2);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame1.m_nHoughCircleParam2);
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
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROIWidth);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROIWidth);
                        break;
                    case 2:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROIHeight);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROIHeight);
                        break;
                    case 3:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI1H_OffsetX);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI1H_OffsetX);
                        break;
                    case 4:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI1H_OffsetY);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI1H_OffsetY);
                        break;
                    case 5:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI5H_OffsetX);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI5H_OffsetX);
                        break;
                    case 6:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI5H_OffsetY);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI5H_OffsetY);
                        break;
                    case 7:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI7H_OffsetX);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI7H_OffsetX);
                        break;
                    case 8:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI7H_OffsetY);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI7H_OffsetY);
                        break;
                    case 9:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI11H_OffsetX);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI11H_OffsetX);
                        break;
                    case 10:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nROI11H_OffsetY);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nROI11H_OffsetY);
                        break;
                    case 11:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer);
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer);
                        break;
                    case 12:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                     m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dDistanceMeasurementTolerance_Min);
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dDistanceMeasurementTolerance_Min);
                        break;
                    case 13:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dDistanceMeasurementTolerance_Max);
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dDistanceMeasurementTolerance_Max);
                        break;
                    case 14:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nDelayTimeGrab);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nDelayTimeGrab);
                        break;
                    case 15:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThresholdBinary);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThresholdBinary);
                        break;
                    case 16:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nContourSizeFindBlob_Min);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nContourSizeFindBlob_Min);
                        break;
                    case 17:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nContourSizeFindBlob_Max);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nContourSizeFindBlob_Max);
                        break;
                    case 18:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshBinary_FindBlobWhite);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshBinary_FindBlobWhite);
                        break;
                    case 19:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshBinary_FindBlobWhite_Max);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshBinary_FindBlobWhite_Max);
                        break;
                    case 20:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshBinary_FindBlobBlack);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshBinary_FindBlobBlack);
                        break;
                    case 21:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshBinary_FindBlobBlack_Max);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshBinary_FindBlobBlack_Max);
                        break;
                    case 22:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nBlobCount_Max);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nBlobCount_Max);
                        break;
                    case 23:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dBlobArea_Min);
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dBlobArea_Min);
                        break;
                    case 24:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_dBlobArea_Max);
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_dBlobArea_Max);
                        break;
                    case 25:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nUseCheckSurface);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nUseCheckSurface);
                        break;
                    case 26:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nSelectMethodFindCircle);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nSelectMethodFindCircle);
                        break;
                    case 27:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThresholdBinary_FindCircle);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThresholdBinary_FindCircle);
                        break;
                    case 28:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nContourSizeMin);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nContourSizeMin);
                        break;
                    case 29:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nContourSizeMax);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nContourSizeMax);
                        break;
                    case 30:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nRadiusMin);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nRadiusMin);
                        break;
                    case 31:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nRadiusMax);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nRadiusMax);
                        break;
                    case 32:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nMinDist_HoughCircle);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nMinDist_HoughCircle);
                        break;
                    case 33:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nParam1_HoughCircle);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nParam1_HoughCircle);
                        break;
                    case 34:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nParam2_HoughCircle);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nParam2_HoughCircle);
                        break;
                    case 35:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshold1_Canny);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshold1_Canny);
                        break;
                    case 36:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.m_nThreshold2_Canny);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.m_nThreshold2_Canny);
                        break;
                    case 37:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_X);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_X);
                        break;
                    case 38:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_Y);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint1H_Y);
                        break;
                    case 39:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_X);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_X);
                        break;
                    case 40:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_Y);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint5H_Y);
                        break;
                    case 41:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_X);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_X);
                        break;
                    case 42:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_Y);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint7H_Y);
                        break;
                    case 43:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_X);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_X);
                        break;
                    case 44:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam1Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam1].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_Y);
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_TopCam[i].TopCam2Value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                      m_sealingInspRecipe_TopCam[nTopCam2].m_recipeFrame2.Offset_ROIFindMeasurePoint11H_Y);
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
                            case 2:
                            case 3:
                            case 4:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_nROI_Top[nIdx - 1]);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nROI_Top[nIdx - 1]);
                                break;
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_nROI_Bottom[nIdx - Defines.ROI_PARAMETER_COUNT - 1]);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nROI_Bottom[nIdx - Defines.ROI_PARAMETER_COUNT - 1]);
                                break;

                            // DistanceMeasurementTolerance_Refer
                            case 9:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer);
                                break;
                            // DistanceMeasurementTolerance_Min
                            case 10:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame1.m_dDistanceMeasurementTolerance_Min);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_dDistanceMeasurementTolerance_Min);
                                break;
                            // DistanceMeasurementTolerance_Max
                            case 11:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame1.m_dDistanceMeasurementTolerance_Max);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_dDistanceMeasurementTolerance_Max);
                                break;
                            // DelayTimeGrab
                            case 12:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame1.m_nDelayTimeGrab);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nDelayTimeGrab);
                                break;
                            // FindStartEndX
                            case 13:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_nFindStartEndX);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nFindStartEndX);
                                break;
                            // FindStartEndSearchRangeX
                            case 14:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_nFindStartEndSearchRangeX);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nFindStartEndSearchRangeX);
                                break;
                            // FindStartEndXThresholdGray
                            case 15:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_nFindStartEndXThresholdGray);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nFindStartEndXThresholdGray);
                                break;
                            // ThresholdCanny1_MakeROI
                            case 16:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_dThresholdCanny1_MakeROI);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_dThresholdCanny1_MakeROI);
                                break;
                            // ThresholdCanny2_MakeROI
                            case 17:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_dThresholdCanny2_MakeROI);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_dThresholdCanny2_MakeROI);
                                break;
                            // UseAdvancedAlgorithms
                            case 18:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_bUseAdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_bUseAdvancedAlgorithms);
                                break;
                            // NumberOfDistanceMaxCount_AdvancedAlgorithms
                            case 19:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
                                break;
                            // UseFindROIAdvancedAlgorithms
                            case 20:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.b_bUseFindROIAdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.b_bUseFindROIAdvancedAlgorithms);
                                break;
                            // OffetY_Top
                            case 21:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_nOffetY_Top);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nOffetY_Top);
                                break;
                            // OffetY_Bottom
                            case 22:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_nOffetY_Bottom);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nOffetY_Bottom);
                                break;
                            // ThresholdBinaryFindROI
                            case 23:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame1.m_nThresholdBinaryFindROI);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame1_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame1.m_nThresholdBinaryFindROI);
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
                            case 2:
                            case 3:
                            case 4:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_nROI_Top[nIdx - 1]);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nROI_Top[nIdx - 1]);
                                break;
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_nROI_Bottom[nIdx - Defines.ROI_PARAMETER_COUNT - 1]);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nROI_Bottom[nIdx - Defines.ROI_PARAMETER_COUNT - 1]);
                                break;
                            // DistanceMeasurementTolerance_Refer
                            case 9:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer);
                                break;
                            // DistanceMeasurementTolerance_Min
                            case 10:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame2.m_dDistanceMeasurementTolerance_Min);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_dDistanceMeasurementTolerance_Min);
                                break;
                            // DistanceMeasurementTolerance_Max
                            case 11:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame2.m_dDistanceMeasurementTolerance_Max);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_dDistanceMeasurementTolerance_Max);
                                break;
                            // DelayTimeGrab 
                            case 12:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame2.m_nDelayTimeGrab);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nDelayTimeGrab);
                                break;
                            // FindStartEndX
                            case 13:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_nFindStartEndX);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nFindStartEndX);
                                break;
                            // FindStartEndSearchRangeX
                            case 14:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_nFindStartEndSearchRangeX);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nFindStartEndSearchRangeX);
                                break;
                            // FindStartEndXThresholdGray
                            case 15:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_nFindStartEndXThresholdGray);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nFindStartEndXThresholdGray);
                                break;
                            // ThresholdCanny1_MakeROI
                            case 16:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_dThresholdCanny1_MakeROI);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_dThresholdCanny1_MakeROI);
                                break;
                            // ThresholdCanny2_MakeROI
                            case 17:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_dThresholdCanny2_MakeROI);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_dThresholdCanny2_MakeROI);
                                break;
                            // UseAdvancedAlgorithms
                            case 18:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_bUseAdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_bUseAdvancedAlgorithms);
                                break;
                            // NumberOfDistanceMaxCount_AdvancedAlgorithms
                            case 19:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
                                break;
                            // UseFindROIAdvancedAlgorithms
                            case 20:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.b_bUseFindROIAdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.b_bUseFindROIAdvancedAlgorithms);
                                break;
                            // OffetY_Top
                            case 21:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_nOffetY_Top);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nOffetY_Top);
                                break;
                            // OffetY_Bottom
                            case 22:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_nOffetY_Bottom);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nOffetY_Bottom);
                                break;
                            // ThresholdBinaryFindROI
                            case 23:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame2.m_nThresholdBinaryFindROI);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame2_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame2.m_nThresholdBinaryFindROI);
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
                            case 2:
                            case 3:
                            case 4:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_nROI_Top[nIdx - 1]);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nROI_Top[nIdx - 1]);
                                break;
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_nROI_Bottom[nIdx - Defines.ROI_PARAMETER_COUNT - 1]);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nROI_Bottom[nIdx - Defines.ROI_PARAMETER_COUNT - 1]);
                                break;
                            // DistanceMeasurementTolerance_Refer
                            case 9:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame3.m_dDistanceMeasurementTolerance_Refer);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_dDistanceMeasurementTolerance_Refer);
                                break;
                            // DistanceMeasurementTolerance_Min
                            case 10:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame3.m_dDistanceMeasurementTolerance_Min);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_dDistanceMeasurementTolerance_Min);
                                break;
                            // DistanceMeasurementTolerance_Max
                            case 11:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame3.m_dDistanceMeasurementTolerance_Max);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_dDistanceMeasurementTolerance_Max);
                                break;
                            // DelayTimeGrab
                            case 12:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame3.m_nDelayTimeGrab);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nDelayTimeGrab);
                                break;
                            // FindStartEndX
                            case 13:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_nFindStartEndX);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nFindStartEndX);
                                break;
                            // FindStartEndSearchRangeX
                            case 14:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_nFindStartEndSearchRangeX);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nFindStartEndSearchRangeX);
                                break;
                            // FindStartEndXThresholdGray
                            case 15:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_nFindStartEndXThresholdGray);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nFindStartEndXThresholdGray);
                                break;
                            // ThresholdCanny1_MakeROI
                            case 16:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_dThresholdCanny1_MakeROI);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_dThresholdCanny1_MakeROI);
                                break;
                            // ThresholdCanny2_MakeROI
                            case 17:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_dThresholdCanny2_MakeROI);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_dThresholdCanny2_MakeROI);
                                break;
                            // UseAdvancedAlgorithms
                            case 18:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_bUseAdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_bUseAdvancedAlgorithms);
                                break;
                            // NumberOfDistanceMaxCount_AdvancedAlgorithms
                            case 19:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
                                break;
                            // UseFindROIAdvancedAlgorithms
                            case 20:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.b_bUseFindROIAdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.b_bUseFindROIAdvancedAlgorithms);
                                break;
                            // OffetY_Top
                            case 21:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_nOffetY_Top);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nOffetY_Top);
                                break;
                            // OffetY_Bottom
                            case 22:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_nOffetY_Bottom);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nOffetY_Bottom);
                                break;
                            // ThresholdBinaryFindROI
                            case 23:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame3.m_nThresholdBinaryFindROI);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame3_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame3.m_nThresholdBinaryFindROI);
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
                            case 2:
                            case 3:
                            case 4:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_nROI_Top[nIdx - 1]);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nROI_Top[nIdx - 1]);
                                break;
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_nROI_Bottom[nIdx - Defines.ROI_PARAMETER_COUNT - 1]);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nROI_Bottom[nIdx - Defines.ROI_PARAMETER_COUNT - 1]);
                                break;
                            // DistanceMeasurementTolerance_Refer
                            case 9:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame4.m_dDistanceMeasurementTolerance_Refer);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_dDistanceMeasurementTolerance_Refer);
                                break;
                            // DistanceMeasurementTolerance_Min
                            case 10:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame4.m_dDistanceMeasurementTolerance_Min);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_dDistanceMeasurementTolerance_Min);
                                break;
                            // DistanceMeasurementTolerance_Max
                            case 11:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame4.m_dDistanceMeasurementTolerance_Max);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_dDistanceMeasurementTolerance_Max);
                                break;
                            // DelayTimeGrab
                            case 12:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                    m_recipeFrame4.m_nDelayTimeGrab);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nDelayTimeGrab);
                                break;
                            // FindStartEndX
                            case 13:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_nFindStartEndX);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nFindStartEndX);
                                break;
                            // FindStartEndSearchRangeX
                            case 14:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_nFindStartEndSearchRangeX);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nFindStartEndSearchRangeX);
                                break;
                            // FindStartEndXThresholdGray
                            case 15:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_nFindStartEndXThresholdGray);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nFindStartEndXThresholdGray);
                                break;
                            // ThresholdCanny1_MakeROI
                            case 16:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_dThresholdCanny1_MakeROI);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_dThresholdCanny1_MakeROI);
                                break;
                            // ThresholdCanny2_MakeROI
                            case 17:
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_dThresholdCanny2_MakeROI);
                                double.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_dThresholdCanny2_MakeROI);
                                break;
                            // UseAdvancedAlgorithms
                            case 18:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_bUseAdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_bUseAdvancedAlgorithms);
                                break;
                            // NumberOfDistanceMaxCount_AdvancedAlgorithms
                            case 19:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms);
                                break;
                            // UseFindROIAdvancedAlgorithms
                            case 20:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.b_bUseFindROIAdvancedAlgorithms);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.b_bUseFindROIAdvancedAlgorithms);
                                break;
                            // OffetY_Top
                            case 21:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_nOffetY_Top);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nOffetY_Top);
                                break;
                            // OffetY_Bottom
                            case 22:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_nOffetY_Bottom);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nOffetY_Bottom);
                                break;
                            // ThresholdBinaryFindROI
                            case 23:
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam1Value,
                                   out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                   m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam1].
                                   m_recipeFrame4.m_nThresholdBinaryFindROI);
                                int.TryParse(MainViewModel.Instance.SettingVM.RecipeFrame4_SideCam[i].SideCam2Value,
                                    out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                    m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[nSideCam2].
                                    m_recipeFrame4.m_nThresholdBinaryFindROI);
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
