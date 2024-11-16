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
            if (nCamIdx >= MainViewModel.Instance.SettingVM.CameraCount)
            {
                // LOCATOR
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_nTemplateROI_OuterX = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.OuterX;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_nTemplateROI_OuterY = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.OuterY;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_nTemplateROI_Outer_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.Outer_Width;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_nTemplateROI_Outer_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.Outer_Height;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_nTemplateROI_InnerX = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.InnerX;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_nTemplateROI_InnerY = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.InnerY;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_nTemplateROI_Inner_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.Inner_Width;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_nTemplateROI_Inner_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.Inner_Height;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_nTemplateCoordinatesX = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CoordinateX;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_nTemplateCoordinatesY = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.CoordinateY;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_dTemplateMatchingRate = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.MatchingRate;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Locator.m_bTemplateShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.Is_Show_Graphics == true ? 1 : 0;

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

                // DECODE
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Decode.m_nDecode_ROI_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.Decode_ROI_X;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Decode.m_nDecode_ROI_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.Decode_ROI_Y;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Decode.m_nDecode_ROI_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.Decode_ROI_Width;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Decode.m_nDecode_ROI_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.Decode_ROI_Height;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_Decode.m_nMaxCodeCount = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.Decode_MaxCodeCount;

                // HSV
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_HSV.m_nHueMin = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.HueMin;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_HSV.m_nHueMax = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.HueMax;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_HSV.m_nSaturationMin = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.SaturationMin;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_HSV.m_nSaturationMax = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.SaturationMax;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_HSV.m_nValueMin = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.ValueMin;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam.m_NVisionInspRecipe_HSV.m_nValueMax = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid.ValueMax;

                InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.
                SaveRecipe_FakeCam(ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe_FakeCam);

                return;
            }

            switch (nCamIdx)
            {
                case 0:
                    // params of Template Matching
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_nTemplateROI_OuterX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_OuterX;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_nTemplateROI_OuterY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_OuterY;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_nTemplateROI_Outer_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Outer_Width;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_nTemplateROI_Outer_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Outer_Height;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_nTemplateROI_InnerX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_InnerX;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_nTemplateROI_InnerY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_InnerY;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_nTemplateROI_Inner_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Inner_Width;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_nTemplateROI_Inner_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Inner_Height;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_nTemplateCoordinatesX = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateCoordinatesX;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_nTemplateCoordinatesY = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateCoordinatesY;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_dTemplateMatchingRate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateMatchingRate;
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_Locator.m_bTemplateShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateShowGraphics == true ? 1 : 0;

                    for (int i = 0; i < Defines.MAX_COUNT_PIXEL_TOOL_COUNT_CAM1; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_X;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_Y;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_Width;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_Height;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Offset_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_Offset_X;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Offset_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_Offset_Y;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_AngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_AngleRotate;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_bCountPixel_UseOffset = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_UseOffset == true ? 1 : 0;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_bCountPixel_UseLocator = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_UseLocator == true ? 1 : 0;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_bCountPixel_ShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_ShowGraphics == true ? 1 : 0;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_GrayThreshold_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_GrayThreshold_Min;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_GrayThreshold_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_GrayThreshold_Max;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_PixelCount_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_PixelCount_Min;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_PixelCount_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI1_PixelCount_Max;
                                break;
                            case 1:
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_X;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_Y;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_Width;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_Height;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Offset_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_Offset_X;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Offset_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_Offset_Y;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_AngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_AngleRotate;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_bCountPixel_UseOffset = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_UseOffset == true ? 1 : 0;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_bCountPixel_UseLocator = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_UseLocator == true ? 1 : 0;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_bCountPixel_ShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_ShowGraphics == true ? 1 : 0;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_GrayThreshold_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_GrayThreshold_Min;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_GrayThreshold_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_GrayThreshold_Max;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_PixelCount_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_PixelCount_Min;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_PixelCount_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI2_PixelCount_Max;
                                break;
                            case 2:
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_X;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_Y;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Width = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_Width;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Height = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_Height;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Offset_X = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_Offset_X;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_Offset_Y = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_Offset_Y;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_ROI_AngleRotate = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_AngleRotate;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_bCountPixel_UseOffset = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_UseOffset == true ? 1 : 0;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_bCountPixel_UseLocator = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_UseLocator == true ? 1 : 0;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_bCountPixel_ShowGraphics = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_ShowGraphics == true ? 1 : 0;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_GrayThreshold_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_GrayThreshold_Min;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_GrayThreshold_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_GrayThreshold_Max;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_PixelCount_Min = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_PixelCount_Min;
                                InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_NVisionInspRecipe_CntPxl[i].m_nCountPixel_PixelCount_Max = MainViewModel.Instance.SettingVM.NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.CountPixel_ROI3_PixelCount_Max;
                                break;
                        }
                    }

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
