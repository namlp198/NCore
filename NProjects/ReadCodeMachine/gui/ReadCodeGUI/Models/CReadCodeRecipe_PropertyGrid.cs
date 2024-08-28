using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Npc.Foundation.Base;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ReadCodeGUI.Models
{
    [DisplayName("Recipe Settings")]
    public class CReadCodeRecipe_PropertyGrid : ModelBase
    {
        [DisplayName("Enable Read Code Tool?")]
        [Description("If true then the program can detect Code 1D and 2D")]
        public bool UseReadCode { get; set; }
        [DisplayName("Enable Inkjet Charaters Inspect Tool?")]
        public bool UseInkjetCharactersInspect { get; set; }
        [DisplayName("Enable Rotate ROI?")]
        public bool UseRotateROI { get; set; }
        [DisplayName("Max Code Count")]
        public int MaxCodeCount { get; set; }
        // params ROI of Template Matching
        [Category("Params Template Matching")]
        [PropertyOrder(1)]
        [DisplayName("Template ROI Outer X")]
        public int TemplateROI_OuterX { get; set; }
        [Category("Params Template Matching")]
        [PropertyOrder(2)]
        [DisplayName("Template ROI Outer Y")]
        public int TemplateROI_OuterY { get; set; }
        [Category("Params Template Matching")]
        [PropertyOrder(3)]
        [DisplayName("Template ROI Outer Width")]
        public int TemplateROI_Outer_Width { get; set; }
        [Category("Params Template Matching")]
        [PropertyOrder(4)]
        [DisplayName("Template ROI Outer Height")]
        public int TemplateROI_Outer_Height { get; set; }
        [Category("Params Template Matching")]
        [PropertyOrder(5)]
        [DisplayName("Template ROI Inner X")]
        public int TemplateROI_InnerX { get; set; }
        [Category("Params Template Matching")]
        [PropertyOrder(6)]
        [DisplayName("Template ROI Inner Y")]
        public int TemplateROI_InnerY { get; set; }
        [Category("Params Template Matching")]
        [PropertyOrder(7)]
        [DisplayName("Template ROI Inner Width")]
        public int TemplateROI_Inner_Width { get; set; }
        [Category("Params Template Matching")]
        [PropertyOrder(8)]
        [DisplayName("Template ROI Inner Height")]
        public int TemplateROI_Inner_Height { get; set; }
        [Category("Params Template Matching")]
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
        [PropertyOrder(12)]
        [DisplayName("Template Angle Rotate")]
        public double TemplateAngleRotate { get; set; }
        [Category("Params Template Matching")]
        [PropertyOrder(13)]
        [DisplayName("Matching Rate")]
        public double TemplateMatchingRate { get; set; }
        // ROI 1
        [Category("ROI 1")]
        [PropertyOrder(1)]
        [DisplayName("Offset X")]
        public int ROI1_OffsetX { get; set; }
        [Category("ROI 1")]
        [PropertyOrder(2)]
        [DisplayName("Offset Y")]
        public int ROI1_OffsetY { get; set; }
        [Category("ROI 1")]
        [PropertyOrder(3)]
        [DisplayName("Width")]
        public int ROI1_Width { get; set; }
        [Category("ROI 1")]
        [PropertyOrder(4)]
        [DisplayName("Height")]
        public int ROI1_Height { get; set; }
        [Category("ROI 1")]
        [PropertyOrder(5)]
        [DisplayName("Angle Rotate")]
        public double ROI1_AngleRotate { get; set; }
        [Category("ROI 1")]
        [PropertyOrder(6)]
        [DisplayName("Gray Threshold Min")]
        public int ROI1_GrayThreshold_Min { get; set; }
        [Category("ROI 1")]
        [PropertyOrder(7)]
        [DisplayName("Gray Threshold Max")]
        public int ROI1_GrayThreshold_Max { get; set; }
        [Category("ROI 1")]
        [PropertyOrder(8)]
        [DisplayName("Pixel Count Min")]
        public int ROI1_PixelCount_Min { get; set; }
        [Category("ROI 1")]
        [PropertyOrder(9)]
        [DisplayName("Pixel Count Max")]
        public int ROI1_PixelCount_Max { get; set; }
        [Category("ROI 1")]
        [PropertyOrder(10)]
        [DisplayName("Enable Show Graphics ROI1?")]
        public bool ROI1ShowGraphics { get; set; }
        // ROI 2
        [Category("ROI 2")]
        [PropertyOrder(1)]
        [DisplayName("Offset X")]
        public int ROI2_OffsetX { get; set; }
        [Category("ROI 2")]
        [PropertyOrder(2)]
        [DisplayName("Offset Y")]
        public int ROI2_OffsetY { get; set; }
        [Category("ROI 2")]
        [PropertyOrder(3)]
        [DisplayName("Width")]
        public int ROI2_Width { get; set; }
        [Category("ROI 2")]
        [PropertyOrder(4)]
        [DisplayName("Height")]
        public int ROI2_Height { get; set; }
        [Category("ROI 2")]
        [PropertyOrder(5)]
        [DisplayName("Angle Rotate")]
        public double ROI2_AngleRotate { get; set; }
        [Category("ROI 2")]
        [PropertyOrder(6)]
        [DisplayName("Gray Threshold Min")]
        public int ROI2_GrayThreshold_Min { get; set; }
        [Category("ROI 2")]
        [PropertyOrder(7)]
        [DisplayName("Gray Threshold Max")]
        public int ROI2_GrayThreshold_Max { get; set; }
        [Category("ROI 2")]
        [PropertyOrder(8)]
        [DisplayName("Pixel Count Min")]
        public int ROI2_PixelCount_Min { get; set; }
        [Category("ROI 2")]
        [PropertyOrder(9)]
        [DisplayName("Pixel Count Max")]
        public int ROI2_PixelCount_Max { get; set; }
        [Category("ROI 2")]
        [PropertyOrder(10)]
        [DisplayName("Enable Show Graphics ROI2?")]
        public bool ROI2ShowGraphics { get; set; }
        // ROI 3
        [Category("ROI 3")]
        [PropertyOrder(1)]
        [DisplayName("Offset X")]
        public int ROI3_OffsetX { get; set; }
        [Category("ROI 3")]
        [PropertyOrder(2)]
        [DisplayName("Offset Y")]
        public int ROI3_OffsetY { get; set; }
        [Category("ROI 3")]
        [PropertyOrder(3)]
        [DisplayName("Width")]
        public int ROI3_Width { get; set; }
        [Category("ROI 3")]
        [PropertyOrder(4)]
        [DisplayName("Height")]
        public int ROI3_Height { get; set; }
        [Category("ROI 3")]
        [PropertyOrder(5)]
        [DisplayName("Angle Rotate")]
        public double ROI3_AngleRotate { get; set; }
        [Category("ROI 3")]
        [PropertyOrder(6)]
        [DisplayName("Gray Threshold Min")]
        public int ROI3_GrayThreshold_Min { get; set; }
        [Category("ROI 3")]
        [PropertyOrder(7)]
        [DisplayName("Gray Threshold Max")]
        public int ROI3_GrayThreshold_Max { get; set; }
        [Category("ROI 3")]
        [PropertyOrder(8)]
        [DisplayName("Pixel Count Min")]
        public int ROI3_PixelCount_Min { get; set; }
        [Category("ROI 3")]
        [PropertyOrder(9)]
        [DisplayName("Pixel Count Max")]
        public int ROI3_PixelCount_Max { get; set; }
        [Category("ROI 3")]
        [PropertyOrder(10)]
        [DisplayName("Enable Show Graphics ROI3?")]
        public bool ROI3ShowGraphics { get; set; }
    }
}
