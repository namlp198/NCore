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
            //SetValueRecipe();
            {
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_bUseReadCode = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.UseReadCode == true ? 1 : 0;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_bUseInkjetCharactersInspect = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.UseInkjetCharactersInspect == true ? 1 : 0;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_bUseRotateROI = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.UseRotateROI == true ? 1 : 0;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nMaxCodeCount = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.MaxCodeCount;
                // params of Template Matching
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nTemplateROI_OuterX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateROI_OuterX;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nTemplateROI_OuterY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateROI_OuterY;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nTemplateROI_Outer_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateROI_Outer_Width;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nTemplateROI_Outer_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateROI_Outer_Height;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nTemplateROI_InnerX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateROI_InnerX;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nTemplateROI_InnerY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateROI_InnerY;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nTemplateROI_Inner_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateROI_Inner_Width;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nTemplateROI_Inner_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateROI_Inner_Height;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nTemplateCoordinatesX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateCoordinatesX;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nTemplateCoordinatesY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateCoordinatesY;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_dTemplateAngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateAngleRotate;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_dTemplateMatchingRate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateMatchingRate;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_bTemplateShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.TemplateShowGraphics == true ? 1 : 0;
                // ROI 1
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI1_OffsetX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI1_OffsetX;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI1_OffsetY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI1_OffsetY;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI1_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI1_Width;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI1_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI1_Height;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI1_AngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI1_AngleRotate;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI1_GrayThreshold_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI1_GrayThreshold_Min;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI1_GrayThreshold_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI1_GrayThreshold_Max;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI1_PixelCount_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI1_PixelCount_Min;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI1_PixelCount_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI1_PixelCount_Max;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_bROI1ShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI1ShowGraphics == true ? 1 : 0;
                // ROI 2
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI2_OffsetX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI2_OffsetX;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI2_OffsetY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI2_OffsetY;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI2_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI2_Width;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI2_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI2_Height;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI2_AngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI2_AngleRotate;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI2_GrayThreshold_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI2_GrayThreshold_Min;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI2_GrayThreshold_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI2_GrayThreshold_Max;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI2_PixelCount_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI2_PixelCount_Min;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI2_PixelCount_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI2_PixelCount_Max;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_bROI2ShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI2ShowGraphics == true ? 1 : 0;
                // ROI 3
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI3_OffsetX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI3_OffsetX;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI3_OffsetY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI3_OffsetY;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI3_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI3_Width;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI3_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI3_Height;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI3_AngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI3_AngleRotate;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI3_GrayThreshold_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI3_GrayThreshold_Min;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI3_GrayThreshold_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI3_GrayThreshold_Max;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI3_PixelCount_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI3_PixelCount_Min;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_nROI3_PixelCount_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI3_PixelCount_Max;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_bROI3ShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.ROI3ShowGraphics == true ? 1 : 0;
            }

            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.
                SaveRecipe(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe);
        }
    }
}
