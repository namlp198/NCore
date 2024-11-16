using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Npc.Foundation.Base;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NVisionInspectGUI.Models.Recipe
{
    [DisplayName("Recipe Cam1 Settings")]
    public class CNVisionInspectRecipe_Cam1_PropertyGrid : ModelBase
    {
        // params ROI of Locator Tool
        [Category("Params ROI Template Matching")]
        [PropertyOrder(1)]
        [DisplayName("Template ROI Outer X")]
        public int TemplateROI_OuterX { get; set; }

        [Category("Params ROI Template Matching")]
        [PropertyOrder(2)]
        [DisplayName("Template ROI Outer Y")]
        public int TemplateROI_OuterY { get; set; }

        [Category("Params ROI Template Matching")]
        [PropertyOrder(3)]
        [DisplayName("Template ROI Outer Width")]
        public int TemplateROI_Outer_Width { get; set; }

        [Category("Params ROI Template Matching")]
        [PropertyOrder(4)]
        [DisplayName("Template ROI Outer Height")]
        public int TemplateROI_Outer_Height { get; set; }

        [Category("Params ROI Template Matching")]
        [PropertyOrder(5)]
        [DisplayName("Template ROI Inner X")]
        public int TemplateROI_InnerX { get; set; }

        [Category("Params ROI Template Matching")]
        [PropertyOrder(6)]
        [DisplayName("Template ROI Inner Y")]
        public int TemplateROI_InnerY { get; set; }

        [Category("Params ROI Template Matching")]
        [PropertyOrder(7)]
        [DisplayName("Template ROI Inner Width")]
        public int TemplateROI_Inner_Width { get; set; }

        [Category("Params ROI Template Matching")]
        [PropertyOrder(8)]
        [DisplayName("Template ROI Inner Height")]
        public int TemplateROI_Inner_Height { get; set; }

        [Category("Params ROI Template Matching")]
        [PropertyOrder(9)]
        [DisplayName("Enable Show Graphics Template Matching?")]
        public bool TemplateShowGraphics { get; set; }

        // params of Template Matching
        [Category("Params Template Matching")]
        [PropertyOrder(10)]
        [DisplayName("Template Coordinates X")]
        public int TemplateCoordinatesX { get; set; }

        [Category("Params Template Matching")]
        [PropertyOrder(11)]
        [DisplayName("Template Coordinates Y")]
        public int TemplateCoordinatesY { get; set; }

        [Category("Params Template Matching")]
        [PropertyOrder(13)]
        [DisplayName("Matching Rate")]
        public double TemplateMatchingRate { get; set; }

        // ROI 1
        [Category("Count Pixel 1")]
        [PropertyOrder(1)]
        [DisplayName("X")]
        [Description("If Use Offset true then this parameter will be offset for X coordinates")]
        public int CountPixel_ROI1_X { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(2)]
        [DisplayName("Y")]
        [Description("If Use Offset true then this parameter will be offset for Y coordinates")]
        public int CountPixel_ROI1_Y { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(3)]
        [DisplayName("Width")]
        public int CountPixel_ROI1_Width { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(4)]
        [DisplayName("Height")]
        public int CountPixel_ROI1_Height { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(5)]
        [DisplayName("Offset X")]
        [Description("If Use Offset true then this parameter will be offset for Template Center Pt")]
        public int CountPixel_ROI1_Offset_X { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(6)]
        [DisplayName("Offset Y")]
        [Description("If Use Offset true then this parameter will be offset for Template Center Pt")]
        public int CountPixel_ROI1_Offset_Y { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(7)]
        [DisplayName("Angle Rotate")]
        public double CountPixel_ROI1_AngleRotate { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(8)]
        [DisplayName("Use Offset for XY coordinates?")]
        public bool CountPixel_ROI1_UseOffset { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(9)]
        [DisplayName("Use Locator Tool?")]
        public bool CountPixel_ROI1_UseLocator { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(10)]
        [DisplayName("Enable Show Graphics ROI1?")]
        public bool CountPixel_ROI1_ShowGraphics { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(11)]
        [DisplayName("Gray Threshold Min")]
        public int CountPixel_ROI1_GrayThreshold_Min { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(12)]
        [DisplayName("Gray Threshold Max")]
        public int CountPixel_ROI1_GrayThreshold_Max { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(13)]
        [DisplayName("Pixel Count Min")]
        public int CountPixel_ROI1_PixelCount_Min { get; set; }

        [Category("Count Pixel 1")]
        [PropertyOrder(14)]
        [DisplayName("Pixel Count Max")]
        public int CountPixel_ROI1_PixelCount_Max { get; set; }

        // ROI 2
        [Category("Count Pixel 2")]
        [PropertyOrder(15)]
        [DisplayName("X")]
        [Description("If Use Offset true then this parameter will be offset for X coordinates")]
        public int CountPixel_ROI2_X { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(16)]
        [DisplayName("Y")]
        [Description("If Use Offset true then this parameter will be offset for Y coordinates")]
        public int CountPixel_ROI2_Y { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(17)]
        [DisplayName("Width")]
        public int CountPixel_ROI2_Width { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(18)]
        [DisplayName("Height")]
        public int CountPixel_ROI2_Height { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(19)]
        [DisplayName("Offset X")]
        [Description("If Use Offset true then this parameter will be offset for Template Center Pt")]
        public int CountPixel_ROI2_Offset_X { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(20)]
        [DisplayName("Offset Y")]
        [Description("If Use Offset true then this parameter will be offset for Template Center Pt")]
        public int CountPixel_ROI2_Offset_Y { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(21)]
        [DisplayName("Angle Rotate")]
        public double CountPixel_ROI2_AngleRotate { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(22)]
        [DisplayName("Use Offset for XY coordinates?")]
        public bool CountPixel_ROI2_UseOffset { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(23)]
        [DisplayName("Use Locator Tool?")]
        public bool CountPixel_ROI2_UseLocator { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(24)]
        [DisplayName("Enable Show Graphics ROI1?")]
        public bool CountPixel_ROI2_ShowGraphics { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(25)]
        [DisplayName("Gray Threshold Min")]
        public int CountPixel_ROI2_GrayThreshold_Min { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(26)]
        [DisplayName("Gray Threshold Max")]
        public int CountPixel_ROI2_GrayThreshold_Max { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(27)]
        [DisplayName("Pixel Count Min")]
        public int CountPixel_ROI2_PixelCount_Min { get; set; }

        [Category("Count Pixel 2")]
        [PropertyOrder(28)]
        [DisplayName("Pixel Count Max")]
        public int CountPixel_ROI2_PixelCount_Max { get; set; }

        // ROI 3
        [Category("Count Pixel 3")]
        [PropertyOrder(29)]
        [DisplayName("X")]
        [Description("If Use Offset true then this parameter will be offset for X coordinates")]
        public int CountPixel_ROI3_X { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(30)]
        [DisplayName("Y")]
        [Description("If Use Offset true then this parameter will be offset for Y coordinates")]
        public int CountPixel_ROI3_Y { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(31)]
        [DisplayName("Width")]
        public int CountPixel_ROI3_Width { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(32)]
        [DisplayName("Height")]
        public int CountPixel_ROI3_Height { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(33)]
        [DisplayName("Offset X")]
        [Description("If Use Offset true then this parameter will be offset for Template Center Pt")]
        public int CountPixel_ROI3_Offset_X { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(34)]
        [DisplayName("Offset Y")]
        [Description("If Use Offset true then this parameter will be offset for Template Center Pt")]
        public int CountPixel_ROI3_Offset_Y { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(35)]
        [DisplayName("Angle Rotate")]
        public double CountPixel_ROI3_AngleRotate { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(36)]
        [DisplayName("Use Offset for XY coordinates?")]
        public bool CountPixel_ROI3_UseOffset { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(37)]
        [DisplayName("Use Locator Tool?")]
        public bool CountPixel_ROI3_UseLocator { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(38)]
        [DisplayName("Enable Show Graphics ROI1?")]
        public bool CountPixel_ROI3_ShowGraphics { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(39)]
        [DisplayName("Gray Threshold Min")]
        public int CountPixel_ROI3_GrayThreshold_Min { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(40)]
        [DisplayName("Gray Threshold Max")]
        public int CountPixel_ROI3_GrayThreshold_Max { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(41)]
        [DisplayName("Pixel Count Min")]
        public int CountPixel_ROI3_PixelCount_Min { get; set; }

        [Category("Count Pixel 3")]
        [PropertyOrder(42)]
        [DisplayName("Pixel Count Max")]
        public int CountPixel_ROI3_PixelCount_Max { get; set; }
    }
}
