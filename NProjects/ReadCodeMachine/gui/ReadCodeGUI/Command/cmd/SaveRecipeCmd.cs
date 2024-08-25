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
            //SetValueRecipe();
            {
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bUseReadCode = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.UseReadCode == true ? 1 : 0;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bUseInkjetCharactersInspect = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.UseInkjetCharactersInspect == true ? 1 : 0;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bUseRotateROI = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.UseRotateROI == true ? 1 : 0;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nMaxCodeCount = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.MaxCodeCount;
                // params of Template Matching
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_OuterX = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateROI_OuterX;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_OuterY = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateROI_OuterY;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Outer_Width = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateROI_Outer_Width;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Outer_Height = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateROI_Outer_Height;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_InnerX = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateROI_InnerX;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_InnerY = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateROI_InnerY;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Inner_Width = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateROI_Inner_Width;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Inner_Height = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateROI_Inner_Height;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateCoordinatesX = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateCoordinatesX;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateCoordinatesY = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateCoordinatesY;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_dTemplateAngleRotate = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateAngleRotate;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_dTemplateMatchingRate = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateMatchingRate;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bTemplateShowGraphics = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.TemplateShowGraphics == true ? 1 : 0;
                // ROI 1
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_OffsetX = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI1_OffsetX;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_OffsetY = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI1_OffsetY;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_Width = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI1_Width;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_Height = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI1_Height;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_AngleRotate = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI1_AngleRotate;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_GrayThreshold_Min = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI1_GrayThreshold_Min;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_GrayThreshold_Max = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI1_GrayThreshold_Max;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_PixelCount_Min = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI1_PixelCount_Min;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_PixelCount_Max = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI1_PixelCount_Max;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bROI1ShowGraphics = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI1ShowGraphics == true ? 1 : 0;
                // ROI 2
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_OffsetX = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI2_OffsetX;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_OffsetY = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI2_OffsetY;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_Width = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI2_Width;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_Height = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI2_Height;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_AngleRotate = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI2_AngleRotate;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_GrayThreshold_Min = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI2_GrayThreshold_Min;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_GrayThreshold_Max = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI2_GrayThreshold_Max;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_PixelCount_Min = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI2_PixelCount_Min;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_PixelCount_Max = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI2_PixelCount_Max;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bROI2ShowGraphics = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI2ShowGraphics == true ? 1 : 0;
                // ROI 3
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_OffsetX = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI3_OffsetX;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_OffsetY = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI3_OffsetY;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_Width = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI3_Width;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_Height = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI3_Height;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_AngleRotate = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI3_AngleRotate;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_GrayThreshold_Min = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI3_GrayThreshold_Min;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_GrayThreshold_Max = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI3_GrayThreshold_Max;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_PixelCount_Min = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI3_PixelCount_Min;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_PixelCount_Max = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI3_PixelCount_Max;
                InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bROI3ShowGraphics = MainViewModel.Instance.SettingVM.ReadCodeRecipePropertyGrid.ROI3ShowGraphics == true ? 1 : 0;
            }

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
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value, 
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bUseReadCode);
                        break;
                    case 2:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bUseInkjetCharactersInspect);
                        break;
                    case 3:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bUseRotateROI);
                        break;
                    case 4:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nMaxCodeCount);
                        break;
                    // Template Matching
                    // Outer
                    case 5:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_OuterX);
                        break;
                    case 6:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_OuterY);
                        break;
                    case 7:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Outer_Width);
                        break;
                    case 8:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Outer_Height);
                        break;
                    // Inner
                    case 9:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_InnerX);
                        break;
                    case 10:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_InnerY);
                        break;
                    case 11:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Inner_Width);
                        break;
                    case 12:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Inner_Height);
                        break;
                    // params of Template Matching
                    case 13:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateCoordinatesX);
                        break;
                    case 14:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateCoordinatesY);
                        break;
                    case 15:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_dTemplateAngleRotate);
                        break;
                    // ROI1
                    case 16:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_OffsetX);
                        break;
                    case 17:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_OffsetY);
                        break;
                    case 18:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_Width);
                        break;
                    case 19:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_Height);
                        break;
                    case 20:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_AngleRotate);
                        break;
                    case 21:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_GrayThreshold_Min);
                        break;
                    case 22:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_GrayThreshold_Max);
                        break;
                    case 23:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_PixelCount_Min);
                        break;
                    case 24:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_PixelCount_Max);
                        break;
                    // ROI2
                    case 25:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_OffsetX);
                        break;
                    case 26:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_OffsetY);
                        break;
                    case 27:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_Width);
                        break;
                    case 28:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_Height);
                        break;
                    case 29:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_AngleRotate);
                        break;
                    case 30:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_GrayThreshold_Min);
                        break;
                    case 31:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_GrayThreshold_Max);
                        break;
                    case 32:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_PixelCount_Min);
                        break;
                    case 33:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_PixelCount_Max);
                        break;
                    // ROI3
                    case 34:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_OffsetX);
                        break;
                    case 35:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_OffsetY);
                        break;
                    case 36:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_Width);
                        break;
                    case 37:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_Height);
                        break;
                    case 38:
                        double.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_AngleRotate);
                        break;
                    case 39:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_GrayThreshold_Min);
                        break;
                    case 40:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_GrayThreshold_Max);
                        break;
                    case 41:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_PixelCount_Min);
                        break;
                    case 42:
                        int.TryParse(MainViewModel.Instance.SettingVM.RecipeMapToDataGridModels[i].Value,
                                     out InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_PixelCount_Max);
                        break;
                }
            }
        }
    }
}
