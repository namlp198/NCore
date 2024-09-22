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
    }
}
