using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoWpf.Commons
{
    public enum ToolSelected
    {
        [Description("[...]")]
        None,
        [Description("Locator Tool")]
        LocatorTool,
        [Description("Select ROI Tool")]
        SelectROITool
    }
    public enum Algorithms
    {
        [Description("Null")]
        None = -1,
        [Description("Count Pixel")]
        CountPixel = 0,
        [Description("Calculate Area")]
        CalculateArea = 1,
        [Description("Calculate Coordinate")]
        CalculateCoordinate = 2,
        [Description("Count Blob")]
        CountBlob = 3,
        [Description("Find Line")]
        FindLine = 4,
        [Description("Find Circle")]
        FindCircle = 5,
        [Description("OCR")]
        OCR = 6
    }
    public class CommonDefines
    {
    }
}
