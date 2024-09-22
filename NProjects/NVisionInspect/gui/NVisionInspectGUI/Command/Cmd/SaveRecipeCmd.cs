using NVisionInspectGUI.Commons;
using NVisionInspectGUI.Manager;
using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
{
    public class SaveRecipeCmd : CommandBase
    {
        public SaveRecipeCmd() { }
        public override void Execute(object parameter)
        {
            if (parameter == null)
                return;
            
            int nCamIdx = int.Parse(parameter.ToString());

            if (nCamIdx < 0)
                return;

            // Fake Cam
            if(nCamIdx == 8)
            {
                // COUNT PIXEL
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_nCountPixel_ROI_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_ROI_X;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_nCountPixel_ROI_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_ROI_Y;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_nCountPixel_ROI_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_ROI_Width;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_nCountPixel_ROI_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_ROI_Height;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_nCountPixel_ROI_Offset_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_ROI_Offset_X;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_nCountPixel_ROI_Offset_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_ROI_Offset_Y;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_nCountPixel_ROI_AngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_ROI_AngleRotate;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_nCountPixel_GrayThreshold_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_GrayThreshold_Min;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_nCountPixel_GrayThreshold_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_GrayThreshold_Max;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_nCountPixel_PixelCount_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_PixelCount_Min;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_nCountPixel_PixelCount_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_PixelCount_Max;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_bCountPixel_ShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_ShowGraphics == true ? 1 : 0;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_bCountPixel_UseOffset = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_UseOffset == true ? 1 : 0;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_CountPixel.m_bCountPixel_UseLocator = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CountPixel_UseLocator == true ? 1 : 0;

                InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.
                SaveRecipe_FakeCam(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam);

                return;
            }

            switch(nCamIdx)
            {
                case 0:
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bUseReadCode = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.UseReadCode == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bUseInkjetCharactersInspect = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.UseInkjetCharactersInspect == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bUseRotateROI = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.UseRotateROI == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nMaxCodeCount = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.MaxCodeCount;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nNumberOfROI = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.NumberOfROI;
                    // params of Template Matching
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_OuterX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_OuterX;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_OuterY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_OuterY;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Outer_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Outer_Width;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Outer_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Outer_Height;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_InnerX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_InnerX;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_InnerY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_InnerY;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Inner_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Inner_Width;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Inner_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Inner_Height;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateCoordinatesX;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateCoordinatesY;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_dTemplateAngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateAngleRotate;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_dTemplateMatchingRate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateMatchingRate;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bTemplateShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateShowGraphics == true ? 1 : 0;
                    // ROI 1
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_X;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_Y;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_Width;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_Height;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Offset_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_Offset_X;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Offset_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_Offset_Y;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_AngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_AngleRotate;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI1UseOffset = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1UseOffset == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI1UseLocator = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1UseLocator == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI1ShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1ShowGraphics == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_GrayThreshold_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_GrayThreshold_Min;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_GrayThreshold_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_GrayThreshold_Max;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_PixelCount_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_PixelCount_Min;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_PixelCount_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_PixelCount_Max;
                    
                    // ROI 2
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_X;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_Y;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_Width;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_Height;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Offset_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_Offset_X;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Offset_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_Offset_Y;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_AngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_AngleRotate;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI2UseOffset = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2UseOffset == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI2UseLocator = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2UseLocator == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI2ShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2ShowGraphics == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_GrayThreshold_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_GrayThreshold_Min;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_GrayThreshold_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_GrayThreshold_Max;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_PixelCount_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_PixelCount_Min;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_PixelCount_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_PixelCount_Max;
                   
                    // ROI 3
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_X;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_Y;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_Width;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_Height;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Offset_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_Offset_X;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Offset_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_Offset_Y;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_AngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_AngleRotate;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI3UseOffset = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3UseOffset == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI3UseLocator = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3UseLocator == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI3ShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3ShowGraphics == true ? 1 : 0;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_GrayThreshold_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_GrayThreshold_Min;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_GrayThreshold_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_GrayThreshold_Max;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_PixelCount_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_PixelCount_Min;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_PixelCount_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_PixelCount_Max;

                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
            }

            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.
                SaveRecipe(nCamIdx, ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe);
        }
    }
}
