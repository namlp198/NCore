using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NVisionInspectGUI.Models.FakeCam.Recipe
{
    [DisplayName("Recipe Fake Cam")]
    public class CNVisionInspectRecipe_FakeCam_PropertyGrid
    {
        #region LOCATOR
        [Category("LOCATOR")]
        [PropertyOrder(1)]
        [DisplayName("Outer X")]
        [Description("")]
        public int OuterX { get; set; }
        [Category("LOCATOR")]
        [PropertyOrder(2)]
        [DisplayName("Outer Y")]
        [Description("")]
        public int OuterY { get; set; }
        [Category("LOCATOR")]
        [PropertyOrder(3)]
        [DisplayName("Outer Width")]
        [Description("")]
        public int Outer_Width { get; set; }
        [Category("LOCATOR")]
        [PropertyOrder(4)]
        [DisplayName("Outer Height")]
        [Description("")]
        public int Outer_Height { get; set; }
        [Category("LOCATOR")]
        [PropertyOrder(5)]
        [DisplayName("Inner X")]
        [Description("")]
        public int InnerX { get; set; }
        [Category("LOCATOR")]
        [PropertyOrder(6)]
        [DisplayName("Inner Y")]
        [Description("")]
        public int InnerY { get; set; }
        [Category("LOCATOR")]
        [PropertyOrder(7)]
        [DisplayName("Inner Width")]
        [Description("")]
        public int Inner_Width { get; set; }
        [Category("LOCATOR")]
        [PropertyOrder(8)]
        [DisplayName("Inner Height")]
        [Description("")]
        public int Inner_Height { get; set; }

        [Category("LOCATOR")]
        [PropertyOrder(9)]
        [DisplayName("Coordinate X")]
        [Description("")]
        public int CoordinateX { get; set; }

        [Category("LOCATOR")]
        [PropertyOrder(10)]
        [DisplayName("Coordinate Y")]
        [Description("")]
        public int CoordinateY { get; set; }

        [Category("LOCATOR")]
        [PropertyOrder(11)]
        [DisplayName("Matching Rate")]
        [Description("")]
        public double MatchingRate { get; set; }

        [Category("LOCATOR")]
        [PropertyOrder(12)]
        [DisplayName("Show Graphics")]
        [Description("")]
        public bool Is_Show_Graphics { get; set; }
        #endregion
        #region COUNT PIXEL
        [Category("COUNT PIXEL")]
        [PropertyOrder(1)]
        [DisplayName("X")]
        [Description("If Use Offset true then this parameter will be offset for X coordinates")]
        public int CountPixel_ROI_X { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(2)]
        [DisplayName("Y")]
        [Description("If Use Offset true then this parameter will be offset for X coordinates")]
        public int CountPixel_ROI_Y { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(3)]
        [DisplayName("Width")]
        [Description("If Use Offset true then this parameter will be offset for X coordinates")]
        public int CountPixel_ROI_Width { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(4)]
        [DisplayName("Height")]
        [Description("")]
        public int CountPixel_ROI_Height { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(5)]
        [DisplayName("Offset X")]
        [Description("")]
        public int CountPixel_ROI_Offset_X { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(6)]
        [DisplayName("Offset Y")]
        [Description("")]
        public int CountPixel_ROI_Offset_Y { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(7)]
        [DisplayName("Angle Rotate")]
        [Description("")]
        public double CountPixel_ROI_AngleRotate { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(8)]
        [DisplayName("Gray Threshold Min")]
        [Description("")]
        public int CountPixel_GrayThreshold_Min { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(9)]
        [DisplayName("Gray Threshold Max")]
        [Description("")]
        public int CountPixel_GrayThreshold_Max { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(10)]
        [DisplayName("Pixel Count Min")]
        [Description("")]
        public int CountPixel_PixelCount_Min { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(11)]
        [DisplayName("Pixel Count Max")]
        [Description("")]
        public int CountPixel_PixelCount_Max { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(12)]
        [DisplayName("Is Show Graphics")]
        [Description("")]
        public bool CountPixel_ShowGraphics { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(13)]
        [DisplayName("Use Offset?")]
        [Description("")]
        public bool CountPixel_UseOffset { get; set; }

        [Category("COUNT PIXEL")]
        [PropertyOrder(14)]
        [DisplayName("Use Locator?")]
        [Description("")]
        public bool CountPixel_UseLocator { get; set; }
        #endregion
        #region DECODE
        [Category("DECODE")]
        [PropertyOrder(1)]
        [DisplayName("ROI X")]
        [Description("")]
        public int Decode_ROI_X { get; set; }

        [Category("DECODE")]
        [PropertyOrder(2)]
        [DisplayName("ROI Y")]
        [Description("")]
        public int Decode_ROI_Y { get; set; }

        [Category("DECODE")]
        [PropertyOrder(3)]
        [DisplayName("ROI Width")]
        [Description("")]
        public int Decode_ROI_Width { get; set; }

        [Category("DECODE")]
        [PropertyOrder(4)]
        [DisplayName("ROI Height")]
        [Description("")]
        public int Decode_ROI_Height { get; set; }

        [Category("DECODE")]
        [PropertyOrder(5)]
        [DisplayName("Max Code Count")]
        [Description("Maximum code number that can be decoded")]
        public int Decode_MaxCodeCount { get; set; }
        #endregion
        #region HSV
        [Category("HSV")]
        [PropertyOrder(1)]
        [DisplayName("Hue Min")]
        [Description("")]
        public int HueMin { get; set; }
        [Category("HSV")]
        [PropertyOrder(2)]
        [DisplayName("Hue Max")]
        [Description("")]
        public int HueMax { get; set; }
        [Category("HSV")]
        [PropertyOrder(3)]
        [DisplayName("Saturation Min")]
        [Description("")]
        public int SaturationMin { get; set; }
        [Category("HSV")]
        [PropertyOrder(4)]
        [DisplayName("Saturation Max")]
        [Description("")]
        public int SaturationMax { get; set; }
        [Category("HSV")]
        [PropertyOrder(5)]
        [DisplayName("Value Min")]
        [Description("")]
        public int ValueMin { get; set; }
        [Category("HSV")]
        [PropertyOrder(6)]
        [DisplayName("Value Max")]
        [Description("")]
        public int ValueMax { get; set; }
        #endregion
    }
}
